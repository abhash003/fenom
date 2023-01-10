#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using PluginBleIDevice = Plugin.BLE.Abstractions.Contracts.IDevice;
using System.Diagnostics;
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

                    var devChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                        .DeviceInfoCharacteristic));

                    var envChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                        .EnvironmentalInfoCharacteristic));

                    var bmChar =
                        await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                            .BreathManeuverCharacteristic));

                    //ICharacteristic[] chars = { fwChar, devChar, envChar };

                    devChar.ValueUpdated += (sender, args) =>
                    {
                        //Services.AppServices.DecodeDeviceInfo(e.Characteristic.Value);
                        Console.WriteLine("updated");
                    };

                    envChar.ValueUpdated += (sender, args) =>
                    {
                        Console.WriteLine("updated");
                    };

                    byte[] data1 = new byte[4];
                    data1[0] = 0;
                    data1[1] = 0; // id
                    data1[2] = 0;
                    data1[3] = 2; // sub

                    await fwChar.WriteAsync(data1);
                    var data2 = await envChar.ReadAsync();

                    await devChar.StartUpdatesAsync();
                    await envChar.StartUpdatesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw ex;
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
                    throw ex;
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
