using System;
using FenomPlus.Interfaces;
using FenomPlus.Services.DeviceService.Interfaces;
using FenomPlus.ViewModels;
using TinyIoC;

namespace FenomPlus.Services
{
    public class AppServices : IAppServices
    {
        public static TinyIoCContainer Container => TinyIoCContainer.Current;
        
        protected IDeviceService _deviceService;
        public IDeviceService DeviceService
        {
            get => _deviceService ??= Container.Resolve<IDeviceService>();
            set => _deviceService = value;
        }

        protected IConfigService _Config;
        public IConfigService Config
        {
            get => _Config ??= Container.Resolve<IConfigService>();
            set => _Config = value;
        }

        protected ICacheService _Cache;
        public ICacheService Cache
        {
            get => _Cache ??= Container.Resolve<ICacheService>();
            set => _Cache = value;
        }

        protected IDialogService _Dialogs;
        public IDialogService Dialogs
        {
            get => _Dialogs ??= Container.Resolve<IDialogService>();
            set => _Dialogs = value;
        }

        protected IDatabaseService _Database;
        public IDatabaseService Database
        {
            get { return _Database ??= Container.Resolve<IDatabaseService>(); }
            set => _Database = value;
        }

        protected IDebugLogFileService _DebugLogFile;
        public IDebugLogFileService DebugLogFile
        {
            get { return _DebugLogFile ??= Container.Resolve<IDebugLogFileService>(); }
            set => _DebugLogFile = value;
        }

        protected ILogCatService _LogCat;
        public ILogCatService LogCat
        {
            get => _LogCat ??= Container.Resolve<ILogCatService>();
            set => _LogCat = value;
        }

        protected INavigationService _Navigation;
        public INavigationService Navigation
        {
            get => _Navigation ??= Container.Resolve<INavigationService>();
            set => _Navigation = value;
        }

        

        public AppServices()
        {
            try
            {
                Container.Register<IConfigService, ConfigService>().AsSingleton();
                Container.Register<IDeviceService, DeviceService.DeviceService>().AsSingleton();
                Container.Register<ICacheService, CacheService>().AsSingleton();
                Container.Register<IDialogService, DialogService>().AsSingleton();
                Container.Register<IDatabaseService, DatabaseService>().AsSingleton();
                Container.Register<IDebugLogFileService, DebugLogFileService>().AsSingleton();
                Container.Register<ILogCatService, LogCatService>().AsSingleton();

                // We only want one instance of this - includes its own timer
                Container.Register<StatusViewModel>().AsSingleton();
                Container.Register<QualityControlViewModel>().AsSingleton();

                Container.Register<PastResultsViewModel>();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}