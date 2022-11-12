using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FenomPlus.SDK.Core.Ble.Interface;
using Microsoft.Extensions.Logging;
using Plugin.BLE.Abstractions.EventArgs;

namespace FenomPlus.SDK.Abstractions
{
    public interface IFenomHubSystemDiscovery
    {
        IFenomHubSystem FenomHubSystem { get; set; }
        void SetLoggerFactory(ILoggerFactory loggerFactory);
        bool IsScanning { get; }
        Task<IEnumerable<IFenomHubSystem>> Scan(TimeSpan scanTime = default, bool scanBondedDevices = true, bool scanBleDevices = true, Action<IBleDevice> deviceFoundCallback = null, Action<IEnumerable<IBleDevice>> scanCompletedCallback = null);
        Task<bool> StopScan();

        //event EventHandler<DeviceEventArgs> DeviceAdvertised;
        //event EventHandler<DeviceEventArgs> DeviceDiscovered;
        event EventHandler<DeviceEventArgs> DeviceConnected;
        //event EventHandler<DeviceEventArgs> DeviceDisconnected;
        //event EventHandler<DeviceErrorEventArgs> DeviceConnectionLost;
    }
}