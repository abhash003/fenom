
using CommunityToolkit.Mvvm.Input;

namespace FenomPlus.ViewModels
{
    public partial class NegativeControlPassViewModel : BaseViewModel
    {
        public NegativeControlPassViewModel()
        {
        }

        [RelayCommand]
        private async void Next()
        {
            await Services.Navigation.HumanControlPerformingView();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        
    }
}
