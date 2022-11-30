using System;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
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
        public override void OnAppearing()
        {
            base.OnAppearing();
            if (Services.Cache.TestType == TestTypeEnum.Standard)
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
        public override void OnDisappearing()
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
            if (Services.Cache.FenomReady == true)
            {
                var model = BreathManeuverResultDBModel.Create(Services.Cache.BreathManeuver);

                ResultsRepo.Insert(model);

                if (Services.Cache.BreathManeuver.StatusCode != 0x00)
                {
                    var errorModel = BreathManeuverErrorDBModel.Create(Services.Cache.BreathManeuver);
                    ErrorsRepo.Insert(errorModel);

                    PlaySounds.PlayFailedSound();
                    Services.Navigation.TestFailedView();
                }
                else
                {
                    PlaySounds.PlaySuccessSound();
                    Services.Navigation.TestResultsView();
                }
            }
            return (Services.Cache.FenomReady == false);
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
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}

