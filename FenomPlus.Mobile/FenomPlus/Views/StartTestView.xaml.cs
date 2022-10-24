using System;
using System.Collections.Generic;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    [QueryProperty(nameof(Test), "test")]
    public partial class StartTestView : BaseContentPage
    {
        private StartTestViewModel model;

        private string _Test;
        public string Test
        {
            get { return _Test; }
            set { _Test = value; }
        }

        public StartTestView()
        {
            InitializeComponent();
            BindingContext = model = new StartTestViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoToTutorial(object sender, EventArgs e)
        {
            await Services.Navigation.TutorialView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancel(object sender, EventArgs e)
        {
            await Services.Navigation.ChooseTestView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartTest(object sender, EventArgs e)
        {
            // ok send test type here
            // wait until breath here

            await Services.Navigation.BreathManeuverFeedbackView();
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