using Xamarin.Forms;
using FenomPlus.Views;
using FenomPlus.Services;
using System;

namespace FenomPlus
{
    public partial class App : Application
    {
        public static MainView mainView { get; set; }

        public App()
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzMxNjc0QDMyMzAyZTMzMmUzMGNPR1AvcXFJQUtROGhzTDFic05UQ1FtTkFEZDY2eHJxTHZDOTd0ZUx1UlU9");
            mainView = new MainView();
            MainPage = mainView;
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
            if (mainView != null)
            {
                mainView.NotifyViews();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void NotifyViewModels()
        {
            if (mainView != null)
            {
                mainView.NotifyViewModels();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Page GetCurrentPage()
        {
            Page page = null;
            if ((Current != null) && (mainView.CurrentPage != null))
            {
                try
                {
                    page = mainView.CurrentPage;
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
