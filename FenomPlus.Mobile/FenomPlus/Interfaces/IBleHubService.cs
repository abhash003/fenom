using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FenomPlus.SDK.Abstractions;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.Interfaces
{
    public interface IBleHubService
    {
        Task<bool> Connect(IBleDevice bleDevice);
        Task<bool> Disconnect();

        bool IsConnected(bool devicePowerOn=false);

        Task<bool> StartTest(BreathTestEnum breathTestEnum);
        Task<bool> StopTest();
        Task<bool> RequestDeviceInfo();
        Task<bool> RequestEnvironmentalInfo();
        Task<bool> SendMessage(MESSAGE message);
        Task<bool> SendSerailNumber(string SerailNumber);
        Task<bool> SendDateTime(DateTime dateTime);
        Task<bool> SendCalibration(Int16 cal1, Int16 cal2, Int16 cal3);

        Task<IEnumerable<IFenomHubSystem>> Scan(TimeSpan scanTime = default, bool scanBondedDevices = true, bool scanBleDevices = true, Action<IBleDevice> deviceFoundCallback = null, Action<IEnumerable<IBleDevice>> scanCompletedCallback = null);
        Task<bool> StopScan();
    }
}
