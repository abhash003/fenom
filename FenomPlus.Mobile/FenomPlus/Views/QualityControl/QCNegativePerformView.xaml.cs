﻿using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class QCNegativePerformView : BaseContentPage
    {
        private readonly NegativeControlPerformViewModel NegativeControlPerformViewModel;

        public QCNegativePerformView()
        {
            InitializeComponent();
            BindingContext = NegativeControlPerformViewModel = new NegativeControlPerformViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NegativeControlPerformViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NegativeControlPerformViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            NegativeControlPerformViewModel.NewGlobalData();
        }
    }
}
