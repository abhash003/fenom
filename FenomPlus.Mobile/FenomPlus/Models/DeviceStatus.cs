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

        public const int BatteryDeviceLow = 3;
        public const int BatteryDeviceWarning = 20;
        public const int BatteryDeviceFull = 100;
        private SensorStatus batteryDevice;
        public SensorStatus BatteryDevice {
            get => batteryDevice;
            set
            {
                batteryDevice = value;
                OnPropertyChanged("BatteryDevice");
            }
        }

        public const int DeviceExpirationLow = 0;
        public const int DeviceExpirationWarning = 60;
        public const int DeviceExpirationFull = 5 * 365 + 10;
        private SensorStatus deviceExpiration;
        public SensorStatus DeviceExpiration
        {
            get => deviceExpiration;
            set
            {
                deviceExpiration = value;
                OnPropertyChanged("DeviceExpiration");
            }
        }

        public const int SensoryExpirationLow = 0;
        public const int SensoryExpirationWarning = 60;
        public const int SensoryExpirationFull = 1 * 365 + 10;
        private SensorStatus sensoryExpiration;
        public SensorStatus SensoryExpiration
        {
            get => sensoryExpiration;
            set
            {
                sensoryExpiration = value;
                OnPropertyChanged("SensoryExpiration");
            }
        }

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

        public const int DeviceStateLow = 0;
        public const int DeviceStateWarning = 1;
        public const int DeviceStateFull = 2;
        private SensorStatus deviceState;
        public SensorStatus DeviceState
        {
            get => deviceState;
            set
            {
                deviceState = value;
                OnPropertyChanged("v");
            }
        }

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
        /// 
        /// </summary>
        public DeviceStatus()
        {
            BarColor = Color.FromRgb(0xBB,0xFF,0xBB);
            BatteryDevice = new SensorStatus();
            DeviceExpiration = new SensorStatus();
            SensoryExpiration = new SensorStatus();
            QualityControlExpiration = new SensorStatus();
            DeviceState = new SensorStatus();
            RelativeHumidity = new SensorStatus();

            UpdateBatteryDevice(BatteryDevice.RawValue);
            UpdateDeviceExpiration(DeviceExpiration.RawValue);
            UpdateSensoryExpiration(SensoryExpiration.RawValue);
            UpdateQualityControlExpiration(QualityControlExpiration.RawValue);
            UpdateDeviceState(DeviceState.RawValue);
            UpdateRelativeHumidity(RelativeHumidity.RawValue);
        }

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
        /// 
        /// </summary>
        public SensorStatus UpdateBatteryDevice(int value)
        {
            BatteryDevice.Title = "Battery Device";
            BatteryDevice.Value = string.Format("{0}%", value);
            BatteryDevice.Type = "CHARGE";
            BatteryDevice.Description = string.Format("{0} tests remaining", value);
            
            if (value <= BatteryDeviceLow) {
                // low
                BatteryDevice.ImageName = "BatteryLow";
                BatteryDevice.Description = "No tests remaining";
                BatteryDevice.Color = Color.Red;
            } else if (value <= BatteryDeviceWarning) {
                // Warning
                BatteryDevice.ImageName = "BatteryWarning";
                BatteryDevice.Color = Color.Orange;
            } else {
                // full
                BatteryDevice.ImageName = "BatteryFull";
                BatteryDevice.Color = Color.Green;
            }
            UpdateBarColor(BatteryDevice.Color);
            BatteryDevice.Image = BatteryDevice.ImageName;
            OnPropertyChanged("BatteryDevice");
            return BatteryDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateDeviceExpiration(int value)
        {
            DeviceExpiration.Title = "Device Expiration";
            DeviceExpiration.Description = "Remaining";
            if (value <= DeviceExpirationLow) {
                // low
                DeviceExpiration.ImageName = "DeviceLow";
                DeviceExpiration.Color = Color.Red;
            } else if (value <= DeviceExpirationWarning) {
                // Warning
                DeviceExpiration.ImageName = "DeviceWarning";
                DeviceExpiration.Color = Color.Orange;
            } else {
                // full
                DeviceExpiration.ImageName = "DeviceFull";
                DeviceExpiration.Color = Color.Green;
            }
            UpdateBarColor(DeviceExpiration.Color);
            DeviceExpiration.Image = DeviceExpiration.ImageName;
            DeviceExpiration.Value = string.Format("{0}", (int)((value < 365) ? value : value / 365));
            DeviceExpiration.Type = (value < 365) ? ((value != 1) ? "DAYS" : "DAY") : "YEARS";
            OnPropertyChanged("DeviceExpiration");
            return deviceExpiration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SensorStatus UpdateSensoryExpiration(int value)
        {
            SensoryExpiration.Title = "Sensory Expiration";
            SensoryExpiration.Description = "Remaining";
            if (value <= SensoryExpirationLow) {
                // low
                SensoryExpiration.ImageName = "SensorLow";
                SensoryExpiration.Color = Color.Red;
            } else if (value <= SensoryExpirationWarning) {
                // Warning
                SensoryExpiration.ImageName = "SensorWarning";
                SensoryExpiration.Color = Color.Orange;
            } else {
                // full
                SensoryExpiration.ImageName = "SensorFull";
                SensoryExpiration.Color = Color.Green;
            }
            UpdateBarColor(SensoryExpiration.Color);
            SensoryExpiration.Image = SensoryExpiration.ImageName;
            SensoryExpiration.Type = (value < 365) ? ((value != 1) ? "DAYS" : "DAY") : "YEARS";
            SensoryExpiration.Value = string.Format("{0}", (int)((value < 365) ? value : value / 365));
            OnPropertyChanged("SensoryExpiration");
            return SensoryExpiration;
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
            return qualityControlExpiration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateDeviceState(int value)
        {
            DeviceState.Title = "Device State";
            DeviceState.Value = string.Format("{0}%", value);
            DeviceState.Type = "CHARGE";
            DeviceState.Description = string.Format("{0} tests remaining", value);

            if (value <= DeviceStateLow)
            {
                // low
                DeviceState.ImageName = "DeviceStateLow";
                DeviceState.Description = "No tests remaining";
                DeviceState.Color = Color.Red;
            }
            else if (value <= DeviceStateWarning)
            {
                // Warning
                DeviceState.ImageName = "DeviceStateWarning";
                DeviceState.Color = Color.Orange;
            }
            else
            {
                // full
                DeviceState.ImageName = "DeviceStateFull";
                DeviceState.Color = Color.Green;
            }
            UpdateBarColor(DeviceState.Color);
            DeviceState.Image = DeviceState.ImageName;
            OnPropertyChanged("DeviceState");
            return DeviceState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SensorStatus UpdateRelativeHumidity(int value)
        {
            RelativeHumidity.Title = "Relative Humidity";
            RelativeHumidity.Value = string.Format("{0}%", value);
            RelativeHumidity.Type = "CHARGE";
            RelativeHumidity.Description = string.Format("{0} tests remaining", value);

            if (value <= RelativeHumidityLow)
            {
                // low
                RelativeHumidity.ImageName = "RelativeHumidityLow";
                RelativeHumidity.Description = "No tests remaining";
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
    }
}
