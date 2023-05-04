using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.Services;
using FenomPlus.Services.DeviceService;
using FenomPlus.Services.DeviceService.Concrete;
using FenomPlus.Services.DeviceService.Interfaces;
using FenomPlus.Services.DeviceService.Utils;
using FenomPlus.Views;
using Plugin.BLE.Abstractions;
using Xamarin.Forms;

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
        }

        void DeviceConnectedHandler(object sender, EventArgs e)
        {
            Helper.WriteDebug("Device connected.");
            //AppServices.Container.Resolve<StatusViewModel>().OnConnected(true);
            // if we lose the connection, start scanning again ...
            //Stop = false;
            //StopScan();
            //StartScan();
        }

        void DeviceConnectionLostHandler(object sender, EventArgs e)
        {
            Helper.WriteDebug("Device connection lost.");
            //AppServices.Container.Resolve<StatusViewModel>().OnConnected(false);
            // if we lose the connection, start scanning again ...
            //Stop = false;
            //StopScan();
            //StartScan();
            Services.Navigation.DevicePowerOnView();
        }


        private void WireEventHandlers()
        {
            Services.DeviceService.DeviceConnected += DeviceConnectedHandler;
            Services.DeviceService.DeviceConnectionLost += DeviceConnectionLostHandler;
        }

        private void UnwireEventHandlers()
        {
            Services.DeviceService.DeviceConnected -= DeviceConnectedHandler;
            Services.DeviceService.DeviceConnectionLost -= DeviceConnectionLostHandler;
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
            Seconds = 10;
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);
            Services.DeviceService.StartDiscovery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeviceInfoTimer()
        {            
            if (Services.DeviceService.Current == null) return true;
            if (Services.DeviceService.Current.DeviceInfo == null) return true;
            Services.DeviceService.Current.EnvironmentalInfo = new SDK.Core.Models.EnvironmentalInfo();
            //Services.Cache.EnvironmentalInfo = null;
            // jac: do not request, this is updated by the device            

            await Services.DeviceService.Current.RequestEnvironmentalInfo();
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(200), EnvironmentalInfo);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool EnvironmentalInfo()
        {            
            if (Services.DeviceService.Current == null) return true;
            if (Services.DeviceService.Current.EnvironmentalInfo == null) return true;
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
                StopScan();
                MessagingCenter.Send(this, "DeviceNotFound");
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
            WireEventHandlers();

            if (Services.DeviceService.Current != null)
            {
                //FoundDevice(Services.DeviceService.Current);
                Services.Navigation.DashboardView();
            }

            Stop = false;
            StopScan();
            StartScan();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            UnwireEventHandlers();

            //if (Stop == false && Services.DeviceService.Discovering)
            {
                Stop = true;
                StopScan();
            }
        }
        
    }
}