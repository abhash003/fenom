#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Threading.Tasks;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.Services.DeviceService.Interfaces
{
    public interface IDevice
    {
        // Properties

        Guid Id { get; }

        string Name { get; }

        bool Connected { get; }

        object NativeDevice { get; }

        // Methods

        Task ConnectAsync();

        Task ConnectToKnownDeviceAsync(Guid id);

        Task DisconnectAsync();

        /*
         * 
         * LEGACY TO BE REMOVED
         * 
         */

        int DeviceReadyCountDown { get; set; }
        bool ReadyForTest { get; set; }
        bool BreathTestInProgress { get; set; }

        //bool IsConnected(bool devicePowerOn = false);
        bool IsNotConnectedRedirect(bool devicePowerOn = false);

        Task<bool> StartTest(BreathTestEnum breathTestEnum);
        Task<bool> StopTest();
        Task<bool> RequestDeviceInfo();
        Task<bool> RequestEnvironmentalInfo();
        Task<bool> SendMessage(MESSAGE message);
        Task<bool> SendSerialNumber(string SerailNumber);
        Task<bool> SendDateTime(string date, string time);
        Task<bool> SendCalibration(double cal1, double cal2, double cal3);
        Task<bool> SendCalibration(ID_SUB iD_SUB, double cal1, double cal2, double cal3);
    }
}
