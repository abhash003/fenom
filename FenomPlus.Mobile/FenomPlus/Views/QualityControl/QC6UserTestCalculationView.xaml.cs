using FenomPlus.Services;
using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserTestCalculationView : BaseContentPage
    {
        private QualityControlViewModel QualityControlViewModel;

        public QCUserTestCalculationView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            QualityControlViewModel.StartUserTestCalculations();

            // Because view is not created for each use we need to reset the animation
            MarigoldProgressWheel.StartOrResumeAnimation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
