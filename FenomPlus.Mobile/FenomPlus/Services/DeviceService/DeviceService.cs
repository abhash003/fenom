#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
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

        //Task _monitorTask;
        //CancellationTokenSource _monitorTaskCancelSource;

        Thread _workerThread;

        Action<IDevice> _deviceFoundAction;

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

                _deviceFoundAction = null;

                ReadyForTest = true;
                DeviceReadyTimer = new Timer(1000);
                DeviceReadyTimer.Elapsed += DeviceReadyTimerOnElapsed;

                trace.Trace("device service all setup");
            }
        }

        // Properties

        public bool Discovering => _discovering;
        public IList<IDevice> Devices => _devices;
        public IDevice Current { get => _current; set => _current = value; }

        // Methods

        public void StartDiscovery(Action<IDevice> deviceFoundAction)
        {
            if (!_discovering)
            {
                foreach (IDeviceDiscoverer dd in _deviceDiscoverers)
                {
                    dd.StartDiscovery();
                }

                _shouldStopDiscovering = false;
                _deviceFoundAction = deviceFoundAction;

                //_workerThread = new Thread(new ThreadStart(DiscoveryWorker));
                //_workerThread.Start();

                _discovering = true;
            }
        }

        public void StopDiscovery()
        {
            if (_workerThread != null && _discovering)
            {
                _shouldStopDiscovering = true;
                //_workerThread?.Join();
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

        protected void DiscoveryWorker()
        {
            Helper.WriteDebug("ENTERING MONITOR THREAD");

            try
            {
                //_devices.Clear();
                _discovering = true;

                while (_shouldStopDiscovering == false)
                {
#if false
                    if (((CancellationTokenSource)cancelToken).IsCancellationRequested)
                    {
                        Helper.WriteDebug("CANCELLATION SIGNALED FOR MONITOR THREAD");
                        break;
                    }
#endif

                    Helper.WriteDebug("HEART BEAT");
                    Thread.Sleep(10000);
                }

                // cleanup

                _discovering = false;
                //_shouldStopDiscovering = true;
            }
            catch (ThreadAbortException ex)
            {
                // thread aborted
                Helper.WriteDebug("ABORTED MONITOR THREAD " + ex.Message);
            }

            // exit monitor
            Helper.WriteDebug("EXITING MONITOR THREAD");
        }


        // Events
        public event EventHandler DeviceConnected;
        public event EventHandler DeviceConnectionLost;
        public event EventHandler DeviceDisconnected;
        public event EventHandler DeviceDiscovered;

        internal void HandleDeviceFound(BleDevice bleDevice)
        {
            Helper.WriteDebug("Invoking DeviceFound");
            _deviceFoundAction?.Invoke((IDevice)bleDevice);
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

        private readonly Timer DeviceReadyTimer;

        #endregion

        public int DeviceReadyCountDown { get; set; }

        private bool _readyForTest;
        public bool ReadyForTest
        {
            get => _readyForTest;
            set
            {
                _readyForTest = value;

                if (_readyForTest == false)
                {
                    DeviceReadyCountDown = 32;
                    DeviceReadyTimer.Start();
                }
            }
        }

        private void DeviceReadyTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Helper.WriteDebug($"DeviceReadyTimerOnElapsed: DeviceReadyCountDown: {DeviceReadyCountDown}, ReadyForTest: {ReadyForTest}");
            DeviceReadyCountDown -= 1;

            if (DeviceReadyCountDown <= 0)
            {
                ReadyForTest = true;
                DeviceReadyTimer.Stop();
            }
        }

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

