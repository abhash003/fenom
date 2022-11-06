using System;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class QualityControlDevicesView : BaseContentPage
    {
        private readonly QualityControlDevicesViewModel QualityControlDevicesViewModel;

        public QualityControlDevicesView()
        {
            InitializeComponent();
            BindingContext = QualityControlDevicesViewModel = new QualityControlDevicesViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            dataGrid.SelectedItem = (sender as Button).BindingContext;
            QualityControlDevicesDataModel dataModel = (QualityControlDevicesDataModel)dataGrid.SelectedItem;
            bool answer = await DisplayAlert("Delete Device", "Are you sure you want to delete device " + dataModel.SerialNumber, "Yes, Delete", "Cancel");
            if (answer == true)
            {
                QualityControlDevicesViewModel.QCDevicesRepo.Delete(dataModel);
                QualityControlDevicesViewModel.UpdateGrid();
            }
        }

        public async void OnAddNew(System.Object sender, System.EventArgs e)
        {
            // prompt for usere here
            string serialNumber = await DisplayPromptAsync("Device", "Type a serial number name here");
            if (string.IsNullOrEmpty(serialNumber)) return;

            // try to find if user is already in database
            QualityControlDevicesTb device = QualityControlDevicesViewModel.QCDevicesRepo.FindDevice(serialNumber);
            if (device == null)
            {
                QualityControlDeviceDBModel deviceDBModel = new Models.QualityControlDeviceDBModel()
                {
                    SerialNumber = serialNumber,
                    LastConnected = DateTime.Now.ToString()
                };
                QualityControlDevicesTb record = QualityControlDevicesViewModel.QCDevicesRepo.Insert(deviceDBModel);
                deviceDBModel = record.Convert();
                QualityControlDevicesViewModel.UpdateGrid();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            QualityControlDevicesViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            QualityControlDevicesViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            QualityControlDevicesViewModel.NewGlobalData();
        }
    }
}
