
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

            AllDevicesDataGrid.GridStyle = new CustomGridStyle()
            {
                GridCellBorderWidth = 1,
                HeaderCellBorderWidth = (float)Density
            };
            AllDevicesDataGrid.RowHeight = 40;


            AllUsersDataGrid.GridStyle = new CustomGridStyle()
            {
                GridCellBorderWidth = 1,
                HeaderCellBorderWidth = (float)Density
            };
            AllUsersDataGrid.RowHeight = 40;

            AllTestsDataGrid.GridStyle = new CustomGridStyle()
            {
                GridCellBorderWidth = 1,
                HeaderCellBorderWidth = (float)Density
            };
            
            AllTestsDataGrid.RowHeight = 40;
        }

        protected override void OnAppearing()
        {
            SettingsTabView.SelectedIndex = 0;

            QualityControlViewModel.UpdateQcDeviceList();
            QualityControlViewModel.UpdateQcUserList();
            QualityControlViewModel.UpdateQcTestList();

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
        }


        private void AllDevicesDataGrid_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            DeleteDeviceButton.IsEnabled = AllDevicesDataGrid.SelectedIndex >= 0;
        }

        private void AllUsersDataGrid_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            DeleteUserButton.IsEnabled = AllUsersDataGrid.SelectedIndex >= 0;
        }


        private void DeleteDeviceButton_OnClicked(object sender, EventArgs e)
        {
            QualityControlViewModel.DeleteDeviceCommand.Execute(AllDevicesDataGrid.SelectedIndex);
            AllDevicesDataGrid.Refresh();
        }

        private void DeleteUserButton_OnClicked(object sender, EventArgs e)
        {
            QualityControlViewModel.DeleteUserCommand.Execute(AllUsersDataGrid.SelectedIndex);
            AllUsersDataGrid.Refresh();
        }
    }
}