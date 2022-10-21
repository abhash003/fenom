using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.Views;
using Xamarin.Forms;

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
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool TimerCallback()
        {
            bool connected = Services.BleHub.IsConnected(true);
            if (connected == true)
            {
                Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(DeviceReadyView)}"), false);
            }
            return ((connected == false) && (Stop == false));
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            Stop = false;
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
        {
            Stop = true;
        }

        /// <summary>
        /// 
        /// </summary>
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
