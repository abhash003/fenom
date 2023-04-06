#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using FenomPlus.Services.DeviceService.Abstract;
using System;
using System.Collections.Generic;




namespace FenomPlus.Services.DeviceService.Interfaces
{
    public interface IDeviceService
    {
        // Events
        event EventHandler DeviceDiscovered;
        event EventHandler DeviceConnected;
        event EventHandler DeviceConnectionLost;
        event EventHandler DeviceDisconnected;

        // Properties
        bool Discovering { get; }
        IList<IDevice> Devices { get; }

#nullable enable
        IDevice? Current { get; set; }
#nullable disable

        // Methods
        void StartDiscovery();
        void StopDiscovery();
        bool IsDeviceFenomDevice(string name);
        Device GetBondedOrPairedFenomDevices();
    }
}
