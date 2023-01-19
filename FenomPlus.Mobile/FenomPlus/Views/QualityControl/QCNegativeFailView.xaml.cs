using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class QCNegativeFailView : BaseContentPage
    {
        private readonly QualityControlViewModel NegativeControlFailViewModel;

        public QCNegativeFailView()
        {
            InitializeComponent();
            BindingContext = NegativeControlFailViewModel = new QualityControlViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NegativeControlFailViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NegativeControlFailViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            NegativeControlFailViewModel.NewGlobalData();
        }
    }
}
