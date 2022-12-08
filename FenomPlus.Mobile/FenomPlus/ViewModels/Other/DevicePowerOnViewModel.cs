using FenomPlus.SDK.Core.Ble.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;
using FenomPlus.SDK.Core.Ble.PluginBLE;

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
            Message = string.Format("Scanning for Device Please Wait...", value);
        }

        [ObservableProperty]
        private string _message;

        public DevicePowerOnViewModel()
        {
            //    WeakReferenceMessenger.Default.Register<DeviceConnectedMessage>(this, (r, m) =>
            //    {
            //        // Handle the message here, with r being the recipient and m being the
            //        // input message. Using the recipient passed as input makes it so that
            //        // the lambda expression doesn't capture "this", improving performance.

            //        // Do not use value, may have changed already
            //        bool isConnected = (bool)m.Value;

            //        if (App.GetCurrentPage() is DevicePowerOnView && isConnected)  // ToDo: Only needed because viewmodels never die
            //        {
            //            // Only navigate if during startup
            //            Services.Navigation.DashboardView();
            //        }

            //    });
        }

        public void StopScan()
        {
            Services.BleHub.StopScan();
        }

        public void StartScan()
        {

            Device.StartTimer(TimeSpan.FromSeconds(1), ScanTimerCallback);

            Seconds = 30;
            TimeSpan timeSpan = new TimeSpan(0,0,0,Seconds);
            BleHub.Scan(timeSpan, true, true, DeviceFoundCallback, (IEnumerable<IBleDevice> bleDevices) =>
            {

            });
        }

        private async void DeviceFoundCallback(IBleDevice bleDevice)
        {
            if ((bleDevice == null) || string.IsNullOrEmpty(bleDevice.Name)) return;

            _ = await BleHub.StopScan();

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await Services.BleHub.Connect(bleDevice) == false)
                {
                    return;
                }

                await FoundDevice(bleDevice);
            });
        }

        private bool ScanTimerCallback()
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

        //private Task async DeviceFoundCallback()
        //{
        //    if ((bleDevice == null) || string.IsNullOrEmpty(bleDevice.Name)) return;
        //    await BleHub.StopScan();
        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        if (await Services.BleHub.Connect(bleDevice) == false) return;
        //        await FoundDevice(bleDevice);
        //    });
        //}

        public async Task FoundDevice(IBleDevice bleDevice)
        {
            Stop = true;
            Cache.DeviceInfo = null;
            await Services.BleHub.RequestDeviceInfo();

            Device.StartTimer(TimeSpan.FromMilliseconds(200), DeviceInfoTimer);

        }

        public bool DeviceInfoTimer()
        {
            if (Cache.DeviceInfo == null) return true;
            Cache.EnvironmentalInfo = null;
            Services.BleHub.RequestEnvironmentalInfo();
            Device.StartTimer(TimeSpan.FromMilliseconds(200), EnvironmentalInfo);
            return false;
        }

        public bool EnvironmentalInfo()
        {
            if (Cache.EnvironmentalInfo == null) return true;
            //Services.Navigation.DashboardView();
            return false;
        }

        public override void OnAppearing()
        {
            Stop = false;
            StopScan();
            StartScan();
        }

        public override void OnDisappearing()
        {
            Stop = true;
            StopScan();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
