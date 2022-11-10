using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Pdf.Graphics;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class TutorialViewModel : BaseViewModel
    {
        private bool Stop;


        [ObservableProperty]
        private int _tutorialIndex = 1;


        //partial void OnTutorialIndexChanging(int value)
        //{
        //    if (TutorialIndex == 5) // Page with Breath Guage
        //    {
        //        BleHub.StartTest(BreathTestEnum.Training);
        //        Services.BleHub.IsNotConnectedRedirect();
        //        Stop = false;

        //        // start timer to read measure constantly
        //        Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
        //        {
        //            GuageData = Cache.BreathFlow;
        //            return !Stop;
        //        });
        //    }
        //    else if (TutorialIndex == 4 || TutorialIndex == 6)
        //    {
        //        Stop = true;
        //        BleHub.StartTest(BreathTestEnum.Stop);
        //        PlaySounds.StopAll();
        //    }
        //}


        partial void OnTutorialIndexChanged(int value)
        {
            if (TutorialIndex < 1)
                TutorialIndex = 1;

            if (TutorialIndex > 6)
                TutorialIndex = 6;

            UpdateContent();
        }

        [ObservableProperty]
        private bool _instructionsVisible;

        [ObservableProperty]
        private bool _illustrationVisible;

        [ObservableProperty]
        private bool _breathGuageVisible;

        [ObservableProperty]
        private bool _successPanelVisible;


        [ObservableProperty] 
        private string _stepTitle;

        [ObservableProperty] 
        private string _instructionsText;

        [ObservableProperty] 
        private string _illustrationSource;


        [ObservableProperty]
        private bool _showGuage;

        [ObservableProperty]
        private float _guageData;

        partial void OnGuageDataChanged(float value)
        {
            if ((Stop == false) && (TutorialIndex == 5))
            {
                PlaySounds.PlaySound(value);
            }
            else
            {
                PlaySounds.StopAll();
            }
        }

        [RelayCommand]
        private void PlaySound()
        {
            if ((Stop == false) && (TutorialIndex == 5))
            {
                PlaySounds.PlaySound(GuageData);
            }
            else
            {
                PlaySounds.StopAll();
            }
        }

        //private float guageData;
        //private float GuageData
        //{
        //    get => guageData;
        //    set
        //    {
        //        guageData = value;
        //        OnPropertyChanged("GuageData");
        //        if ((Stop == false) && (TutorialIndex == 5))
        //        {
        //            PlaySounds.PlaySound(GuageData);
        //        }
        //        else
        //        {
        //            PlaySounds.StopAll();
        //        }
        //    }
        //}

        [ObservableProperty]
        private string _guageStatus;

        [ObservableProperty]
        protected bool _showBack;

        [ObservableProperty]
        protected bool _showNext;

        [ObservableProperty]
        private string _testType;

        [ObservableProperty]
        private int _testTime;

        [ObservableProperty]
        protected bool _showTutorial = true;

        [ObservableProperty]
        protected bool _showSuccess;


        public TutorialViewModel()
        {
        }

        private void UpdateContent()
        {
            switch (TutorialIndex)
            {
                // Pages
                case 1:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGuageVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = false;
                    ShowNext = true;

                    StepTitle = "Step 1";
                    IllustrationSource = "TutStep1";
                    InstructionsText = "Snap new mouthpiece onto the device";
                    break;

                case 2:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGuageVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 2";
                    IllustrationSource = "TutStep2";
                    InstructionsText = "Firmly grasp the device";
                    break;

                case 3:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGuageVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 3";
                    IllustrationSource = "TutStep3";
                    InstructionsText = "Sit up straight";
                    break;

                case 4:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGuageVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 4";
                    IllustrationSource = "TutStep4";
                    InstructionsText = "Take a deep breath\n\nPlace your lips around the mouthpiece";
                    break;

                case 5:
                    InstructionsVisible = true;
                    IllustrationVisible = false;
                    BreathGuageVisible = true;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 5";
                    IllustrationSource = "TutStep5";
                    InstructionsText = "Exhale into the device now\n\nPoint the needle at the star";

                    //BleHub.StartTest(BreathTestEnum.Training);
                    //Services.BleHub.IsNotConnectedRedirect();
                    break;

                case 6: // success Page
                    InstructionsVisible = false;
                    IllustrationVisible = false;
                    BreathGuageVisible = false;
                    SuccessPanelVisible = true;

                    ShowBack = true;
                    ShowNext = false;

                    StepTitle = string.Empty;
                    IllustrationSource = "TutStep5";
                    InstructionsText = "Exhale into the device now\n\nPoint the needle at the star";
                    break;
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            TutorialIndex = 1;

            // Force update no matter the TutorialIndex
            UpdateContent();

            BleHub.StartTest(BreathTestEnum.Training);
            Services.BleHub.IsNotConnectedRedirect();
            Stop = false;

            // start timer to read measure constantly
            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                GuageData = Cache.BreathFlow;
                return !Stop;
            });
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            Stop = true;
            BleHub.StartTest(BreathTestEnum.Stop);
            PlaySounds.StopAll();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            GuageData = Cache.BreathFlow;
        }

        [RelayCommand]
        private void Next()
        {
            TutorialIndex += 1;
        }

        [RelayCommand]
        private void Back()
        {
            TutorialIndex -= 1;
        }
    }
}
