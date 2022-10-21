using System;
using Xamarin.Forms;

namespace FenomPlus.Models
{
    public class DeviceStatus : BaseModel
    {
        private Color barColor;
        public Color BarColor
        {
            get => barColor;
            set
            {
                barColor = value;
                OnPropertyChanged("BarColor");
            }
        }

        /// <summary>
        /// BatteryDevice
        /// </summary>
        public const int BatteryLow = 3;
        public const int BatteryWarning = 20;
        public const int BatteryFull = 100;
        private SensorStatus battery;
        public SensorStatus Battery {
            get => battery;
            set
            {
                battery = value;
                OnPropertyChanged("Battery");
            }
        }

        /// <summary>
        /// QualityControlExpiration
        /// </summary>
        public const int QualityControlExpirationLow = 0;
        public const int QualityControlExpirationWarning = 1;
        public const int QualityControlExpirationFull = 7;
        private SensorStatus qualityControlExpiration;
        public SensorStatus QualityControlExpiration
        {
            get => qualityControlExpiration;
            set
            {
                qualityControlExpiration = value;
                OnPropertyChanged("QualityControlExpiration");
            }
        }

        /// <summary>
        /// Sensor
        /// </summary>
        public const int SensorLow = 0;
        public const int SensorWarning = 60;
        public const int SensorFull = 1 * 365 + 10;
        private SensorStatus sensor;
        public SensorStatus Sensor
        {
            get => sensor;
            set
            {
                sensor = value;
                OnPropertyChanged("Sensor");
            }
        }

        /// <summary>
        /// DeviceState
        /// </summary>
        public const int DeviceLow = 0;
        public const int DeviceWarning = 1;
        public const int DeviceFull = 2;
        private SensorStatus device;
        public SensorStatus Device
        {
            get => device;
            set
            {
                device = value;
                OnPropertyChanged("Device");
            }
        }

        /// <summary>
        /// RelativeHumidity
        /// </summary>
        public const int RelativeHumidityLow = 0;
        public const int RelativeHumidityWarning = 1;
        public const int RelativeHumidityFull = 2;
        private SensorStatus relativeHumidity;
        public SensorStatus RelativeHumidity
        {
            get => relativeHumidity;
            set
            {
                relativeHumidity = value;
                OnPropertyChanged("RelativeHumidity");
            }
        }

        /// <summary>
        /// Temperature
        /// </summary>
        public const int TemperatureLow = 0;
        public const int TemperatureWarning = 60;
        public const int TemperatureFull = 5 * 365 + 10;
        private SensorStatus temperature;
        public SensorStatus Temperature
        {
            get => temperature;
            set
            {
                temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        /// <summary>
        /// Pressure
        /// </summary>
        public const int PressureLow = 0;
        public const int PressureWarning = 60;
        public const int PressureFull = 5 * 365 + 10;
        private SensorStatus pressure;
        public SensorStatus Pressure
        {
            get => pressure;
            set
            {
                pressure = value;
                OnPropertyChanged("Pressure");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DeviceStatus()
        {
            BarColor = Color.FromRgb(0xBB,0xFF,0xBB);
            Battery = new SensorStatus();
            Sensor = new SensorStatus();
            QualityControlExpiration = new SensorStatus();
            Device = new SensorStatus();
            RelativeHumidity = new SensorStatus();
            Temperature = new SensorStatus();
            Pressure = new SensorStatus();

            UpdateBattery(Battery.RawValue);
            UpdateSensor(Sensor.RawValue);
            UpdateQualityControlExpiration(QualityControlExpiration.RawValue);
            UpdateDevice(Device.RawValue);
            UpdateRelativeHumidity(RelativeHumidity.RawValue);
            UpdateTemperature(Temperature.RawValue);
            UpdatePressure(Pressure.RawValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetBarColor()
        {
            BarColor = Color.FromRgb(0xBB, 0xFF, 0xBB);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void UpdateBarColor(Color color)
        {
            if(color == Color.Green)
            {
                BarColor = Color.FromRgb(0xBB, 0xFF, 0xBB);
            }
            if (color == Color.Orange)
            {
                if (BarColor == Color.Red) return;
                BarColor = Color.FromRgb(0xFF, 0xFF, 0xBB);
            }
            if (color == Color.Red)
            {
                BarColor = Color.FromRgb(0xFF, 0x66, 0x44);
            }
        }

        /// <summary>
        /// UpdateBatteryDevice
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateBattery(int value)
        {
            Battery.Title = "Battery";
            Battery.Value = string.Format("{0}%", value);
            Battery.Type = "CHARGE";
            Battery.Description = string.Format("{0} tests remaining", value);
            
            if (value <= BatteryLow) {
                // low
                Battery.ImageName = "BatteryLow";
                Battery.Description = "No tests remaining";
                Battery.Color = Color.Red;
            } else if (value <= BatteryWarning) {
                // Warning
                Battery.ImageName = "BatteryWarning";
                Battery.Color = Color.Orange;
            } else {
                // full
                Battery.ImageName = "BatteryFull";
                Battery.Color = Color.Green;
            }
            UpdateBarColor(Battery.Color);
            Battery.Image = Battery.ImageName;
            OnPropertyChanged("Battery");
            return Battery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateSensor(int value)
        {
            Sensor.Title = "Sensor";
            Sensor.Description = "Remaining";
            if (value <= SensorLow) {
                // low
                Sensor.ImageName = "SensorLow";
                Sensor.Color = Color.Red;
            } else if (value <= SensorWarning) {
                // Warning
                Sensor.ImageName = "SensorWarning";
                Sensor.Color = Color.Orange;
            } else {
                // full
                Sensor.ImageName = "SensorFull";
                Sensor.Color = Color.Green;
            }
            UpdateBarColor(Sensor.Color);
            Sensor.Image = Sensor.ImageName;
            Sensor.Type = (value < 365) ? ((value != 1) ? "DAYS" : "DAY") : "YEARS";
            Sensor.Value = string.Format("{0}", (int)((value < 365) ? value : value / 365));
            OnPropertyChanged("Sensor");
            return Sensor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateQualityControlExpiration(int value)
        {
            QualityControlExpiration.Title = "Quality Control Expiration";
            QualityControlExpiration.Description = "Remaining";
            if (value <= QualityControlExpirationLow) {
                // low
                QualityControlExpiration.ImageName = "QualityControlAlert";
                QualityControlExpiration.Color = Color.Red;
            } else if (value <= QualityControlExpirationWarning) {
                // Warning
                QualityControlExpiration.ImageName = "QualityControlWarning";
                QualityControlExpiration.Color = Color.Orange;
            } else {
                // full
                QualityControlExpiration.ImageName = "QualityControlFull";
                QualityControlExpiration.Color = Color.Green;
            }
            UpdateBarColor(QualityControlExpiration.Color);
            QualityControlExpiration.Image = QualityControlExpiration.ImageName;
            QualityControlExpiration.Type = (value < 365) ? ((value != 1) ? "DAYS" : "DAY") : "YEARS";
            QualityControlExpiration.Value = string.Format("{0}", (int)((value < 365) ? value : value / 365));
            OnPropertyChanged("QualityControlExpiration");
            return QualityControlExpiration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateDevice(int value)
        {
            Device.Title = "Device";
            Device.Value = string.Format("{0}", value);
            Device.Type = "";

            if (value <= DeviceLow)
            {
                // low
                Device.ImageName = "DeviceLow";
                Device.Description = "Device Disconnected";
                Device.Color = Color.Red;
            }
            else if (value <= DeviceWarning)
            {
                // Warning
                Device.ImageName = "DeviceWarning";
                Device.Description = "Device Not Ready";
                Device.Color = Color.Orange;
            }
            else
            {
                // full
                Device.ImageName = "DeviceFull";
                Device.Description = "Device Ready";
                Device.Color = Color.Green;
            }
            UpdateBarColor(Device.Color);
            Device.Image = Device.ImageName;
            OnPropertyChanged("Device");
            return Device;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateRelativeHumidity(int value)
        {
            RelativeHumidity.Title = "Relative Humidity";
            RelativeHumidity.Value = string.Format("{0}", value);
            RelativeHumidity.Type = "";
            RelativeHumidity.Description = "";

            if (value <= RelativeHumidityLow)
            {
                // low
                RelativeHumidity.ImageName = "RelativeHumidityLow";
                RelativeHumidity.Color = Color.Red;
            }
            else if (value <= RelativeHumidityWarning)
            {
                // Warning
                RelativeHumidity.ImageName = "RelativeHumidityWarning";
                RelativeHumidity.Color = Color.Orange;
            }
            else
            {
                // full
                RelativeHumidity.ImageName = "RelativeHumidityFull";
                RelativeHumidity.Color = Color.Green;
            }
            UpdateBarColor(RelativeHumidity.Color);
            RelativeHumidity.Image = RelativeHumidity.ImageName;
            OnPropertyChanged("RelativeHumidity");
            return RelativeHumidity;
        }

        /// <summary>
        /// Temperature
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateTemperature(int value)
        {
            Temperature.Title = "Temperature";
            Temperature.Value = string.Format("{0}", value);
            Temperature.Type = "";
            Temperature.Description = "";

            if (value <= TemperatureLow)
            {
                // low
                Temperature.ImageName = "TemperatureLow";
                Temperature.Color = Color.Red;
            }
            else if (value <= TemperatureWarning)
            {
                // Warning
                Temperature.ImageName = "TemperatureWarning";
                Temperature.Color = Color.Orange;
            }
            else
            {
                // full
                Temperature.ImageName = "TemperatureFull";
                Temperature.Color = Color.Green;
            }
            UpdateBarColor(Temperature.Color);
            Temperature.Image = Temperature.ImageName;
            OnPropertyChanged("Temperature");
            return Temperature;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdatePressure(int value)
        {
            Pressure.Title = "Pressure";
            Pressure.Value = string.Format("{0}", value);
            Pressure.Type = "";
            Pressure.Description = "";

            if (value <= RelativeHumidityLow)
            {
                // low
                Pressure.ImageName = "PressureLow";
                Pressure.Color = Color.Red;
            }
            else if (value <= RelativeHumidityWarning)
            {
                // Warning
                Pressure.ImageName = "PressureWarning";
                Pressure.Color = Color.Orange;
            }
            else
            {
                // full
                Pressure.ImageName = "PressureFull";
                Pressure.Color = Color.Green;
            }
            UpdateBarColor(Pressure.Color);
            Pressure.Image = Pressure.ImageName;
            OnPropertyChanged("Pressure");
            return Pressure;
        }
    }
}
