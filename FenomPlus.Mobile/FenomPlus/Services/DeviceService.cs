﻿#region Operational Defines

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
using System.Dynamic;
using Plugin.Settings;
using Syncfusion.Data;

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

        //Task ConnectAsync(IDevice device);
        //Task DisconnectAsync(IDevice device);

        //private EventHandler<EventArgs> _onConnected;
        //public event EventHandler<EventArgs> OnConnected;
    }

    public class DeviceService : BaseService, IDeviceService, IDisposable
    {
        /*
        private EventHandler<EventArgs> _onConnected;
        public event EventHandler<EventArgs> OnConnected
        {
            add { _onConnected += value; }
            remove { _onConnected -= value; }
        }
        */

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
                    WriteDebug("EXITING MONITOR THREAD");
                    break;
                }

                WriteDebug("HEART BEAT");
            }

            // cleanup

            // exit monitor
        }

    #endregion

        #region BLE

        Plugin.BLE.Abstractions.Contracts.IBluetoothLE _ble = null;

        private static int _count = 0;

        void SetupBLE()
        {
            _ble = CrossBluetoothLE.Current;

            // configure tracing for Plugin.BLE
            Plugin.BLE.Abstractions.Trace.TraceImplementation = (format, @params) =>
            {
                System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: {format}", @params);
            };

            AssignEventHandlers();
        }

        private static void WriteDebug(String msg, int prependCount = 20, char prependChar = '$')
        {
#if DEBUG
            Console.WriteLine(String.Format("{0} : [{1}] - {2}", new string(prependChar, prependCount), Thread.CurrentThread.ManagedThreadId, msg));
#endif
        }

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

            int _state = -1;

            _ble.Adapter.DeviceConnected += (object sender, DeviceEventArgs e) =>
            {
               _count += 1;

                WriteDebug(String.Format("Begin DeviceConnected: State == {0}", _state));
                WriteDeviceConnectionState(e.Device);
                WriteDebug("BLE adapter DeviceConnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                WriteDebug(String.Format("End DeviceConnected: State == {0}", _state));

                if (_currentDevice == null)
                {
                    // android: auto-connect happened without advertisement
                    _availableDevices.Add(new BleDevice(e.Device));
                    _currentDevice = _availableDevices[0];
                }

#if false
                foreach (var device in _availableDevices)
                {
                    if (device.TransportDevice == e.Device)
                    {
                        // early out if device already exists and is connected
                        // avoid double connection generated by Plugin BLE
                        if (device.Connected)
                            return;
                    }
                }
#endif
                if (_currentDevice.Connected == false)
                {
                    _currentDevice.Connected = true;
                }

                //_currentDevice.OnConnected?.Invoke(this, e);
            };

            _ble.Adapter.DeviceConnectionLost += (object sender, DeviceErrorEventArgs e) =>
            {
                // android: error 61 (mic (message integrity check) error), happens randomly

                // get status code from error message
                string[] words = e?.ErrorMessage?.Split(' ');
                int statusCode = (words != null) ? int.Parse(words[words.Length - 1]) : 0;

                WriteDebug(String.Format("Begin DeviceConnectionLost: State == {0}", _state));
                WriteDeviceConnectionState(e.Device);
                WriteDebug("BLE adapter DeviceConnectionLost: " + e.Device.Name + " : RSSI: " + e.Device.Rssi + " : " + e.ErrorMessage);
                WriteDebug(String.Format("End DeviceConnectionLost: State == {0}", _state));

                if (_currentDevice.Connected == true)
                {
                    _count = 0;
                    _currentDevice.Connected = false;
                }
            };

            _ble.Adapter.DeviceDisconnected += (object sender, DeviceEventArgs e) =>
            {
                WriteDebug(String.Format("Begin DeviceDisconnected: State == {0}", _state));
                WriteDebug("BLE adapter DeviceDisconnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                WriteDeviceConnectionState(e.Device);
                WriteDebug(String.Format("Begin DeviceDisconnected: State == {0}", _state));

                if (_currentDevice.Connected == true)
                {
                    _count = 0;
                    _currentDevice.Connected = false;
                }
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

        bool Connected { get; set; }

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
        void Disconnect();


        // events

        event EventHandler<EventArgs> OnConnected;
        event EventHandler<EventArgs> OnDisconnected;
        
        //event EventHandler<EventArgs> OnConnectionLost;
        //event EventHandler<EventArgs> OnDeviceInfo;
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

        public bool Connected
        {
            get => _connected;
            set
            {
                _connected = value;

                if (value == true)
                {
                    _onConnected?.Invoke(this, null);
                }
                else
                {
                    _onDisconnected?.Invoke(this, null);
                }
            }
        }

#endregion

#region Events

        private EventHandler<EventArgs> _onConnected;
        public event EventHandler<EventArgs> OnConnected
        {
            add { _onConnected += value; }
            remove { _onConnected -= value; }
        }

        private EventHandler<EventArgs> _onDisconnected;
        public event EventHandler<EventArgs> OnDisconnected
        {
            add { _onDisconnected += value; }
            remove { _onDisconnected -= value; }
        }

        //{
        //add { _onConnected += value; }
        //remove { _onConnected -= value; }
        //}

        // events
        //protected abstract event EventHandler<EventArgs> OnDisconnected;
        //protected abstract event EventHandler<EventArgs> OnConnectionLost;


#endregion

        public abstract Task ConnectAsync();

        public abstract void Disconnect();

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
            //OnConnected?.Invoke(this, new EventArgs());
        }

        public override void Disconnect()
        {
            var d = (PluginBleIDevice)_transportDevice;
            CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(d);
        }

#endregion
    }

    public class UsbDevice : Device
    {
#region Properties
        
        public override IDevice.TransportTypes TransportType => (_transportDevice == null) ? IDevice.TransportTypes.None : IDevice.TransportTypes.USB;

#endregion

        public override Task ConnectAsync() => throw new NotImplementedException();
        public override void Disconnect() => throw new NotImplementedException();
    }
}
