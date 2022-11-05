
namespace FenomPlus.ViewModels
{
    public class ChooseTestViewModel : BaseViewModel
    {

        public ChooseTestViewModel()
        {
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            Services.BleHub.IsConnected();
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
