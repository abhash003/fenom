using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BreathManeuverFeedbackView : BaseContentPage
    {
        private readonly BreathManeuverFeedbackViewModel BreathManeuverFeedbackViewModel;

        public BreathManeuverFeedbackView()
        {
            InitializeComponent();
            BindingContext = BreathManeuverFeedbackViewModel = new BreathManeuverFeedbackViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BreathManeuverFeedbackViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            BreathManeuverFeedbackViewModel.OnDisappearing();
        }

    }
}
