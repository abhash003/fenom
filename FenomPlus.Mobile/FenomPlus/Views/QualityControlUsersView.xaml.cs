using System;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class QualityControlUsersView : BaseContentPage
    {
        private QualityControlUsersViewModel model;

        /// <summary>
        /// 
        /// </summary>
        public QualityControlUsersView()
        {
            InitializeComponent();
            BindingContext = model = new QualityControlUsersViewModel();
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
            QualityControlUsersDataModel dataModel = (QualityControlUsersDataModel)dataGrid.SelectedItem;
            bool answer = await DisplayAlert("Delete User", "Are you sure you want to delete user " + dataModel.User, "Yes, Delete", "Cancel");
            if (answer == true)
            {
                QCUsersRepo.Delete(dataModel);
                model.UpdateGrid();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenew(object sender, EventArgs e)
        {
            dataGrid.SelectedItem = (sender as Button).BindingContext;
            QualityControlUsersDataModel dataModel = (QualityControlUsersDataModel)dataGrid.SelectedItem;
            Services.Cache.QCUsername = dataModel.User;
            // goto test now
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnAddNew(System.Object sender, System.EventArgs e)
        {
            // prompt for usere here
            string userName = await DisplayPromptAsync("User Name", "Type a user name here");
            if (string.IsNullOrEmpty(userName)) return;

            // try to find if user is already in database
            QualityControlUsersTb user = QCUsersRepo.FindUser(userName);
            if (user == null)
            {
                QualityControlUsersDBModel userDBModel = new Models.QualityControlUsersDBModel() {
                    User = userName,
                    QCStatus = "Conditional",
                    DateAdded = DateTime.Now.ToString()
                };
                QualityControlUsersTb record = QCUsersRepo.Insert(userDBModel);
                userDBModel = record.Convert();
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
