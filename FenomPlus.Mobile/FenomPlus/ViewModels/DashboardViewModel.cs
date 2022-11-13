
namespace FenomPlus.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {

        public DashboardViewModel()
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
