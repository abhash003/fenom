using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using Microsoft.Extensions.Logging;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace FenomPlus.Services
{
    public class CacheService : BaseService, ICacheService, IDisposable
    {
        private readonly RingBuffer BreathBuffer;

        public CacheService(IAppServices services) : base(services)
        {
            BreathBuffer = new RingBuffer(Services.Config.RingBufferSample, Services.Config.RingBufferTimeout);

            BreathFlowTimer = Services.Config.BreathFlowTimeout;
            DeviceSerialNumber = string.Empty;
            Firmware = string.Empty;
            FenomReady = true;

            Logger = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug)
                    .AddFilter("FenomPlus", LogLevel.Debug);
            });

            DebugList = new RangeObservableCollection<DebugLog>();
            EnvironmentalInfo = new EnvironmentalInfo();
            BreathManeuver = new BreathManeuver();
            DeviceInfo = new DeviceInfo();
            DebugMsg = new DebugMsg();

            // write path to debug
            DebugList.Insert(0, DebugLog.Create("App Starting"));
            DebugList.Insert(0, DebugLog.Create(Services.DebugLogFile.GetFilePath()));
        }

        public RangeObservableCollection<DebugLog> DebugList {get;set;}

        public ILoggerFactory Logger { get; set; }

        public int BatteryLevel { get; set; }

        public DateTime DeviceExpireDate { get; set; }

        public DateTime SensorExpireDate { get; set; }

        public TestTypeEnum TestType { get; set; }

        public int BreathFlowTimer { get; set; }

        public int NOScore { get; set; }

        public float HumanControlResult { get; set; }

        private float _breathFlow { get; set; }
        public float BreathFlow 
        { 
            get => _breathFlow / 1000;
            set { _breathFlow = value; }
        }
        
        public EnvironmentalInfo EnvironmentalInfo { get; set; }

        public DeviceInfo DeviceInfo { get; set; }

        public ErrorStatusInfo ErrorStatusInfo { get; set; }

        public DeviceStatusInfo DeviceStatusInfo { get; set; }

        public BreathManeuver BreathManeuver { get; set; }

        public DebugMsg DebugMsg { get; set; }

        public string _deviceConnectedStatus = "Unknown";
        public string DeviceConnectedStatus
        {
            get => _deviceConnectedStatus;
            set {
                _deviceConnectedStatus = value;
                NotifyViews();
                NotifyViewModels();
            }
        }

        public string _firmware;
        public string Firmware
        {
            get => _firmware;
            set {
                _firmware = value;
                NotifyViews();
                NotifyViewModels();
            }
        }

        public string _deviceSerialNumber;
        public string DeviceSerialNumber
        {
            get => _deviceSerialNumber;
            set {
                _deviceSerialNumber = value;
                NotifyViews();
                NotifyViewModels();
            }
        }

        private static ISettings AppSettings => CrossSettings.Current;

        public bool ReadyForTest { get; set; }

        public bool FenomReady { get; set; }

        public float FenomValue { get; set; }

        public string QCUsername { get; set; }

        public EnvironmentalInfo DecodeEnvironmentalInfo(byte[] data)
        {
            try
            {
                EnvironmentalInfo ??= new EnvironmentalInfo();
                EnvironmentalInfo.Decode(data);

                BatteryLevel = EnvironmentalInfo.BatteryLevel;

                NotifyViews();
                NotifyViewModels();
            } finally { }
            return EnvironmentalInfo;
        }

        public DeviceInfo DecodeDeviceInfo(byte[] data)
        {
            try
            {
                DeviceInfo ??= new DeviceInfo();

                DeviceInfo.Decode(data);

                // setup serial number
                if ((DeviceInfo.SerialNumber != null) && (DeviceInfo.SerialNumber.Length > 0))
                {
                    DeviceSerialNumber = $"{Encoding.Default.GetString(DeviceInfo.SerialNumber)}";
                    Debug.WriteLine($"----> Device Serial Number: {DeviceSerialNumber}");

                    // update the database
                    Services.Database.QualityControlDevicesRepo.UpdateDateOrAdd(DeviceSerialNumber);
                }

                // setup firmware version
                Firmware = $"{DeviceInfo.MajorVersion}.{DeviceInfo.MinorVersion}";

                // get SensorExpireDate
                SensorExpireDate = new DateTime(DeviceInfo.SensorExpDateYear, DeviceInfo.SensorExpDateMonth, DeviceInfo.SensorExpDateDay);

                NotifyViews();
                NotifyViewModels();
            }
            finally { }
            return DeviceInfo;
        }

        public ErrorStatusInfo DecodeErrorStatusInfo(byte[] data)
        {
            try
            {
                ErrorStatusInfo ??= new ErrorStatusInfo();
                ErrorStatusInfo.Decode(data);

                NotifyViews();
                NotifyViewModels();
            }
            finally { }
            return ErrorStatusInfo;
        }

        public DeviceStatusInfo DecodeDeviceStatusInfo(byte[] data)
        {
            try
            {
                DeviceStatusInfo ??= new DeviceStatusInfo();
                DeviceStatusInfo.Decode(data);

                NotifyViews();
                NotifyViewModels();
            }
            finally { }
            return DeviceStatusInfo;
        }

        public event EventHandler BreathFlowChanged;

        public BreathManeuver DecodeBreathManeuver(byte[] data)
        {
            try
            {
                BreathManeuver ??= new BreathManeuver();

                BreathManeuver.Decode(data);

                if (BreathManeuver.TimeRemaining == 0xff)
                {
                    FenomReady = false;
                    ReadyForTest = true;
                    DeviceConnectedStatus = "Ready For Test";
                } 
                else if (BreathManeuver.TimeRemaining == 0xfe) 
                {
                    ReadyForTest = false;
                    FenomReady = true;
                    FenomValue = BreathManeuver.NOScore;
                } 
                else if (BreathManeuver.TimeRemaining == 0xf0) 
                {
                    // log ??
                } 
                else 
                {
                    DeviceConnectedStatus = "Processing Test";
                    FenomReady = false;

                    // add new value and average it
                    BreathFlow = BreathBuffer.Add(BreathManeuver.BreathFlow);
                    BreathFlowChanged?.Invoke(null, null);

                    // get the noscores
                    NOScore = BreathManeuver.NOScore;
                }

                NotifyViews();

                NotifyViewModels();

            } finally { }
            return BreathManeuver;
        }

        static volatile object _s_debugLogLock = new object();
        static volatile bool _s_logFileWriterWorkerStarted = false;
        static volatile bool _s_logFileWriterWorkerStop = false;

        public virtual void Dispose()
        {
            _s_logFileWriterWorkerStop = true;
        }

        private void LogFileWriterWorker()
        {
            _s_logFileWriterWorkerStarted = true;

            var current = DateTime.Now;

            while (_s_logFileWriterWorkerStop == false)
            {
                if ((DateTime.Now - current).TotalSeconds > 60)
                {
                    // flush the log
                    var debugList = Services.Cache.DebugList;

                    lock (_s_debugLogLock)
                    {
                        Services.DebugLogFile.Write(debugList);
                        debugList.Clear();
                    }

                    // reset the time
                    current = DateTime.Now;
                }
            }
        }

        public DebugMsg DecodeDebugMsg(byte[] data)
        {
            if (_s_logFileWriterWorkerStarted == false)
            {
                // start the worker
                Thread workerThread = new Thread(LogFileWriterWorker);
                workerThread.Start();
            }

            try
            {
                DebugMsg ??= new DebugMsg();

                DebugMsg.Decode(data);

                DebugLog debugLog = DebugLog.Create(data);

                lock (_s_debugLogLock)
                {
                    DebugList.Insert(0, debugLog);
                }

            }

            catch (Exception ex)
            {
                Services.LogCat.Print(ex);
            }

            return DebugMsg;
        }

        // ToDo: Remove - bad design
        private void NotifyViews()
        {
            App.NotifyViews();
        }

        // ToDo: Remove - bad design
        private void NotifyViewModels()
        {
            App.NotifyViewModels();
        }

        public enum DeviceCheckEnum
        {
            Ready,
            DevicePurging,
            HumidityOutOfRange,
            PressureOutOfRange,
            TemperatureOutOfRange,
            BatteryCriticallyLow
        }

        public DeviceCheckEnum CheckDeviceBeforeTest()
        {
            // Get the latest environmental info - updates Cache
            Services.DeviceService.Current?.RequestEnvironmentalInfo();

            if (Services.DeviceService.Current is { ReadyForTest: false })
            {
                Debug.WriteLine("Device is purging");
                return DeviceCheckEnum.DevicePurging;
            }

            if (Services.Cache.EnvironmentalInfo.BatteryLevel < Constants.BatteryCritical3)
            {
                return DeviceCheckEnum.BatteryCriticallyLow;
            }

            if (Services.Cache.EnvironmentalInfo.Humidity < Constants.HumidityLow18 ||
                Services.Cache.EnvironmentalInfo.Humidity > Constants.HumidityHigh92)
            {
                return DeviceCheckEnum.HumidityOutOfRange;
            }

            if (Services.Cache.EnvironmentalInfo.Pressure < Constants.PressureLow75 ||
                Services.Cache.EnvironmentalInfo.Pressure > Constants.PressureHigh110)
            {
                return DeviceCheckEnum.PressureOutOfRange;
            }

            if (Services.Cache.EnvironmentalInfo.Temperature < Constants.TemperatureLow14 ||
                Services.Cache.EnvironmentalInfo.Temperature > Constants.TemperatureHigh35)
            {
                return DeviceCheckEnum.TemperatureOutOfRange;
            }

            return DeviceCheckEnum.Ready;
        }
    }
}

