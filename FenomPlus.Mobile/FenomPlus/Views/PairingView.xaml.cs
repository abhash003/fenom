using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class PairingView : BaseContentPage
    {
        private PairingViewModel PairingViewModel;

        public PairingView()
        {
            InitializeComponent();
            BindingContext = PairingViewModel = new PairingViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            PairingViewModel.OnAppearing();
            PairingViewModel.ExitToDashboardCommand.Execute(null);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            PairingViewModel.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            PairingViewModel.NewGlobalData();
        }
    }
}
