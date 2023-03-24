using FenomPlus.Controls;
using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreparingStandardTestResultView : BaseContentPage
    {
        private PreparingStandardTestResultViewModel model;

        public PreparingStandardTestResultView()
        {
            InitializeComponent();
            BindingContext = model = new PreparingStandardTestResultViewModel();
            MarigoldProgressWheel.Callback = model.Callback;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();

            // Because view is not created for each use we need to reset the animation.
            MarigoldProgressWheel.StartAnimation();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
