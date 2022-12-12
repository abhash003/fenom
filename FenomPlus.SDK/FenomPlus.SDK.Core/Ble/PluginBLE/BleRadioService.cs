using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FenomPlus.SDK.Core.Ble.PluginBLE
{
    public class BleRadioService : IBleRadioService
    {
        private static readonly IBluetoothLE Ble = CrossBluetoothLE.Current;
        private static readonly IAdapter Adapter = Ble.Adapter;

        private List<IBleDevice> _bondeddevices = new List<IBleDevice>();
        public IEnumerable<IBleDevice> BondedDevices { get { return _bondeddevices; } }

        private List<IBleDevice> _devices = new List<IBleDevice>();
        public IEnumerable<IBleDevice> Devices { get { return _devices; } }

        public bool IsScanning => Adapter.IsScanning;

        private EventHandler<DeviceEventArgs> _deviceAdvertised;
        private EventHandler<DeviceEventArgs> _deviceDiscovered;
        private EventHandler<DeviceEventArgs> _deviceConnected;
        private EventHandler<DeviceEventArgs> _deviceDisconnected;
        private EventHandler<DeviceErrorEventArgs> _deviceConnectionLost;

        private Logger _logger = new Logger(nameof(BleRadioService));

        /// <summary>
        /// 
        /// </summary>
        public BleRadioService()
        {
            //PerformanceLogger.StartLog(typeof(BleRadioService), "BleRadioService");

            Adapter.DeviceAdvertised += Adapter_DeviceAdvertised;
            Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            Adapter.DeviceConnected += Adapter_DeviceConnected;
            Adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
            Adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;
            //PerformanceLogger.EndLog(typeof(BleRadioService), "BleRadioService");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            _deviceDiscovered?.Invoke(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceAdvertised(object sender, DeviceEventArgs e)
        {
            _deviceAdvertised?.Invoke(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            _deviceConnectionLost?.Invoke(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            _deviceDisconnected?.Invoke(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceConnected(object sender, DeviceEventArgs e)
        {
            _deviceConnected?.Invoke(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DeviceEventArgs> DeviceAdvertised
        {
            add
            {
                _deviceAdvertised += value;
            }

            remove
            {
                _deviceAdvertised -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DeviceEventArgs> DeviceDiscovered
        {
            add
            {
                _deviceDiscovered += value;
            }

            remove
            {
                _deviceDiscovered -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DeviceEventArgs> DeviceConnected
        {
            add
            {
                _deviceConnected += value;
            }

            remove
            {
                _deviceConnected -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DeviceEventArgs> DeviceDisconnected
        {
            add
            {
                _deviceDisconnected += value;
            }

            remove
            {
                _deviceDisconnected -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> DeviceConnectionLost
        {
            add
            {
                _deviceConnectionLost += value;
            }

            remove
            {
                _deviceConnectionLost -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanTime"></param>
        /// <param name="deviceFoundCallback"></param>
        /// <param name="devicesFoundCallback"></param>
        /// <param name="scanTimeoutCallback"></param>
        /// <returns></returns>
        
        private static readonly Object m_Lock = new Object();
        
        /
        public async Task<bool> Scan(double scanTime, bool scanBondedDevices, bool scanBleDevices, Action<IBleDevice> deviceFoundCallback = null, Action<IEnumerable<IBleDevice>> scanCompletedCallback = null, Action scanTimeoutCallback = null)
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(BleRadioService), "Scan");
                if (IsScanning)
                {
                    return false;
                }

                if (scanTime != 0)
                {
                    Adapter.ScanTimeout = Convert.ToInt32(scanTime);
                }

                // scan for bonded devices here
                if(scanBondedDevices == true)
                {
                    _bondeddevices.Clear();
                    IReadOnlyList<IDevice> bondedDevices = Adapter.GetSystemConnectedOrPairedDevices(null);

                    lock (m_Lock)
                    {
                        foreach (var device in bondedDevices)
                        {
                            if (string.IsNullOrEmpty(device?.Name))
                            {
                                continue;
                            }

                            try
                            {
                                //PerformanceLogger.StartLog(typeof(BleRadioService), "Scan.discoverEventHandler");
                                BleDevice bleDevice = new BleDevice(device);
                                _bondeddevices.Add(bleDevice);
                                deviceFoundCallback?.Invoke(bleDevice);
                            }
                            finally
                            {
                                //PerformanceLogger.EndLog(typeof(BleRadioService), "Scan.discoverEventHandler");
                            }
                        }
                    }
                }


                // scan for ble devices
                if (scanBleDevices == true)
                {
                    void deviceDiscovered(object sender, DeviceEventArgs e)
                    {
                        if (string.IsNullOrEmpty(e.Device?.Name))
                        {
                            //return;
                        }

                        try
                        {
                            //PerformanceLogger.StartLog(typeof(BleRadioService), "Scan.discoverEventHandler");


                            //if (e.Device != null && e.Device.Name.ToUpper().Contains("FENOM"))
                            //{
                                // create ble device and push to caller
                                BleDevice bleDevice = new BleDevice(e.Device);
                                _devices.Add(bleDevice);
                                deviceFoundCallback?.Invoke(bleDevice);
                            //}

                        }
                        finally
                        {
                            //PerformanceLogger.EndLog(typeof(BleRadioService), "Scan.discoverEventHandler");
                        }
                    }

                    //if (_devices.Count > 1)
                    //{
                    //    foreach (var device in _devices)
                    //    {

                    //    }
                    //}

                    _devices.Clear();

                    Adapter.DeviceDiscovered += deviceDiscovered;
                    Adapter.ScanMode = ScanMode.Balanced;

                    await Adapter.StartScanningForDevicesAsync();
                    scanCompletedCallback?.Invoke(Devices);

                    Adapter.DeviceDiscovered -= deviceDiscovered;
                }


                return scanBleDevices || scanBondedDevices;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return false;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleRadioService), "Scan");
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="device"></param>
        ///// <param name="id"></param>
        ///// <param name="disconnect"></param>
        ///// <returns></returns>
        //private async Task<BleDevice> ConnectToDeviceAsync(IDevice device = null, Guid id = default, bool disconnect = false)
        //{
        //    try
        //    {
        //        //PerformanceLogger.StartLog(typeof(BleRadioService), "ConnectToDeviceAsync");

        //        if (device == null && id == default)
        //        {
        //            throw new Exception("need device or id to connect");
        //        }

        //        if (device != null && id != default)
        //        {
        //            throw new Exception("both device and id can't be active");
        //        }

        //        IDevice connectedDevice = null;

        //        if (device != null)
        //        {
        //            connectedDevice = await Adapter.ConnectToKnownDeviceAsync(device.Id, new ConnectParameters(true, true));
        //        }
        //        else if (id != default)
        //        {
        //            connectedDevice = await Adapter.ConnectToKnownDeviceAsync(id, new ConnectParameters(true, true));
        //        }

        //        if (connectedDevice == null)
        //        {
        //            _logger.LogWarning("BleRadioService.ConnectToDeviceAsync() - null connected device");
        //            return null;
        //        }

        //        var bleDevice = new BleDevice(connectedDevice);

        //        if (!(await bleDevice.GetCharacterasticsAync() is SynchronizedList<IGattCharacteristic>
        //            gattCharacteristics))
        //        {
        //            _logger.LogWarning("BleRadioService.ConnectToDeviceAsync() - null gatt characteristics");
        //            return null;
        //        }

        //        if (disconnect)
        //        {
        //            await Adapter.DisconnectDeviceAsync(connectedDevice);
        //        }

        //        return bleDevice;
        //    }
        //    catch (DeviceConnectionException ex)
        //    {
        //        _logger.LogException(ex);
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogException(ex);
        //        return null;
        //    }
        //    finally
        //    {
        //        //PerformanceLogger.EndLog(typeof(BleRadioService), "ConnectToDeviceAsync");
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopScan()
        {
            try
            {
                
                //PerformanceLogger.StartLog(typeof(BleRadioService), "StopScan");

                if (IsScanning)
                {
                    await Adapter.StopScanningForDevicesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return false;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleRadioService), "StopScan");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characteristic"></param>
        /// <returns></returns>
        private static async Task<string> CharacteristicToString(IGattCharacteristic characteristic)
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(BleRadioService), "CharacteristicToString");

                if (characteristic == null)
                {
                    Console.WriteLine("BleRadioService.CharacteristicToString() - null characteristic");
                    return string.Empty;
                }

                var data = await characteristic.ReadAsync();
                var str = System.Text.Encoding.Default.GetString(data);

                return str;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleRadioService), "CharacteristicToString");
            }
        }
    }
}
