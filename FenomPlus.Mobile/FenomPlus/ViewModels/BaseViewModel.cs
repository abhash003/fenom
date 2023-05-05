using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using System.Threading.Tasks;
using FenomPlus.Models;
using Xamarin.Essentials;
using System.Reflection;
using System.Diagnostics;
using Syncfusion.SfNumericTextBox.XForms;
using FenomPlus.Helpers;

namespace FenomPlus.ViewModels
{
    public partial class BaseViewModel : ObservableObject, IBaseServices, IUnhandledExceptionHandler
    {
        public IAppServices Services => IOC.Services;
        //public IBleHubService BleHub => Services.BleHub;
        //public ICacheService Cache => Services.Cache;
        public IConfigService Config => Services.Config;
        public IDialogService Dialogs => Services.Dialogs;

        // repos here
        public IBreathManeuverErrorRepository ErrorsRepo => Services.Database.BreathManeuverErrorRepo;
        public IBreathManeuverResultRepository ResultsRepo => Services.Database.BreathManeuverResultRepo;
        //public IQualityControlRepository QCRepo => Services.Database.QualityControlRepo;
        //public IQualityControlDeviceRepository QCDevicesRepo => Services.Database.QualityControlDevicesRepo;
        //public IQualityControlUsersRepository QCUsersRepo => Services.Database.QualityControlUsersRepo;

        [ObservableProperty]
        bool _isBusy = false;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _appSoftwareVersion;

        [ObservableProperty]
        private string _deviceSerialNumber;

        [ObservableProperty]
        private string _deviceFirmwareVersion;

        [ObservableProperty]
        private string _deviceConnectedStatus;

        [ObservableProperty]
        private bool _isDeviceConnected;

        //[ObservableProperty] private DeviceStatus _deviceStatus;

        [ObservableProperty]
        private bool errorVisible;

        [ObservableProperty]
        private int _errorHeight;

        public BaseViewModel()
        {
            Services.LogCat.Print($"{new StackFrame().GetMethod().ReflectedType.FullName}: {DebugHelper.GetCallingMethodString(2)}");

            AppSoftwareVersion = $"Software ({VersionTracking.CurrentVersion})";
        }

        [RelayCommand]
        public async Task ExitToDashboard()
        {
            await Services.Navigation.DashboardView();
        }

        public virtual void OnAppearing()
        {
            
        }

        public virtual void OnDisappearing()
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
