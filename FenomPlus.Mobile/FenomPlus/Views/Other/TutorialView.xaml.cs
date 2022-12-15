using System;
using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialView : BaseContentPage
    {
        private readonly TutorialViewModel TutorialViewModel;

        public TutorialView()
        {
            InitializeComponent();
            BindingContext = TutorialViewModel = new TutorialViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            TutorialViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            TutorialViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            TutorialViewModel.NewGlobalData();
        }
    }
}