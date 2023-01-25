using System;
using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
            await TestErrorViewModel.Services.Navigation.DashboardView();
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
        }
    }
}
