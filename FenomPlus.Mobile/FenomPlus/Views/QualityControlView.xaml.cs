using System;
using System.Collections.Generic;
using FenomPlus.Database.Adapters;
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
        private QualityControlViewModel model;

        /// <summary>
        /// 
        /// </summary>
        public QualityControlView()
        {
            InitializeComponent();
            BindingContext = model = new QualityControlViewModel();
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
            QualityControlDataModel dataModel = (QualityControlDataModel)dataGrid.SelectedItem;
            bool answer = await DisplayAlert("Delete Test Record", "Are you sure you want to delete record?", "Yes, Delete", "Cancel");
            if (answer == true)
            {
                QCRepo.Delete(dataModel);
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
            IEnumerable<QualityControlUsersTb> records = QCUsersRepo.SelectAll();
            List<string> userRecords = new List<string>();
            foreach (QualityControlUsersTb userRecord in records)
            {
                userRecords.Add(userRecord.User);
            }

            if (userRecords.Count <= 0) return;

            string userName = await DisplayActionSheet("Select User", "Cancel", "", userRecords.ToArray());
            if (string.IsNullOrEmpty(userName)) return;

            // ok goto test, for now create random
            Services.Cache.QCUsername = userName;

            // send QC model
            await BleHub.StartTest(BreathTestEnum.QuailtyControl);

            //ok goto test now.
            await Services.Navigation.NegativeControlPerformView();

            model.UpdateGrid();
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
