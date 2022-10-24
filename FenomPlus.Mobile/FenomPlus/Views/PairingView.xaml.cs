using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class PairingView : BaseContentPage
    {
        private PairingViewModel model;

        public PairingView()
        {
            InitializeComponent();
            BindingContext = model = new PairingViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
            OnExistToDashboard(this, null);
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
