
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
        QCDisabled,
        ERROR_SYSTEM_NEGATIVE_QC_FAILED
    }
}
