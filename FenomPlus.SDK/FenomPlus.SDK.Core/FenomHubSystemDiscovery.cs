using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using FenomPlus.Interfaces;
using FenomPlus.SDK.Abstractions;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Ble.PluginBLE;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Utils;
using Microsoft.Extensions.Logging;
using Plugin.BLE.Abstractions.EventArgs;
using TinyIoC;
using Xamarin.Forms;

namespace FenomPlus.SDK.Core
{
    public class FenomHubSystemDiscovery : IFenomHubSystemDiscovery
    {
        public static TinyIoCContainer Container => TinyIoCContainer.Current;

        private LoggingManager _loggingMaager;
        private Logger _logger;
        private IFenomHubSystem _FenomHubSystem;
        public readonly IBleRadioService BleRadio;
        private IDialogService DialogService;

        // Define message
        public class DeviceConnectedMessage : ValueChangedMessage<bool>
        {
            public DeviceConnectedMessage(bool isConnected) : base(isConnected)
            {
            }
        }

        public FenomHubSystemDiscovery()
        {
            //PerformanceLogger.StartLog(typeof(FenomHubSystemDiscovery), "FenomHubSystemDiscovery");
            BleRadio = new BleRadioService();
            BleRadio.DeviceConnected += BleRadioOnDeviceConnected;
            BleRadio.DeviceConnectionLost += DeviceConnectionLost;
            _loggingMaager = LoggingManager.GetInstance;
            _logger = new Logger("FenomBLE");

            //PerformanceLogger.EndLog(typeof(FenomHubSystemDiscovery), "FenomHubSystemDiscovery");

            DialogService =  Container.Resolve<IDialogService>();
        }


        private void BleRadioOnDeviceConnected(object sender, DeviceEventArgs e)
        {
            // Send message
            WeakReferenceMessenger.Default.Send(new DeviceConnectedMessage(true));
            Debug.WriteLine("!!!!!  Device Connected");

            DialogService.ShowToast("Bluetooth connected", 4);
        }

        private void DeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            // Send message
            WeakReferenceMessenger.Default.Send(new DeviceConnectedMessage(false));
            Debug.WriteLine("!!!!!  Device Lost connection");

            DialogService.ShowToast("Bluetooth connection was lost...", 4);
        }

        public IFenomHubSystem FenomHubSystem
        {
            get => _FenomHubSystem;
            set => _FenomHubSystem = value as IFenomHubSystem;
        }

        public void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggingMaager.SetLoggingFactory(loggerFactory);
        }

        public bool IsScanning => BleRadio.IsScanning;

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

                await BleRadio.Scan(scanTime.TotalMilliseconds, scanBondedDevices, scanBleDevices,
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

        public async Task<bool> StopScan()
        {
            //PerformanceLogger.StartLog(typeof(FenomHubSystemDiscovery), "StopScan");
            _logger.LogDebug("FenomHubSystemDiscovery: StopScan");

            await BleRadio.StopScan();

            //PerformanceLogger.EndLog(typeof(FenomHubSystemDiscovery), "StopScan");
            return true;
        }
    }
}