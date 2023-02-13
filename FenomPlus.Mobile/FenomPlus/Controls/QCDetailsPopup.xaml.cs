using FenomPlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FenomPlus.Controls
{
    public partial class QCDetailsPopup : Popup
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCDetailsPopup(QualityControlViewModel viewModel)
        {
            InitializeComponent();
            QualityControlViewModel = viewModel;
            BindingContext = QualityControlViewModel;

            //MoreHelpButton.IsVisible = !string.IsNullOrEmpty(QualityControlViewModel.HelpLink);
        }

        private void CloseButton_OnClicked(object sender, System.EventArgs e)
        {
            Dismiss(null);
        }

        //private async void MoreHelpButton_OnClicked(object sender, EventArgs e)
        //{
        //    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        //    {
        //        Uri uri = new Uri(StatusButtonViewModel.HelpLink);
        //        await Browser.OpenAsync(uri);
        //    }
        //    else
        //    {
        //        StatusButtonViewModel.Services.Dialogs.ShowAlert(
        //            "Internet connection currently not available.",
        //            "Connection Error",
        //            "Exit");
        //    }
        //}
    }
}
