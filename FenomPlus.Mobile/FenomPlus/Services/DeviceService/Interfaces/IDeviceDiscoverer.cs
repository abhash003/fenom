#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion



namespace FenomPlus.Services.DeviceService.Interfaces
{
    public interface IDeviceDiscoverer
    {
        // Methods

        void StartDiscovery();

        void StopDiscoveryAsync();
    }
}

