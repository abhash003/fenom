using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using FenomPlus.Controls;
using FenomPlus.ViewModels.QualityControl;
using Xamarin.Forms.Xaml;
using FenomPlus.Services;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCSettingsView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCSettingsView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();
        }

        public void DevicesButtonClicked(object sender, EventArgs eventArgs)
        {
            // Refresh the list of devices
            QualityControlViewModel.UpdateQcDevicesList();

            DevicesTabContent.IsVisible = true;
            UsersTabContent.IsVisible = false;
            TestsTabContent.IsVisible = false;
            DebugTabContent.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void UsersButtonClicked(object sender, EventArgs eventArgs)
        {
            DevicesTabContent.IsVisible = false;
            UsersTabContent.IsVisible = true;
            TestsTabContent.IsVisible = false;
            DebugTabContent.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void TestsButtonClicked(object sender, EventArgs eventArgs)
        {
            DevicesTabContent.IsVisible = false;
            UsersTabContent.IsVisible = false;
            TestsTabContent.IsVisible = true;
            DebugTabContent.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void DebugButtonClicked(object sender, EventArgs eventArgs)
        {
            DevicesTabContent.IsVisible = false;
            UsersTabContent.IsVisible = false;
            TestsTabContent.IsVisible = false;
            DebugTabContent.IsVisible = true;

            UpdateTabButtonBorder();
        }

        private void UpdateTabButtonBorder()
        {
            Tab1ButtonBorder.IsVisible = DevicesTabContent.IsVisible;
            Tab2ButtonBorder.IsVisible = UsersTabContent.IsVisible;
            Tab3ButtonBorder.IsVisible = TestsTabContent.IsVisible;
            Tab4ButtonBorder.IsVisible = DebugTabContent.IsVisible;
        }

        protected override void OnAppearing()
        {
            // Refresh the list of devices
            QualityControlViewModel.UpdateQcDevicesList();

            base.OnAppearing();


            // Set tab page
            DevicesTabContent.IsVisible = true;
            UsersTabContent.IsVisible = false;
            TestsTabContent.IsVisible = false;
            DebugTabContent.IsVisible = false;

            UpdateTabButtonBorder();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
