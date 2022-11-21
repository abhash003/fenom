using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using FenomPlus.Interfaces;
using FenomPlus.SDK.Abstractions;
using FenomPlus.SDK.Core;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Views;
using Xamarin.Forms;

namespace FenomPlus.Services
{
    public class BleHubService : BaseService, IBleHubService
    {
        private readonly Timer DeviceReadyTimer;
        readonly IDialogService DialogService;


        public BleHubService(IAppServices services) : base(services)
        {
            DeviceReadyTimer = new Timer(1000);
            DeviceReadyTimer.Elapsed += DeviceReadyTimerOnElapsed;

            DialogService = Container.Resolve<IDialogService>();

            ReadyForTest = true;
        }

        public int DeviceReadyCountDown { get; set; }

        private bool _readyForTest;
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

        private void DeviceReadyTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            DeviceReadyCountDown -= 1;

            if (DeviceReadyCountDown <= 0)
            {
                ReadyForTest = true;
                DeviceReadyTimer.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IBleDevice BleDevice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private IFenomHubSystemDiscovery fenomHubSystemDiscovery;
        public  IFenomHubSystemDiscovery FenomHubSystemDiscovery
        {
            get
            {
                if (fenomHubSystemDiscovery == null)
                {
                    fenomHubSystemDiscovery = new FenomHubSystemDiscovery();
                    fenomHubSystemDiscovery.SetLoggerFactory(Services.Cache.Logger);
                }
                return fenomHubSystemDiscovery;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Connecting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopScan()
        {
            return await FenomHubSystemDiscovery.StopScan();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanTime"></param>
        /// <param name="deviceFoundCallback"></param>
        /// <param name="scanCompletedCallback"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IFenomHubSystem>> Scan(TimeSpan scanTime = default, bool scanBondedDevices = true, bool scanBleDevices = true, Action<IBleDevice> deviceFoundCallback = null, Action<IEnumerable<IBleDevice>> scanCompletedCallback = null)
        {
            return await FenomHubSystemDiscovery.Scan(scanTime, scanBondedDevices, scanBleDevices, deviceFoundCallback, scanCompletedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bleDevice"></param>
        /// <returns></returns>
        public async Task<bool> Connect(IBleDevice bleDevice)
        {
            await Disconnect();
            BleDevice = bleDevice;
            return await bleDevice.ConnectAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bleDevice"></param>
        /// <returns></returns>
        public async Task<bool> Disconnect()
        {
            if (BleDevice != null)
            {
                if (BleDevice.Connected == true)
                {
                    await BleDevice.DisconnectAsync();
                    BleDevice = null;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bleDevice"></param>
        /// <param name="completed"></param>
        /// <returns></returns>
        public bool IsConnected(bool devicePowerOn = false)
        {
            // do we have a device
            if(BleDevice != null)
            {
                // if disconnected try to re-connect
                if((BleDevice.Connected == false) && (devicePowerOn == false))
                {
                    Debug.WriteLine("Error: Trying to reconnect...");
                    
                    //DialogService.ShowToast("Error: Trying to reconnect...", 4);
                    // try to connect
                    BleDevice.ConnectAsync();
                }

                return BleDevice.Connected;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicePowerOn"></param>
        /// <returns></returns>
        public bool IsNotConnectedRedirect(bool devicePowerOn = false)
        {
            //return true;
            if(IsConnected(devicePowerOn)) {
                return true;
            }
            Services.Navigation.DevicePowerOnView();
            return false;
        }

        //public async Task<bool> ReadyForTest()
        //{
        //    if (IsConnected())
        //    {
        //        Stopwatch stopwatch = new Stopwatch();
        //        stopwatch.Start();

        //        await Services.BleHub.RequestDeviceInfo();

        //        var status = Services.Cache.DeviceInfo.DeviceStatus;


        //        //_ = await BleDevice.BREATHTEST(BreathTestEnum.Start6Second);
        //        //bool isReady = Services.Cache.FenomReady;
        //        //_ = await BleDevice.BREATHTEST(BreathTestEnum.Stop);

        //        stopwatch.Stop();
        //        var milliseconds = stopwatch.ElapsedMilliseconds;

        //        bool isReady = true;
        //        return isReady;
        //    }

        //    return false;
        //}


        public bool BreathTestInProgress
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="breathTestEnum"></param>
        /// <returns></returns>
        public async Task<bool> StartTest(BreathTestEnum breathTestEnum)
        {
            if(IsConnected())
            {
                BreathTestInProgress = true;
                return await BleDevice.BREATHTEST(breathTestEnum);
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
                return await BleDevice.BREATHTEST(BreathTestEnum.Stop);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RequestDeviceInfo()
        {
            if (IsConnected())
            {
                return await BleDevice.DEVICEINFO();
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
                return await BleDevice.ENVIROMENTALINFO();
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> SendMessage(MESSAGE message)
        {
            if (IsConnected())
            {
                return await BleDevice.MESSAGE(message);
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
                return await BleDevice.SERIALNUMBER(serialNumber);
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
                return await BleDevice.DATETIME(date, time);
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
                return await BleDevice.CALIBRATION(iD_SUB, cal1, cal2, cal3);
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
    }
}
