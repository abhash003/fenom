
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
using FenomPlus.Helpers;
using Syncfusion.Drawing;
using Syncfusion.SfDataGrid.XForms;

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

#if false
            AllDevicesDataGrid.GridStyle = new CustomGridStyle();
            AllDevicesDataGrid.GridStyle.GridCellBorderWidth = 1;
            AllDevicesDataGrid.RowHeight = 40;


            AllUsersDataGrid.GridStyle = new CustomGridStyle();
            AllUsersDataGrid.GridStyle.GridCellBorderWidth = 1;
            AllUsersDataGrid.RowHeight = 40;

            AllTestsDataGrid.GridStyle = new CustomGridStyle();
            AllTestsDataGrid.GridStyle.GridCellBorderWidth = 1;
            AllTestsDataGrid.RowHeight = 40;
#endif
        }

        protected override void OnAppearing()
        {
#if false
            SettingsTabView.SelectedIndex = 0;

            QualityControlViewModel.UpdateQcDeviceList();
            QualityControlViewModel.UpdateQcUserList();
            QualityControlViewModel.UpdateQcTestList();

            //
            ToggleSwitch.IsToggled = Services.DeviceService.Current.IsQCEnabled();
#endif
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
#if false
            switch (SettingsTabView.SelectedIndex)
            {
                case 0:
                    QualityControlViewModel.UpdateQcDeviceList();
                    AllDevicesDataGrid.RefreshColumns();
                    break;

                case 1:
                    QualityControlViewModel.UpdateQcUserList();
                    AllUsersDataGrid.RefreshColumns();
                    break;

                case 2:
                    QualityControlViewModel.UpdateQcTestList();
                    AllTestsDataGrid.RefreshColumns();
                    break;

                default:
                    // Nothing to do here
                    break;
            }
#endif
        }


        private void AllDevicesDataGrid_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //DeleteDeviceButton.IsEnabled = AllDevicesDataGrid.SelectedIndex >= 0;
        }

        private void AllUsersDataGrid_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //DeleteUserButton.IsEnabled = AllUsersDataGrid.SelectedIndex >= 0;
        }


        private void DeleteDeviceButton_OnClicked(object sender, EventArgs e)
        {
            //QualityControlViewModel.DeleteDeviceCommand.Execute(AllDevicesDataGrid.SelectedIndex);
            //AllDevicesDataGrid.Refresh();
        }

        private void DeleteUserButton_OnClicked(object sender, EventArgs e)
        {
            //QualityControlViewModel.DeleteUserCommand.Execute(AllUsersDataGrid.SelectedIndex);
            //AllUsersDataGrid.Refresh();
        }
    }
}