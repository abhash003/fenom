﻿using System.Diagnostics;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using System.Threading.Tasks;

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

        private async Task<bool> CalculationsCompleted()
        {
            var device = Services.DeviceService.Current;
            
            if (device.FenomReady == true)
            {
                var model = BreathManeuverResultDBModel.Create(device.BreathManeuver, device.ErrorStatusInfo);
                ResultsRepo.Insert(model);

                var str = ResultsRepo.ToString();

                Debug.WriteLine($"Cache.BreathManeuver.StatusCode = {device.ErrorStatusInfo.ErrorCode}");

                if (device.ErrorStatusInfo.ErrorCode != 0x00 || device.LastErrorCode != 0)
                {
                    var errorModel = BreathManeuverErrorDBModel.Create(device.BreathManeuver, device.ErrorStatusInfo);
                    ErrorsRepo.Insert(errorModel);

                    PlaySounds.PlayFailedSound();
                    await Services.Navigation.TestErrorView();
                }
                else
                {
                    await Services.Navigation.TestResultsView();
                }
            }

            return (device.FenomReady == false);
        }
        
    }
}
