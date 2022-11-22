using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using FenomPlus.Controls;

namespace FenomPlus.Views
{
    public partial class QualityControlView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QualityControlView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = new QualityControlViewModel();

            NegativeControlButton.BindingContext = QualityControlViewModel.NegativeControlViewModel;

            User1Button.BindingContext = QualityControlViewModel.QcUser1ViewModel;
            User2Button.BindingContext = QualityControlViewModel.QcUser2ViewModel;
            User3Button.BindingContext = QualityControlViewModel.QcUser3ViewModel;
            User4Button.BindingContext = QualityControlViewModel.QcUser4ViewModel;
            User5Button.BindingContext = QualityControlViewModel.QcUser5ViewModel;
            User6Button.BindingContext = QualityControlViewModel.QcUser6ViewModel;
            ImageButton.BindingContext = QualityControlViewModel.ImageButtonViewModel;
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
