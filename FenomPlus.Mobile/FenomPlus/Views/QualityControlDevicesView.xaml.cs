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
        private QualityControlDevicesViewModel model;

        /// <summary>
        /// 
        /// </summary>
        public QualityControlDevicesView()
        {
            InitializeComponent();
            BindingContext = model = new QualityControlDevicesViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDelete(object sender, EventArgs e)
        {
            dataGrid.SelectedItem = (sender as Button).BindingContext;
            QualityControlDevicesDataModel dataModel = (QualityControlDevicesDataModel)dataGrid.SelectedItem;
            bool answer = await DisplayAlert("Delete Device", "Are you sure you want to delete device " + dataModel.SerialNumber, "Yes, Delete", "Cancel");
            if (answer == true)
            {
                QCDevicesRepo.Delete(dataModel);
                model.UpdateGrid();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnAddNew(System.Object sender, System.EventArgs e)
        {
            // prompt for usere here
            string serialNumber = await DisplayPromptAsync("Device", "Type a serial number name here");
            if (string.IsNullOrEmpty(serialNumber)) return;

            // try to find if user is already in database
            QualityControlDevicesTb device = QCDevicesRepo.FindDevice(serialNumber);
            if (device == null)
            {
                QualityControlDeviceDBModel deviceDBModel = new Models.QualityControlDeviceDBModel()
                {
                    SerialNumber = serialNumber,
                    LastConnected = DateTime.Now.ToString()
                };
                QualityControlDevicesTb record = QCDevicesRepo.Insert(deviceDBModel);
                deviceDBModel = record.Convert();
                model.UpdateGrid();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }
    }
}
