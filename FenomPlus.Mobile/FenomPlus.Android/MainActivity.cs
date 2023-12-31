﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using FenomPlus.Services;
using FenomPlus.Interfaces;
using System.Linq;
using Android;
using Acr.UserDialogs;
using TinySvgHelper;
using System;
using FenomPlus.Services.DeviceService;
using FenomPlus.Services.DeviceService.Interfaces;
using FenomPlus.Helpers;

namespace FenomPlus.Droid
{
    [Activity(Label = "FenomPlus", Icon = "@mipmap/icon", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            UserDialogs.Init(this);
            SvgHelper.Init();
            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // register the navigation here
            AppServices.Container.Register<INavigationService, NavigationService>().AsSingleton();            
            AppServices.Container.Register<IDeviceService, DeviceService>().AsSingleton();            


            CheckPermissions();

            LoadApplication(new App());
            PlaySounds.InitSound();
        }
        protected override void OnDestroy()
        {
            PlaySounds.Close();
            base.OnDestroy();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            var permissionsList = permissions.ToList();
            if (permissionsList.Any(p => p.Contains("BLUETOOTH")))
            {
                var permission = Android.OS.Build.VERSION.SdkInt >= ((Android.OS.BuildVersionCodes)0x1F) ? "android.permission.BLUETOOTH_SCAN" : "android.permission.BLUETOOTH";

                var blePermission = permissionsList.FirstOrDefault(p => p == permission);
                
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void CheckPermissions()
        {
            string[] Permissions =
            {
                Manifest.Permission.Bluetooth,
                Manifest.Permission.BluetoothAdmin,
                "android.permission.BLUETOOTH_SCAN",
                "android.permission.BLUETOOTH_CONNECT",
                "android.permission.BLUETOOTH_ADVERTISE",
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission_group.BluetoothNetwork,
                Manifest.Permission_group.Location
            };

            bool minimumPermissionsGranted = true;

            foreach (string permission in Permissions)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    minimumPermissionsGranted = false;
                }
            }

            if (!minimumPermissionsGranted)
            {
                RequestPermissions(Permissions, 0);
            }
        }
    }
}