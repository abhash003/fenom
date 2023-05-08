using FenomPlus.Services;
using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserTestView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCUserTestView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //QualityControlViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Services.DeviceService.Current.BreathFlowChanged -=  QualityControlViewModel.Cache_BreathFlowChanged;
            base.OnDisappearing();
            //QualityControlViewModel.OnDisappearing();
        }
    }
}
