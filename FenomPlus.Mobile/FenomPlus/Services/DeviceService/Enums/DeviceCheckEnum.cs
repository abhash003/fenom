
namespace FenomPlus.Services.DeviceService.Enums
{
    public enum DeviceCheckEnum
    {
        Ready,
        DevicePurging,
        HumidityOutOfRange,
        PressureOutOfRange,
        TemperatureOutOfRange,
        BatteryCriticallyLow,
        NoSensorMissing,
        NoSensorCommunicationFailed,
        Unknown,
    }
}
