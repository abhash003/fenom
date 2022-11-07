using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class PreparingStandardTestResultView : BaseContentPage
    {
        private PreparingStandardTestResultViewModel model;

        public PreparingStandardTestResultView()
        {
            InitializeComponent();
            BindingContext = model = new PreparingStandardTestResultViewModel();
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
            model.NewGlobalData();
        }
    }
}
