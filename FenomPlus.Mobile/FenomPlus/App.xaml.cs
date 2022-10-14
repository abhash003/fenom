using Xamarin.Forms;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Abstractions;
using Xamarin.Essentials;
using Microsoft.Extensions.Logging;
using FenomPlus.SDK.Core;
using FenomPlus.Views;
using System.Threading.Tasks;
using FenomPlus.Models;
using System.Text;
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
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjQwNDYxQDMyMzAyZTMxMmUzMGlPTklYM3hoQmpKc2F2bVlEUFBBS29YU1FGQTBWSTZyY2RJbkJBVm1pbEU9");
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
