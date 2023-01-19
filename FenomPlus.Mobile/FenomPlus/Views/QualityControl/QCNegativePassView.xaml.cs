using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class QCNegativePassView : BaseContentPage
    {
        private QualityControlViewModel model;

        public QCNegativePassView()
        {
            InitializeComponent();
            BindingContext = model = new QualityControlViewModel();
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
            model.NewGlobalData();
        }
    }
}
