using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class HumanControlPerformingView : BaseContentPage
    {
        private readonly HumanControlPerformingViewModel HumanControlPerformingViewModel;

        public HumanControlPerformingView()
        {
            InitializeComponent();
            BindingContext = HumanControlPerformingViewModel = new HumanControlPerformingViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            HumanControlPerformingViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HumanControlPerformingViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            HumanControlPerformingViewModel.NewGlobalData();
        }
    }
}
