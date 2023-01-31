#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using PluginBleIDevice = Plugin.BLE.Abstractions.Contracts.IDevice;
using IDevice = FenomPlus.Services.DeviceService.Interfaces.IDevice;
using FenomPlus.SDK.Core.Models;
using FenomPlus.SDK.Core.Utils;
using System.Timers;
using FenomPlus.Services.DeviceService.Enums;
using System.Threading;
using FenomPlus.Models;
using Timer = System.Timers.Timer;
using FenomPlus.Helpers;
using System.Text;
using System.Diagnostics;
using FenomPlus.SDK.Core.Features;

namespace FenomPlus.Services.DeviceService.Abstract
{
    public abstract partial class Device : IDevice, IDisposable
    {
        // Fields

        protected object _nativeDevice = null;
        private int calls = 0;

        // Constructor

        public Device(object nativeDevice)
        {
            _nativeDevice = (PluginBleIDevice)nativeDevice;

            /*
             * LEGACY CODE
             */
            DeviceReadyTimer = new Timer(1000);
            DeviceReadyTimer.Elapsed += DeviceReadyTimerOnElapsed;
            ReadyForTest = true;

            EnvironmentalInfo = new EnvironmentalInfo();
            BreathManeuver = new BreathManeuver();
            DeviceInfo = new DeviceInfo();
            DebugMsg = new DebugMsg();
            DeviceStatusInfo = new DeviceStatusInfo();
            ErrorStatusInfo = new ErrorStatusInfo();

            DeviceSerialNumber = string.Empty;
            Firmware = string.Empty;
            FenomReady = true;

            DebugList = new RangeObservableCollection<DebugLog>();

            // write path to debug
            DebugList.Insert(0, DebugLog.Create("App Starting"));
            DebugList.Insert(0, DebugLog.Create(IOC.Services.DebugLogFile.GetFilePath()));
        }

        // Properties

        #region Properties

        public RangeObservableCollection<DebugLog> DebugList { get; set; }

        public bool FenomReady { get; set; }

        public abstract string Name { get; }

        public abstract Guid Id { get; }

        public abstract bool Connected { get; }

        public abstract Task<bool> StartTest(BreathTestEnum breathTestEnum);

        public abstract Task<bool> WRITEREQUEST(MESSAGE message, short idvar_size);

        public abstract Task<bool> StopTest();

        public abstract Task<bool> DEVICEINFO();

        public abstract Task<bool> CALIBRATION(ID_SUB iD_SUB, double cal1, double cal2, double cal3);

        public abstract Task<bool> DATETIME(string date, string time);

        public abstract Task<bool> SERIALNUMBER(string SerailNumber);

        public abstract Task<bool> MESSAGE(MESSAGE message);

        public abstract Task<bool> DEBUGMSG();

        public abstract Task<bool> ENVIROMENTALINFO();

        public abstract Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second);

        public abstract Task<bool> BREATHMANUEVER();

        public object NativeDevice { get => _nativeDevice; }
        public EnvironmentalInfo EnvironmentalInfo { get; set; }

        public DeviceInfo DeviceInfo { get; set; }

        public int NOScore { get; set; }

        public ErrorStatusInfo ErrorStatusInfo { get; set; }

        public DeviceStatusInfo DeviceStatusInfo { get; set; }

        public BreathManeuver BreathManeuver { get; set; }

        public DebugMsg DebugMsg { get; set; }

        public float FenomValue { get; set; }

        public int BatteryLevel { get; set; }

        private float _breathFlow { get; set; }
        public float BreathFlow
        {
            get => _breathFlow / 1000;
            set { _breathFlow = value; }
        }

        public string _deviceConnectedStatus = "Unknown";
        public string DeviceConnectedStatus
        {
            get => _deviceConnectedStatus;
            set
            {
                _deviceConnectedStatus = value;                
            }
        }

        public string _firmware;
        public string Firmware
        {
            get => _firmware;
            set
            {
                _firmware = value;
            }
        }

        public string _deviceSerialNumber;
        public string DeviceSerialNumber
        {
            get => _deviceSerialNumber;
            set
            {
                _deviceSerialNumber = value;
            }
        }

        public DateTime SensorExpireDate { get; set; }

        public bool BreathTestInProgress
        {
            get;
            set;
        }

        

        public IEnumerable<IService> GattServices { get; } = new SynchronizedList<IService>();

        public event EventHandler BreathFlowChanged;

        static volatile object _s_debugLogLock = new object();
        static volatile bool _s_logFileWriterWorkerStarted = false;
        static volatile bool _s_logFileWriterWorkerStop = false;

        #endregion

        // Methods

        #region Methods

        public abstract Task ConnectAsync();

        public abstract Task ConnectToKnownDeviceAsync(Guid Id);

        public abstract Task DisconnectAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bleDevice"></param>
        /// <param name="completed"></param>
        /// <returns></returns>
        public bool IsConnected(bool devicePowerOn = false)
        {
            return Connected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicePowerOn"></param>
        /// <returns></returns>
        public bool IsNotConnectedRedirect(bool devicePowerOn = false)
        {
            if (IsConnected(devicePowerOn))
            {
                return true;
            }

            //AppServices.Container.Resolve<INavigationService>().DevicePowerOnView();

            return false;
        }        

        public DeviceCheckEnum CheckDeviceBeforeTest()
        {
            // Get the latest environmental info - updates Cache
            //Services.DeviceService.Current?.RequestEnvironmentalInfo();

            if (ReadyForTest == false)
            {
                Console.WriteLine("Device is purging");
                return DeviceCheckEnum.DevicePurging;
            }

            if (EnvironmentalInfo.BatteryLevel < Constants.BatteryCritical3)
            {
                return DeviceCheckEnum.BatteryCriticallyLow;
            }

            if (EnvironmentalInfo.Humidity < Constants.HumidityLow18 ||
                EnvironmentalInfo.Humidity > Constants.HumidityHigh92)
            {
                return DeviceCheckEnum.HumidityOutOfRange;
            }

            if (EnvironmentalInfo.Pressure < Constants.PressureLow75 ||
                EnvironmentalInfo.Pressure > Constants.PressureHigh110)
            {
                return DeviceCheckEnum.PressureOutOfRange;
            }

            if (EnvironmentalInfo.Temperature < Constants.TemperatureLow14 ||
                EnvironmentalInfo.Temperature > Constants.TemperatureHigh35)
            {
                return DeviceCheckEnum.TemperatureOutOfRange;
            }

            if (BreathManeuver.StatusCode == 0x70)
            {
                return DeviceCheckEnum.NoSensorMissing;
            }

            return DeviceCheckEnum.Ready;
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
                    var debugList = DebugList;

                    lock (_s_debugLogLock)
                    {
                        Console.WriteLine(debugList);
                        debugList.Clear();
                    }

                    // reset the time
                    current = DateTime.Now;
                }
            }
        }

        #endregion

        #region Timer

        private bool _readyForTest;
        public int DeviceReadyCountDown { get; set; }

        public bool ReadyForTest
        {
            get => _readyForTest;
            set
            {
                _readyForTest = value;

                if (_readyForTest == false)
                {
                    DeviceReadyCountDown = 32;
                    DeviceReadyTimer.Start();
                }
            }
        }

        private readonly Timer DeviceReadyTimer;
        private void DeviceReadyTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            DeviceReadyCountDown -= 1;

            if (DeviceReadyCountDown <= 0)
            {
                ReadyForTest = true;
                DeviceReadyTimer.Stop();
            }
        }

        #endregion        

        #region Decode Characteristics

        public EnvironmentalInfo DecodeEnvironmentalInfo(byte[] data)
        {
            calls++;
            try
            {
                //data[0] = 20; // temp
                //data[1] = 60; // humidity
                //data[2] = 90; // pressure
                //data[3] = 80; // battery
                EnvironmentalInfo ??= new EnvironmentalInfo();
                EnvironmentalInfo.Decode(data);

                BatteryLevel = EnvironmentalInfo.BatteryLevel;
                
            }
            finally { }
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
                    Debug.WriteLine($"----> Device Serial Number: {DeviceSerialNumber} {DateTime.Now}");

                    // update the database
                    //Services.Database.QualityControlDevicesRepo.UpdateDateOrAdd(DeviceSerialNumber);
                }

                // setup firmware version
                Firmware = $"{DeviceInfo.MajorVersion}.{DeviceInfo.MinorVersion}";

                // get SensorExpireDate
                SensorExpireDate = new DateTime(DeviceInfo.SensorExpDateYear, DeviceInfo.SensorExpDateMonth, DeviceInfo.SensorExpDateDay);
                
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
                                
            }
            finally { }
            return DeviceStatusInfo;
        }

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

                    // add new value
                    BreathFlow = BreathManeuver.BreathFlow;
                    BreathFlowChanged?.Invoke(null, null);

                    // get the noscores
                    NOScore = BreathManeuver.NOScore;
                }
                                

            }
            finally { }
            return BreathManeuver;
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
                Console.WriteLine(ex);
            }

            return DebugMsg;
        }

        #endregion

       
        #region Dispose

        public void Dispose()
        {
            _s_logFileWriterWorkerStop = true;
        }

        #endregion

    }
}
