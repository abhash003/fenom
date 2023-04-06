using FenomPlus.ViewModels;
using FenomPlus.ViewModels.QualityControl.Models;
using Xamarin.CommunityToolkit.UI.Views;

namespace FenomPlus.Controls
{
    public partial class QCChartPopup : Popup
    {
        public QCChartPopup(QualityControlViewModel viewModel, QCUser user)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Load Test Data for the specified user
            viewModel.InitializeUserDataForChart(user);
        }

        private void CloseButton_OnClicked(object sender, System.EventArgs e)
        {
            Dismiss(null);
        }
    }
}
