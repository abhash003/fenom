using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.Views;
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
        public DevicePowerOnViewModel()
        {
            WeakReferenceMessenger.Default.Register<DeviceConnectedMessage>(this, (r, m) =>
            {
                // Handle the message here, with r being the recipient and m being the
                // input message. Using the recipient passed as input makes it so that
                // the lambda expression doesn't capture "this", improving performance.

                bool isConnected = (bool)m.Value;

                if (isConnected && Services.BleHub.BleDevice.Connected) // Todo: We shouldn't need both but trying to resolve weak connections
                {
                    Debug.WriteLine("********* Device Connected!");

                    if (App.GetCurrentPage() is DevicePowerOnView)  // ToDo: Only needed because vie wmodels never die
                    {
                        // Only navigate if during startup
                        Services.Navigation.ChooseTestView();
                    }

                }
                else
                {
                    Debug.WriteLine("********* Device Disconnected!");
                }

            });
        }

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
            _ = BleHub.Scan(new TimeSpan(0, 0, 0, Seconds), true, true, async (IBleDevice bleDevice) =>
                        {
                if ((bleDevice == null) || string.IsNullOrEmpty(bleDevice.Name)) return;
                await BleHub.StopScan();
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
            Cache.DeviceInfo = null;
            await Services.BleHub.RequestDeviceInfo();
            Device.StartTimer(TimeSpan.FromMilliseconds(200), DeviceInfoTimer);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DeviceInfoTimer()
        {
            if (Cache.DeviceInfo == null) return true;
            Cache.EnvironmentalInfo = null;
            Services.BleHub.RequestEnvironmentalInfo();
            Device.StartTimer(TimeSpan.FromMilliseconds(200), EnvironmentalInfo);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool EnvironmentalInfo()
        {
            if (Cache.EnvironmentalInfo == null) return true;
            //Services.Navigation.ChooseTestView();
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
                Message = string.Format("Scanning for Device Please Wait...", seconds);
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
