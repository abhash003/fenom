﻿using System;
using System.Collections.ObjectModel;
using FenomPlus.Models;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    [QueryProperty(nameof(Source), "source")]
    public partial class TutorialView : BaseContentPage
    {
        private TutorialViewModel model;

        // ShortTestView, StartTestView, TestErrorView, TestFailedView,
        private string _Source;
        public string Source
        {
            get { return string.IsNullOrEmpty(_Source) ? nameof(ChooseTestView) : _Source; }
            set { _Source = value; }
        }

        ObservableCollection<Tutorial> Tutorials { get; set; }

        public TutorialView()
        {
            InitializeComponent();
            BindingContext = model = new TutorialViewModel();
            InitializeCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeCollection()
        {
            header.Text = $"Step {carousel.Position + 1}";

            Tutorials = new ObservableCollection<Tutorial>();

            

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 1",
                Illustration = "TutStep2",
                Info = "Insert new mouthpiece.",
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 2",
                Illustration = "TutStep3",
                Info = "Hold the device with your hand firmly grasping the grip area.",
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 3",
                Illustration = "TutStep1",
                Info = "Make sure patient is seated,\nSitting in an upright position.",
            });

            Tutorials.Add(new Tutorial()
            {
                Title = "Step 4",
                Illustration = "TutStep4",
                Info = "Breathe in deeply and tightly seal your mouth around the mouthpiece.",
            });
            
            Tutorials.Add(new Tutorial()
            {
                Title = "Step 5",
                Illustration = "TutStep5",
                Info = "Exhale steadily. This moves the needle in the direction of the white dot at the center of the meter.\n\nExhaling to reach the white star is your goal.",
                ShowStep5 = false,
                ShowStep6 = false,
                ShowStep7 = true,
            });

            carousel.ItemsSource = Tutorials;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNext(object sender, EventArgs e)
        {
            if (carousel.Position + 1 < Tutorials.Count)
            {
                GotoPostion(carousel.Position+1);
            } else {
                GotoPostion(Tutorials.Count - 1);
                Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TutorialSuccessView)}?source={Source}"), false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBack(object sender, EventArgs e)
        {
            if (carousel.Position > 0)
            {
                GotoPostion(carousel.Position - 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postion"></param>
        public void GotoPostion(int postion)
        {
            carousel.Position = postion;
            model.Position = carousel.Position;
            header.Text = $"Step {carousel.Position + 1}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancelled(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{Source}"), false);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
            GotoPostion(0);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
            GotoPostion(0);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }
    }
}