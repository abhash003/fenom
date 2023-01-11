#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion



#region Namespace Aliases


using FenomPlus.Services.DeviceService.Interfaces;

#endregion


namespace FenomPlus.Services.DeviceService.Abstract
{
    public abstract class DeviceDiscoverer : IDeviceDiscoverer
    {
        // Fields

        protected DeviceService _deviceService;

        // Constructor

        public DeviceDiscoverer(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // Interface implementation

        public abstract void StartDiscovery();

        public abstract void StopDiscoveryAsync();
    }
}

