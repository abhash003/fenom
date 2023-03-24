#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using PluginBleIDevice = Plugin.BLE.Abstractions.Contracts.IDevice;
using System.Diagnostics;
using FenomPlus.SDK.Core.Models;
using Plugin.BLE.Abstractions;
using FenomPlus.Services.DeviceService.Utils;
using FenomPlus.Services.DeviceService.Abstract;
using FenomPlus.SDK.Core.Ble.Interface;
using System.Collections.Generic;
using FenomPlus.SDK.Core.Utils;
using FenomPlus.SDK.Core.Features;

namespace FenomPlus.Services.DeviceService.Concrete
{
    internal class BleDevice : Device
    {
        readonly IAdapter _bleAdapter = CrossBluetoothLE.Current.Adapter;
        PluginBleIDevice _bleDevice;


        public BleDevice(object bleDevice) : base(bleDevice)
        {
            _bleDevice = (PluginBleIDevice)bleDevice;
        }

        public override string Name
        {
            get { return _bleDevice.Name; }
        }

        public override Guid Id
        {
            get { return ((PluginBleIDevice)_nativeDevice).Id; }
        }

        public override bool Connected
        {
            get
            {
                if (_bleDevice.State == DeviceState.Limited)
                {
                    Helper.WriteDebug("Connection state: LIMITED, reconnecting.");
                    _bleAdapter.ConnectToDeviceAsync(_bleDevice);
                }

                bool connected = ((PluginBleIDevice)_nativeDevice).State == DeviceState.Connected;
                if (connected)
                {
                    ReadyForTest = true;
                }
                return connected;
            }
        }

        private object _handlerLock = new object();

        public IEnumerable<IGattCharacteristic> GattCharacteristics { get; } = new SynchronizedList<IGattCharacteristic>();

        public override async Task ConnectAsync()
        {
            if (_bleAdapter != null && ((PluginBleIDevice)_nativeDevice).State != DeviceState.Connected)
                {
                    try
                    {
                        var device = (PluginBleIDevice)_nativeDevice;

                        // connect to the device
                        await _bleAdapter.ConnectToDeviceAsync((PluginBleIDevice)_nativeDevice, default, default);

                        if (device.State != DeviceState.Connected)
                        {
                            throw new Exception("ConnectToDeviceAsync() error");
                        }
                        else
                        {
                            Helper.WriteDebug($"Connection state: {device.State}");
                        }

                        // get service
                        var service = await device.GetServiceAsync(new Guid(FenomPlus.SDK.Core.Constants.FenomService));

                        if (service == null)
                        {
                            throw new Exception("GetServiceAsync() returned null, this isn't correct behavior!");
                        }

                        // get characteristics
                        var fwChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                            .FeatureWriteCharacteristic));

                        FwCharacteristic = fwChar;

                        var devChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                            .DeviceInfoCharacteristic));

                        var envChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                            .EnvironmentalInfoCharacteristic));

                        var bmChar =
                            await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                                .BreathManeuverCharacteristic));

                        var dbgChar =
                            await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                                .DebugMessageCharacteristic));

                        var deviceStatusChar =
                            await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                                .DeviceStatusCharacteristic));

                        var errorStatusChar =
                            await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                                .ErrorStatusCharacteristic));

                        devChar.ValueUpdated += (sender, e) =>
                        {
                            lock (_handlerLock)
                            {                                
                                DecodeDeviceInfo(e.Characteristic.Value);
                                Console.WriteLine("updated characteristic: device info");
                            }
                        };

                        envChar.ValueUpdated += (sender, e) =>
                        {
                            lock (_handlerLock)
                            {
                                DecodeEnvironmentalInfo(e.Characteristic.Value);
                                Console.WriteLine("updated characteristic: environmental info");
                            }
                        };

                        bmChar.ValueUpdated += (sender, e) =>
                        {
                            lock (_handlerLock)
                            {
                                DecodeBreathManeuver(e.Characteristic.Value);
                                Console.WriteLine($"updated characteristic: breath maneuver (flow: {BreathManeuver.BreathFlow} score: {BreathManeuver.NOScore})");
                            }
                        };

                        dbgChar.ValueUpdated += (sender, e) =>
                        {
                            lock (_handlerLock)
                            {
                                DecodeDebugMsg(e.Characteristic.Value);
                                //Console.WriteLine("updated characteristic: debug message");
                            }
                        };

                        deviceStatusChar.ValueUpdated += (sender, e) =>
                        {
                            lock (_handlerLock)
                            {
                                DecodeDeviceStatusInfo(e.Characteristic.Value);
                                Console.WriteLine($"DEVICE STATUS:  (value={DeviceStatusInfo.StatusCode:X})");
                            }
                        };

                        errorStatusChar.ValueUpdated += (sender, e) =>
                        {
                            lock (_handlerLock)
                            {
                                LastErrorCode = ErrorStatusInfo.ErrorCode;
                                DecodeErrorStatusInfo(e.Characteristic.Value);
                                Console.WriteLine($"ERROR STATUS:  (value={ErrorStatusInfo.ErrorCode:X})");
                            }
                        };

                        {
                            DeviceInfo = new DeviceInfo();
                            EnvironmentalInfo = new EnvironmentalInfo();
                        }

                        await bmChar.StartUpdatesAsync();
                        await devChar.StartUpdatesAsync();
                        await envChar.StartUpdatesAsync();
                        await dbgChar.StartUpdatesAsync();
                        await deviceStatusChar.StartUpdatesAsync();
                        await errorStatusChar.StartUpdatesAsync();
                    }
                    catch (Exception ex)
                    {
                        Helper.WriteDebug(ex);
                        throw;
                    }
                }

        }

        public override async Task ConnectToKnownDeviceAsync(Guid id)
        {
            var systemDevice = _bleAdapter.GetSystemConnectedOrPairedDevices();
            if (_bleAdapter != null && id != Guid.Empty)
            {
                try
                {
                    await _bleAdapter.StopScanningForDevicesAsync();
                    await _bleAdapter.ConnectToKnownDeviceAsync(id);
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public override async Task DisconnectAsync()
        {
            if (((PluginBleIDevice)_nativeDevice).State == DeviceState.Connected)
            {
                await _bleAdapter.DisconnectDeviceAsync((PluginBleIDevice)_nativeDevice);
            }
        }


        #region Partial Class

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> DEVICEINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEVICEINFO);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> ENVIROMENTALINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_ENVIROMENTALINFO);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_BREATHMANUEVER, (byte)breathTestEnum);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> BREATHMANUEVER()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_BREATHMANUEVER);
            return await WRITEREQUEST(message, 1);
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public async Task<bool> TRAININGMODE()
        //{
        //    MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_TRAININGMODE);
        //    return await WRITEREQUEST(message, 1);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> DEBUGMSG()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEBUGMSG);
            return await WRITEREQUEST(message, 1);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public async Task<bool> DEBUGMANUEVERTYPE()
        //{
        //    MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEBUGMANUEVERTYPE);
        //    return await WRITEREQUEST(message, 1);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override async Task<bool> MESSAGE(MESSAGE message)
        {
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerailNumber"></param>
        /// <returns></returns>
        public override async Task<bool> SERIALNUMBER(string SerailNumber)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_SERIALNUMBER, SerailNumber);
            return await WRITEREQUEST(message, 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public override async Task<bool> DATETIME(string date, string time)
        {
            string strDateTime;

            strDateTime = date + "T" + time;

            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_DATETIME, strDateTime);
            return await WRITEREQUEST(message, (short)strDateTime.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iD_SUB"></param>
        /// <param name="cal"></param>
        /// <returns></returns>
        public override async Task<bool> CALIBRATION(ID_SUB iD_SUB, double cal1, double cal2, double cal3)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_CALIBRATION_DATA, iD_SUB, cal1, cal2, cal3);
            return await WRITEREQUEST(message, 24);
        }

        public Plugin.BLE.Abstractions.Contracts.ICharacteristic FwCharacteristic = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override async Task<bool> WRITEREQUEST(MESSAGE message, short idvar_size)
        {
            using (var tracer = new Helper.FunctionTrace())
            {
                byte[] data = new byte[2 + 2 + idvar_size];

                data[0] = (byte)(message.IDMSG >> 8);
                data[1] = (byte)message.IDMSG;
                data[2] = (byte)(message.IDSUB >> 8);
                data[3] = (byte)message.IDSUB;

                Buffer.BlockCopy(message.IDVAR, 0, data, 4, idvar_size);

                if (true)
                {
                    // get service
                    var device = (PluginBleIDevice)_nativeDevice;

                    var service = await device.GetServiceAsync(new Guid(FenomPlus.SDK.Core.Constants.FenomService));

                    // get characteristics
                    var fwChar = await service.GetCharacteristicAsync(new Guid(FenomPlus.SDK.Core.Constants
                        .FeatureWriteCharacteristic));

                    fwChar.WriteType = Plugin.BLE.Abstractions.CharacteristicWriteType.WithoutResponse;
                    bool result = await fwChar.WriteAsync(data);

                    if (result == true)
                    {
                        tracer.Trace("write without response okay");
                    }
                    else
                    {
                        tracer.Trace("something went wrong");
                    }
                    return result;
                }
            }
        }



        #endregion

        #region Send Characteristics

        /// <summary>
        /// 
        /// </summary>
        /// <param name="breathTestEnum"></param>
        /// <returns></returns>
        public override async Task<bool> StartTest(BreathTestEnum breathTestEnum)
        {
            if (IsConnected())
            {
                BreathTestInProgress = true;
                return await BREATHTEST(breathTestEnum);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> StopTest()
        {
            if (IsConnected())
            {
                BreathTestInProgress = false;
                return await BREATHTEST(BreathTestEnum.Stop);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> SendMessage(MESSAGE message)
        {
            if (IsConnected())
            {
                return await MESSAGE(message);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public async Task<bool> SendSerialNumber(string serialNumber)
        {
            if (IsConnected())
            {
                return await SERIALNUMBER(serialNumber);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<bool> SendDateTime(string date, string time)
        {
            if (IsConnected())
            {
                return await DATETIME(date, time);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iD_SUB"></param>
        /// <param name="cal1"></param>
        /// <returns></returns>
        public async Task<bool> SendCalibration(ID_SUB iD_SUB, double cal1, double cal2, double cal3)
        {
            if (IsConnected())
            {
                return await CALIBRATION(iD_SUB, cal1, cal2, cal3);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cal1"></param>
        /// <param name="cal2"></param>
        /// <param name="cal3"></param>
        /// <returns></returns>
        public async Task<bool> SendCalibration(double cal1, double cal2, double cal3)
        {
            bool result = true;
            await SendCalibration(ID_SUB.ID_CALIBRATION1, cal1, cal2, cal3);
            return result;
        }

        #endregion

        #region Request Characteristics

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RequestDeviceInfo()
        {
            if (IsConnected())
            {
                return await DEVICEINFO();
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RequestEnvironmentalInfo()
        {
            if (IsConnected())
            {
                return await ENVIROMENTALINFO();
            }
            return false;
        }

        #endregion
    }
}
