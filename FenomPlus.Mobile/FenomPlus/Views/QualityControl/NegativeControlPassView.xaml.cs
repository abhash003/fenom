using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class NegativeControlPassView : BaseContentPage
    {
        private NegativeControlPassViewModel model;

        public NegativeControlPassView()
        {
            InitializeComponent();
            BindingContext = model = new NegativeControlPassViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
