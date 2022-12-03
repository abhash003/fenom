using CommunityToolkit.Mvvm.Messaging.Messages;
using Plugin.BLE.Abstractions.Extensions;
//using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Interfaces
{
    public interface IDeviceService
    {
        /*
                enum TransportType
                {
                    BLE,
                    USB,
                    SERIAL
                }
        */

        /*
         * Known:
         * 
         * BLE: Paired/Bonded BLE device
         * USB: Registered USB device??? 
         * 
         * Unknown:
         * 
         * BLE: Scanned, but not paired/bonded BLE device
         * USB: Enumerated but not registred
         * 
         * 
         */

        void Start();

        void Stop();
    }

#if false
    public interface IDevice
    {
        string Name { get; }
        bool IsConnected { get; }
    }

    public interface IDeviceService
    {
        void Start();
        void Stop();


    #region Discovery
        /*
         * Discover BLE devices of the type we are looking for, add it to the list
         * Enumerate USB devices of the type we are looking for, add it to the list
         * 
         * IDevice
         * 
         * IsConnected()
         * IsReady()
         * IsTesting()
         * 
         * SendCommand(cbResult)
         * 
         * 
         * 
         * Connect
         * Disconnect
         * 
         * onConnected
         * onConnectionLost
         * onDisconnected
         * 
         * IDeviceService
         * 
         * - IBleDeviceService
         * --
         * - IUsbDeviceService
         * 
         * 
         */
    #endregion

    #region Connection Management
    #endregion

        IDevice Device { get; set; }

        public class DeviceConnectedMessage : ValueChangedMessage<bool>
        {
            public DeviceConnectedMessage(bool isConnected) : base(isConnected)
            {
            }
        }

        public class DeviceConnectionLostMessage : ValueChangedMessage<bool>
        {
            public DeviceConnectionLostMessage(bool isConnected) : base(isConnected)
            {
            }
        }
        public class DeviceDisconnectedMessage : ValueChangedMessage<bool>
        {
            public DeviceDisconnectedMessage(bool isConnected) : base(isConnected)
            {
            }
        }
    }

    namespace Imp
    {
        public class DeviceService : IDeviceService
        {
            public IDevice Device { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public void Start()
            {
                var ble = Plugin.BLE.CrossBluetoothLE.Current;
                var bleAdapter = ble.Adapter;

                if (!bleAdapter.IsScanning)
                {
                    bleAdapter.StartScanningForDevicesAsync();
                    bleAdapter.StopScanningForDevicesAsync();
                }
            }

            public void Stop()
            {
                throw new NotImplementedException();
            }
        }

        public class BleDevice : IDevice
        {
            public BleDevice()
            {
            }

            public string Name => throw new NotImplementedException();

            public bool IsConnected => throw new NotImplementedException();
        }
    }
#endif
}
