#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using PluginBleIDevice = Plugin.BLE.Abstractions.Contracts.IDevice;
using IDevice = FenomPlus.Services.DeviceService.Interfaces.IDevice;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;
using FenomPlus.SDK.Core.Ble.PluginBLE;
using FenomPlus.SDK.Core.Utils;
using FenomPlus.Services.DeviceService.Utils;
using System.Timers;
using FenomPlus.Services.DeviceService.Enums;
using System.Threading;
using FenomPlus.Models;
using Timer = System.Timers.Timer;
using FenomPlus.Helpers;
using System.Text;
using System.Diagnostics;

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
                NotifyViews();
                NotifyViewModels();
            }
        }

        public string _firmware;
        public string Firmware
        {
            get => _firmware;
            set
            {
                _firmware = value;
                NotifyViews();
                NotifyViewModels();
            }
        }

        public string _deviceSerialNumber;
        public string DeviceSerialNumber
        {
            get => _deviceSerialNumber;
            set
            {
                _deviceSerialNumber = value;
                NotifyViews();
                NotifyViewModels();
            }
        }

        public DateTime SensorExpireDate { get; set; }

        public bool BreathTestInProgress
        {
            get;
            set;
        }

        public IEnumerable<IGattCharacteristic> GattCharacteristics { get; } = new SynchronizedList<IGattCharacteristic>();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="breathTestEnum"></param>
        /// <returns></returns>
        public async Task<bool> StartTest(BreathTestEnum breathTestEnum)
        {
            if (IsConnected())
            {
                BreathTestInProgress = true;
                return await BREATHTEST(breathTestEnum);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopTest()
        {
            if (IsConnected())
            {
                BreathTestInProgress = false;
                return await BREATHTEST(BreathTestEnum.Stop);
            }
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

            if (false /*Services.Cache.EnvironmentalInfo.BatteryLevel < Constants.BatteryCritical3*/)
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

                NotifyViews();
                NotifyViewModels();
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
                    Debug.WriteLine($"----> Device Serial Number: {DeviceSerialNumber}");

                    // update the database
                    //Services.Database.QualityControlDevicesRepo.UpdateDateOrAdd(DeviceSerialNumber);
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

                NotifyViews();

                NotifyViewModels();

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

        #region Notify View and View Models
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

        #endregion

        #region Request Characteristics

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RequestDeviceInfo()
        {
            if (IsConnected())
            {
                return await DEVICEINFO();
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RequestEnvironmentalInfo()
        {
            if (IsConnected())
            {
                return await ENVIROMENTALINFO();
            }
            return false;
        }

        #endregion

        #region Send Characteristics

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> SendMessage(MESSAGE message)
        {
            if (IsConnected())
            {
                return await MESSAGE(message);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public async Task<bool> SendSerialNumber(string serialNumber)
        {
            if (IsConnected())
            {
                return await SERIALNUMBER(serialNumber);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<bool> SendDateTime(string date, string time)
        {
            if (IsConnected())
            {
                return await DATETIME(date, time);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iD_SUB"></param>
        /// <param name="cal1"></param>
        /// <returns></returns>
        public async Task<bool> SendCalibration(ID_SUB iD_SUB, double cal1, double cal2, double cal3)
        {
            if (IsConnected())
            {
                return await CALIBRATION(iD_SUB, cal1, cal2, cal3);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cal1"></param>
        /// <param name="cal2"></param>
        /// <param name="cal3"></param>
        /// <returns></returns>
        public async Task<bool> SendCalibration(double cal1, double cal2, double cal3)
        {
            bool result = true;
            await SendCalibration(ID_SUB.ID_CALIBRATION1, cal1, cal2, cal3);
            return result;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _s_logFileWriterWorkerStop = true;
        }

        #endregion
    }

    public partial class Device
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DEVICEINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEVICEINFO);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ENVIROMENTALINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_ENVIROMENTALINFO);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_BREATHMANUEVER, (byte)breathTestEnum);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BREATHMANUEVER()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_BREATHMANUEVER);
            return await WRITEREQUEST(message, 1);
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public async Task<bool> TRAININGMODE()
        //{
        //    MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_TRAININGMODE);
        //    return await WRITEREQUEST(message, 1);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DEBUGMSG()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEBUGMSG);
            return await WRITEREQUEST(message, 1);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public async Task<bool> DEBUGMANUEVERTYPE()
        //{
        //    MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEBUGMANUEVERTYPE);
        //    return await WRITEREQUEST(message, 1);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> MESSAGE(MESSAGE message)
        {
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerailNumber"></param>
        /// <returns></returns>
        public async Task<bool> SERIALNUMBER(string SerailNumber)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_SERIALNUMBER, SerailNumber);
            return await WRITEREQUEST(message, 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<bool> DATETIME(string date, string time)
        {
            string strDateTime;

            strDateTime = date + "T" + time;

            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_DATETIME, strDateTime);
            return await WRITEREQUEST(message, (short)strDateTime.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iD_SUB"></param>
        /// <param name="cal"></param>
        /// <returns></returns>
        public async Task<bool> CALIBRATION(ID_SUB iD_SUB, double cal1, double cal2, double cal3)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_CALIBRATION_DATA, iD_SUB, cal1, cal2, cal3);
            return await WRITEREQUEST(message, 24);
        }

        public Plugin.BLE.Abstractions.Contracts.ICharacteristic FwCharacteristic = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> WRITEREQUEST(MESSAGE message, short idvar_size)
        {
            using (var tracer = new Helper.FunctionTrace())
            {
                byte[] data = new byte[2 + 2 + idvar_size];

                data[0] = (byte)(message.IDMSG >> 8);
                data[1] = (byte)message.IDMSG;
                data[2] = (byte)(message.IDSUB >> 8);
                data[3] = (byte)message.IDSUB;

                Buffer.BlockCopy(message.IDVAR, 0, data, 4, idvar_size);

                if (true)
                {
                    // get service
                    var device = (PluginBleIDevice)_nativeDevice;

                    var service = await device.GetServiceAsync(new Guid(FenomPlus.SDK.Core.Constants.FenomService));

                    // get characteristics
                    var fwChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                        .FeatureWriteCharacteristic));

                    fwChar.WriteType = Plugin.BLE.Abstractions.CharacteristicWriteType.WithoutResponse;
                    bool result = await fwChar.WriteAsync(data);

                    if (result == true)
                    {
                        tracer.Trace("write without response okay");
                    }
                    else
                    {
                        tracer.Trace("something went wrong");
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<IGattCharacteristic> FindCharacteristic(string uuid)
        {

            Guid guid = new Guid(uuid);
            IGattCharacteristic gatt = null;

            var gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;
            if (gattCharacteristics.Count <= 0)
            {
                _ = await GetCharacterasticsAync();
                gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;
            }
            foreach (IGattCharacteristic item in new List<IGattCharacteristic>(gattCharacteristics))
            {
                if (!item.Uuid.Equals(guid)) continue;
                gatt = item;
                break;
            }

            return gatt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGattCharacteristic>> GetCharacterasticsAync()
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(BleDevice), "GetCharacterasticsAync");

                var gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;

                if (gattCharacteristics == null)
                {
                    //_logger.LogWarning("BleDevice.GetCharacteristicsAsync() - list is null");
                    return null;
                }

                gattCharacteristics.Clear();


                var gattService = GattServices as SynchronizedList<IService>;

                var services = await ((PluginBleIDevice)_nativeDevice).GetServicesAsync();

                foreach (var service in services)
                {
                    // add service here
                    gattService.Add(service);

                    var characteristics = await service.GetCharacteristicsAsync();
                    foreach (var characteristic in characteristics)
                    {
                        IGattCharacteristic gattCharacteristic = new GattCharacteristic(characteristic);
                        gattCharacteristics.Add(gattCharacteristic);
                    }
                }

                return gattCharacteristics;
            }
            catch (Exception ex)
            {
                Helper.WriteDebug(ex);
                return null;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleDevice), "GetCharacterasticsAync");
            }
        }
    }
}
