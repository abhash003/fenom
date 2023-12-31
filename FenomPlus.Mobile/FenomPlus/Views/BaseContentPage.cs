﻿using System;
using System.Diagnostics;
using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Helpers;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public class BaseContentPage : ContentPage, IUnhandledExceptionHandler //IBaseServices
    {
        public IAppServices Services => IOC.Services;
        public double Density = 0;
        //public ICacheService Cache => Services.Cache;
        //public IBleHubService BleHub => Services.BleHub;

        //// repos here
        //public IBreathManeuverErrorRepository ErrorsRepo => Services.Database.BreathManeuverErrorRepo;
        //public IBreathManeuverResultRepository ResultsRepo => Services.Database.BreathManeuverResultRepo;
        //public IQualityControlRepository QCRepo => Services.Database.QualityControlRepo;
        //public IQualityControlDevicesRepository QCDevicesRepo => Services.Database.QualityControlDevicesRepo;
        //public IQualityControlUsersRepository QCUsersRepo => Services.Database.QualityControlUsersRepo;


        public BaseContentPage()
        {
            // Set Background
            Services.LogCat.Print($"{new StackFrame().GetMethod().ReflectedType.FullName}: {DebugHelper.GetCallingMethodString(2)}");
            if (Density == 0)
                DisplayInfo();
        }

        private void DisplayInfo()
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            // Orientation (Landscape, Portrait, Square, Unknown)
            var orientation = mainDisplayInfo.Orientation;
            // Rotation (0, 90, 180, 270)
            var rotation = mainDisplayInfo.Rotation;
            // Width (in pixels)
            var width = mainDisplayInfo.Width;
            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            // Screen density
            var density = mainDisplayInfo.Density;
            Density = density;
            Debug.WriteLine($"Device density is {density}, w:{width}, h:{height}, orientation: {orientation}");
        }

        protected override void OnAppearing()
        {
            Services.LogCat.Print($"---->: {new StackFrame().GetMethod().ReflectedType.FullName}: {DebugHelper.GetCallingMethodString(2)}");
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Services.LogCat.Print($"<----: {new StackFrame().GetMethod().ReflectedType.FullName}: {DebugHelper.GetCallingMethodString(2)}");
            base.OnDisappearing();
        }

        public virtual void NewGlobalData()
        {
        }
        public void RegisterUnhandledExceptionHandler()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
        }
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string exceptionStr = e.ExceptionObject.ToString();
            Debug.WriteLine(exceptionStr);
        }
    }
}
