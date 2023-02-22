using FenomPlus.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;

namespace FenomPlus.Controls
{
    public partial class QCChartPopup : Popup
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCChartPopup(QualityControlViewModel viewModel)
        {
            InitializeComponent();
            QualityControlViewModel = viewModel;
            BindingContext = QualityControlViewModel;
        }

        private void CloseButton_OnClicked(object sender, System.EventArgs e)
        {
            Dismiss(null);
        }
    }
}
