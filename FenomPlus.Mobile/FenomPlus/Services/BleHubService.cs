using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private static Thread ScanBleDeviceThread = null;

        public BleHubService(IAppServices services) : base(services)
        {
            if (ScanBleDeviceThread == null)
            {
                ScanBleDeviceThread = new Thread(new ThreadStart(ScanBleDevices));
                ScanBleDeviceThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IBleDevice BleDevice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private IFenomHubSystemDiscovery fenomHubSystemDiscovery;
        protected  IFenomHubSystemDiscovery FenomHubSystemDiscovery
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

        public void ScanBleDevices()
        {
            Connecting = false;
            //IsScanning = false;
            while (1 == 1)
            {
                if (((BleDevice == null) || (BleDevice.Connected == false)) && (FenomHubSystemDiscovery.IsScanning == false))
                {
                    Services.Cache.DeviceConnectedStatus = "Scanning...";
                    Services.Cache.DeviceSerialNumber = "";
                    Services.Cache.Firmware = "";
                    _ = Scan(new TimeSpan(0, 0, 0, 30), false, true, async (IBleDevice bleDevice) =>
                    {
                        if ((bleDevice == null) || string.IsNullOrEmpty(bleDevice.Name) || (Connecting == true)) return;
                        Connecting = true;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (await Connect(bleDevice) != false)
                            {
                                Thread.Sleep(1000);

                                Services.Cache.DeviceConnectedStatus = "Initializing...";
                                Services.Cache._DeviceInfo = null;
                                await RequestDeviceInfo();
                                Thread.Sleep(1000);

                                Services.Cache._EnvironmentalInfo = null;
                                await RequestEnvironmentalInfo();
                                Thread.Sleep(1000);
                                Services.Cache.DeviceConnectedStatus = "Device Connected";

                                await StopScan();

                            }
                            // need to restart the scanning
                            Connecting = false;
                        });
                    }, (IEnumerable<IBleDevice> bleDevices) =>
                    {
                        //Services.Cache.DeviceConnectedStatus = "Device Not Found";
                        Thread.Sleep(1000);
                        Connecting = false;
                    });
                    Thread.Sleep(1000);
                } else {
                    Thread.Sleep(100);
                }
            }
        }


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
                // if disconnected try to re-conenct
                if((BleDevice.Connected == false) && (devicePowerOn == false))
                {
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
            Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(DevicePowerOnView)}"), false);
            return false;
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
        /// <param name="SerailNumber"></param>
        /// <returns></returns>
        public async Task<bool> SendSerailNumber(string SerailNumber)
        {
            if (IsConnected())
            {
                return await BleDevice.SERIALNUMBER(SerailNumber);
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
