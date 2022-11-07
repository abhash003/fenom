﻿

using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using FenomPlus.Services;
using System;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class TitleContentViewModel : BaseViewModel
    {
        public const int BatteryLow = 3;
        public const int BatteryWarning = 20;
        public const int BatteryFull = 100;

        public const int SensorLow = 0;
        public const int SensorWarning = 60;
        public const int SensorFull = 1 * 365 + 10;

        public const int QualityControlExpirationLow = 0;
        public const int QualityControlExpirationWarning = 1;
        public const int QualityControlExpirationFull = 7;

        public const int DeviceLow = 0;
        public const int DeviceWarning = 60;
        public const int DeviceFull = 1 * 365 + 10;

        public const int RelativeHumidityLow = 0;
        public const int RelativeHumidityWarning = 80;
        public const int RelativeHumidityFull = 100;

        public const int TemperatureLow = 0;
        public const int TemperatureWarning = 80;
        public const int TemperatureFull = 100;

        public const int PressureLow = 0;
        public const int PressureWarning = 80;
        public const int PressureFull = 100;

        [ObservableProperty]
        private SensorStatus _battery;

        [ObservableProperty]
        private SensorStatus _sensor;

        [ObservableProperty]
        private SensorStatus _qualityControlExpiration;

        [ObservableProperty]
        private SensorStatus _device;

        [ObservableProperty]
        private SensorStatus _relativeHumidity;

        [ObservableProperty]
        private SensorStatus _temperature;

        [ObservableProperty]
        private SensorStatus _pressure;

        public TitleContentViewModel()
        {
            Battery = new SensorStatus();
            Sensor = new SensorStatus();
            QualityControlExpiration = new SensorStatus();
            Device = new SensorStatus();
            Temperature = new SensorStatus();
            Pressure = new SensorStatus();
        }

        public void RefreshIconStatus()
        {
            int BatteryLevel = Cache.BatteryLevel;
            int daysRemaining = (Cache.SensorExpireDate > DateTime.Now) ? (int)(Cache.SensorExpireDate - DateTime.Now).TotalDays : 0;

            UpdateBattery(BatteryLevel);
            UpdateDevice(daysRemaining);
            UpdateSensor(daysRemaining);
            UpdateQualityControlExpiration(0);
            UpdatePressure(0);
            UpdateRelativeHumidity(0);
            UpdateTemperature(0);
        }

        public SensorStatus UpdateBattery(int value)
        {
            Battery.Value = $"{value}%";

            if (value <= BatteryLow)
            {
                // low
                Battery.Image = "BatteryLow";
                Battery.Color = Color.Red;
            }
            else if (value <= BatteryWarning)
            {
                // warning
                Battery.Image = "BatteryWarning";
                Battery.Color = Color.Orange;
            }
            else
            {
                // full
                Battery.Image = "BatteryFull";
                Battery.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Battery));
            return Battery;
        }

        public SensorStatus UpdateSensor(int value)
        {
            if (value <= SensorLow)
            {
                // low
                Sensor.Image = "SensorLow";
                Sensor.Color = Color.Red;
            }
            else if (value <= SensorWarning)
            {
                // warning
                Sensor.Image = "SensorWarning";
                Sensor.Color = Color.Orange;
            }
            else
            {
                // full
                Sensor.Image = "SensorFull";
                Sensor.Color = Color.Green;
            }

            Sensor.Value = $"{(int)((value < 365) ? value : value / 365)}";
            OnPropertyChanged(nameof(Sensor));
            return Sensor;
        }

        public SensorStatus UpdateQualityControlExpiration(int value)
        {
            if (value <= QualityControlExpirationLow)
            {
                // low
                QualityControlExpiration.Image = "QualityControlLow";
                QualityControlExpiration.Color = Color.Red;
            }
            else if (value <= QualityControlExpirationWarning)
            {
                // warning
                QualityControlExpiration.Image = "QualityControlWarning";
                QualityControlExpiration.Color = Color.Orange;
            }
            else
            {
                // full
                QualityControlExpiration.Image = "QualityControlFull";
                QualityControlExpiration.Color = Color.Green;
            }

            QualityControlExpiration.Value = $"{(int)((value < 365) ? value : value / 365)}";
            OnPropertyChanged(nameof(QualityControlExpiration));
            return QualityControlExpiration;
        }

        public SensorStatus UpdateDevice(int value)
        {
            Device.Value = $"{value}";

            if (value <= DeviceLow)
            {
                // low
                Device.Image = "DeviceLow";
                Device.Color = Color.Red;
            }
            else if (value <= DeviceWarning)
            {
                // warning
                Device.Image = "DeviceWarning";
                Device.Color = Color.Orange;
            }
            else
            {
                // full
                Device.Image = "DeviceFull";
                Device.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Device));
            return Device;
        }

        public SensorStatus UpdateRelativeHumidity(int value)
        {
            RelativeHumidity.Value = $"{value}";

            if (value <= RelativeHumidityLow)
            {
                // low
                RelativeHumidity.Image = "RelativeHumidityLow";
                RelativeHumidity.Color = Color.Red;
            }
            else if (value <= RelativeHumidityWarning)
            {
                // warning
                RelativeHumidity.Image = "RelativeHumidityWarning";
                RelativeHumidity.Color = Color.Orange;
            }
            else
            {
                // full
                RelativeHumidity.Image = "RelativeHumidityFull";
                RelativeHumidity.Color = Color.Green;
            }

            OnPropertyChanged(nameof(RelativeHumidity));
            return RelativeHumidity;
        }

        public SensorStatus UpdateTemperature(int value)
        {
            Temperature.Value = $"{value}";

            if (value <= TemperatureLow)
            {
                // low
                Temperature.Image = "TemperatureLow";
                Temperature.Color = Color.Red;
            }
            else if (value <= TemperatureWarning)
            {
                // warning
                Temperature.Image = "TemperatureWarning";
                Temperature.Color = Color.Orange;
            }
            else
            {
                // full
                Temperature.Image = "TemperatureFull";
                Temperature.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Temperature));
            return Temperature;
        }

        public SensorStatus UpdatePressure(int value)
        {
            Pressure.Value = $"{value}";

            if (value <= PressureLow)
            {
                // low
                Pressure.Image = "PressureLow";
                Pressure.Color = Color.Red;
            }
            else if (value <= PressureWarning)
            {
                // warning
                Pressure.Image = "PressureWarning";
                Pressure.Color = Color.Orange;
            }
            else
            {
                // full
                Pressure.Image = "PressureFull";
                Pressure.Color = Color.Green;
            }

            OnPropertyChanged(nameof(Pressure));
            return Pressure;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
