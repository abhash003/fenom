
namespace FenomPlus.Interfaces
{
    public interface IAppServices
    {
        IConfigService Config { get; }
        IBleHubService BleHub { get; }
        ICacheService Cache { get; }
        IDialogService Dialogs { get; }
        IDatabaseService Database { get; }
        IDebugLogFileService DebugLogFile { get; }
        ILogCatService LogCat { get; }
        INavigationService Navigation { get; }
        IDeviceService Device { get; }
    }
}
