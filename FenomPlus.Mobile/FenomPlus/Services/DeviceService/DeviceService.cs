#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Collections.Generic;
using FenomPlus.Interfaces;


#region Namespace Aliases

using Timer = System.Timers.Timer;
using System.Collections.ObjectModel;
using FenomPlus.Services.DeviceService.Interfaces;
using FenomPlus.Services.DeviceService.Utils;
using IDevice = FenomPlus.Services.DeviceService.Interfaces.IDevice;
using IDeviceService = FenomPlus.Services.DeviceService.Interfaces.IDeviceService;
using FenomPlus.Services.DeviceService.Concrete;
using FenomPlus.Services.DeviceService.Abstract;
using Device = FenomPlus.Services.DeviceService.Abstract.Device;
using System.Timers;
using System.Threading.Tasks;

#endregion


namespace FenomPlus.Services.DeviceService
{

    public class DeviceService : BaseService, IDeviceService
    {
        // Fields

        protected volatile bool _discovering;
        protected volatile bool _shouldStopDiscovering;

        // not thread safe, needs to be ...
        protected volatile ObservableCollection<IDevice> _devices;

        protected IDevice _current;

        protected List<DeviceDiscoverer> _deviceDiscoverers;


        // public bool DeviceNotFound => _deviceNotFound;
        private bool _deviceNotFound;
        public bool DeviceNotFound
        {
            get { return _deviceNotFound; }
            set
            {
                if (value == _deviceNotFound) return;
                DiscoveryWorkerTimer.Enabled = value;
            }
        }


        // Constructors

        public DeviceService(IAppServices appServices) : base(appServices)
        {
            using (var trace = new Helper.FunctionTrace())
            {
                _discovering = false;
                _shouldStopDiscovering = false;

                _devices = new ObservableCollection<IDevice>();

                _deviceDiscoverers = new List<DeviceDiscoverer> { new BleScanner(this), new UsbEnumerator(this) };

                _current = null;

                DeviceDiscovered += DeviceDiscoveredHandler;

                DiscoveryWorkerTimer = new Timer(15_000) { Enabled = true };
                DiscoveryWorkerTimer.Elapsed += Timer_Elapsed;

                trace.Trace("device service all setup");
            }
        }
        ~DeviceService()
        {
            DeviceDiscovered -= DeviceDiscoveredHandler;
        }
        void DeviceDiscoveredHandler(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    Services.DeviceService.StopDiscovery();
                    var ea = (DeviceServiceEventArgs)e;
                    Helper.WriteDebug("Device discovered.");
                    await ea.Device.ConnectAsync();
                    // await FoundDevice(ea.Device);
                }
                catch (Exception ex)
                {                
                    Helper.WriteDebug($"{DateTime.Now.Millisecond} : Exception at DeviceDiscoveredHandler ConnectAsync: " + ex.Message);
                    Helper.WriteDebug("Exception at DeviceDiscoveredHandler: " + ex.Message);
                }
            });
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool connected = Current?.Connected ?? false;
            if (!connected && !Discovering)
                StartDiscoveryBackground();
        }

        // Properties

        public bool Discovering => _discovering;
        public IList<IDevice> Devices => _devices;
        public IDevice Current { get => _current; set => _current = value; }

        // Methods

        public void StartDiscovery()
        {
            if (!_discovering)
            {
                StartDiscoveryBackground();
                _shouldStopDiscovering = false;
                _discovering = true;
            }
        }
        public void StartDiscoveryBackground()
        {
            foreach (IDeviceDiscoverer dd in _deviceDiscoverers)
            {
                dd.StartDiscovery();
            }
        }

        public void StopDiscovery()
        {
            if (_discovering)
            {
                _shouldStopDiscovering = true;
            }

            foreach (var dd in _deviceDiscoverers)
            {
                if (dd is BleScanner)
                {
                    dd.StopDiscoveryAsync();
                }
            }

            _discovering = false;
        }

        // Events
        public event EventHandler DeviceConnected;
        public event EventHandler DeviceConnectionLost;
        public event EventHandler DeviceDisconnected;
        public event EventHandler DeviceDiscovered;

        internal void HandleDeviceFound(BleDevice bleDevice)
        {
            Helper.WriteDebug("Invoking DeviceFound");
            // _deviceFoundAction?.Invoke((IDevice)bleDevice);
            Helper.WriteDebug("Invoking DeviceFound finshed");
        }

        internal void HandleDeviceConnected(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceConnected");
            DeviceConnected?.Invoke(this, new DeviceServiceEventArgs(device));
        }
        internal void HandleDeviceConnectionLost(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceConnectionLost");
            DeviceConnectionLost?.Invoke(this, new DeviceServiceEventArgs(device));
        }

        internal void HandleDeviceDisconnected(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceDisconnected");
            DeviceDisconnected?.Invoke(this, new DeviceServiceEventArgs(device));
        }
        internal void HandleDeviceDiscovered(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceDiscovered");
               DeviceDiscovered?.Invoke(this, new DeviceServiceEventArgs(device));
        }

        #region Fields

        private Timer DiscoveryWorkerTimer;

        #endregion
        public bool IsDeviceFenomDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                return false;
            }

            return deviceName.ToLower().StartsWith("fenom") || deviceName.ToLower().ToLower().StartsWith("fp");
        }

        public Device GetBondedOrPairedFenomDevices()
        {
            if (Devices != null)
            {
                if (Devices.Count > 0)
                {
                    foreach (IDevice device in Devices)
                    {
                        // Get the first BleDevice
                        if (device is BleDevice)
                        {
                            return (Device)device;
                        }
                    }
                }
            }
            return null;
        }
    }
}

