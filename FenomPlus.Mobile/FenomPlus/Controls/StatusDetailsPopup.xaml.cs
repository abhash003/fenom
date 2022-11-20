using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FenomPlus.Controls
{
    public partial class StatusDetailsPopup : Popup
    {
        private readonly StatusInfoBoxViewModel StatusInfoBoxViewModel;

        public StatusDetailsPopup(StatusInfoBoxViewModel viewModel)
        {
            InitializeComponent();
            StatusInfoBoxViewModel = viewModel;
            BindingContext = StatusInfoBoxViewModel;

            MoreHelpButton.IsVisible = !string.IsNullOrEmpty(StatusInfoBoxViewModel.HelpLink);
        }

        private void CloseButton_OnClicked(object sender, System.EventArgs e)
        {
            Dismiss(null);
        }

        private async void MoreHelpButton_OnClicked(object sender, EventArgs e)
        {
            Uri uri = new Uri(StatusInfoBoxViewModel.HelpLink);
            await Browser.OpenAsync(uri);
        }
    }
}
