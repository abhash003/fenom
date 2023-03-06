using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Helpers;
using FenomPlus.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class StopExhalingViewModel : BaseViewModel
    {
        private bool Stop;

        [ObservableProperty]
        private int _seconds;

        public StopExhalingViewModel()
        {

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            Stop = false;
            Seconds = Config.StopExhalingReadyWait;
            PlaySounds.PlaySuccessSound();
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Stop = true;
        }

        private bool TimerCallback()
        {
            Seconds--;
            if (Stop == true) 
                Seconds = 0;

            if ((Seconds <= 0) && (Stop == false))
            {
                if (Services.DeviceService.Current != null && Services.DeviceService.Current.ErrorStatusInfo.ErrorCode != 0x00)
                {
                    var model = BreathManeuverErrorDBModel.Create(Services.DeviceService.Current.BreathManeuver, Services.DeviceService.Current.ErrorStatusInfo);
                    ErrorsRepo.Insert(model);

                    PlaySounds.PlayFailedSound();
                    Services.Navigation.TestErrorView();
                }
                else
                {
                    Services.Navigation.PreparingStandardTestResultView();
                }
            }

            return Seconds > 0;
        }


    }
}
