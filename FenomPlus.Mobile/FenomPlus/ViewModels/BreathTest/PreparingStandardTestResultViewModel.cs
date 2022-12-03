using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class PreparingStandardTestResultViewModel : BaseViewModel
    {
        [ObservableProperty]
        private int _seconds;

        [ObservableProperty]
        private string _testType;

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
            if (Seconds > 0) 
                Seconds--;

            if (Cache.FenomReady == true)
            {
                var model = BreathManeuverResultDBModel.Create(Cache.BreathManeuver);
                ResultsRepo.Insert(model);

                var str = ResultsRepo.ToString();

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



        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}

