using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class NegativeControlFailView : BaseContentPage
    {
        private readonly NegativeControlFailViewModel NegativeControlFailViewModel;

        public NegativeControlFailView()
        {
            InitializeComponent();
            BindingContext = NegativeControlFailViewModel = new NegativeControlFailViewModel();
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
