using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class TutorialViewModel : BaseViewModel
    {
        public ObservableCollection<Tutorial> Tutorials { get; set; }

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

        public bool ShowImage => (!ShowStep);

        public bool ShowGuage => (ShowStep);



        private float guageData;
        public float GuageData
        {
            get => guageData;
            set
            {
                guageData = value;
                OnPropertyChanged("GuageData");
                if ((Stop == false) && (TutorialIndex == 4))
                {
                    PlaySounds.PlaySound(GuageData);
                }
                else
                {
                    PlaySounds.StopAll();
                }
            }
        }

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

        protected bool showBack;
        public bool ShowBack
        {
            get => showBack;
            set
            {
                showBack = value;
                OnPropertyChanged("ShowBack");
            }
        }

        protected bool showNext;
        public bool ShowNext
        {
            get => showNext;
            set
            {
                showNext = value;
                OnPropertyChanged("ShowNext");
            }
        }

        protected bool showTutorial;
        public bool ShowTutorial
        {
            get => showTutorial;
            set
            {
                showTutorial = value;
                OnPropertyChanged("ShowTutorial");
            }
        }

        protected bool showSuccess;
        public bool ShowSuccess
        {
            get => showSuccess;
            set
            {
                showSuccess = value;
                OnPropertyChanged("ShowSuccess");
            }
        }



        protected int _tutorialIndex;
        public int TutorialIndex
        {
            get => _tutorialIndex;
            set
            {
                _tutorialIndex = value;

                Title = Tutorials[_tutorialIndex].Title;
                Info = Tutorials[_tutorialIndex].Info;
                Illustration = Tutorials[_tutorialIndex].Illustration;
                ShowStep = Tutorials[_tutorialIndex].ShowStep;

                OnPropertyChanging(nameof(ShowImage));
                OnPropertyChanging(nameof(ShowGuage));

                UpdateButtons();
            }
        }

        private bool Stop;

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





        public TutorialViewModel()
        {
            InitializeCollection();
        }

        private void InitializeCollection()
        {
            Tutorials = new ObservableCollection<Tutorial>();
            Tutorials.Add(new Tutorial()
            {
                Title = "Step 1",
                Illustration = "TutStep1",
                Info = "Insert new mouthpiece.",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 2",
                Illustration = "TutStep2",
                Info = "Hold the device with your hand firmly grasping the grip area.",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 3",
                Illustration = "TutStep3",
                Info = "Make sure patient is seated,\nSitting in an upright position.",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 4",
                Illustration = "TutStep4",
                Info = "Breathe in deeply and tightly seal your mouth around the mouthpiece.",
                ShowStep = false,
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 5",
                Illustration = "TutStep5",
                Info = "Exhale steadily. This moves the needle in the direction of the white dot at the center of the meter.\n\nExhaling to reach the white star is your goal.",
                ShowStep = true,
            });
        }

        public void UpdateButtons()
        {
            if (TutorialIndex <= 0)
            {
                ShowBack = false;
                ShowNext = true;
                ShowTutorial = true;
                ShowSuccess = false;
            }
            else if (TutorialIndex < Tutorials.Count)
            {
                ShowBack = true;
                ShowNext = true;
                ShowTutorial = true;
                ShowSuccess = false;
            }
            else
            {
                ShowBack = true;
                ShowNext = false;
                ShowTutorial = false;
                ShowSuccess = true;
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
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
    }
}
