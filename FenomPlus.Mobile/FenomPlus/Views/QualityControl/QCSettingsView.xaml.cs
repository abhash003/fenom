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
