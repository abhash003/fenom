﻿#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Threading;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;


#region Namespace Aliases


using System.Linq;
using FenomPlus.Services.DeviceService.Utils;
using FenomPlus.Services.DeviceService.Abstract;

#endregion


namespace FenomPlus.Services.DeviceService.Concrete
{
    public class BleScanner : DeviceDiscoverer
    {
        private static bool _constructed = false;
        private CancellationTokenSource _cancelTokenSource;

        // Fields

        IBluetoothLE _ble = null;

        // Constructor

        public BleScanner(DeviceService deviceService) : base(deviceService)
        {
            if (!_constructed)
            {
                _ble = CrossBluetoothLE.Current;
                _ble.Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
                _ble.Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
                _ble.Adapter.DeviceConnected += Adapter_DeviceConnected;
                _ble.Adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
                _ble.Adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;

                _constructed = true;
            }
            else
            {
                throw new Exception();
            }
        }

        // Methods

        public override async void StartDiscovery()
        {
            if (_ble.Adapter.IsScanning)
                return;

            {
                var devices = _ble.Adapter.GetSystemConnectedOrPairedDevices();
                foreach (var device in devices)
                {
                    var name = device.Name.ToLower();
                    if (_deviceService.IsDeviceFenomDevice(name))
                    {
                        var args = new DeviceEventArgs();
                        args.Device = device;
                        Adapter_DeviceDiscovered(null, args);
                        return;
                        if (false)
                        {
                            try
                            {
                                var bleDevice = await _ble.Adapter.ConnectToKnownDeviceAsync(device.Id);
                                return;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        }
                    }
                } 
            }

            // advertisement interval, window
            // scanning interval, window

            _ble.Adapter.ScanMode = ScanMode.LowLatency; // high duty cycle
            _ble.Adapter.ScanTimeout = 30 * 1000;
            _ble.Adapter.ScanMatchMode = ScanMatchMode.STICKY;

            _cancelTokenSource = new CancellationTokenSource();

            await _ble.Adapter.StartScanningForDevicesAsync(
                deviceFilter: (d) =>
                {
                    if (_deviceService.IsDeviceFenomDevice(d.Name))
                        return true;

                    return false;
                },
                cancellationToken: _cancelTokenSource.Token);
        }

        public override async void StopDiscoveryAsync()
        {
            if (!_ble.Adapter.IsScanning)
                return;

            //_ble.Adapter.DeviceDiscovered -= Adapter_DeviceDiscovered;
            //_ble.Adapter.DeviceConnected -= Adapter_DeviceConnected;
            //_ble.Adapter.ScanTimeoutElapsed -= Adapter_ScanTimeoutElapsed;

            try
            {
                //_cancelTokenSource.Cancel();
                await _ble.Adapter.StopScanningForDevicesAsync();
            }
            catch (Exception e)
            {
                Helper.WriteDebug(e);
            }
        }

        private void Adapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            Helper.WriteDebug(" ... Adapter_DeviceDiscovered ... ");

            if (!_deviceService.IsDeviceFenomDevice(e.Device.Name))
                return;

            bool exists = _deviceService.Devices.Any(d => d.Id == e.Device.Id);
            if (!exists)
            {
                _deviceService.Devices.Add(new BleDevice(e.Device));
                _deviceService.HandleDeviceDiscovered(new BleDevice(e.Device));
            }
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Helper.WriteDebug($"... Adapter_ScanTimeoutElapsed ... {DateTime.Now}");
        }

        private void Adapter_DeviceConnected(object sender, DeviceEventArgs e)
        {
            _deviceService.Current = _deviceService.Devices.First(d => d.Id == e.Device.Id);
            if (_deviceService.Current != null)
            {
                //Thread.SpinWait(1000);
                _deviceService.StopDiscovery();
                //_ble.Adapter.StopScanningForDevicesAsync();
            }

            _deviceService.HandleDeviceConnected(_deviceService.Devices.First(d => d.Id == e.Device.Id));
        }
        private void Adapter_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            if (e.Device.Id == _deviceService.Current.Id)
                _deviceService.Current = null;
            //_deviceService.HandleDeviceDisconnected(_deviceService.Devices.First(d => d.Id == e.Device.Id));
        }
        private async void Adapter_DeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            if (_deviceService.Current != null && e.Device.Id == _deviceService.Current.Id)
                _deviceService.Current = null;

            bool handleLostConnection = true;

            if (handleLostConnection)
            {
                _deviceService.HandleDeviceConnectionLost(_deviceService.Devices.First(d => d.Id == e.Device.Id));
            }
            else
            {
                await _deviceService.Current.ConnectAsync();
            }
        }
    }
}

