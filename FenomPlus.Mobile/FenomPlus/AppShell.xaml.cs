using FenomPlus.Services;
using System;
using FenomPlus.Views;
using Xamarin.Forms;

namespace FenomPlus
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        public void NotifyViews()
        {
            if (Current?.CurrentPage == null) return;

            try
            {
                ((BaseContentPage)Current.CurrentPage).NewGlobalData();
            } 
            catch(Exception ex) 
            {
                IOC.Services.LogCat.Print(ex);
            }
        }

        public void NotifyViewModels()
        {

        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            bool IsDeviceConnected = IOC.Services.DeviceService?.Current?.Connected ?? false;
            if (!IsDeviceConnected && args.Target.Location.OriginalString.Contains("QualityControlView"))
            {
                // When disconnected, and user tap 'Quality Control'
                args.Cancel();  // hijack it, and redirect to QC Settings
                Shell.Current.GoToAsync("//QCSettingsView");
            }
        }
    }
}
