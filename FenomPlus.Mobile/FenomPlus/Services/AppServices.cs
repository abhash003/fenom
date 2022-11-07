using System;
using FenomPlus.Interfaces;
using TinyIoC;

namespace FenomPlus.Services
{
    public class AppServices : IAppServices
    {
        protected IBleHubService _BleHub;
        public IBleHubService BleHub
        {
            get => _BleHub ??= Container.Resolve<IBleHubService>();
            set => _BleHub = value;
        }

        protected IConfigService _Config;
        public IConfigService Config
        {
            get => _Config ??= Container.Resolve<IConfigService>();
            set { _Config = value; }
        }

        protected ICacheService _Cache;
        public ICacheService Cache
        {
            get => _Cache ??= Container.Resolve<ICacheService>();
            set => _Cache = value;
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

        protected IUsbDeviceService _Usb;
        public IUsbDeviceService Usb
        {
            get => _Usb ??= Container.Resolve<IUsbDeviceService>();
            set => _Usb = value;
        }

        public static TinyIoCContainer Container => TinyIoCContainer.Current;

        public AppServices()
        {
            try
            {
                Container.Register<IConfigService, ConfigService>().AsSingleton();
                Container.Register<IBleHubService, BleHubService>().AsSingleton();
                Container.Register<ICacheService, CacheService>().AsSingleton();
                Container.Register<IDatabaseService, DatabaseService>().AsSingleton();
                Container.Register<IDebugLogFileService, DebugLogFileService>().AsSingleton();
                Container.Register<ILogCatService, LogCatService>().AsSingleton();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}