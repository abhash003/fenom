using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace FenomPlus.Models
{
    public partial class DeviceStatus : ObservableObject
    {
        public DeviceStatus()
        {
            Battery = new SensorStatus();
            Sensor = new SensorStatus();
            QualityControlExpiration = new SensorStatus();
            Device = new SensorStatus();
            RelativeHumidity = new SensorStatus();
            Temperature = new SensorStatus();
            Pressure = new SensorStatus();

            UpdateBattery(0);
            UpdateSensor(0);
            UpdateQualityControlExpiration(0);
            UpdateDevice(0);
            UpdateRelativeHumidity(0);
            UpdateTemperature(0);
            UpdatePressure(0);
        }

        public const int BatteryLow = 3;
        public const int BatteryWarning = 20;
        public const int BatteryFull = 100;

        [ObservableProperty]
        private SensorStatus _battery;

        public SensorStatus UpdateBattery(int value)
        {
            Battery.Value = $"{value}%";

            if (value <= BatteryLow) 
            {
                // low
                Battery.ImageName = "BatteryLow";
                Battery.Color = Color.Red;
            } 
            else if (value <= BatteryWarning) 
            {
                // warning
                Battery.ImageName = "BatteryWarning";
                Battery.Color = Color.Orange;
            } 
            else 
            {
                // full
                Battery.ImageName = "BatteryFull";
                Battery.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Battery));
            return Battery;
        }


        public const int SensorLow = 0;
        public const int SensorWarning = 60;
        public const int SensorFull = 1 * 365 + 10;

        [ObservableProperty]
        private SensorStatus _sensor;

        public SensorStatus UpdateSensor(int value)
        {
            if (value <= SensorLow) 
            {
                // low
                Sensor.ImageName = "SensorLow";
                Sensor.Color = Color.Red;
            } 
            else if (value <= SensorWarning) 
            {
                // warning
                Sensor.ImageName = "SensorWarning";
                Sensor.Color = Color.Orange;
            } 
            else 
            {
                // full
                Sensor.ImageName = "SensorFull";
                Sensor.Color = Color.Green;
            }

            Sensor.Value = $"{(int)((value < 365) ? value : value / 365)}";
            OnPropertyChanged(nameof(Sensor));
            return Sensor;
        }

        public const int QualityControlExpirationLow = 0;
        public const int QualityControlExpirationWarning = 1;
        public const int QualityControlExpirationFull = 7;

        [ObservableProperty]
        private SensorStatus _qualityControlExpiration;

        public SensorStatus UpdateQualityControlExpiration(int value)
        {
            if (value <= QualityControlExpirationLow) 
            {
                // low
                QualityControlExpiration.ImageName = "QualityControlLow";
                QualityControlExpiration.Color = Color.Red;
            } 
            else if (value <= QualityControlExpirationWarning) 
            {
                // warning
                QualityControlExpiration.ImageName = "QualityControlWarning";
                QualityControlExpiration.Color = Color.Orange;
            } 
            else 
            {
                // full
                QualityControlExpiration.ImageName = "QualityControlFull";
                QualityControlExpiration.Color = Color.Green;
            }

            QualityControlExpiration.Value = $"{(int)((value < 365) ? value : value / 365)}";
            OnPropertyChanged(nameof(QualityControlExpiration));
            return QualityControlExpiration;
        }

        public const int DeviceLow = 0;
        public const int DeviceWarning = 60;
        public const int DeviceFull = 1 * 365 + 10;

        [ObservableProperty]
        private SensorStatus _device;

        public SensorStatus UpdateDevice(int value)
        {
            Device.Value = $"{value}";

            if (value <= DeviceLow)
            {
                // low
                Device.ImageName = "DeviceLow";
                Device.Color = Color.Red;
            }
            else if (value <= DeviceWarning)
            {
                // warning
                Device.ImageName = "DeviceWarning";
                Device.Color = Color.Orange;
            }
            else
            {
                // full
                Device.ImageName = "DeviceFull";
                Device.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Device));
            return Device;
        }

        public const int RelativeHumidityLow = 0;
        public const int RelativeHumidityWarning = 80;
        public const int RelativeHumidityFull = 100;

        [ObservableProperty]
        private SensorStatus _relativeHumidity;

        public SensorStatus UpdateRelativeHumidity(int value)
        {
            RelativeHumidity.Value = $"{value}";

            if (value <= RelativeHumidityLow)
            {
                // low
                RelativeHumidity.ImageName = "RelativeHumidityLow";
                RelativeHumidity.Color = Color.Red;
            }
            else if (value <= RelativeHumidityWarning)
            {
                // warning
                RelativeHumidity.ImageName = "RelativeHumidityWarning";
                RelativeHumidity.Color = Color.Orange;
            }
            else
            {
                // full
                RelativeHumidity.ImageName = "RelativeHumidityFull";
                RelativeHumidity.Color = Color.Green;
            }

            OnPropertyChanged(nameof(RelativeHumidity));
            return RelativeHumidity;
        }

        public const int TemperatureLow = 0;
        public const int TemperatureWarning = 80;
        public const int TemperatureFull = 100;

        [ObservableProperty]
        private SensorStatus _temperature;

        public SensorStatus UpdateTemperature(int value)
        {
            Temperature.Value = $"{value}";

            if (value <= TemperatureLow)
            {
                // low
                Temperature.ImageName = "TemperatureLow";
                Temperature.Color = Color.Red;
            }
            else if (value <= TemperatureWarning)
            {
                // warning
                Temperature.ImageName = "TemperatureWarning";
                Temperature.Color = Color.Orange;
            }
            else
            {
                // full
                Temperature.ImageName = "TemperatureFull";
                Temperature.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Temperature));
            return Temperature;
        }

        public const int PressureLow = 0;
        public const int PressureWarning = 80;
        public const int PressureFull = 100;

        [ObservableProperty]
        private SensorStatus _pressure;

        public SensorStatus UpdatePressure(int value)
        {
            Pressure.Value = $"{value}";

            if (value <= PressureLow)
            {
                // low
                Pressure.ImageName = "PressureLow";
                Pressure.Color = Color.Red;
            }
            else if (value <= PressureWarning)
            {
                // warning
                Pressure.ImageName = "PressureWarning";
                Pressure.Color = Color.Orange;
            }
            else
            {
                // full
                Pressure.ImageName = "PressureFull";
                Pressure.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Pressure));
            return Pressure;
        }
    }
}
