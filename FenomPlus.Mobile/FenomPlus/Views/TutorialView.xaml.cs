using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class TutorialView : BaseContentPage
    {
        private TutorialViewModel model;

        public TutorialView()
        {
            InitializeComponent();
            BindingContext = model = new TutorialViewModel();
        }

        private async void OnCancelled(object sender, EventArgs e)
        {
            await Services.Navigation.ChooseTestView();
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