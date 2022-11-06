using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class StartTestView : BaseContentPage
    {
        private readonly StartTestViewModel StartTestViewModel;

        public StartTestView()
        {
            InitializeComponent();
            BindingContext = StartTestViewModel = new StartTestViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartTestViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StartTestViewModel.OnDisappearing();
        }

        private async void GoToTutorial(object sender, EventArgs e)
        {
            await StartTestViewModel.Services.Navigation.TutorialView();
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            await StartTestViewModel.Services.Navigation.ChooseTestView();
        }

        private async void StartTest(object sender, EventArgs e)
        {
            // ok send test type here
            // wait until breath here

            await StartTestViewModel.Services.Navigation.BreathManeuverFeedbackView();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            StartTestViewModel.NewGlobalData();
        }
    }
}