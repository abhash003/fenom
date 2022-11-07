using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FenomPlus.SDK.Abstractions;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Ble.PluginBLE;
using FenomPlus.SDK.Core.Utils;
using Microsoft.Extensions.Logging;
using Plugin.BLE.Abstractions.EventArgs;

namespace FenomPlus.SDK.Core
{
    public class FenomHubSystemDiscovery : IFenomHubSystemDiscovery
    {
        private LoggingManager _loggingMaager;
        private Logger _logger;
        private IFenomHubSystem _FenomHubSystem;
        private readonly IBleRadioService _bleRadio;

        /// <summary>
        /// 
        /// </summary>
        public FenomHubSystemDiscovery()
        {
            //PerformanceLogger.StartLog(typeof(FenomHubSystemDiscovery), "FenomHubSystemDiscovery");
            _bleRadio = new BleRadioService();
            _bleRadio.DeviceConnectionLost += DeviceConnectionLost;
            _loggingMaager = LoggingManager.GetInstance;
            _logger = new Logger("FenomBLE");
            //PerformanceLogger.EndLog(typeof(FenomHubSystemDiscovery), "FenomHubSystemDiscovery");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public IFenomHubSystem FenomHubSystem
        {
            get => _FenomHubSystem;
            set => _FenomHubSystem = value as IFenomHubSystem;
        }

        /// <summary>
        /// SetLoggerFactory
        /// </summary>
        public void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggingMaager.SetLoggingFactory(loggerFactory);
        }

        /// <summary>
        /// IsScanning
        /// </summary>
        public bool IsScanning => _bleRadio.IsScanning;

        /// <summary>
        /// Scan
        /// </summary>
        public async Task<IEnumerable<IFenomHubSystem>> Scan(TimeSpan scanTime = default, bool scanBondedDevices = true, bool scanBleDevices = true, Action<IBleDevice> deviceFoundCallback = null, Action<IEnumerable<IBleDevice>> scanCompletedCallback = null)
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(FenomHubSystemDiscovery), "Scan");
                _logger.LogDebug("FenomHubSystemDiscovery: Scan");

                if (IsScanning)
                {
                    _logger.LogError("FenomHubSystemDiscovery.Scan() - already scanning");
                    return null;
                }

                await _bleRadio.Scan(scanTime.TotalMilliseconds, scanBondedDevices, scanBleDevices,
                    ((IBleDevice bleDevice) =>
                    {
                        if ((bleDevice != null) && (!string.IsNullOrEmpty(bleDevice.Name)))
                        {
                            if (bleDevice.Name.ToUpper().StartsWith("FENOM"))
                            {
                                deviceFoundCallback?.Invoke(bleDevice);
                            }
                        }
                    }),
                    ((IEnumerable<IBleDevice> bleDevices) =>
                    {
                        scanCompletedCallback?.Invoke(bleDevices);
                    }),
                    (() =>
                    {

                    })
                );

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(FenomHubSystemDiscovery), "Scan");
            }
        }

        /// <summary>
        /// StopScan
        /// </summary>
        public async Task<bool> StopScan()
        {
            //PerformanceLogger.StartLog(typeof(FenomHubSystemDiscovery), "StopScan");
            _logger.LogDebug("FenomHubSystemDiscovery: StopScan");

            await _bleRadio.StopScan();

            //PerformanceLogger.EndLog(typeof(FenomHubSystemDiscovery), "StopScan");
            return true;
        }
    }
}