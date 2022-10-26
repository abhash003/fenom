namespace FenomPlus.ViewModels
{
    public class StatusDeviceInfoViewModel : BaseViewModel
    {
        public StatusDeviceInfoViewModel()
        {
            DeviceSerialNumber = Services.Cache.DeviceSerialNumber;
            Firmware = Services.Cache.Firmware;
            DeviceConnectedStatus = Services.Cache.DeviceConnectedStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            //Services.BleHub.IsNotConnectedRedirect();
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
