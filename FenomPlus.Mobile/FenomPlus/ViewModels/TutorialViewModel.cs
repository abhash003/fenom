﻿using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class TutorialViewModel : BaseViewModel
    {
        private bool Stop;


        [ObservableProperty]
        private int _tutorialIndex = 0;

        private ObservableCollection<Tutorial> Tutorials { get; set; }

        [ObservableProperty]
        private string _header;

        [ObservableProperty] 
        private string _title;

        [ObservableProperty] 
        private string _info;

        [ObservableProperty] 
        private string _illustration;

        [ObservableProperty]
        private bool _showStep;

        [ObservableProperty]
        private bool _showImage;

        [ObservableProperty]
        private bool _showGuage;

        [ObservableProperty]
        private float _guageData;

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
            InitializeCollection();
        }

        // These are called when corresponding property changes

        partial void OnTutorialIndexChanged(int value)
        {
            UpdateContent();
        }

        partial void OnShowStepChanged(bool value)
        {
            UpdateContent();
        }

        partial void OnGuageDataChanged(float value)
        {
            if ((Stop == false) && (TutorialIndex == 4))
            {
                PlaySounds.PlaySound(GuageData);
            }
            else
            {
                PlaySounds.StopAll();
            }
        }

        private void InitializeCollection()
        {
            Tutorials = new ObservableCollection<Tutorial>();
            Tutorials.Add(new Tutorial()
            {
                Title = "Step 1",
                Illustration = "TutStep1",
                Info = "Place new mouthpiece",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 2",
                Illustration = "TutStep2",
                Info = "Firmly grasp the device",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 3",
                Illustration = "TutStep3",
                Info = "Sit up straight",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 4",
                Illustration = "TutStep4",
                Info = "Take a deep breath\n\nPlace your lips around the mouthpiece",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 5",
                Illustration = "TutStep5",
                Info = "Exhale steadily\n\nHit the star",
                ShowStep = true,
            });
        }

        private void UpdateContent()
        {
            Header = $"Step {TutorialIndex + 1}";

            ShowImage = !ShowStep;

            ShowGuage = ShowStep;

            Title = Tutorials[TutorialIndex].Title;
            Info = Tutorials[TutorialIndex].Info;
            Illustration = Tutorials[TutorialIndex].Illustration;
            ShowStep = Tutorials[TutorialIndex].ShowStep;

            if (TutorialIndex <= 0)
            {
                ShowBack = false;
                ShowNext = true;
                ShowTutorial = true;
                ShowSuccess = false;
            }
            else
            {
                ShowBack = true;
                ShowNext = true;
                ShowTutorial = true;
                ShowSuccess = false;
            }

            if ((Stop == false) && (TutorialIndex == 4))
            {
                PlaySounds.PlaySound(GuageData);
            }
            else
            {
                PlaySounds.StopAll();
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            TutorialIndex = 0;

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
            if (TutorialIndex + 1 < Tutorials.Count)
            {
                TutorialIndex = TutorialIndex + 1;
            }
            else
            {
                TutorialIndex = Tutorials.Count - 1;

                UpdateContent();
                ShowBack = true;
                ShowNext = false;
                ShowTutorial = false;
                ShowSuccess = true;
            }
        }

        [RelayCommand]
        private void Back()
        {
            if (TutorialIndex >= Tutorials.Count)
            {
                TutorialIndex = Tutorials.Count;
            }
            if (TutorialIndex > 0)
            {
                TutorialIndex = TutorialIndex - 1;
            }
        }
    }
}
