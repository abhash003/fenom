
using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using FenomPlus.Controls;
using FenomPlus.ViewModels.QualityControl;
using Xamarin.Forms.Xaml;
using FenomPlus.Services;
using Xamarin.CommunityToolkit.UI.Views;

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
            SettingsTabView.SelectedIndex = 0;
            QualityControlViewModel.UpdateAllQcDevicesCommand.Execute(null);

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

        private void SettingsTabView_OnSelectionChanged(object sender, TabSelectionChangedEventArgs e)
        {
            switch (SettingsTabView.SelectedIndex)
            {
                case 0:
                    QualityControlViewModel.UpdateAllQcDevicesCommand.Execute(null);
                    break;

                case 1:
                    QualityControlViewModel.UpdateAllQcUsersCommand.Execute(null);
                    break;

                case 2:
                    QualityControlViewModel.UpdateAllQcTestsCommand.Execute(null);
                    break;

                default:
                    // Nothing to do here
                    break;
            }
        }
    }
}