

namespace FenomPlus.ViewModels
{
    public class PairingViewModel : BaseViewModel
    {
        public PairingViewModel()
        {
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            Services.Navigation.GotoBluetoothSettings();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
