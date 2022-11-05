using FenomPlus.Helpers;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class ViewRecentErrorsView : BaseContentPage
    {
        private ViewRecentErrorsViewModel ViewRecentErrorsViewModel;

        public ViewRecentErrorsView()
        {
            InitializeComponent();
            BindingContext = ViewRecentErrorsViewModel = new ViewRecentErrorsViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewRecentErrorsViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewRecentErrorsViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            ViewRecentErrorsViewModel.NewGlobalData();
        }
    }
}
