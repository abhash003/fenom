using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FenomPlus.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace FenomPlus.Services
{
#if false
    public class DeviceService : BaseService, IDeviceService
    {
#region Member variables
        protected volatile bool _isRunning;
        protected IBluetoothLE _ble;
        Thread _workerThread;
#endregion

#region Constructors
        public DeviceService()
        {
            _ble = CrossBluetoothLE.Current;
            AssignEventHandlers();
        }

        public DeviceService(IAppServices services) : base(services)
        {
            AssignEventHandlers();

        }

        public DeviceService(IBluetoothLE ble)
        {
            _ble = ble;
            AssignEventHandlers();
        }
#endregion

#region Interface implementation (IDeviceService)

        public void Start()
        {
            if (_workerThread == null)
            {
                _workerThread = new Thread(Worker);
            }

            _workerThread.Start();
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _workerThread.Join();
            }
        }

#endregion

        private void AssignEventHandlers()
        {
            _ble.StateChanged += (object sender, BluetoothStateChangedArgs e) =>
            {
#if DEBUG
                Console.WriteLine("BLE state changed");
#endif
            };

            _ble.Adapter.DeviceAdvertised += (object sender, DeviceEventArgs e) =>
            {

                // TODO: move this to the "filter" callback in the BLE scan call
                if (e.Device.Name != null && e.Device.Name.ToLower().Contains("fenom"))
                {
                    Console.WriteLine("BLE adapter DeviceAdvertised: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);

                    _ble.Adapter.ConnectToDeviceAsync(e.Device);
                }
                else
                {
#if DEBUG
                    Console.WriteLine("BLE adapter DeviceAdvertised: " + e.Device.Name);
#endif
                }
            };

            _ble.Adapter.DeviceConnected += (object sender, DeviceEventArgs e) =>
            {
#if DEBUG
                Console.WriteLine("BLE adapter DeviceConnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
#endif
            };

            _ble.Adapter.DeviceConnectionLost += (object sender, DeviceErrorEventArgs e) =>
            {
#if DEBUG
                Console.WriteLine("BLE adapter DeviceConnectionLost: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
#endif
            };

            _ble.Adapter.DeviceDisconnected += (object sender, DeviceEventArgs e) =>
            {
#if DEBUG
                Console.WriteLine("BLE adapter DeviceDisconnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
#endif
            };

            _ble.Adapter.DeviceDiscovered += (object sender, DeviceEventArgs e) =>
            {

                if (e.Device.Name != null && e.Device.Name.ToLower().Contains("fenom"))
                {
                    Console.WriteLine("BLE adapter DeviceDiscovered: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                }
                else
                {
#if DEBUG
                    Console.WriteLine("BLE adapter DeviceDiscovered: " + e.Device.Name);
#endif
                }
            };
        }

        protected void Worker()
        {
            _isRunning = true;

            int count = 0;

            Console.WriteLine("begin thread" + _isRunning.ToString());

            while (_isRunning)
            {
                if (!_ble.Adapter.IsScanning && _ble.Adapter.ConnectedDevices.Count == 0)
                {
                    ScanForBleDevicesAsync();
                }

                // TODO: add contiional USB enumeration flags here
                if (true)
                {
                    _ = ScanForUsbDevicesAsync();
                }

#if DEBUG
                Console.WriteLine("_isRunning = " + _isRunning.ToString() + " : " + count);
#endif

                // sleep for a while, don't be a CPU hog
                Thread.Sleep(1000);

                ++count;
            }
#if DEBUG
            Console.WriteLine("end thread: " + _isRunning.ToString());
#endif
        }

        protected void ScanForBleDevicesAsync()
        {
            _ble.Adapter.StartScanningForDevicesAsync();
        }
        protected void ScanForUsbDevicesAsync()
        {
            // enumerate USB devices here
        }
    }
#endif
}

namespace FenomPlus.Services.NewArch
{
    public interface IDeviceService
    {
        List<IDevice> AvailableDevices { get; }


    }

    public class DeviceService : BaseService, IDeviceService, IDisposable
    {
        #region Properties

        List<IDevice> _availableDevices = new List<IDevice>();
        public List<IDevice> AvailableDevices
        {
            get { return _availableDevices; }
        }

        IDevice _currentDevice = null;
        public IDevice CurrentDevice { get => _currentDevice; }

        #endregion

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
                if (((CancellationTokenSource)cancelToken).IsCancellationRequested)
                {
                    // cleanup
                    break;
                }

                //ScanFakeDevices();
                ScanBleDevices();
                //ScanUsbDevices();

                // do work here (one million iterations)
                //Thread.SpinWait(1000000);

                Console.WriteLine("Geo");
                Thread.Sleep(1000);
            }
        }

    #endregion

        #region BLE

        Plugin.BLE.Abstractions.Contracts.IBluetoothLE _ble = null;
        //Plugin.BLE.Abstractions.Contracts.IAdapter _bleAdapter = null;
        void SetupBLE()
        {
            _ble = CrossBluetoothLE.Current;
            //_bleAdapter = _ble.Adapter;

            AssignEventHandlers();
        }

        private void WriteDebug(String msg)
        {
#if DEBUG
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + msg);
#endif
        }

        void AssignEventHandlers()
        {
            _ble.StateChanged += (object sender, BluetoothStateChangedArgs e) =>
            {
                WriteDebug("BLE state changed");
            };

            _ble.Adapter.DeviceAdvertised += (object sender, DeviceEventArgs e) =>
            {
                // TODO: move this to the "filter" callback in the BLE scan call
                if (e.Device.Name != null && e.Device.Name.ToLower().Contains("fenom"))
                {
                    WriteDebug("BLE adapter DeviceAdvertised: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);

                    _ble.Adapter.ConnectToDeviceAsync(e.Device);
                }
                else
                {
                    WriteDebug("BLE adapter DeviceAdvertised: " + e.Device.Name);
                }
            };

            _ble.Adapter.DeviceConnected += (object sender, DeviceEventArgs e) =>
            {
                WriteDebug("BLE adapter DeviceConnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
            };

            _ble.Adapter.DeviceConnectionLost += (object sender, DeviceErrorEventArgs e) =>
            {
                WriteDebug("BLE adapter DeviceConnectionLost: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
            };

            _ble.Adapter.DeviceDisconnected += (object sender, DeviceEventArgs e) =>
            {
                WriteDebug("BLE adapter DeviceDisconnected: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
            };

            _ble.Adapter.DeviceDiscovered += (object sender, DeviceEventArgs e) =>
            {

                if (e.Device.Name != null && e.Device.Name.ToLower().Contains("fenom"))
                {
                    WriteDebug("BLE adapter DeviceDiscovered: " + e.Device.Name + " : RSSI: " + e.Device.Rssi);
                }
                else
                {
                    WriteDebug("BLE adapter DeviceDiscovered: " + e.Device.Name);
                }
            };
        }

        protected void ScanBleDevices()
        {
            if (_ble.Adapter.IsScanning)
                return;

            var scanFilterOptions = new Plugin.BLE.Abstractions.ScanFilterOptions();

            scanFilterOptions = null;

            CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync();

            /*
            CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync(
                scanFilterOptions,
                (Plugin.BLE.Abstractions.Contracts.IDevice device) =>
                {
                    return true;
                },
                false,
                default
            );
            */
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
        #region Properties

        bool Connected { get;  }


        #endregion
    }
    public class Device : IDevice
    {
        #region Properties

        protected bool _connected = false;
        public bool Connected { get => _connected; }

        #endregion
    }

    public class BleDevice : Device
    {
        
    }

    public class UsbDevice : Device
    {
    }
}
