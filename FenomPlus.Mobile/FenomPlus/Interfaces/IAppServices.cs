
namespace FenomPlus.Interfaces
{
    public interface IAppServices
    {
        IConfigService Config { get; }
        IBleHubService BleHub { get; }
        ICacheService Cache { get; }
        IDatabaseService Database { get; }
        IDebugLogFileService DebugLogFile { get; }
        ILogCatService LogCat { get; }
        INavigationService Navigation { get; }
        IUsbDeviceService Usb { get; }
    }
}
