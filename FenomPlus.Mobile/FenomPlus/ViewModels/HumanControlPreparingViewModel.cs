using System;
using FenomPlus.Controls;
using FenomPlus.Models;
using FenomPlus.Views;
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
        override public void OnAppearing()
        {
            base.OnAppearing();
            //Services.BleHub.IsConnected();
            TestTime = 10;
            TestSeconds = TestTime * (1000 / Cache.BreathFlowTimer);
            Stop = false;
            Device.StartTimer(TimeSpan.FromMilliseconds(Cache.BreathFlowTimer), () =>
            {
                TestSeconds--;
                TestTime = TestSeconds / (1000 / Cache.BreathFlowTimer);
                if ((TestSeconds <= 0) && (Stop == false))
                {
                    QualityControlDataModel model = new QualityControlDataModel()
                    {
                        DateTaken = DateTime.Now.ToString(),
                        User = Services.Cache.QCUsername,
                        TestResult = Cache.BreathFlow,
                        SerialNumber = this.DeviceSerialNumber,
                        QCStatus = "",
                        QCExpiration = "",
                    };

                    // depending on result
                    if ((Cache.HumanControlResult >= BreathGuage.LowGreen) && (Cache.HumanControlResult <= BreathGuage.HighGreen))
                    {
                        model.QCStatus = "Qualified";
                        QCRepo.Insert(model);
                        // log passed here
                        Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(HumanControlPassedView)}"), false);
                    }
                    else
                    {
                        model.QCStatus = "Disqualified";
                        QCRepo.Insert(model);
                        // log failed here
                        Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(HumanControlDisqualifiedView)}"), false);
                    }

                }

                return (TestSeconds > 0) && (Stop == false);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
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

        /// <summary>
        /// 
        /// </summary>
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
