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

            // https://www.syncfusion.com/account
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA2NTAxNUAzMjMwMmUzNDJlMzBXM0ozeG1ZbnowOHRMSklJZ25DbldybjdYdU10NVlrSjVZQWFISFVMdExRPQ==");

            SvgImageSource.RegisterAssembly();

            AppShell = new AppShell();
            MainPage = AppShell;
            IOC.Services.Cache.TestType = Enums.TestTypeEnum.None;
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

            var curr = App.GetCurrentPage();
            if (curr is TestErrorView)
            {
                services.Navigation.DashboardView();
            }

            else if (curr is TestFailedView)
            {
                services.Navigation.DashboardView();
            }
            else if (curr is PreparingStandardTestResultView) 
            {
                MessagingCenter.Send(this, "AppSleeping");
            }
            else if (curr is QCNegativeControlTestView)
            {
                MessagingCenter.Send(this, "AppSleeping");
            }
            else if (curr is QCUserTestCalculationView)
            {
                MessagingCenter.Send(this, "AppSleeping");
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
