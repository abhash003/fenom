#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion


using FenomPlus.Services.DeviceService.Abstract;

namespace FenomPlus.Services.DeviceService.Concrete
{
    public class UsbEnumerator : DeviceDiscoverer
    {
        // Fields

        // will have to create a wrapper for platform agnostic usb
        // protected IUsbManager _usb = null;

        // Consturctors

        public UsbEnumerator(DeviceService deviceService) : base(deviceService)
        {
        }

        // Methods

        public override void StartDiscovery()
        {
            //throw new NotImplementedException();
        }

        public override void StopDiscoveryAsync()
        {
            //throw new NotImplementedException();
        }
    }
}
