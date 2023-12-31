﻿#region Operational Defines

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
using FenomPlus.ViewModels.QualityControl.Models;
using Xamarin.Forms;

namespace FenomPlus.Services.DeviceService.Abstract
{
    public abstract partial class Device : IDevice, IDisposable
    {
        // Fields

        protected object _nativeDevice = null;

        // Constructor

        public Device(object nativeDevice)
        {
            _nativeDevice = nativeDevice;

            /*
             * LEGACY CODE
             */
            DeviceReadyTimer = new Timer(1000);
            DeviceReadyTimer.Elapsed += DeviceReadyTimerOnElapsed;
            ReadyForTest = false;

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


            // start the log cleaner 
            LogCleaningTimer = new Timer(60_000);
            LogCleaningTimer.Elapsed += LogCleaningTimerOnElapsed;
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

        public abstract Task<bool> WriteRequest(MESSAGE message, short sz);

        public abstract Task<bool> ENVIROMENTALINFO();

        public abstract Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second);

        public abstract Task<bool> BREATHMANUEVER();

        public object NativeDevice { get => _nativeDevice; }
        public EnvironmentalInfo EnvironmentalInfo { get; set; }

        public DeviceInfo DeviceInfo { get; set; }

        public int? NOScore { get; set; }

        public ErrorStatusInfo ErrorStatusInfo { get; set; }

        public DeviceStatusInfo DeviceStatusInfo { get; set; }

        public BreathManeuver BreathManeuver { get; set; }
        public byte LastStatusCode { get; set; }
        public int LastErrorCode { get; set; }

        public DebugMsg DebugMsg { get; set; }

        public float? FenomValue { get; set; }

        public int BatteryLevel { get; set; }

        public int? DeviceLifeRemaining { get; set; }

        private float _breathFlow { get; set; }
        public float BreathFlow
        {
            get => _breathFlow;
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

        private static object _s_debugLogLock = new object();

        #endregion

        // Methods

        #region Methods

        public abstract Task ConnectAsync();

        public abstract Task ConnectToKnownDeviceAsync(Guid Id);

        public abstract Task DisconnectAsync();

        public abstract Task<bool> RequestDeviceInfo();
        public abstract Task<bool> RequestEnvironmentalInfo();

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

        public DeviceCheckEnum CheckDeviceBeforeTest(bool isQCCheck = false)
        {
            // Get the latest environmental info - updates Cache
            //Services.DeviceService.Current?.RequestEnvironmentalInfo();

            if (ReadyForTest == false)
            {
                Console.WriteLine("Device is purging");
                return DeviceCheckEnum.DevicePurging;
            }

            // battery lockout

            // 0x4a -- not charging
            // 0x4b -- charging
            // 0x00 -- unknown
            // If battery is critical but is charging then dont raise the error
            
            if ((EnvironmentalInfo.BatteryLevel < Constants.BatteryCritical3) && (DeviceStatusInfo.StatusCode == 0x4a))
            {
                return DeviceCheckEnum.BatteryCriticallyLow;
            }

            if (isQCCheck)
            {
                if (!IsQCEnabled())
                {
                    return DeviceCheckEnum.QCDisabled;
                }
            }

            // environmental lockouts
            bool appEnvironmentalLockouts = false;

            if (appEnvironmentalLockouts)
            {
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
            }
            else
            {
                switch (ErrorStatusInfo.ErrorCode)
                {
                    case 0x00:
                        break;
                    
                    // Decoded byte for 0x70 is 112 => NO Sensor Missing.
                    case 0x70:
                        return DeviceCheckEnum.NoSensorMissing;

                    // Decoded byte for 0x71 is 113 => No Sensor Communication Failed.
                    case 0x71:
                        return DeviceCheckEnum.NoSensorCommunicationFailed;

                    case 0x81:
                        return DeviceCheckEnum.ERROR_SYSTEM_NEGATIVE_QC_FAILED;

                    default:
                        return DeviceCheckEnum.Unknown;
                }
            }

            return DeviceCheckEnum.Ready;
        }
        #endregion

        #region Timer
        private readonly Timer LogCleaningTimer;
        private void LogCleaningTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_s_debugLogLock)
            {
                Console.WriteLine(DebugList);
                DebugList.Clear();
            }
        }

        private bool _readyForTest;
        public int DeviceReadyCountDown { get; set; }

        public bool ReadyForTest
        {
            get => _readyForTest;
            set
            {
                if (value == _readyForTest) { return; }

                // it already set as 'not ready for test',during the count down time, cannot revoke it to 'ready for test' 
                if (value == true && DeviceReadyCountDown > 0)  
                {
                    return;
                }
                if ((_readyForTest = value) == false)
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

        public void DecodeEnvironmentalInfo(byte[] data)
        {
            try
            {
                EnvironmentalInfo ??= new EnvironmentalInfo();
                EnvironmentalInfo.Decode(data);
            }

            catch (Exception ex)
            {
                throw; // bubble up
            }

            return;
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
                Firmware = $"{DeviceInfo.FirmwareVersionMajor}.{DeviceInfo.FirmwareVersionMinor}";

                // get SensorExpireDate
                SensorExpireDate = new DateTime(DeviceInfo.SensorExpDateYear, DeviceInfo.SensorExpDateMonth, DeviceInfo.SensorExpDateDay);

                DeviceLifeRemaining = DeviceInfo.DeviceDaysRemaining;

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

                    if (FenomValue!= null)
                    {
                        // Only read the score when receiving the special code "0xFE" meaning "score ready" in the 'time remaining' field.
                        MessagingCenter.Send(this, "NOScore", FenomValue.ToString());
                    }
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
            DeviceReadyTimer.Dispose();
            LogCleaningTimer.Dispose();
        }

        #endregion


        #region Device Communication

        public abstract Task<bool> SendMessage(MESSAGE message, short sz = 1);

        #endregion

        #region Quality Control (QC)

        public async Task<bool> ToggleQC(bool Enable = true)
        {
            MESSAGE msg = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_QUALITYCONTROL,  (byte)(Enable ? 1 : 0));
            return await SendMessage(msg);
        }

        public bool IsQCEnabled()
        {
            return DeviceInfo.IsQcEnabled;
        }

        public string GetDeviceQCStatus()
        {
            if (!IsQCEnabled())
            {
                return "Disabled";
            }
            string _currentDeviceStatus = string.Empty;
            short hour = 0;
            RequestDeviceInfo().GetAwaiter();
            bool? result = GetQCHoursRemaining(ref hour);
            if (result != null)
            {
                _currentDeviceStatus = result == true ? QCDevice.DeviceValid : (hour == unchecked((short)0x8000) ? QCDevice.DeviceFail : QCDevice.DeviceExpired);
            }

            return _currentDeviceStatus;
        }

        public bool GetQCHoursRemaining(ref short hour)
        {
            //hour (type short) >=0:valid, <0:expired, (<0 and ==0x8000):failed
            hour = (short)DeviceInfo.QcValidity;
            return hour >= 0;
        }

        public bool ExtendQC(int hours)
        {
            return true;
        }

        public async Task<bool> ExtendDeviceValidity(short hour)
        {
            var msg = new MESSAGE(ID_MESSAGE.ID_CALIBRATION_DATA, ID_SUB.ID_REQUEST_QUALITYCONTROL, hour);
            return await SendMessage(msg, 2);
        }
        public async Task<bool> SendFailMsg(ushort val)
        {
            var msg = new MESSAGE(ID_MESSAGE.ID_CALIBRATION_DATA, ID_SUB.ID_REQUEST_QUALITYCONTROL, val);
            return await SendMessage(msg, 2);
        }
        #endregion
    }
}
