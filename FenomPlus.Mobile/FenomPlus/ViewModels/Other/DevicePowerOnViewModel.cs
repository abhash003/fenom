using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.Services;
using FenomPlus.Services.NewArch.R2;
using FenomPlus.Views;
using Plugin.BLE.Abstractions;
using Xamarin.Forms;
using static FenomPlus.SDK.Core.FenomHubSystemDiscovery;

namespace FenomPlus.ViewModels
{
    public partial class DevicePowerOnViewModel : BaseViewModel
    {

        private bool Stop;
		
		[ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Message))]
        private int _seconds;

        partial void OnSecondsChanged(int value)
        {
            Message = string.Format("Scanning for Device Please Wait... {0}", value);
        }

        public DevicePowerOnViewModel()
        {
            Services.DeviceService.DeviceConnected += (object sender, EventArgs e) =>
            {
                AppServices.Container.Resolve<StatusViewModel>().OnConnected(true);
                // if we lose the connection, start scanning again ...
                //Stop = false;
                //StopScan();
                //StartScan();
            };

            Services.DeviceService.DeviceConnectionLost += (object sender, EventArgs e) =>
            {
                AppServices.Container.Resolve<StatusViewModel>().OnConnected(false);
                // if we lose the connection, start scanning again ...
                Stop = false;
                StopScan();
                StartScan();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopScan()
        {
            Services.DeviceService.StopDiscovery();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartScan()
        {
            Seconds = 30;
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);

            Services.DeviceService.StartDiscovery(async (FenomPlus.Services.NewArch.R2.IDevice device) =>
            {
                try
                {
                    if (device.Name != null)
                    {
                        if (device.Name.ToLower().Contains("fenom") || device.Name.ToLower().StartsWith("fp"))
                        {
                            Stop = true;
                            await device.ConnectAsync();
                            FoundDevice(device);
                        }
                    }

                    Console.WriteLine("id: {0} name: {1}", device.Id, (device.Name != null) ? device.Name : "<null>");
                }

                catch(Exception ex)
                {
                    Console.WriteLine("exception: {0}", ex.Message);
                }
            });

#if false
            _ = Services.DeviceService.Scan(new TimeSpan(0, 0, 0, Seconds), true, false, async (IBleDevice bleDevice) =>
                        {
                if ((bleDevice == null) || string.IsNullOrEmpty(bleDevice.Name)) return;
                await Services.DeviceService.StopScan();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await Services.DeviceService.Connect(bleDevice) == false) return;
                    await FoundDevice(bleDevice);
                });

            }, (IEnumerable<IBleDevice> bleDevices) =>
            {
				await Services.Navigation.DashboardView();
				await Services.Dialogs.ShowAlertAsync("Bluetooth could not connect to device", 
                        "No Device found",
                        "Exit");

            });
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bleDevice"></param>
        public async Task FoundDevice(IBleDevice bleDevice)
        {
            Stop = true;
            Services.Cache.DeviceInfo = new SDK.Core.Models.DeviceInfo();
            Services.Cache.DeviceInfo = null;
            // jac: do not request, this is updated by the device
            await Services.DeviceService.Current.RequestDeviceInfo();
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(200), DeviceInfoTimer);
        }

        public void FoundDevice(IDevice device)
        {
            Stop = true;
            Services.Cache.DeviceInfo = new SDK.Core.Models.DeviceInfo();
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DeviceInfoTimer()
        {
            if (Services.Cache.DeviceInfo == null) return true;
            Services.Cache.EnvironmentalInfo = new SDK.Core.Models.EnvironmentalInfo();
            //Services.Cache.EnvironmentalInfo = null;
            // jac: do not request, this is updated by the device
            Services.DeviceService.Current.RequestEnvironmentalInfo();
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(200), EnvironmentalInfo);
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
                //_ = Services.DeviceService.Current.DisconnectAsync();
                StopScan();
                StartScan();
                return false;
            }

            return ((Seconds >= 0) && (Stop == false));
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