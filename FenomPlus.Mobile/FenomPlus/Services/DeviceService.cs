#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FenomPlus.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;


#region Namespace Aliases

using PlginBleIDevice = Plugin.BLE.Abstractions.Contracts;

using aliasNamespace = Plugin.BLE;

using PluginBleIDevice = Plugin.BLE.Abstractions.Contracts.IDevice;

#endregion

namespace FenomPlus.Services.NewArch
{
    public interface IDeviceService
    {
        #region Properties: Configuration
        int MinRssi { get; set; }
        int MaxRssi { get; set; }
        #endregion

        #region Properties: Status
        bool IsScanning { get; }
        bool IsConnected { get; }
        #endregion

        #region Properties: Operational
        abstract List<IDevice> AvailableDevices { get; }
        abstract IDevice CurrentDevice { get; }
        #endregion
    }

    public class DeviceService : BaseService, IDeviceService, IDisposable
    {
        #region Creation / Disposal

        public DeviceService()
        {
            SetupBLE();
            SetupUSB();
            StartMonitor();
        }

        public void Dispose()
        {
            StopMonitor();
        }

        #endregion

        #region Fields
        bool _scanningBLE;
        bool _scanningUSB;
        #endregion

        #region Properties: Operational
        List<IDevice> _availableDevices = new List<IDevice>();
        public List<IDevice> AvailableDevices
        {
            get { return _availableDevices; }
        }
        IDevice _currentDevice = null;
        public IDevice CurrentDevice { get => _currentDevice; }
        #endregion

        #region Properties: Configuration
        int _minRssi = -100;
        public int MinRssi { get => _minRssi; set => _minRssi = value; }
        int _maxRssi = +100;
        public int MaxRssi { get => _maxRssi; set => _maxRssi = value; }
        #endregion

        #region Properties: Status
        public bool IsScanning => (bool)(_ble?.Adapter.IsScanning) || /*usbIsScanning*/ false;
        public bool IsConnected { get { foreach (var d in _availableDevices) { if (d.Connected) return true; }; return false; } }
        #endregion

        #region Monitoring

        Task _monitorTask;
        CancellationTokenSource _monitorTaskCancelSource;

        protected void StartMonitor()
        {
            if (_monitorTaskCancelSource == null)
            {
                _monitorTaskCancelSource = new CancellationTokenSource();
                _monitorTask = new Task(MonitorTask, _monitorTaskCancelSource);
                _monitorTask.Start();
            }
        }

        protected void StopMonitor()
        {
            _monitorTaskCancelSource.Cancel();
        }

        protected void MonitorTask(object cancelToken)
        {
            while (true)
            {
                // scan for fake devices
                /*
                if (!_scanningFake)
                {
                    ScanFakeDevices();
                }
                */

                // scan for ble devices
                if (!_scanningBLE)
                {
                    ScanBleDevices();
                }
                else
                {
                    WriteDebug("Already scanning for BLE");
                }

                // scan for usb devices
                if (!_scanningUSB)
                {
                    //ScanUsbDevices();
                }
                else
                {
                    WriteDebug("Already scanning for USB");
                }

                // wait some time, dont be a cpu hog ...
                Thread.Sleep(1000);

                if (((CancellationTokenSource)cancelToken).IsCancellationRequested)
                {
                    break;
                }
            }

            // cleanup

            // exit monitor
        }

    #endregion

        #region BLE

        Plugin.BLE.Abstractions.Contracts.IBluetoothLE _ble = null;

        void SetupBLE()
        {
            _ble = CrossBluetoothLE.Current;
            AssignEventHandlers();
        }

        private static void WriteDebug(String msg, int prependCount = 20, char prependChar = '$')
        {
#if DEBUG
            Console.WriteLine(String.Format("{0} : [{1}] - {2}", new string(prependChar, prependCount), Thread.CurrentThread.ManagedThreadId, msg));
#endif
        }

        volatile int _state = 0;

        private bool IsDeviceMatch(PluginBleIDevice device)
        {
            if ((device.Name != null) && device.Name.ToLower().Contains("fenom"))
            {
                return true;
            }
            
            return false;
        }
        void AssignEventHandlers()
        {
            _ble.StateChanged += (object sender, BluetoothStateChangedArgs e) =>
            {
                WriteDebug("BLE state changed");
            };

            _ble.Adapter.DeviceAdvertised += (object sender, DeviceEventArgs e) =>
            {
#if DS_FILTER_DEVICES_ON_ADVERTS

                if (IsDeviceMatch(e.Device) && NotInList(e.Device))
                {
                    _availableDevices.Add(new BleDevice(e.Device));
                    _currentDevice = _availableDevices[0];
                }

#else
                WriteDebug(String.Format("Begin DeviceAdvertised: State == {0}", _state));
                WriteDeviceConnectionState(e.Device);
                WriteDebug(String.Format("End DeviceAdvertised: State == {0}", _state));
#endif

            };

            _ble.Adapter.DeviceConnected += (object sender, DeviceEventArgs e) =>
            {
                WriteDebug(String.Format("Begin DeviceConnected: State == {0}", _state));
                WriteDeviceConnectionState(e.Device);
                WriteDebug("BLE adapter DeviceConnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                WriteDebug(String.Format("End DeviceConnected: State == {0}", _state));
                _state = 1;
            };

            _ble.Adapter.DeviceConnectionLost += (object sender, DeviceErrorEventArgs e) =>
            {
                WriteDebug(String.Format("Begin DeviceConnectionLost: State == {0}", _state));
                WriteDeviceConnectionState(e.Device);
                WriteDebug("BLE adapter DeviceConnectionLost: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                WriteDebug(String.Format("End DeviceConnectionLost: State == {0}", _state));
                _state = 2;
            };

            _ble.Adapter.DeviceDisconnected += (object sender, DeviceEventArgs e) =>
            {
                WriteDebug(String.Format("Begin DeviceDisconnected: State == {0}", _state));
                WriteDebug("BLE adapter DeviceDisconnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                WriteDeviceConnectionState(e.Device);
                WriteDebug(String.Format("Begin DeviceDisconnected: State == {0}", _state));
                _state = 3;
            };

            bool NotInList(PluginBleIDevice device)
            {
                foreach (IDevice d in _availableDevices)
                {
                    if (((PluginBleIDevice)d.TransportDevice).Name == device.Name)
                    {
                        return false;
                    }
                }
                return true;
            }

            _ble.Adapter.DeviceDiscovered += async (object sender, DeviceEventArgs e) =>
            {
                WriteDebug(String.Format("DeviceDiscovered: State == {0}", _state));

                if (IsDeviceMatch(e.Device) && NotInList(e.Device))
                {
                    WriteDebug("BLE adapter DeviceDiscovered: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                    WriteDeviceConnectionState(e.Device);

                    //await _ble.Adapter.ConnectToDeviceAsync(e.Device);
                    //_state = 4;

                    //IDevice device = new BleDevice(e.Device);
                    // add device to list ...
                }
                else
                {
                    WriteDebug("BLE adapter DeviceDiscovered: " + e.Device.Name);
                }
            };
        }

        private static void WriteDeviceConnectionState(PluginBleIDevice device)
        {
            switch (device.State)
            {
                case Plugin.BLE.Abstractions.DeviceState.Connected:
                    WriteDebug("Device Connection State: Connected");
                    break;

                case Plugin.BLE.Abstractions.DeviceState.Connecting:
                    WriteDebug("Device Connection State: Connecting");
                    break;

                case Plugin.BLE.Abstractions.DeviceState.Disconnected:
                    WriteDebug("Device Connection State: Disconnected");
                    break;

                case Plugin.BLE.Abstractions.DeviceState.Limited:
                    WriteDebug("Device Connection State: Limited");
                    break;
            }
        }

        protected void ScanBleDevices()
        {
            if (_ble.Adapter.IsScanning || _ble.Adapter.ConnectedDevices.Count > 0)
                return;

#if DS_FILTER_DEVICES_ON_ADVERTS

            CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync();

#else
            var scanFilterOptions = new Plugin.BLE.Abstractions.ScanFilterOptions();

            scanFilterOptions = null;

            CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync(
                scanFilterOptions,
                (Plugin.BLE.Abstractions.Contracts.IDevice device) =>
                {
                    using (var ft = new FuncTrace())
                    {
                        WriteDebug("Begin DeviceConnectFilter: ");

                        bool nameCheck = (device.Name != null) && (device.Name.ToLower().Contains("fenom"));
                        bool rssiCheck = ((device.Rssi >= MinRssi && device.Rssi <= MaxRssi) == false);

                        if (nameCheck && rssiCheck)
                        {
                            return true;
                        }

                        WriteDebug("End DeviceConnectFilter: ");

                        //ft.Dispose();
                        return false;
                    }

                },
                false,
                default
            );
#endif
        }

        public class FuncTrace : IDisposable
        {
            public FuncTrace()
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!! bueno");
            }

            public void Dispose()
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!! adios");
            }
        }

#endregion // BLE

        #region USB
        void SetupUSB()
        {

        }

        protected async void ScanUsbDevices()
        {
        }
#endregion
    }

    public interface IDevice
    {
#region Enumerations (enums)
        enum TransportTypes
        {
            None,
            BLE,
            USB
        }
#endregion

        #region Properties

        object TransportDevice { get; }
        TransportTypes TransportType { get; }

        bool Connected { get; }

#if false
        int Humidity { get; }
        int ReadyForTest { get; }
        int TestInProgress { get; }
        int TimeRemaining { get; }
        int LastTestResult { get; }
        int BatteryLevel { get; }

        /*
        Temperature;
         Humidity;
         Pressure;
         BatteryLevel; 
        */

        #region Event Handlers
        void OnDeviceInfo();
        void OnEnvironmentInfo();
        void OnBreathManeuver();
        #endregion

        #region Operations

        void StartTest();
        void StopTest();
        //... etc

        void SendMessage();
        #endregion
#endif

        #endregion
        Task ConnectAsync();


        // events

        event EventHandler<EventArgs> OnConnected;
        //event EventHandler<EventArgs> OnDisconnected;
        //event EventHandler<EventArgs> OnConnectionLost;

        event EventHandler<EventArgs> OnDeviceInfo;

    }

    public abstract class Device : IDevice
    {
        #region Fields
        #endregion

        #region Properties

        protected object _transportDevice = null;
        public virtual object TransportDevice { get => _transportDevice; }

        protected IDevice.TransportTypes _transportType = IDevice.TransportTypes.None;
        public abstract IDevice.TransportTypes TransportType { get; }

        protected bool _connected = false;
        public bool Connected { get => _connected; }

        #endregion

        public abstract Task ConnectAsync();

        // events

        public virtual event EventHandler<EventArgs> OnConnected;
        //protected abstract event EventHandler<EventArgs> OnDisconnected;
        //protected abstract event EventHandler<EventArgs> OnConnectionLost;

    }

    public class BleDevice : Device
    {
        #region Constructors
        public BleDevice(PluginBleIDevice device)
        {
            _transportDevice = device;
        }
        #endregion

        #region Properties:
        public override IDevice.TransportTypes TransportType { get => IDevice.TransportTypes.BLE; }

        #endregion

        #region Operations

        public override async Task ConnectAsync()
        {
            var d = (PluginBleIDevice)_transportDevice;
            await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(d);
            _connected = true;
            OnConnected?.Invoke(this, new EventArgs());
        }

        #endregion

        public override event EventHandler<EventArgs> OnConnected;
        //public override event EventHandler<EventArgs> OnDisconnected;
        //public override event EventHandler<EventArgs> OnConnectionLost;

    }

    public class UsbDevice : Device
    {
        #region Properties
        
        public override IDevice.TransportTypes TransportType => (_transportDevice == null) ? IDevice.TransportTypes.None : IDevice.TransportTypes.USB;

        #endregion

        public override Task ConnectAsync() => throw new NotImplementedException();

        //public override event EventHandler<EventArgs> OnConnected;
        //public override event EventHandler<EventArgs> OnDisconnected;
        //public override event EventHandler<EventArgs> OnConnectionLost;

    }
}
