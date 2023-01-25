using Xamarin.Forms;
using FenomPlus.Views;
using FenomPlus.Services;
using System;
using Xamarin.Forms.Svg;

namespace FenomPlus
{
    public partial class App : Application
    {
        public static AppShell AppShell { get; set; }

        public App()
        {
            InitializeComponent();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzY0MTE5QDMyMzAyZTMzMmUzMFQ4aE9PM1F3RlN6NDhLU0lEbi9RRmt3akc4dWVBZGw5VEtRWG1sTituU289");

            SvgImageSource.RegisterAssembly();

            AppShell = new AppShell();
            MainPage = AppShell;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            var services = IOC.Services;

            if (services == null)
                return;

            if (App.GetCurrentPage() is TestErrorView)
            {
                services.Navigation.DashboardView();
            }

            if (App.GetCurrentPage() is TestFailedView)
            {
                services.Navigation.DashboardView();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
        

        public static Page GetCurrentPage()
        {
            Page page = null;
            if ((Current != null) && (AppShell.CurrentPage != null))
            {
                try
                {
                    page = AppShell.CurrentPage;
                }
                catch (Exception ex)
                {
                    IOC.Services.LogCat.Print(ex);
                }
            }
            return page;
        }
    }
}
