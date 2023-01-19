using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardView : BaseContentPage
    {
        private readonly DashboardViewModel DashboardViewModel;

        public DashboardView()
        {
            InitializeComponent();
            BindingContext = DashboardViewModel = new DashboardViewModel();
        }

        private void OnStandardTest(object sender, EventArgs e)
        {
            DashboardViewModel.StartStandardTestCommand.Execute(null);
        }

        private void OnShortTest(object sender, EventArgs e)
        {
            DashboardViewModel.StartShortTestCommand.Execute(null);
        }

        private async void OnTutorial(object sender, EventArgs e)
        {
            await DashboardViewModel.Services.Navigation.TutorialView();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DashboardViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (DashboardViewModel.Dialogs.SecondsProgressDialogShowing())
            {
                DashboardViewModel.Dialogs.DismissSecondsProgressDialog();
            }

            base.OnDisappearing();
            DashboardViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            DashboardViewModel.NewGlobalData();
        }
    }
}