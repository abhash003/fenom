using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FenomPlus.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace FenomPlus.Services
{
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

        protected async Task ScanForBleDevicesAsync()
        {
            await _ble.Adapter.StartScanningForDevicesAsync();
        }
        protected async Task ScanForUsbDevicesAsync()
        {
            // enumerate USB devices here
        }
    }
}
