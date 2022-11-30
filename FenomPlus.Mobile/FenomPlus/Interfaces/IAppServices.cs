
namespace FenomPlus.Interfaces
{
    public interface IAppServices
    {
        IConfigService Config { get; }
        IFenomDeviceService FenomDeviceService { get; }
        IBleHubService BleHub { get; }
        ICacheService Cache { get; }
        IDialogService Dialogs { get; }
        IDatabaseService Database { get; }
        IDebugLogFileService DebugLogFile { get; }
        ILogCatService LogCat { get; }
        INavigationService Navigation { get; }
    }
}
