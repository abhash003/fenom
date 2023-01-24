using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using FenomPlus.Controls;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QualityControlView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QualityControlView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = new QualityControlViewModel();

            NegativeControlButton.BindingContext = QualityControlViewModel.QcNegativeControlViewModel;

            User1Button.BindingContext = QualityControlViewModel.QcUser1ViewModel;
            User2Button.BindingContext = QualityControlViewModel.QcUser2ViewModel;
            User3Button.BindingContext = QualityControlViewModel.QcUser3ViewModel;
            User4Button.BindingContext = QualityControlViewModel.QcUser4ViewModel;
            User5Button.BindingContext = QualityControlViewModel.QcUser5ViewModel;
            User6Button.BindingContext = QualityControlViewModel.QcUser6ViewModel;

            QcSettings.BindingContext = QualityControlViewModel.ImageButtonViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refresh Data - ToDo: Later optimize and only refresh when needed?
            QualityControlViewModel.LoadData();
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
