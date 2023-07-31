using System.Diagnostics;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using System.Threading.Tasks;
using System;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class PreparingStandardTestResultViewModel : BaseViewModel
    {
        public async Task<bool> Callback()
        {
            var device = Services.DeviceService.Current;

            if (device.ErrorStatusInfo.ErrorCode != 0x00)
            {
                CalculationsTimer.Stop();
                await  CalculationsCompleted();
                return true;
            }

            return false;
        }

        [ObservableProperty]
        private string _testType;

        public PreparingStandardTestResultViewModel()
        {
            MessagingCenter.Subscribe<Services.DeviceService.Abstract.Device, string>(this, "NOScore", async (sender, arg) =>
            {
                CalculationsTimer.Stop();
                await  CalculationsCompleted();
            });
        }

        private Timer CalculationsTimer;

        public override void OnAppearing()
        {
            base.OnAppearing();

            CalculationsTimer = new Timer(Config.TestResultReadyWait * 1000);
            CalculationsTimer.Elapsed += async (sender, e) => await CalculationsCompleted();
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
            
            if (device == null)
            {
                return false;
            }

            if (device.FenomReady == true)
            {
                var model = BreathManeuverResultDBModel.Create(device.BreathManeuver, device.ErrorStatusInfo);
                ResultsRepo.Insert(model);                

                Debug.WriteLine($"Cache.DeviceStatusInfo.StatusCode = {device.ErrorStatusInfo.ErrorCode}");

                if (device.ErrorStatusInfo.ErrorCode != 0x00)
                {
                    var errorModel = BreathManeuverErrorDBModel.Create(device.BreathManeuver, device.ErrorStatusInfo);
                    ErrorsRepo.Insert(errorModel);

                    PlaySounds.PlayFailedSound();
                    await Services.Navigation.TestErrorView();
                }
                else
                {
                    PlaySounds.PlaySuccessSound();
                    await Services.Navigation.TestResultsView();
                }
            }

            return (device.FenomReady == false);
        }
        
    }
}
