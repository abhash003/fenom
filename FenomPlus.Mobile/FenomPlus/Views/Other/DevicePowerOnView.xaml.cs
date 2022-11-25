using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class DevicePowerOnView : BaseContentPage
    {
        private DevicePowerOnViewModel DevicePowerOnViewModel;

        public DevicePowerOnView()
        {
            InitializeComponent();

            BindingContext = DevicePowerOnViewModel = new DevicePowerOnViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DevicePowerOnViewModel.OnAppearing();

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DevicePowerOnViewModel.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            DevicePowerOnViewModel.NewGlobalData();
        }
    }
}
