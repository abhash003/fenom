using System.Diagnostics;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public partial class PreparingStandardTestResultViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _testType;

        public PreparingStandardTestResultViewModel()
        {

        }

        private Timer CalculationsTimer;

        public override void OnAppearing()
        {
            base.OnAppearing();

            TestType = Services.Cache.TestType == TestTypeEnum.Standard ? "10-second Test Result" : "6-second Test Result";

            CalculationsTimer = new Timer(Config.TestResultReadyWait * 1000);
            CalculationsTimer.Elapsed += (sender, e) => CalculationsCompleted();
            CalculationsTimer.Start();
        }

        public override void OnDisappearing()
        {
            CalculationsTimer.Start(); // Just in case
            CalculationsTimer.Dispose();

            base.OnDisappearing();
        }

        private bool CalculationsCompleted()
        {
            if (Services.DeviceService.Current.FenomReady == true)
            {
                var model = BreathManeuverResultDBModel.Create(Services.DeviceService.Current.BreathManeuver);
                ResultsRepo.Insert(model);

                var str = ResultsRepo.ToString();

                Debug.WriteLine($"Cache.BreathManeuver.StatusCode = {Services.DeviceService.Current.BreathManeuver.StatusCode}");

                if (Services.DeviceService.Current.BreathManeuver.StatusCode != 0x00)
                {
                    var errorModel = BreathManeuverErrorDBModel.Create(Services.DeviceService.Current.BreathManeuver);
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

            return (Services.DeviceService.Current.FenomReady == false);
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
