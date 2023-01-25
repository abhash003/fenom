using System.Collections.Generic;
using System.Net.Http.Headers;
using FenomPlus.Controls;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.ViewModels
{
    public class QualityControlViewModel : BaseViewModel
    {
        public QcButtonViewModel NegativeControlViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser1ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser2ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser3ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser4ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser5ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser6ViewModel = new QcButtonViewModel();
        public ImageButtonViewModel ImageButtonViewModel = new ImageButtonViewModel();

        public QualityControlViewModel()
        {
            NegativeControlViewModel.Header = "Negative Control";
            QcUser1ViewModel.Header = "QC User 1";
            QcUser2ViewModel.Header = "QC User 2";
            QcUser3ViewModel.Header = "QC User 3";
            QcUser4ViewModel.Header = "QC User 4";
            QcUser5ViewModel.Header = "QC User 5";
            QcUser6ViewModel.Header = "QC User 6";

            ImageButtonViewModel.ImageName = "TutStep2";

            NegativeControlViewModel.Assigned = true; // Always
            QcUser1ViewModel.Assigned = true;
            QcUser2ViewModel.Assigned = true;
            QcUser3ViewModel.Assigned = true;
            QcUser4ViewModel.Assigned = false;
            QcUser5ViewModel.Assigned = false;
            QcUser6ViewModel.Assigned = false;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        private void LoadData()
        {
            // ToDo: finish
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
       
    }
}
