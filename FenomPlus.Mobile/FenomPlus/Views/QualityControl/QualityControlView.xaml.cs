 using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using FenomPlus.Controls;
using Xamarin.Forms.Xaml;
using FenomPlus.Services;
using FenomPlus.SDK.Core.Utils;

namespace FenomPlus.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QualityControlView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QualityControlView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Must reset on each new appearing
            QualityControlViewModel.SelectedUserIndex = -1;

            // Refresh Data - ToDo: Later optimize and only refresh when needed?
            QualityControlViewModel.LoadData();

            if (string.IsNullOrEmpty(QualityControlViewModel.CurrentDeviceSerialNumber))
                return;

            // Don't assign in case device is not connected
            NegativeControlButton.BindingContext = QualityControlViewModel.QcButtonViewModels[0];
            User1Button.BindingContext = QualityControlViewModel.QcButtonViewModels[1];
            User2Button.BindingContext = QualityControlViewModel.QcButtonViewModels[2];
            User3Button.BindingContext = QualityControlViewModel.QcButtonViewModels[3];
            User4Button.BindingContext = QualityControlViewModel.QcButtonViewModels[4];
            User5Button.BindingContext = QualityControlViewModel.QcButtonViewModels[5];
            User6Button.BindingContext = QualityControlViewModel.QcButtonViewModels[6];
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
