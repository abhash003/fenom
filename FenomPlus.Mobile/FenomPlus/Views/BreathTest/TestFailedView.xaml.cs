using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class TestFailedView : BaseContentPage
    {
        private readonly TestFailedViewModel TestFailedViewModel;

        public TestFailedView()
        {
            InitializeComponent();
            BindingContext = TestFailedViewModel = new TestFailedViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            TestFailedViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            TestFailedViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            TestFailedViewModel.NewGlobalData();
        }
    }
}
