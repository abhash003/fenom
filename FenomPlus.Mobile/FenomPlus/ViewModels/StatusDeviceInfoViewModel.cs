namespace FenomPlus.ViewModels
{
    public class StatusDeviceInfoViewModel : BaseViewModel
    {
        public StatusDeviceInfoViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            Services.BleHub.IsNotConnectedRedirect();
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
