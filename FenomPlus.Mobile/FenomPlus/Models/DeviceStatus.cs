using Xamarin.Forms;

namespace FenomPlus.Models
{
    public class DeviceStatus : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// BatteryDevice
        /// </summary>
        public const int BatteryLow = 3;
        public const int BatteryWarning = 20;
        public const int BatteryFull = 100;
        private SensorStatus battery;
        public SensorStatus Battery
        {
            get => battery;
            set
            {
                battery = value;
                OnPropertyChanged("Battery");
            }
        }

        /// <summary>
        /// UpdateBatteryDevice
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateBattery(int value)
        {
            Battery.Value = string.Format("{0}%", value);
            if (value <= BatteryLow) {
                // low
                Battery.Image = "BatteryLow";
                Battery.Color = Color.Red;
            } else if (value <= BatteryWarning) {
                // Warning
                Battery.Image = "BatteryWarning";
                Battery.Color = Color.Orange;
            } else {
                // full
                Battery.Image = "BatteryFull";
                Battery.Color = Color.Green;
            }
            OnPropertyChanged("Battery");
            return Battery;
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
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateSensor(int value)
        {
            if (value <= SensorLow) {
                // low
                Sensor.Image = "SensorLow";
                Sensor.Color = Color.Red;
            } else if (value <= SensorWarning) {
                // Warning
                Sensor.Image = "SensorWarning";
                Sensor.Color = Color.Orange;
            } else {
                // full
                Sensor.Image = "SensorFull";
                Sensor.Color = Color.Green;
            }
            Sensor.Value = string.Format("{0}", (int)((value < 365) ? value : value / 365));
            OnPropertyChanged("Sensor");
            return Sensor;
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
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateQualityControlExpiration(int value)
        {
            if (value <= QualityControlExpirationLow) {
                // low
                QualityControlExpiration.Image = "QualityControlLow";
                QualityControlExpiration.Color = Color.Red;
            } else if (value <= QualityControlExpirationWarning) {
                // Warning
                QualityControlExpiration.Image = "QualityControlWarning";
                QualityControlExpiration.Color = Color.Orange;
            } else {
                // full
                QualityControlExpiration.Image = "QualityControlFull";
                QualityControlExpiration.Color = Color.Green;
            }
            QualityControlExpiration.Value = string.Format("{0}", (int)((value < 365) ? value : value / 365));
            OnPropertyChanged("QualityControlExpiration");
            return QualityControlExpiration;
        }

        /// <summary>
        /// DeviceState
        /// </summary>
        public const int DeviceLow = 0;
        public const int DeviceWarning = 60;
        public const int DeviceFull = 1 * 365 + 10;
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
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateDevice(int value)
        {
            Device.Value = string.Format("{0}", value);

            if (value <= DeviceLow)
            {
                // low
                Device.Image = "DeviceLow";
                Device.Color = Color.Red;
            }
            else if (value <= DeviceWarning)
            {
                // Warning
                Device.Image = "DeviceWarning";
                Device.Color = Color.Orange;
            }
            else
            {
                // full
                Device.Image = "DeviceFull";
                Device.Color = Color.Green;
            }
            OnPropertyChanged("Device");
            return Device;
        }


        /// <summary>
        /// RelativeHumidity
        /// </summary>
        public const int RelativeHumidityLow = 0;
        public const int RelativeHumidityWarning = 80;
        public const int RelativeHumidityFull = 100;
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
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateRelativeHumidity(int value)
        {
            RelativeHumidity.Value = string.Format("{0}", value);

            if (value <= RelativeHumidityLow)
            {
                // low
                RelativeHumidity.Image = "RelativeHumidityLow";
                RelativeHumidity.Color = Color.Red;
            }
            else if (value <= RelativeHumidityWarning)
            {
                // Warning
                RelativeHumidity.Image = "RelativeHumidityWarning";
                RelativeHumidity.Color = Color.Orange;
            }
            else
            {
                // full
                RelativeHumidity.Image = "RelativeHumidityFull";
                RelativeHumidity.Color = Color.Green;
            }
            OnPropertyChanged("RelativeHumidity");
            return RelativeHumidity;
        }

        /// <summary>
        /// Temperature
        /// </summary>
        public const int TemperatureLow = 0;
        public const int TemperatureWarning = 80;
        public const int TemperatureFull = 100;
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
        /// Temperature
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateTemperature(int value)
        {
            Temperature.Value = string.Format("{0}", value);

            if (value <= TemperatureLow)
            {
                // low
                Temperature.Image = "TemperatureLow";
                Temperature.Color = Color.Red;
            }
            else if (value <= TemperatureWarning)
            {
                // Warning
                Temperature.Image = "TemperatureWarning";
                Temperature.Color = Color.Orange;
            }
            else
            {
                // full
                Temperature.Image = "TemperatureFull";
                Temperature.Color = Color.Green;
            }
            OnPropertyChanged("Temperature");
            return Temperature;
        }


        /// <summary>
        /// Pressure
        /// </summary>
        public const int PressureLow = 0;
        public const int PressureWarning = 80;
        public const int PressureFull = 100;
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
        /// <param name="value"></param>
        public SensorStatus UpdatePressure(int value)
        {
            Pressure.Value = string.Format("{0}", value);

            if (value <= PressureLow)
            {
                // low
                Pressure.Image = "PressureLow";
                Pressure.Color = Color.Red;
            }
            else if (value <= PressureWarning)
            {
                // Warning
                Pressure.Image = "PressureWarning";
                Pressure.Color = Color.Orange;
            }
            else
            {
                // full
                Pressure.Image = "PressureFull";
                Pressure.Color = Color.Green;
            }
            OnPropertyChanged("Pressure");
            return Pressure;
        }
    }
}
