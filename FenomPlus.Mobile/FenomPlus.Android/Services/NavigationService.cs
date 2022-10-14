using System;
using Android.Content;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using Xamarin.Forms;
using static Xamarin.Essentials.Platform;
using Intent = Android.Content.Intent;

namespace FenomPlus.Droid.Services
{
    public class NavigationService : BaseService, INavigationService
    {
        public NavigationService(IAppServices services) : base(services)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        [Obsolete]
        public void GotoBluetoothSettings()
        {
            Intent bluetoothPicker = new Intent("android.bluetooth.devicepicker.action.LAUNCH");
            Forms.Context.StartActivity(bluetoothPicker);
        }
    }
}

