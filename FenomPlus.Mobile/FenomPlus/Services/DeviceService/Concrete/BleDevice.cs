#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using PluginBleIDevice = Plugin.BLE.Abstractions.Contracts.IDevice;
using System.Diagnostics;
using FenomPlus.Interfaces;
using FenomPlus.SDK.Core.Models;
using Plugin.BLE.Abstractions;
using FenomPlus.Services.DeviceService.Utils;
using FenomPlus.Services.DeviceService.Abstract;

namespace FenomPlus.Services.DeviceService.Concrete
{
    internal class BleDevice : Device
    {
        readonly IAdapter _bleAdapter = CrossBluetoothLE.Current.Adapter;
        PluginBleIDevice _bleDevice;


        public BleDevice(object bleDevice) : base(bleDevice)
        {
            _bleDevice = (PluginBleIDevice)bleDevice;
        }

        public override string Name
        {
            get { return _bleDevice.Name; }
        }

        public override Guid Id
        {
            get { return ((PluginBleIDevice)_nativeDevice).Id; }
        }

        public override bool Connected
        {
            get
            {
                if (_bleDevice.State == DeviceState.Limited)
                {
                    Helper.WriteDebug("Connection state: LIMITED, reconnecting.");
                    _bleAdapter.ConnectToDeviceAsync(_bleDevice);
                }

                return ((PluginBleIDevice)_nativeDevice).State == DeviceState.Connected;
            }
        }

        private object _handlerLock = new object();

        public override async Task ConnectAsync()
        {
            if (_bleAdapter != null && ((PluginBleIDevice)_nativeDevice).State != DeviceState.Connected)
            {
                try
                {
                    await _bleAdapter.ConnectToDeviceAsync((PluginBleIDevice)_nativeDevice, default, default);

                    // get service
                    var device = (PluginBleIDevice)_nativeDevice;

                    var service = await device.GetServiceAsync(new Guid(FenomPlus.SDK.Core.Constants.FenomService));

                    // get characteristics
                    var fwChar  = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                        .FeatureWriteCharacteristic));

                    FwCharacteristic = fwChar;

                    var devChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                        .DeviceInfoCharacteristic));

                    var envChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                        .EnvironmentalInfoCharacteristic));

                    var bmChar =
                        await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                            .BreathManeuverCharacteristic));

                    var dbgChar =
                        await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                            .DebugMessageCharacteristic));

                    //ICharacteristic[] chars = { fwChar, devChar, envChar };

                    devChar.ValueUpdated += (sender, e) =>
                    {
                        lock (_handlerLock)
                        {
                            var cache = AppServices.Container.Resolve<ICacheService>();
                            cache.DecodeDeviceInfo(e.Characteristic.Value);
                            Console.WriteLine("updated characteristic: device info");
                        }

                    };


                    envChar.ValueUpdated += (sender, e) =>
                    {
                        lock (_handlerLock)
                        {
                            var cache = AppServices.Container.Resolve<ICacheService>();
                            cache.DecodeEnvironmentalInfo(e.Characteristic.Value);
                            Console.WriteLine("updated characteristic: environmental info");
                        }
                    };
                    
                    bmChar.ValueUpdated += (sender, e) =>
                    {
                        lock (_handlerLock)
                        {
                            var cache = AppServices.Container.Resolve<ICacheService>();
                            cache.DecodeBreathManeuver(e.Characteristic.Value);
                            Console.WriteLine($"updated characteristic: breath maneuver (flow: {cache.BreathManeuver.BreathFlow})");
                        }
                    };

                    dbgChar.ValueUpdated += (sender, e) =>
                    {
                        lock (_handlerLock)
                        {
                            var cache = AppServices.Container.Resolve<ICacheService>();
                            cache.DecodeDebugMsg(e.Characteristic.Value);
                            Console.WriteLine("updated characteristic: debug message");
                        }
                    };

                    {
                        var cache = AppServices.Container.Resolve<ICacheService>();
                        cache.DeviceInfo = new DeviceInfo();
                        cache.EnvironmentalInfo = new EnvironmentalInfo();
                    }

                    await devChar.StartUpdatesAsync();
                    await envChar.StartUpdatesAsync();
                    await bmChar.StartUpdatesAsync();
                    await dbgChar.StartUpdatesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public override async Task ConnectToKnownDeviceAsync(Guid id)
        {
            var systemDevice = _bleAdapter.GetSystemConnectedOrPairedDevices();
            if (_bleAdapter != null && id != Guid.Empty)
            {
                try
                {
                    await _bleAdapter.StopScanningForDevicesAsync();
                    await _bleAdapter.ConnectToKnownDeviceAsync(id);
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public override async Task DisconnectAsync()
        {
            if (((PluginBleIDevice)_nativeDevice).State == DeviceState.Connected)
            {
                await _bleAdapter.DisconnectDeviceAsync((PluginBleIDevice)_nativeDevice);
            }
        }
    }
}
