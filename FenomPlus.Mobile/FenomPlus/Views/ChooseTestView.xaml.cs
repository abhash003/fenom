using System;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;

namespace FenomPlus.Views
{
    public partial class ChooseTestView : BaseContentPage
    {
        private readonly ChooseTestViewModel ChooseTestViewModel;

        public ChooseTestView()
        {
            InitializeComponent();
            BindingContext = ChooseTestViewModel = new ChooseTestViewModel();
        }

        private async void OnStandardTest(object sender, EventArgs e)
        {
            if (ChooseTestViewModel.Services.BleHub.IsNotConnectedRedirect())
            {
                ChooseTestViewModel.Cache.TestType = TestTypeEnum.Standard;
                await ChooseTestViewModel.BleHub.StartTest(BreathTestEnum.Start10Second);
                await ChooseTestViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private async void OnShortTest(object sender, EventArgs e)
        {
            if (ChooseTestViewModel.Services.BleHub.IsNotConnectedRedirect())
            {
                ChooseTestViewModel.Cache.TestType = TestTypeEnum.Short;
                await ChooseTestViewModel.BleHub.StartTest(BreathTestEnum.Start6Second);
                await ChooseTestViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private async void OnTutorial(object sender, EventArgs e)
        {
            await ChooseTestViewModel.Services.Navigation.TutorialView();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ChooseTestViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ChooseTestViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            ChooseTestViewModel.NewGlobalData();
        }
    }
}
