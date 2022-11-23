using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class TestResultsView : BaseContentPage
    {
        private readonly TestResultsViewModel TestResultsViewModel;

        public TestResultsView()
        {
            InitializeComponent();
            BindingContext = TestResultsViewModel = new TestResultsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            TestResultsViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            TestResultsViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            TestResultsViewModel.NewGlobalData();
        }
    }
}

