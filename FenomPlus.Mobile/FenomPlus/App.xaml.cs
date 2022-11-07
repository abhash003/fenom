using Xamarin.Forms;
using FenomPlus.Views;
using FenomPlus.Services;
using System;

namespace FenomPlus
{
    public partial class App : Application
    {
        public static AppShell AppShell { get; set; }

        public App()
        {
            InitializeComponent();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzMxNjc0QDMyMzAyZTMzMmUzMGNPR1AvcXFJQUtROGhzTDFic05UQ1FtTkFEZDY2eHJxTHZDOTd0ZUx1UlU9");

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
