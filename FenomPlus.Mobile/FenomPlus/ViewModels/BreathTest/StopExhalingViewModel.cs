using System;
using FenomPlus.Helpers;
using FenomPlus.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class StopExhalingViewModel : BaseViewModel
    {
        public StopExhalingViewModel()
        {

        }

        private bool Stop;

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            Stop = false;
            Seconds = Config.StopExhalingReadyWait;
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerCallback);
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
        /// <returns></returns>
        private bool TimerCallback()
        {
            Seconds--;
            if (Stop == true) seconds = 0;
            if ((Seconds <= 0) && (Stop == false))
            {
                if (Cache.BreathManeuver.StatusCode != 0x00)
                {
                    var model = BreathManeuverErrorDBModel.Create(Cache.BreathManeuver);
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

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
