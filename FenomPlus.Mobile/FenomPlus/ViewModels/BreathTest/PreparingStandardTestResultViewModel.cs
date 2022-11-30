using System;
using System.Diagnostics;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class PreparingStandardTestResultViewModel : BaseViewModel
    {
        public PreparingStandardTestResultViewModel()
        {

        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            TestType = Cache.TestType == TestTypeEnum.Standard ? "10-second Test Result" : "6-second Test Result";

            Seconds = Config.TestResultReadyWait;
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private bool TimerCallback()
        {
            if (Seconds > 0) Seconds--;
            //if (Cache.FenomReady == true)
            {
                bool isReady = Cache.FenomReady; // ToDo: Just for debugging purposes

                var model = BreathManeuverResultDBModel.Create(Cache.BreathManeuver);

                ResultsRepo.Insert(model);

                Debug.WriteLine($"Cache.BreathManeuver.StatusCode = {Cache.BreathManeuver.StatusCode}");

                if (Cache.BreathManeuver.StatusCode != 0x00)
                {
                    var errorModel = BreathManeuverErrorDBModel.Create(Cache.BreathManeuver);
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

            return (Cache.FenomReady == false);
        }

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

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}

