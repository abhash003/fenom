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
        }

        protected override void OnResume()
        {
        }

        public static void NotifyViews()
        {
            if (AppShell != null)
            {
                AppShell.NotifyViews();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void NotifyViewModels()
        {
            if (AppShell != null)
            {
                AppShell.NotifyViewModels();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
