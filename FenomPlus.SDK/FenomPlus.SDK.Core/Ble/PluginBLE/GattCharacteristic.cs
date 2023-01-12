using Plugin.BLE.Abstractions;
using FenomPlus.SDK.Core.Ble.Interface;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.EventArgs;
using FenomPlus.Services;
using FenomPlus.Interfaces;

namespace FenomPlus.SDK.Core.Ble.PluginBLE
{
    public class GattCharacteristic : IGattCharacteristic
    {
        private IAppServices Services => IOC.Services;
        private ICacheService Cache => Services.Cache;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim _deviceInfolock = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim _environmentalIngolock = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim _errorStatusInfolock = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim _debugMsglock = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim _breathManeuverlock = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim _deviceStatuslock = new SemaphoreSlim(1, 1);
        private Plugin.BLE.Abstractions.Contracts.ICharacteristic Characteristic { get; }
        public Guid Uuid => Characteristic.Id;

        public GattCharacteristic(Plugin.BLE.Abstractions.Contracts.ICharacteristic characteristic)
        {
            try
            {
                Characteristic = characteristic;
                return;
                try
                {
                    if (Uuid.ToString().ToUpper() == Constants.DeviceInfoCharacteristic.ToUpper())
                    {
                        //Characteristic.ValueUpdated += DeviceInfoHandler;
                        if (Characteristic.CanUpdate)
                        {
                            Characteristic.StartUpdatesAsync();
                        }
                    }
                    else if (Uuid.ToString().ToUpper() == Constants.EnvironmentalInfoCharacteristic.ToUpper())
                    {
                        //Characteristic.ValueUpdated += EnvironmentalInfoHandler;
                        if (Characteristic.CanUpdate)
                        {
                            Characteristic.StartUpdatesAsync();
                        }
                    }
                    else if (Uuid.ToString().ToUpper() == Constants.ErrorStatusCharacteristic.ToUpper())
                    {
                        Characteristic.ValueUpdated += ErrorStatusInfoHandler;
                        if (Characteristic.CanUpdate)
                        {
                            Characteristic.StartUpdatesAsync();
                        }
                    }
                    else if (Uuid.ToString().ToUpper() == Constants.DeviceStatusCharacteristic.ToUpper())
                    {
                        Characteristic.ValueUpdated += DeviceStatusInfoHandler;
                        if (Characteristic.CanUpdate)
                        {
                            Characteristic.StartUpdatesAsync();
                        }
                    }
                    else if (Uuid.ToString().ToUpper() == Constants.BreathManeuverCharacteristic.ToUpper())
                    {
                        //Characteristic.ValueUpdated += BreathManeuverHandler;
                        if (Characteristic.CanUpdate)
                        {
                            Characteristic.StartUpdatesAsync();
                        }
                    }
                    else if (Uuid.ToString().ToUpper() == Constants.DebugMessageCharacteristic.ToUpper())
                    {
                        //Characteristic.ValueUpdated += DebugMsgHandler;
                        if (Characteristic.CanUpdate)
                        {
                            Characteristic.StartUpdatesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Services.LogCat.Print(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex.Message);
            }
        }

        public async Task<byte[]> ReadAsync()
        {
            await _lock.WaitAsync();

            try
            {
                if (!Characteristic.CanRead)
                {
                    throw new Exception("Characteristic cannot be read");
                }

                return await Characteristic.ReadAsync();
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex.Message);
                return null;
            }
            finally
            {
                _lock.Release();
            }
        }        

        static object __lock = new object();

        public async Task<bool> WriteWithoutResponseAsync(byte[] value)
        {
            //await _lock.WaitAsync();
            lock (__lock)
            {
                try
                {
                    if (!Characteristic.CanWrite)
                    {
                        throw new Exception("Characteristic cannot be written");
                    }

                    Characteristic.WriteType = CharacteristicWriteType.WithoutResponse;

                    Characteristic.WriteAsync(value).ConfigureAwait(continueOnCapturedContext: false);
                    return true;
                }
                catch (Exception ex)
                {
                    Services.LogCat.Print(ex.Message);
                    return false;
                }
                finally
                {
                    //_lock.Release();
                }
            }
        }

        private void DeviceInfoHandler(object sender, CharacteristicUpdatedEventArgs e)
        {
            _deviceInfolock.Wait();
            try
            {
                Cache.DecodeDeviceInfo(e.Characteristic.Value);
                Debug.WriteLine("***** DeviceInfoHandler called: DeviceInfo Updated in Cache");
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex.Message);
            }
            finally
            {
                _deviceInfolock.Release();
            }
        }

        private void EnvironmentalInfoHandler(object sender, CharacteristicUpdatedEventArgs e)
        {
            _environmentalIngolock.Wait();
            try
            {
                Cache.DecodeEnvironmentalInfo(e.Characteristic.Value);
                Debug.WriteLine("***** EnvironmentalInfoHandler called: EnvironmentalInfo Updated in Cache");
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex.Message);
            }
            finally
            {
                _environmentalIngolock.Release();
            }
        }

        private void ErrorStatusInfoHandler(object sender, CharacteristicUpdatedEventArgs e)
        {
            _errorStatusInfolock.Wait();
            try
            {
                Cache.DecodeErrorStatusInfo(e.Characteristic.Value);
                Debug.WriteLine("***** ErrorInfoHandler called: ErrorInfo Updated in Cache");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _errorStatusInfolock.Release();
            }
        }

        private void DeviceStatusInfoHandler(object sender, CharacteristicUpdatedEventArgs e)
        {
            _deviceStatuslock.Wait();
            try
            {
                Cache.DecodeDeviceStatusInfo(e.Characteristic.Value);
                Debug.WriteLine("***** StatusInfoHandler called: StatusInfo Updated in Cache");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _deviceStatuslock.Release();
            }
        }

        private void BreathManeuverHandler(object sender, CharacteristicUpdatedEventArgs e)
        {
            _breathManeuverlock.Wait();
            try
            {
                Cache.DecodeBreathManeuver(e.Characteristic.Value);
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex.Message);
            }
            finally
            {
                _breathManeuverlock.Release();
            }
        }

        private void DebugMsgHandler(object sender, CharacteristicUpdatedEventArgs e)
        {
            _debugMsglock.Wait();
            try
            {
                Cache.DecodeDebugMsg(e.Characteristic.Value);
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex.Message);
            }
            finally
            {
                _debugMsglock.Release();
            }
        }
    }
}