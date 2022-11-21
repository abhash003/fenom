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
        //bool IsScanning { get; set; }
        IBleDevice BleDevice { get; set; }

        int DeviceReadyCountDown { get; set; }
        bool ReadyForTest { get; set; }

        Task<bool> Connect(IBleDevice bleDevice);
        Task<bool> Disconnect();

        bool IsConnected(bool devicePowerOn=false);
        bool IsNotConnectedRedirect(bool devicePowerOn = false);

        bool BreathTestInProgress { get; set; }
        Task<bool> StartTest(BreathTestEnum breathTestEnum);
        Task<bool> StopTest();
        Task<bool> RequestDeviceInfo();
        Task<bool> RequestEnvironmentalInfo();
        Task<bool> SendMessage(MESSAGE message);
        Task<bool> SendSerialNumber(string SerailNumber);
        Task<bool> SendDateTime(string date, string time);
        Task<bool> SendCalibration(double cal1, double cal2, double cal3);
        Task<bool> SendCalibration(ID_SUB iD_SUB, double cal1, double cal2, double cal3);

        Task<IEnumerable<IFenomHubSystem>> Scan(TimeSpan scanTime = default, bool scanBondedDevices = true, bool scanBleDevices = true, Action<IBleDevice> deviceFoundCallback = null, Action<IEnumerable<IBleDevice>> scanCompletedCallback = null);
        Task<bool> StopScan();
    }
}
