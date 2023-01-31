using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserTestCalculationView : BaseContentPage
    {
        private QCUserTestCalculationViewModel QCUserTestCalculationViewModel;

        public QCUserTestCalculationView()
        {
            InitializeComponent();
            BindingContext = QCUserTestCalculationViewModel = new QCUserTestCalculationViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            QCUserTestCalculationViewModel.OnAppearing();

            // Because view is not created for each use we need to reset the animation.
            MarigoldProgressWheel.StartAnimation();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            QCUserTestCalculationViewModel.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
