using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class BreathManeuverFeedbackView : BaseContentPage
    {
        private BreathManeuverFeedbackViewModel model;

        public BreathManeuverFeedbackView()
        {
            InitializeComponent();
            BindingContext = model = new BreathManeuverFeedbackViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            Services.Navigation.ChooseTestView();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
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
