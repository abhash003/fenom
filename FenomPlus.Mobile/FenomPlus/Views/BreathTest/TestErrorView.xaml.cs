using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class TestErrorView : BaseContentPage
    {
        private readonly TestErrorViewModel TestErrorViewModel;

        public TestErrorView()
        {
            InitializeComponent();
            BindingContext = TestErrorViewModel = new TestErrorViewModel();
        }

        private async void GoToTutorial(object sender, EventArgs e)
        {
            await TestErrorViewModel.Services.Navigation.TutorialView();
        }

        private async void StartTest(object sender, EventArgs e)
        {
            await TestErrorViewModel.Services.Navigation.ChooseTestView();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            TestErrorViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            TestErrorViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            TestErrorViewModel.NewGlobalData();
        }
    }
}
