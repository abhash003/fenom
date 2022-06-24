﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.SDK.Core.Ble.Interface
{
    public interface IBleDevice
    {
        string Name { get; }
        int? Rssi { get; }
        string Manufacturer { get; }
        string Model { get; }
        string HardwareVersion { get; }
        string SoftwareVersion { get; }
        string SerialNumber { get; }
        object NativeDevice { get; }
        Guid Uuid { get; }
        bool IsBonded { get; }
        bool Connected { get; }

        IEnumerable<IGattCharacteristic> GattCharacteristics { get; }
        Task<bool> ConnectAsync();
        Task<bool> DisconnectAsync();

        Task<bool> DEVICEINFO();
        Task<bool> ENVIROMENTALINFO();
        Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second);
        Task<bool> BREATHMANUEVER();
        Task<bool> TRAININGMODE();
        Task<bool> DEBUGMSG();
        Task<bool> DEBUGMANUEVERTYPE();
        Task<bool> MESSAGE(MESSAGE message);

    }
}
