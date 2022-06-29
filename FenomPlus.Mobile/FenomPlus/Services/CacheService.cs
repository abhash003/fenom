﻿using System;
using System.Text;
using FenomPlus.Helpers;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using Microsoft.Extensions.Logging;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace FenomPlus.Services
{
    public class CacheService : BaseService, ICacheService
    {
        private RingBuffer BreathBuffer;

        public CacheService(IAppServices services) : base(services)
        {
            BreathBuffer = new RingBuffer(20, 100);

            BreathFlowTimer = 50;
            DeviceSerialNumber = "F150-??????";
            Firmware = "Firmware ?.?.?";

            Logger = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug)
                    .AddFilter("FenomPlus", LogLevel.Debug);
            });

            DebugList = new RangeObservableCollection<DebugLog>();
            _EnvironmentalInfo = new EnvironmentalInfo();
            _BreathManeuver = new BreathManeuver();
            _DeviceInfo = new DeviceInfo();
            _DebugMsg = new DebugMsg();

            // write path to debug
            DebugList.Insert(0, DebugLog.Create("App Starting"));
            DebugList.Insert(0, DebugLog.Create(Services.DebugLogFile.GetFilePath()));
        }

        public RangeObservableCollection<DebugLog> DebugList {get;set;}
        public ILoggerFactory Logger { get; set; }

        public int BatteryLevel { get; set; }
        public string Firmware { get; set; }
        public string DeviceSerialNumber { get; set; }
        public DateTime SensorExpireDate { get; set; }
        public TestTypeEnum TestType { get; set; }
        public int BreathFlow { get; set; }
        public int BreathFlowTimer { get; set; }
        public int NOScore { get; set; }

        public EnvironmentalInfo _EnvironmentalInfo { get; set; }
        public BreathManeuver _BreathManeuver { get; set; }
        public DeviceInfo _DeviceInfo { get; set; }
        public DebugMsg _DebugMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private static ISettings AppSettings
        {
            get { return CrossSettings.Current; }
        }

        public bool BatteryStatus
        {
            get { return AppSettings.GetValueOrDefault("DeviceBatteryConfirmed_key", false); }
            set { AppSettings.AddOrUpdateValue("DeviceBatteryConfirmed_key", value); }
        }

        public bool DeviceSensorExpiring
        {
            get { return AppSettings.GetValueOrDefault("DeviceSensorExpiringConfirmed_key", false); }
            set { AppSettings.AddOrUpdateValue("DeviceSensorExpiringConfirmed_key", value); }
        }

        public bool DeviceExpiring
        {
            get { return AppSettings.GetValueOrDefault("DeviceExpiringConfirmed_key", false); }
            set { AppSettings.AddOrUpdateValue("DeviceExpiringConfirmed_key", value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public string QCUsername { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public EnvironmentalInfo DecodeEnvironmentalInfo(byte[] data)
        {
            if(_EnvironmentalInfo == null)
            {
                _EnvironmentalInfo = new EnvironmentalInfo();
            }
            _EnvironmentalInfo.Decode(data);

            BatteryLevel = _EnvironmentalInfo.BatteryLevel;

            NotifyViews();
            NotifyViewModels();
            return _EnvironmentalInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public BreathManeuver DecodeBreathManeuver(byte[] data)
        {
            if (_BreathManeuver == null)
            {
                _BreathManeuver = new BreathManeuver();
            }

            _BreathManeuver.Decode(data);

            // add new value and average it
            BreathFlow = BreathBuffer.Add(_BreathManeuver.BreathFlow);

            // get the noscores
            NOScore = _BreathManeuver.NOScore;

            NotifyViews();
            NotifyViewModels();
            return _BreathManeuver;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DeviceInfo DecodeDeviceInfo(byte[] data)
        {
            try
            {
                if (_DeviceInfo == null)
                {
                    _DeviceInfo = new DeviceInfo();
                }

                _DeviceInfo.Decode(data);

                // setup serial number
                if ((_DeviceInfo.SerialNumber != null) && (_DeviceInfo.SerialNumber.Length > 0))
                {
                    DeviceSerialNumber = string.Format("F150-{0}",Encoding.Default.GetString(_DeviceInfo.SerialNumber));

                    // update the database
                    Services.Database.QualityControlDevicesRepo.UpdateDateOrAdd(DeviceSerialNumber);
                }

                // setup firmware version
                Firmware = string.Format("Firmware {0}.{1}.{2}", _DeviceInfo.MajorVersion, _DeviceInfo.MinorVersion, _DeviceInfo.BuildVersion);

                // get SensorExpireDate
                SensorExpireDate = new DateTime(_DeviceInfo.SensorExpDateYear, _DeviceInfo.SensorExpDateMonth, _DeviceInfo.SensorExpDateDay);
            }
            catch(Exception ex)
            {
                Console.Write(ex);
            }
            NotifyViews();
            NotifyViewModels();
            return _DeviceInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DebugMsg DecodeDebugMsg(byte[] data)
        {
            if (_DebugMsg == null)
            {
                _DebugMsg = new DebugMsg();
            }


            _DebugMsg.Decode(data);

            DebugLog debugLog = DebugLog.Create(data);

            DebugList.Insert(0, debugLog);
            Services.DebugLogFile.Write(debugLog);
            NotifyViews();
            NotifyViewModels();
            return _DebugMsg;
        }

        /// <summary>
        /// 
        /// </summary>
        private void NotifyViews()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private void NotifyViewModels()
        {
        }

    }
}

