using FenomPlus.Helpers;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class ViewRecentErrorsView : BaseContentPage
    {
        private ViewRecentErrorsViewModel model;

        public ViewRecentErrorsView()
        {
            InitializeComponent();
            BindingContext = model = new ViewRecentErrorsViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
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
