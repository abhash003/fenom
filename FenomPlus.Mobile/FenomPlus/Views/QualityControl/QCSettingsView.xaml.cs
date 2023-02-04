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

            Tab1Content.IsVisible = true;
            Tab2Content.IsVisible = false;
            Tab3Content.IsVisible = false;
            Tab4Content.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void Tab1ButtonClicked(object sender, EventArgs eventArgs)
        {
            Tab1Content.IsVisible = true;
            Tab2Content.IsVisible = false;
            Tab3Content.IsVisible = false;
            Tab4Content.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void Tab2ButtonClicked(object sender, EventArgs eventArgs)
        {
            Tab1Content.IsVisible = false;
            Tab2Content.IsVisible = true;
            Tab3Content.IsVisible = false;
            Tab4Content.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void Tab3ButtonClicked(object sender, EventArgs eventArgs)
        {
            Tab1Content.IsVisible = false;
            Tab2Content.IsVisible = false;
            Tab3Content.IsVisible = true;
            Tab4Content.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void Tab4ButtonClicked(object sender, EventArgs eventArgs)
        {
            Tab1Content.IsVisible = false;
            Tab2Content.IsVisible = false;
            Tab3Content.IsVisible = false;
            Tab4Content.IsVisible = true;

            UpdateTabButtonBorder();
        }

        private void UpdateTabButtonBorder()
        {
            Tab1ButtonBorder.IsVisible = Tab1Content.IsVisible;
            Tab2ButtonBorder.IsVisible = Tab2Content.IsVisible;
            Tab3ButtonBorder.IsVisible = Tab3Content.IsVisible;
            Tab4ButtonBorder.IsVisible = Tab4Content.IsVisible;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
