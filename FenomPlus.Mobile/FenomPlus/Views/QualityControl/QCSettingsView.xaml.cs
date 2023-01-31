using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using FenomPlus.Controls;
using FenomPlus.ViewModels.QualityControl;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCSettingsView : BaseContentPage
    {
        private readonly QCSettingsViewModel QCSettingsViewModel;

        public QCSettingsView()
        {
            InitializeComponent();
            BindingContext = QCSettingsViewModel = new QCSettingsViewModel();
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
