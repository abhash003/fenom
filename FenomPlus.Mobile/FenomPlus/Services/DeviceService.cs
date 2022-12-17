#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FenomPlus.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;


#region Namespace Aliases

using PlginBleIDevice = Plugin.BLE.Abstractions.Contracts;

using aliasNamespace = Plugin.BLE;

using PluginBleIDevice = Plugin.BLE.Abstractions.Contracts.IDevice;
using System.Dynamic;
using Plugin.Settings;
using Syncfusion.Data;
using FenomPlus.SDK.Abstractions;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;
using FenomPlus.SDK.Core;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Collections.ObjectModel;
using Syncfusion.SfDataGrid.XForms;
using System.Linq;
using Plugin.BLE.Abstractions.Exceptions;
using FenomPlus.SDK.Core.Ble.PluginBLE;
using System.ComponentModel;
using FenomPlus.SDK.Core.Utils;
using Xamarin.Forms;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Plugin.BLE.Abstractions.Extensions;

#endregion

/*
    [Section Ordering Within Classes/Structs/Interfaces]

    Constant Fields
    Fields
    Constructors
    Finalizers (Destructors)
    Delegates
    Events
    Enums
    Interfaces (interface implementations)
    Properties
    Indexers
    Methods
    Structs
    Classes
*/

namespace FenomPlus.Services.NewArch.R2
{
    public struct Helper
    {
        public static void WriteDebug(String msg, int prependCount = 0, char prependChar = ' ')
        {
#if DEBUG
            var logMessage = String.Format("{0} : [{1}] - {2}", new string(prependChar, prependCount), Thread.CurrentThread.ManagedThreadId, msg);

            var logger = AppServices.Container.Resolve<ILogCatService>();

            if (logger == null)
            {
                throw new ArgumentNullException("No logger available. Something is wrong!");
            }

            logger.Print(logMessage);
#endif
        }

        public static void WriteDebug(Exception ex)
        {
            WriteDebug($"Exception: {ex.Message} (${ex.ToString()})");
        }

        public class FunctionTrace : IDisposable
        {
            //private static int _indentLevel = 1;

            private readonly static string _pre  = "--------> Entering: ";
            private readonly static string _post = "<-------- Exiting:  ";
            private readonly static char _indentChar = ' ';

            private readonly static int _skipFrames = 1;
            private string _functionName;

            public FunctionTrace()
            {
                _functionName = new StackFrame(_skipFrames).GetMethod().ReflectedType.FullName;
                Prologue();
            }

            public void Dispose()
            {
                Epilogue();
            }

            private void Prologue()
            {
                Helper.WriteDebug($"{_pre} {_functionName}");
            }
            private void Epilogue()
            {
                Helper.WriteDebug($"{_post} {_functionName}");
            }

            public void Trace(string msg)
            {
                var indent = 2;
                var spacer = $"{new String(_indentChar, indent)}";
                Helper.WriteDebug($"{spacer} {msg}");
            }
        }
    }
}

namespace FenomPlus.Services.NewArch.R2
{
    public interface IDeviceService
    {
        // Events
        event EventHandler DeviceDiscovered;
        event EventHandler DeviceConnected;
        event EventHandler DeviceConnectionLost;
        event EventHandler DeviceDisconnected;

        // Properties
        bool Discovering { get; }
        IList<IDevice> Devices { get; }

#nullable enable
        IDevice? Current { get; set; }
#nullable disable

        // Methods
        void StartDiscovery(Action<IDevice> deviceFoundAction);
        void StopDiscovery();
    }
}

namespace FenomPlus.Services.NewArch.R2
{
    public class DeviceServiceEventArgs : EventArgs
    {
        public IDevice Device;

        public DeviceServiceEventArgs(IDevice device)
        {
            Device = device;
        }
    }

    public class DeviceService : BaseService, IDeviceService
    {
        // Fields

        protected volatile bool _discovering;
        protected volatile bool _shouldStopDiscovering;

        // not thread safe, needs to be ...
        protected volatile ObservableCollection<IDevice> _devices;

        protected IDevice _current;

        protected List<DeviceDiscoverer> _deviceDiscoverers;

        //Task _monitorTask;
        //CancellationTokenSource _monitorTaskCancelSource;

        Thread _workerThread;

        Action<IDevice> _deviceFoundAction;

        // Constructors

        public DeviceService(IAppServices appServices) : base(appServices)
        {
            using (var trace = new Helper.FunctionTrace())
            {
                _discovering = false;
                _shouldStopDiscovering = false;

                _devices = new ObservableCollection<IDevice>();

                _deviceDiscoverers = new List<DeviceDiscoverer> { new BleScanner(this), new UsbEnumerator(this) };

                _current = null;

                _deviceFoundAction = null;

                ReadyForTest = true;
                DeviceReadyTimer = new Timer(1000);
                DeviceReadyTimer.Elapsed += DeviceReadyTimerOnElapsed;

                trace.Trace("device service all setup");
            }
        }

        // Properties

        public bool Discovering => _discovering;
        public IList<IDevice> Devices => _devices;
        public IDevice Current { get => _current; set => _current = value; }

        // Methods

        public void StartDiscovery(Action<IDevice> deviceFoundAction)
        {
            if (!_discovering)
            {
                foreach (IDeviceDiscoverer dd in _deviceDiscoverers)
                {
                    dd.StartDiscovery();
                }

                _shouldStopDiscovering = false;
                _deviceFoundAction = deviceFoundAction;

                //_workerThread = new Thread(new ThreadStart(DiscoveryWorker));
                //_workerThread.Start();

                _discovering = true;
            }
        }

        public void StopDiscovery()
        {
            if (_workerThread != null && _discovering)
            {
                _shouldStopDiscovering = true;
                //_workerThread?.Join();
            }

            foreach (var dd in _deviceDiscoverers)
            {
                dd.StopDiscoveryAsync();
            }

            _discovering= false;
        }

        protected void DiscoveryWorker()
        {
            Helper.WriteDebug("ENTERING MONITOR THREAD");

            try
            {
                //_devices.Clear();
                _discovering = true;

                while (_shouldStopDiscovering == false)
                {
#if false
                    if (((CancellationTokenSource)cancelToken).IsCancellationRequested)
                    {
                        Helper.WriteDebug("CANCELLATION SIGNALED FOR MONITOR THREAD");
                        break;
                    }
#endif

                    Helper.WriteDebug("HEART BEAT");
                    Thread.Sleep(10000);
                }

                // cleanup

                _discovering = false;
                //_shouldStopDiscovering = true;
            }
            catch (ThreadAbortException ex)
            {
                // thread aborted
                Helper.WriteDebug("ABORTED MONITOR THREAD");
            }

            // exit monitor
            Helper.WriteDebug("EXITING MONITOR THREAD");
        }


        // Events
        public event EventHandler DeviceConnected;
        public event EventHandler DeviceConnectionLost;
        public event EventHandler DeviceDisconnected;
        public event EventHandler DeviceDiscovered;

        internal void HandleDeviceFound(BleDevice bleDevice)
        {
            Helper.WriteDebug("Invoking DeviceFound");
            _deviceFoundAction?.Invoke(bleDevice);
            Helper.WriteDebug("Invoking DeviceFound finshed");
        }

        internal void HandleDeviceConnected(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceConnected");
            DeviceConnected?.Invoke(this, new DeviceServiceEventArgs(device));
        }
        internal void HandleDeviceConnectionLost(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceConnectionLost");
            DeviceConnectionLost?.Invoke(this, new DeviceServiceEventArgs(device));
        }

        internal void HandleDeviceDisconnected(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceDisconnected");
            DeviceDisconnected?.Invoke(this, new DeviceServiceEventArgs(device));
        }
        internal void HandleDeviceDiscovered(IDevice device)
        {
            Helper.WriteDebug("Invoking DeviceDiscovered");
            DeviceDiscovered?.Invoke(this, new DeviceServiceEventArgs(device));
        }

        #region Fields

        private readonly Timer DeviceReadyTimer;

        #endregion

        public int DeviceReadyCountDown { get; set; }

        private bool _readyForTest;
        public bool ReadyForTest
        {
            get => _readyForTest;
            set
            {
                _readyForTest = value;

                if (_readyForTest == false)
                {
                    DeviceReadyCountDown = 32;
                    DeviceReadyTimer.Start();
                }
            }
        }

        private void DeviceReadyTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Helper.WriteDebug($"DeviceReadyTimerOnElapsed: DeviceReadyCountDown: {DeviceReadyCountDown}, ReadyForTest: {ReadyForTest}");
            DeviceReadyCountDown -= 1;

            if (DeviceReadyCountDown <= 0)
            {
                ReadyForTest = true;
                DeviceReadyTimer.Stop();
            }
        }
    }


    public interface IDeviceDiscoverer
    {
        // Methods

        void StartDiscovery();

        void StopDiscoveryAsync();
    }

    public abstract class DeviceDiscoverer : IDeviceDiscoverer
    {
        // Fields

        protected DeviceService _deviceService;

        // Constructor

        public DeviceDiscoverer(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // Interface implementation

        public abstract void StartDiscovery();

        public abstract void StopDiscoveryAsync();
    }

    public class BleScanner : DeviceDiscoverer
    {
        private static bool _constructed = false;
        private CancellationTokenSource _cancelTokenSource;

        // Fields

        Plugin.BLE.Abstractions.Contracts.IBluetoothLE _ble = null;

        // Constructor

        public BleScanner(DeviceService deviceService) : base(deviceService)
        {
            if (!_constructed)
            {
                _ble = CrossBluetoothLE.Current;
                _ble.Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
                _ble.Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
                _ble.Adapter.DeviceConnected += Adapter_DeviceConnected;
                _ble.Adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
                _ble.Adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;

                _constructed = true;
            }
            else
            {
                throw new Exception();
            }
        }

        // Methods

        public override async void StartDiscovery()
        {
            if (_ble.Adapter.IsScanning)
                return;

            // advertisement interval, window
            // scanning interval, window

            _ble.Adapter.ScanMode = ScanMode.LowLatency; // high duty cycle
            _ble.Adapter.ScanTimeout = 30 * 1000;
            _ble.Adapter.ScanMatchMode = ScanMatchMode.STICKY;

            _cancelTokenSource = new CancellationTokenSource();

            await _ble.Adapter.StartScanningForDevicesAsync(
                deviceFilter: (PluginBleIDevice d) =>
                {
                    if (d.Name != null && (d.Name.ToLower().Contains("fenom") || d.Name.ToLower().StartsWith("fp")))
                        return true; 

                    return false;   
                },
                cancellationToken: _cancelTokenSource.Token);
        }

        public override async void StopDiscoveryAsync()
        {
            if (!_ble.Adapter.IsScanning)
                return;

            //_ble.Adapter.DeviceDiscovered -= Adapter_DeviceDiscovered;
            //_ble.Adapter.DeviceConnected -= Adapter_DeviceConnected;
            //_ble.Adapter.ScanTimeoutElapsed -= Adapter_ScanTimeoutElapsed;
            
            try
            {
                //_cancelTokenSource.Cancel();
                await _ble.Adapter.StopScanningForDevicesAsync();
            }
            catch (Exception e)
            {
                Helper.WriteDebug(e);
            }
        }

        private void Adapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            Helper.WriteDebug(" ... Adapter_DeviceDiscovered ... ");

            if ((e.Device.Name == null) || (e.Device.Name != null && (!e.Device.Name.ToLower().Contains("fenom") && !e.Device.Name.ToLower().StartsWith("fp"))))
                return;

            bool exists = _deviceService.Devices.Any(d => d.Id == e.Device.Id);
            if (!exists)
            {
                _deviceService.Devices.Add(new BleDevice(e.Device));
            }

            //_deviceService.HandleDeviceFound(new BleDevice(e.Device));
            _deviceService.HandleDeviceDiscovered(new BleDevice(e.Device));
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Helper.WriteDebug($"... Adapter_ScanTimeoutElapsed ... {DateTime.Now}");
        }

        private void Adapter_DeviceConnected(object sender, DeviceEventArgs e)
        {
            _deviceService.Current = _deviceService.Devices.First(d => d.Id == e.Device.Id);
            if (_deviceService.Current != null)
            {
                //Thread.SpinWait(1000);
                _deviceService.StopDiscovery();
                //_ble.Adapter.StopScanningForDevicesAsync();
            }

            _deviceService.HandleDeviceConnected(_deviceService.Devices.First(d => d.Id == e.Device.Id));
        }
        private void Adapter_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            if (e.Device.Id == _deviceService.Current.Id)
                _deviceService.Current = null;
            //_deviceService.HandleDeviceDisconnected(_deviceService.Devices.First(d => d.Id == e.Device.Id));
        }
        private async void Adapter_DeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            if (_deviceService.Current != null && e.Device.Id == _deviceService.Current.Id)
                _deviceService.Current = null;

            bool handleLostConnection = true;

            if (handleLostConnection)
            {
                _deviceService.HandleDeviceConnectionLost(_deviceService.Devices.First(d => d.Id == e.Device.Id));
            }
            else
            {
                await _deviceService.Current.ConnectAsync();
            }
        }
    }

    public class UsbEnumerator : DeviceDiscoverer
    {
        // Fields

        // will have to create a wrapper for platform agnostic usb
        // protected IUsbManager _usb = null;

        // Consturctors

        public UsbEnumerator(DeviceService deviceService) : base(deviceService)
        {
        }

        // Methods

        public override void StartDiscovery()
        {
            //throw new NotImplementedException();
        }

        public override void StopDiscoveryAsync()
        {
            //throw new NotImplementedException();
        }
    }
}

namespace FenomPlus.Services.NewArch.R2
{
    public interface IDevice
    {
        // Properties

        Guid Id { get; }

        string Name { get; }

        bool Connected { get; }

        object NativeDevice { get; }

        // Methods

        Task ConnectAsync();

        Task DisconnectAsync();

        /*
         * 
         * LEGACY TO BE REMOVED
         * 
         */

        int DeviceReadyCountDown { get; set; }
        bool ReadyForTest { get; set; }
        bool BreathTestInProgress { get; set; }

        //bool IsConnected(bool devicePowerOn = false);
        bool IsNotConnectedRedirect(bool devicePowerOn = false);

        Task<bool> StartTest(BreathTestEnum breathTestEnum);
        Task<bool> StopTest();
        Task<bool> RequestDeviceInfo();
        Task<bool> RequestEnvironmentalInfo();
        Task<bool> SendMessage(MESSAGE message);
        Task<bool> SendSerialNumber(string SerailNumber);
        Task<bool> SendDateTime(string date, string time);
        Task<bool> SendCalibration(double cal1, double cal2, double cal3);
        Task<bool> SendCalibration(ID_SUB iD_SUB, double cal1, double cal2, double cal3);
    }

    public abstract partial class Device : IDevice
    {
        // Fields

        protected object _nativeDevice = null;

        // Constructor

        public Device(object nativeDevice)
        {
            _nativeDevice = (PluginBleIDevice) nativeDevice;

            /*
             * LEGACY CODE
             */
            DeviceReadyTimer = new Timer(1000);
            DeviceReadyTimer.Elapsed += DeviceReadyTimerOnElapsed;
            ReadyForTest = true;
        }

        // Properties

        public abstract string Name { get; }
        
        public abstract Guid Id { get; }

        public abstract bool Connected { get; }

        public object NativeDevice { get => _nativeDevice; }

        // Methods

        public abstract Task ConnectAsync();

        public abstract Task DisconnectAsync();

        /*
         * 
         * LEGACY CODE
         * 
         */
        public IEnumerable<IGattCharacteristic> GattCharacteristics { get; } = new SynchronizedList<IGattCharacteristic>();

        public IEnumerable<IService> GattServices { get; } = new SynchronizedList<IService>();

        public int DeviceReadyCountDown { get; set; }

        private bool _readyForTest;
        public bool ReadyForTest
        {
            get => _readyForTest;
            set
            {
                _readyForTest = value;

                if (_readyForTest == false)
                {
                    DeviceReadyCountDown = 32;
                    DeviceReadyTimer.Start();
                }
            }
        }

        private readonly Timer DeviceReadyTimer;
        private void DeviceReadyTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            DeviceReadyCountDown -= 1;

            if (DeviceReadyCountDown <= 0)
            {
                ReadyForTest = true;
                DeviceReadyTimer.Stop();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bleDevice"></param>
        /// <param name="completed"></param>
        /// <returns></returns>
        public bool IsConnected(bool devicePowerOn = false)
        {
            return Connected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicePowerOn"></param>
        /// <returns></returns>
        public bool IsNotConnectedRedirect(bool devicePowerOn = false)
        {
            if (IsConnected(devicePowerOn))
            {
                return true;
            }
            
            //AppServices.Container.Resolve<INavigationService>().DevicePowerOnView();
            
            return false;
        }

        public bool BreathTestInProgress
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="breathTestEnum"></param>
        /// <returns></returns>
        public async Task<bool> StartTest(BreathTestEnum breathTestEnum)
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
        public async Task<bool> StopTest()
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
    }

    public partial class Device
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DEVICEINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEVICEINFO);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ENVIROMENTALINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_ENVIROMENTALINFO);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_BREATHMANUEVER, (Byte)breathTestEnum);
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BREATHMANUEVER()
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
        public async Task<bool> DEBUGMSG()
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
        public async Task<bool> MESSAGE(MESSAGE message)
        {
            return await WRITEREQUEST(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerailNumber"></param>
        /// <returns></returns>
        public async Task<bool> SERIALNUMBER(string SerailNumber)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_SERIALNUMBER, SerailNumber);
            return await WRITEREQUEST(message, 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<bool> DATETIME(string date, string time)
        {
            string strDateTime;

            strDateTime = (date + "T" + time);

            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_DATETIME, strDateTime);
            return await WRITEREQUEST(message, (short)strDateTime.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iD_SUB"></param>
        /// <param name="cal"></param>
        /// <returns></returns>
        public async Task<bool> CALIBRATION(ID_SUB iD_SUB, double cal1, double cal2, double cal3)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_CALIBRATION_DATA, iD_SUB, cal1, cal2, cal3);
            return await WRITEREQUEST(message, 24);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> WRITEREQUEST(MESSAGE message, Int16 idvar_size)
        {
            using (var tracer = new Helper.FunctionTrace())
            {
                byte[] data = new byte[2 + 2 + idvar_size];

                data[0] = (byte)(message.IDMSG >> 8);
                data[1] = (byte)(message.IDMSG);
                data[2] = (byte)(message.IDSUB >> 8);
                data[3] = (byte)(message.IDSUB);

                Buffer.BlockCopy(message.IDVAR, 0, data, 4, idvar_size);

                IGattCharacteristic Characteristic = await FindCharacteristic(FenomPlus.SDK.Core.Constants.FeatureWriteCharacteristic);
                if (Characteristic != null)
                {
                    await Characteristic.WriteWithoutResponseAsync(data);
                    tracer.Trace("write without response okay");
                    return true;
                }
                tracer.Trace("something went wrong");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<IGattCharacteristic> FindCharacteristic(string uuid)
        {

            Guid guid = new Guid(uuid);
            IGattCharacteristic gatt = null;

            var gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;
            if (gattCharacteristics.Count <= 0)
            {
                _ = await GetCharacterasticsAync();
                gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;
            }
            foreach (IGattCharacteristic item in gattCharacteristics)
            {
                if (!item.Uuid.Equals(guid)) continue;
                gatt = item;
                break;
            }

            return gatt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGattCharacteristic>> GetCharacterasticsAync()
        {
            try
            {
                //PerformanceLogger.StartLog(typeof(BleDevice), "GetCharacterasticsAync");

                var gattCharacteristics = GattCharacteristics as SynchronizedList<IGattCharacteristic>;

                if (gattCharacteristics == null)
                {
                    //_logger.LogWarning("BleDevice.GetCharacteristicsAsync() - list is null");
                    return null;
                }

                gattCharacteristics.Clear();


                var gattService = GattServices as SynchronizedList<IService>;

                var services = await ((PluginBleIDevice)_nativeDevice).GetServicesAsync();

                foreach (var service in services)
                {
                    // add service here
                    gattService.Add(service);

                    var characteristics = await service.GetCharacteristicsAsync();
                    foreach (var characteristic in characteristics)
                    {
                        IGattCharacteristic gattCharacteristic = new GattCharacteristic(characteristic);
                        gattCharacteristics.Add(gattCharacteristic);
                    }
                }

                return gattCharacteristics;
            }
            catch (Exception ex)
            {
                Helper.WriteDebug(ex);
                return null;
            }
            finally
            {
                //PerformanceLogger.EndLog(typeof(BleDevice), "GetCharacterasticsAync");
            }
        }
    }

    internal class BleDevice : Device
    {
        readonly Plugin.BLE.Abstractions.Contracts.IAdapter _bleAdapter = CrossBluetoothLE.Current.Adapter;
        PluginBleIDevice _bleDevice;


        public BleDevice(object bleDevice) : base(bleDevice)
        {
            _bleDevice = (PluginBleIDevice) bleDevice;
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
                if (_bleDevice.State == Plugin.BLE.Abstractions.DeviceState.Limited)
                {
                    Helper.WriteDebug("Connection state: LIMITED, reconnecting.");
                    _bleAdapter.ConnectToDeviceAsync(_bleDevice);
                }

                return ((PluginBleIDevice)_nativeDevice).State == Plugin.BLE.Abstractions.DeviceState.Connected; 
            }
        }

        public override async Task ConnectAsync()
        {
            if (_bleAdapter != null && ((PluginBleIDevice)_nativeDevice).State != Plugin.BLE.Abstractions.DeviceState.Connected)
            {
                try
                {
                     await _bleAdapter.ConnectToDeviceAsync((PluginBleIDevice)_nativeDevice, default, default);
                }

                catch (DeviceConnectionException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public override async Task DisconnectAsync()
        {
            if (((PluginBleIDevice)_nativeDevice).State == Plugin.BLE.Abstractions.DeviceState.Connected)
            {
                await _bleAdapter.DisconnectDeviceAsync((PluginBleIDevice)_nativeDevice);
            }
        }
    }
}
