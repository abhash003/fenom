using System;
using FenomPlus.Interfaces;
using TinyIoC;

namespace FenomPlus.Services
{
    public class AppServices : IAppServices
    {
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

        /// <summary>
        /// 
        /// </summary>
        public static TinyIoCContainer Container
        {
            get { return TinyIoCContainer.Current; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IBleHubService _BleHub;
        public IBleHubService BleHub
        {
            get { return _BleHub ?? (_BleHub = Container.Resolve<IBleHubService>()); }
            set { _BleHub = value; }
        }

        /// <summary>
        /// 
        /// </summary>


        ///
        protected IConfigService _Config;
        public IConfigService Config
        {
            get { return _Config ?? (_Config = Container.Resolve<IConfigService>()); }
            set { _Config = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ICacheService _Cache;
        public ICacheService Cache
        {
            get { return _Cache ?? (_Cache = Container.Resolve<ICacheService>()); }
            set { _Cache = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IDatabaseService _Database;
        public IDatabaseService Database
        {
            get { return _Database ?? (_Database = Container.Resolve<IDatabaseService>()); }
            set { _Database = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IDebugLogFileService _DebugLogFile;
        public IDebugLogFileService DebugLogFile
        {
            get { return _DebugLogFile ?? (_DebugLogFile = Container.Resolve<IDebugLogFileService>()); }
            set { _DebugLogFile = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ILogCatService _LogCat;
        public ILogCatService LogCat
        {
            get { return _LogCat ?? (_LogCat = Container.Resolve<ILogCatService>()); }
            set { _LogCat = value; }
        }

        protected INavigationService _Navigation;
        public INavigationService Navigation
        {
            get { return _Navigation ?? (_Navigation = Container.Resolve<INavigationService>()); }
            set { _Navigation = value; }
        }

        protected IUsbDeviceService _Usb;
        public IUsbDeviceService Usb
        {
            get { return _Usb ?? (_Usb = Container.Resolve<IUsbDeviceService>()); }
            set { _Usb = value; }
        }
    }
}