using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicePowerOnView : BaseContentPage
    {
        private DevicePowerOnViewModel DevicePowerOnViewModel;

        public DevicePowerOnView()
        {
            InitializeComponent();

            BindingContext = DevicePowerOnViewModel = new DevicePowerOnViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DevicePowerOnViewModel.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DevicePowerOnViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            DevicePowerOnViewModel.NewGlobalData();
        }
    }
}
