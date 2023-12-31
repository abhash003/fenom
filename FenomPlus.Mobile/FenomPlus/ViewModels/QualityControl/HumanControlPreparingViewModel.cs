using System;
using System.Globalization;
using FenomPlus.Controls;
using FenomPlus.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class HumanControlPreparingViewModel : BaseViewModel
    {
        public HumanControlPreparingViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            if (Services.DeviceService.Current == null) return;
            Services.DeviceService.Current.IsNotConnectedRedirect();
            TestTime = 10;
            TestSeconds = TestTime * (1000 / Services.Cache.BreathFlowTimer);
            Stop = false;
            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                TestSeconds--;
                TestTime = TestSeconds / (1000 / Services.Cache.BreathFlowTimer);
                if ((TestSeconds <= 0) && (Stop == false))
                {
                    QualityControlDataModel model = new QualityControlDataModel()
                    {
                        DateTaken = DateTime.Now.ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture),
                        User = Services.Cache.QCUsername,
                        TestResult = Services.DeviceService.Current.BreathFlow,
                        SerialNumber = this.DeviceSerialNumber,
                        QCStatus = "",
                        QCExpiration = "",
                    };

                    // depending on result
                    if ((Services.Cache.HumanControlResult >= BreathGauge.Green1) && (Services.Cache.HumanControlResult <= BreathGauge.Green1Top))
                    {
                        model.QCStatus = "Qualified";
                        QCRepo.Insert(model);
                        // log passed here
                        Services.Navigation.HumanControlPassedView();
                    }
                    else
                    {
                        model.QCStatus = "Disqualified";
                        QCRepo.Insert(model);
                        // log failed here
                        Services.Navigation.HumanControlDisqualifiedView();
                    }

                }

                return (TestSeconds > 0) && (Stop == false);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Stop = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private int _TestTime;
        public int TestTime
        {
            get => _TestTime;
            set
            {
                _TestTime = value;
                OnPropertyChanged("TestTime");
            }
        }

        private bool Stop;
        private int TestSeconds;

        
    }
}