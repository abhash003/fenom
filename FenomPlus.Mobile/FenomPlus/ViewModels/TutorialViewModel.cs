using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class TutorialViewModel : BaseViewModel
    {
        public ObservableCollection<Tutorial> Tutorials { get; set; }

        public TutorialViewModel()
        {
            InitializeCollection();
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void UpdateViews()
        {
            if (TutorialPosition <= 0)
            {
                ShowBack = false;
                ShowNext = true;
                ShowTutorial = true;
                ShowSuccess = false;
            }
            else if (TutorialPosition < Tutorials.Count)
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
        public override void OnAppearing()
        {
            base.OnAppearing();
            BleHub.StartTest(BreathTestEnum.Training);
            Services.BleHub.IsNotConnectedRedirect();
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
        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Stop = true;
            BleHub.StartTest(BreathTestEnum.Stop);
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
                if ((Stop == false) && (TutorialPosition == 4))
                {
                    PlaySounds.PlaySound(GuageData);
                } else {
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
        
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        protected string header;
        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected int tutorialPosition;
        public int TutorialPosition
        {
            get => tutorialPosition;
            set
            {
                tutorialPosition = value;
                OnPropertyChanged("TutorialPosition");
                UpdateViews();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            GuageData = Cache.BreathFlow;
        }
    }
}
