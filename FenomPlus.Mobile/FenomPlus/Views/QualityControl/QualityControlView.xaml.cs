using System;
using System.Collections.Generic;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class QualityControlView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QualityControlView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = new QualityControlViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            dataGrid.SelectedItem = (sender as Button).BindingContext;
            QualityControlDataModel dataModel = (QualityControlDataModel)dataGrid.SelectedItem;
            bool answer = await DisplayAlert("Delete Test Record", "Are you sure you want to delete record?", "Yes, Delete", "Cancel");
            if (answer == true)
            {
                QualityControlViewModel.QCRepo.Delete(dataModel);
                QualityControlViewModel.UpdateGrid();
            }
        }

        public async void OnAddNew(System.Object sender, System.EventArgs e)
        {
            IEnumerable<QualityControlUsersTb> records = QualityControlViewModel.QCUsersRepo.SelectAll();
            List<string> userRecords = new List<string>();
            foreach (QualityControlUsersTb userRecord in records)
            {
                userRecords.Add(userRecord.User);
            }

            if (userRecords.Count <= 0) return;

            string userName = await DisplayActionSheet("Select User", "Cancel", "", userRecords.ToArray());
            if (string.IsNullOrEmpty(userName)) return;

            // ok goto test, for now create random
            QualityControlViewModel.Services.Cache.QCUsername = userName;

            // send QC model
            await QualityControlViewModel.BleHub.StartTest(BreathTestEnum.QualityControl);

            //ok goto test now.
            await QualityControlViewModel.Services.Navigation.NegativeControlPerformView();

            QualityControlViewModel.UpdateGrid();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            QualityControlViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            QualityControlViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            QualityControlViewModel.NewGlobalData();
        }
    }
}
