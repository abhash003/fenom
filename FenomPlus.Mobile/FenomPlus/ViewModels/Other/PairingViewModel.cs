

namespace FenomPlus.ViewModels
{
    public class PairingViewModel : BaseViewModel
    {
        public PairingViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            Services.Navigation.GotoBluetoothSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
