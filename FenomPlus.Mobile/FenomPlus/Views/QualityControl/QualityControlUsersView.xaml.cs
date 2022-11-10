using System;
using System.Globalization;
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
        private readonly QualityControlUsersViewModel QualityControlUsersViewModel;

        public QualityControlUsersView()
        {
            InitializeComponent();
            BindingContext = QualityControlUsersViewModel = new QualityControlUsersViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            dataGrid.SelectedItem = (sender as Button).BindingContext;
            QualityControlUsersDataModel dataModel = (QualityControlUsersDataModel)dataGrid.SelectedItem;
            bool answer = await DisplayAlert("Delete User", "Are you sure you want to delete user " + dataModel.User, "Yes, Delete", "Cancel");
            if (answer == true)
            {
                QualityControlUsersViewModel.QCUsersRepo.Delete(dataModel);
                QualityControlUsersViewModel.UpdateGrid();
            }
        }

        private void OnRenew(object sender, EventArgs e)
        {
            dataGrid.SelectedItem = (sender as Button).BindingContext;
            QualityControlUsersDataModel dataModel = (QualityControlUsersDataModel)dataGrid.SelectedItem;
            QualityControlUsersViewModel.Services.Cache.QCUsername = dataModel.User;
            // goto test now
        }

        public async void OnAddNew(System.Object sender, System.EventArgs e)
        {
            // prompt for usere here
            string userName = await DisplayPromptAsync("User Name", "Type a user name here");
            if (string.IsNullOrEmpty(userName)) return;

            // try to find if user is already in database
            QualityControlUsersTb user = QualityControlUsersViewModel.QCUsersRepo.FindUser(userName);
            if (user == null)
            {
                QualityControlUsersDBModel userDBModel = new Models.QualityControlUsersDBModel() {
                    User = userName,
                    QCStatus = "Conditional",
                    DateAdded = DateTime.Now.ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture)
                };
                QualityControlUsersTb record = QualityControlUsersViewModel.QCUsersRepo.Insert(userDBModel);
                userDBModel = record.Convert();
                QualityControlUsersViewModel.UpdateGrid();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            QualityControlUsersViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            QualityControlUsersViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            QualityControlUsersViewModel.NewGlobalData();
        }
    }
}
