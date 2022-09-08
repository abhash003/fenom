using System;
using FenomPlus.Helpers;
using FenomPlus.SDK.Core.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class TutorialViewModel : BaseViewModel
    {
        public TutorialViewModel()
        {
        }

        private bool Stop;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            BleHub.StartTest(BreathTestEnum.Training);
            //Services.BleHub.IsConnected();
            Stop = false;

            // start timer to read measure constally
            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                GuageData = Cache.BreathFlow;
                return !Stop;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
        {
            base.OnDisappearing();
            BleHub.StartTest(BreathTestEnum.Stop);
            Stop = true;
            PlaySounds.StopAll();
        }

        /// <summary>
        /// 
        /// </summary>
        private float guageData;
        public float GuageData
        {
            get => guageData;
            set
            {
                guageData = value;
                OnPropertyChanged("GuageData");
                if ((!Stop) && (Postion >= 4 && Postion <= 7))
                {
                    PlaySounds.PlaySound(GuageData);
                } else
                {
                    PlaySounds.StopAll();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string guageStatus;
        public string GuageStatus
        {
            get => guageStatus;
            set
            {
                guageStatus = value;
                OnPropertyChanged("GuageStatus");
            }
        }

        protected int postion;
        public int Postion
        {
            get => postion;
            set
            {
                postion = value;
                OnPropertyChanged("Postion");
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
