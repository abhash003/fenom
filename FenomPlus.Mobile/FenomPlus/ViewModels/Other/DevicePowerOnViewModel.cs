using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.Views;
using Plugin.BLE.Abstractions;
using Xamarin.Forms;
using static FenomPlus.SDK.Core.FenomHubSystemDiscovery;

namespace FenomPlus.ViewModels
{
    public class DevicePowerOnViewModel : BaseViewModel
    {

        private bool Stop;

        
        /// <summary>
        /// 
        /// </summary>
        public void StopScan()
        {
            Services.BleHub.StopScan();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartScan()
        {
            Seconds = 30;
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);
            _ = Services.BleHub.Scan(new TimeSpan(0, 0, 0, Seconds), true, false, async (IBleDevice bleDevice) =>
                        {
                if ((bleDevice == null) || string.IsNullOrEmpty(bleDevice.Name)) return;
                await Services.BleHub.StopScan();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await Services.BleHub.Connect(bleDevice) == false) return;
                    await FoundDevice(bleDevice);
                });

            }, (IEnumerable<IBleDevice> bleDevices) =>
            {

            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bleDevice"></param>
        public async Task FoundDevice(IBleDevice bleDevice)
        {
            Stop = true;
            Services.Cache.DeviceInfo = null;
            await Services.BleHub.RequestDeviceInfo();
            Device.StartTimer(TimeSpan.FromMilliseconds(200), DeviceInfoTimer);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DeviceInfoTimer()
        {
            if (Services.Cache.DeviceInfo == null) return true;
            Services.Cache.EnvironmentalInfo = null;
            // jac: do not request, this is updated by the device
            //Services.BleHub.RequestEnvironmentalInfo();
            Device.StartTimer(TimeSpan.FromMilliseconds(200), EnvironmentalInfo);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool EnvironmentalInfo()
        {
            if (Services.Cache.EnvironmentalInfo == null) return true;
            //Services.Navigation.DashboardView();
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool TimerCallback()
        {
            Seconds--;
            if (Seconds <= 0)
            {
                _ = Services.BleHub.Disconnect();
                StopScan();
                StartScan();
                return false;
            }

            return ((Seconds >= 0) && (Stop == false));
        }

        /// <summary>
        /// 
        /// </summary>
        private int seconds;
        public int Seconds
        {
            get => seconds;
            set
            {
                seconds = value;
                Message = string.Format("Scanning for Device Please Wait {0} seconds...", seconds);
                OnPropertyChanged("Seconds");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            Stop = false;
            StopScan();
            StartScan();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            Stop = true;
            StopScan();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
