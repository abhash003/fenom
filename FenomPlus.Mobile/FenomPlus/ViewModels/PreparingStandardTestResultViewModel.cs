using System;
using FenomPlus.Models;
using FenomPlus.Views;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class PreparingStandardTestResultViewModel : BaseViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public PreparingStandardTestResultViewModel()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            if (Cache.TestType == TestTypeEnum.Standard)
            {
                TestType = "10-second Test Result";
            }
            else
            {
                TestType = "6-second Test Result";
            }
            Seconds = Config.TestResultReadyWait;
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
        {
            base.OnDisappearing();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool TimerCallback()
        {
            if (Seconds > 0) Seconds--;
            if (Seconds <= 0)
            {
                var model = BreathManeuverResultDBModel.Create(Cache._BreathManeuver);

                ResultsRepo.Insert(model);

                if (Cache._BreathManeuver.StatusCode != 0x00)
                {
                    var errorModel = BreathManeuverErrorDBModel.Create(Cache._BreathManeuver);
                    ErrorsRepo.Insert(errorModel);

                    Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TestFailedView)}"), false);
                }
                else
                {
                    Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TestResultsView)}"), false);
                }
            }
            return (Seconds > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private int seconds;
        public int Seconds
        {
            get => seconds;
            set
            {
                seconds = value;
                OnPropertyChanged("Seconds");
            }
        }

        private string _TestType;
        public string TestType
        {
            get => _TestType;
            set
            {
                _TestType = value;
                OnPropertyChanged("TestType");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}

