using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using FenomPlus.SDK.Core.Ble.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FenomPlus.SDK.Core.Utils;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Exceptions;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.SDK.Core.Ble.PluginBLE
{
    internal partial class BleDevice : IBleDevice
    {
        // need Adapter
        private static readonly IBluetoothLE Ble = CrossBluetoothLE.Current;
        private static readonly IAdapter Adapter = Ble.Adapter;
        private static IAppServices Services => IOC.Services;
        private Logger _logger = new Logger(nameof(BleDevice));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public BleDevice(IDevice device)
        {
            //PerformanceLogger.StartLog(typeof(BleDevice), "BleDevice");
            Device = device;
            //PerformanceLogger.EndLog(typeof(BleDevice), "BleDevice");
        }

        // get device here injected from constructor
        private IDevice Device { get; set; }
        public object NativeDevice => Device.NativeDevice;

        public string Name => Device?.Name;
        public int? Rssi => Device?.Rssi;
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string HardwareVersion { get; set; }
        public string FirmwareVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public string SerialNumber { get; set; }
        public string PairSerialNumber { get; set; }
        public bool IsBonded { get; set; }


        //public bool Connected => Device.State == DeviceState.Connected */ // after on connected event this is FALSE! WHY!!!;
        public bool Connected => (CrossBluetoothLE.Current.Adapter.ConnectedDevices.Count > 0); 

        public IEnumerable<IGattCharacteristic> GattCharacteristics { get; } = new SynchronizedList<IGattCharacteristic>();
        public IEnumerable<IService> GattServices { get; } = new SynchronizedList<IService>();

        public Guid Uuid => Device.Id;

        /// <summary>
        /// Connects to this Device
        /// </summary>
        /// <returns>bool for success or failure</returns>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(BleDevice), "ConnectAsync");

                if (!(GattCharacteristics is SynchronizedList<IGattCharacteristic> gattCharacteristics))
                {
                    _logger.LogError("BleDevice.ConnectAsync() - did not find gatt characteristics");
                    return false;
                }

                ConnectParameters connectParameters = new ConnectParameters(autoConnect: true, forceBleTransport: false);
                if (Device.State == DeviceState.Disconnected)
                {
                    var device = await Adapter.ConnectToKnownDeviceAsync(Device.Id, connectParameters);//, cancellationToken.Token);

                    //var device = await Adapter.ConnectToKnownDeviceAsync(Device.Id, new ConnectParameters(true, true));

                    if (device is null)
                    {
                        _logger.LogError("BleDevice.ConnectAsync() - ConnectToKnownDeviceAsync failed.");
                        return false;
                    }

                    if (device.State == DeviceState.Connected)
                    {
                        Device = device;

                        try
                        {
                            //int mtuActual = await Device.RequestMtuAsync(164);
                        }
                        catch (Exception e)
                        {
                            Services.LogCat.Print(e);
                        }

                        Device.UpdateConnectionInterval(ConnectionInterval.Normal);

                        _ = await GetCharacterasticsAync();

                        return true;
                    }
                }
                else if (Device.State == DeviceState.Limited)
                {
                    await Adapter.ConnectToDeviceAsync(Device, connectParameters);//, cancellationToken.Token);
                }

                return Device.State == DeviceState.Connected;
            }
            catch (DeviceConnectionException ex)
            {
                Services.LogCat.Print("BleDevice.DeviceConnectionException(): " + ex.Message);
                _logger.LogException(ex);
                return false;
            }
            catch (Exception ex)
            {
                Services.LogCat.Print("BleDevice.ConnectAsync(): " + ex.Message);
                _logger.LogException(ex);
                return false;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleDevice), "ConnectAsync");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DisconnectAsync()
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(BleDevice), "DisconnectAsync");

                var gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;
                gattCharacteristics?.Clear();

                if (!Connected)
                {
                    return true;
                }

                await Adapter.DisconnectDeviceAsync(Device);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return false;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleDevice), "DisconnectAsync");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGattCharacteristic>> GetCharacterasticsAync()
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(BleDevice), "GetCharacterasticsAync");

                var gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;

                if (gattCharacteristics == null)
                {
                    _logger.LogWarning("BleDevice.GetCharacteristicsAsync() - list is null");
                    return null;
                }

                gattCharacteristics.Clear();

                
                var gattService = GattServices as SynchronizedList<IService>;

                var services = await Device.GetServicesAsync();

                foreach (var service in services)
                {
                    // add service here
                    if (gattService != null) 
                        gattService.Add(service);

                    var characteristics = await service.GetCharacteristicsAsync();
                    foreach (var characteristic in characteristics)
                    {
                        IGattCharacteristic gattCharacteristic = new GattCharacteristic(characteristic);
                        gattCharacteristics.Add(gattCharacteristic);
                    }
                }

                return gattCharacteristics;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleDevice), "GetCharacterasticsAync");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<IGattCharacteristic> FindCharacteristic(string uuid)
        {
            
            Guid guid = new Guid(uuid);
            IGattCharacteristic gatt = null;

            var gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;
            if(gattCharacteristics.Count <= 0)
            {
                _ = await GetCharacterasticsAync();
                gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;
            }
            foreach (IGattCharacteristic item in gattCharacteristics)
            {
                if (!item.Uuid.Equals(guid)) continue;
                gatt = item;
                break;
            }

            return gatt;
        }
    }
}