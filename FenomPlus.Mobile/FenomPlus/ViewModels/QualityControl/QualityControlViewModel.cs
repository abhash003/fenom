using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Controls;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services.DeviceService.Concrete;
using FenomPlus.Services.DeviceService.Enums;
using FenomPlus.ViewModels.QualityControl;
using FenomPlus.ViewModels.QualityControl.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class QualityControlViewModel : BaseViewModel
    {
        //QcButtonViewModels[0] is the Negative Control, the other elements are users
        public List<QcButtonViewModel> QcButtonViewModels = new List<QcButtonViewModel>();

        private string CurrentDeviceSerialNumber = string.Empty;

        public string SerialNumberString => $"Device Serial Number ({CurrentDeviceSerialNumber})";

        [ObservableProperty]
        private string _newUserName = string.Empty;

        [ObservableProperty]
        private int _testTime;

        [ObservableProperty]
        private int _gaugeSeconds;

        [ObservableProperty]
        private float _gaugeData;

        partial void OnGaugeDataChanged(float value)
        {
            PlaySounds.PlaySound(GaugeData);
        }

        [ObservableProperty]
        private string _gaugeStatus;

        public List<QCUser> AllDevices => GetAllQcDevices();
        public List<QCUser> AllUsers => ReadAllQcUsers(CurrentDeviceSerialNumber);
        public List<QCTest> AllTests => ReadAllQcTests(CurrentDeviceSerialNumber, CurrentQcUser.UserName);

        private readonly string QCUserRecordsPath;
        private readonly string QCTestRecordsPath;

        public QualityControlViewModel()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            QCUserRecordsPath = Path.Combine(localFolder, @"QCUserRecords.db");
            QCTestRecordsPath = Path.Combine(localFolder, @"QCTestRecords.db");

            // ToDo: Read from database
            //MockData();

            for (int i = 0; i < 7; i++) // Negative control + 6 users
            {
                QcButtonViewModels.Add(new QcButtonViewModel());
            }
        }

        // Required for QualityControlView (Hub)
        private QCUser QCDevice; // For a specific device with unique serial number - only one per device
        private QCUser QCNegativeControl; // For a specific device with unique serial number - only one per device
        private List<QCUser> QCUsers; // Users assigned to a device with the device's serial number

        private bool CheckDeviceConnection()
        {
            if (Services?.DeviceService?.Current == null)
                return false;

            return Services.DeviceService.Current.Connected; ;
        }

        public void LoadData()
        {
            if (!CheckDeviceConnection())
            {
                // ToDo: Put up alert
                Services.Dialogs.ShowAlert("You must have a connection to a device to continue.", "Device Not Connected", "OK");
                Services.Navigation.DashboardView();
                return;
            }

            CurrentDeviceSerialNumber = Services?.DeviceService?.Current?.DeviceSerialNumber;

            if (Services == null || Services.Cache == null || string.IsNullOrEmpty(CurrentDeviceSerialNumber))
            {
                Debug.Assert(true);
            }

            WipeDataBase(); // ToDo: For debugging only

            // Get currently connected device's status or create a new one
            QCDevice = ReadQcDevice(CurrentDeviceSerialNumber);

            // Get currently connected device's negative control or create one
            QCNegativeControl = ReadQcNegativeControl(CurrentDeviceSerialNumber) ?? CreateQcNegativeControl(CurrentDeviceSerialNumber);

            // Get all users for this currently connected device
            QCUsers = ReadAllQcUsers(CurrentDeviceSerialNumber);



            // Need to clear the button viewModels without creating new since they may already be bound to buttons
            for (int i = 0; i < 7; i++)
            {
                QcButtonViewModels[i].QCUserModel = null;
            }

            // Now assign Negative Control and users to the buttons
            QcButtonViewModels[0].QCUserModel = QCNegativeControl;

            // Now assign users from the database
            for (int i = 0; i <= QCUsers.Count - 1; i++)
            {
                QcButtonViewModels[i + 1].QCUserModel = QCUsers[i];
            }
        }

        public int SelectedUserIndex = -1;

        public QCUser CurrentQcUser
        {
            get
            {
                if (SelectedUserIndex >= 0)
                {
                    return QcButtonViewModels[SelectedUserIndex].QCUserModel;
                }

                return null;
            }
        }

        public void ResetSelectedUserIndex()
        {
            SelectedUserIndex = -1;
        }


        #region "QC Device CRUD"

        private QCUser CreateQcDevice(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return null;
            }

            try
            {
                var newDevice = new QCUser(serialNumber, QCUser.DeviceName, QCUser.DeviceInsufficientData, DateTime.Now, DateTime.MinValue, DateTime.MinValue);

                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Insert(newDevice);
                    userCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }

                return newDevice;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private QCUser ReadQcDevice(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return null;
            }

            // Only ONE in the collection
            using (var db = new LiteDatabase(QCUserRecordsPath))
            {
                try
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var devices = userCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == QCUser.DeviceName)
                        .ToList();

                    // Should only be one for each device serial number
                    return devices.Count == 1 ? devices[0] : null;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return null;
                }
            }
        }

        private bool UpdateQcDevice(QCUser device)
        {
            if (device == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Update(device);

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private bool DeleteQcDevice(QCUser device)
        {
            if (device == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    // Get all user records for this device
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Delete(device.Id);

                    // Get all test records for this device
                    var testsCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testsCollection.Query()
                        .Where(x => x.DeviceSerialNumber == device.DeviceSerialNumber)
                        .ToList();

                    foreach (var t in tests)
                    {
                        testsCollection.Delete(t.Id);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private List<QCUser> GetAllQcDevices()
        {
            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var devices = userCollection.Query()
                        .Where(x => x.UserName == QCUser.DeviceName)
                        .ToList();

                    return devices;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<QCUser>(); // Empty
            }
        }

        #endregion


        #region "QC Negative Control CRUD"

        private QCUser CreateQcNegativeControl(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return null;
            }

            try
            {
                var newNegativeControl = new QCUser(serialNumber, QCUser.NegativeControlName, QCUser.NegativeControlNone, DateTime.Now, DateTime.MinValue, DateTime.MinValue);

                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Insert(newNegativeControl);
                    userCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }

                return newNegativeControl;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private QCUser ReadQcNegativeControl(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return null;
            }

            // Only ONE in the collection
            using (var db = new LiteDatabase(QCUserRecordsPath))
            {
                try
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var negativeControls = userCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == QCUser.NegativeControlName)
                        .ToList();

                    // Should only be one for each device serial number
                    return negativeControls.Count == 1 ? negativeControls[0] : null;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return null;
                }
            }
        }

        private bool UpdateQcNegativeControl(QCUser negativeControl)
        {
            if (negativeControl == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Update(negativeControl);

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private bool DeleteQcNegativeControl(QCUser negativeControl)
        {
            if (negativeControl == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    // Get all user records for this device
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Delete(negativeControl.Id);

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private List<QCUser> GetAllNegativeControls()
        {
            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var negativeControls = userCollection.Query()
                        .Where(x => x.UserName == QCUser.NegativeControlName)
                        .ToList();

                    return negativeControls;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<QCUser>(); // Empty
            }
        }

        #endregion


        #region "QC Users CRUD"

        private QCUser CreateQcUser(string serialNumber, string userName)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName))
            {
                return null;
            }

            try
            {
                var newQcUser = new QCUser(serialNumber, userName, QCUser.UserNone, DateTime.Now, DateTime.MinValue, DateTime.MinValue);

                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Insert(newQcUser);
                    userCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }

                return newQcUser;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private QCUser ReadQcUser(string serialNumber, string userName)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName))
            {
                return null;
            }

            using (var db = new LiteDatabase(QCUserRecordsPath))
            {
                try
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var user = userCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == userName)
                        .ToList();

                    // Should only be one for each device serial number
                    return user.Count == 1 ? user[0] : null;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return null;
                }
            }
        }

        private bool UpdateQcUser(QCUser user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Update(user);

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private bool DeleteQcUser(QCUser user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    // Get all user records for this device
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Delete(user.Id);

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private List<QCUser> ReadAllQcUsers(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return new List<QCUser>(); // Empty
            }

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var users = userCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber)
                        .ToList();

                    return users;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<QCUser>(); // Empty
            }
        }

        #endregion


        #region "QCTests CRUD"

        private QCTest CreateQcTest(string serialNumber, string userName, int result)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName) || result <= 0)
            {
                return null;
            }

            try
            {
                var newTest = new QCTest(CurrentDeviceSerialNumber, userName, DateTime.Now, result);

                using (var db = new LiteDatabase(QCTestRecordsPath))
                {
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    testCollection.Insert(newTest);
                    testCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }

                return newTest;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private List<QCTest> ReadQcTests(string serialNumber, string userName)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName))
            {
                return new List<QCTest>(); // Empty
            }

            try
            {
                using (var db = new LiteDatabase(QCTestRecordsPath))
                {
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == userName)
                        .ToList();

                    return tests;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new List<QCTest>(); // Empty
            }
        }

        // No updates to QC Tests

        private bool DeleteAllQcTests(string serialNumber, string userName)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName))
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCTestRecordsPath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == userName)
                        .ToList();

                    foreach (var t in tests)
                    {
                        testCollection.Delete(t.Id);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private List<QCTest> ReadAllQcTests(string serialNumber, string userName)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName))
            {
                return new List<QCTest>(); // Empty
            }

            try
            {
                using (var db = new LiteDatabase(QCTestRecordsPath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == userName)
                        .ToList();
                    return tests;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new List<QCTest>(); // Empty
            }
        }

        #endregion


        #region "Main Screen Commands"

        [RelayCommand]
        private void UpdateNegativeControl()
        {
            SelectedUserIndex = 0;

            //QCNegativeControl
            //QcNegativeControlViewModel

            // Open QC Negative control view and do automatic breath test


            if (QcButtonViewModels[0].Assigned)
            {
                // Open and run automatic test for negative control
            }
            else
            {
                Debugger.Break(); // Should never happen
            }
        }

        [RelayCommand]
        private async Task UpdateUser1Async()
        {
            SelectedUserIndex = 1;

            if (QcButtonViewModels[1].Assigned)
            {
                // Open and run new breath test for this user
                await StartUserBreathTest();
            }
            else
            {
                // Open user name dialog and create user, then open breath test view
                string userName = await Services.Dialogs.UserNamePromptAsync();

                if (!string.IsNullOrEmpty(userName))
                {
                    //QcButtonViewModels[1].UserName = userName;
                    //QcButtonViewModels[1].Assigned = true;

                    if (QCUsers.Any(user => userName == user.UserName))
                    {
                        Services.Dialogs.ShowAlert($"User name [{userName}] already exists.", "Conflicting User Name", "OK");
                        return;
                    }

                    QCUser newUserModel = CreateQcUser(CurrentDeviceSerialNumber, userName);
                    QcButtonViewModels[1].QCUserModel = newUserModel;

                    // Open and run new breath test for this user
                    await StartUserBreathTest();
                }
            }
        }

        [RelayCommand]
        private void UpdateUser2()
        {
            SelectedUserIndex = 2;

            //QCUsers[1]
            //QcUser2ViewModel
        }

        [RelayCommand]
        private void UpdateUser3()
        {
            SelectedUserIndex = 3;
            //QCUsers[2]
            //QcUser3ViewModel
        }

        [RelayCommand]
        private void UpdateUser4()
        {
            SelectedUserIndex = 4;

            //QCUsers[3]
            //QcUser4ViewModel
        }

        [RelayCommand]
        private void UpdateUser5()
        {
            SelectedUserIndex = 5;

            //QCUsers[4]
            //QcUser5ViewModel
        }

        [RelayCommand]
        private void UpdateUser6()
        {
            SelectedUserIndex = 6;

            //QCUsers[5]
            //QcUser6ViewModel
        }

        [RelayCommand]
        private async Task ShowQCSettings()
        {
            await Services.Navigation.QCSettingsView();
        }

        #endregion


        #region "QC User Breath Test"

        public async Task StartUserBreathTest()
        {
            if (Services.DeviceService.Current != null)
            {
                if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
                {
                    DeviceCheckEnum deviceStatus = Services.DeviceService.Current.CheckDeviceBeforeTest();

                    switch (deviceStatus)
                    {
                        case DeviceCheckEnum.Ready:
                            await InitializeBreathGauge();
                            await Services.Navigation.QCUserTestView();
                            break;
                        case DeviceCheckEnum.DevicePurging:
                            await Services.Dialogs.NotifyDevicePurgingAsync(Services.DeviceService.Current.DeviceReadyCountDown);
                            if (Services.Dialogs.PurgeCancelRequest)
                                return;
                            await InitializeBreathGauge();
                            await Services.Navigation.QCUserTestView();
                            break;
                        case DeviceCheckEnum.HumidityOutOfRange:
                            Services.Dialogs.ShowAlert(
                                $"Unable to run test. Humidity level ({Services.DeviceService.Current.EnvironmentalInfo.Humidity}%) is out of range.",
                                "Humidity Warning", "Close");
                            break;
                        case DeviceCheckEnum.PressureOutOfRange:
                            Services.Dialogs.ShowAlert(
                                $"Unable to run test. Pressure level ({Services.DeviceService.Current.EnvironmentalInfo.Pressure} kPa) is out of range.",
                                "Pressure Warning", "Close");
                            break;
                        case DeviceCheckEnum.TemperatureOutOfRange:
                            Services.Dialogs.ShowAlert(
                                $"Unable to run test. Temperature level ({Services.DeviceService.Current.EnvironmentalInfo.Temperature} °C) is out of range.",
                                "Temperature Warning", "Close");
                            break;
                        case DeviceCheckEnum.BatteryCriticallyLow:
                            Services.Dialogs.ShowAlert(
                                $"Unable to run test. Battery Level ({Services.DeviceService.Current.EnvironmentalInfo.BatteryLevel}%) is critically low: ",
                                "Battery Warning", "Close");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private async Task InitializeBreathGauge()
        {
            Services.Cache.TestType = TestTypeEnum.Standard;

            if (Services.DeviceService.Current != null)
            {
                // Allows Updating the Breath Gauge in UI

                Services.DeviceService.Current.BreathFlowChanged += Cache_BreathFlowChanged;

                Services.DeviceService.Current.IsNotConnectedRedirect();

                GaugeData = Services.DeviceService.Current!.BreathFlow = 0;
                GaugeSeconds = 6; // ToDo: change to 10 after debugging
                GaugeStatus = "Start Blowing";

                await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second); // ToDo: Change tp 10 after debugging
            }
        }

        public async void Cache_BreathFlowChanged(object sender, EventArgs e)
        {
            if (Services.DeviceService.Current != null)
            {
                GaugeData = Services.DeviceService.Current.BreathFlow;
                GaugeSeconds = Services.DeviceService.Current.BreathManeuver.TimeRemaining;

                if (GaugeSeconds <= 0)
                {
                    await Services.DeviceService.Current.StopTest();
                    await Services.Navigation.QCUserStopTestView();
                    return;
                }
            }

            if (GaugeData < Config.GaugeDataLow)
            {
                GaugeStatus = "Exhale Harder";
            }
            else if (GaugeData > Config.GaugeDataHigh)
            {
                GaugeStatus = "Exhale Softer";
            }
            else
            {
                GaugeStatus = "Good Job!";
            }
        }

        #endregion


        #region "QC User Stop Test:

        public void InitUserStopBreathTest()
        {
            //Stop = false;
            //Seconds = Config.StopExhalingReadyWait;
            PlaySounds.PlaySuccessSound();

            //UiTimer = new Timer(1000);
            //UiTimer.Elapsed += (sender, e) => StopBreathCompleted();
            //UiTimer.Start();

            //UiTimer.Stop();
            Task.Delay(Config.StopExhalingReadyWait);

            if (Services.DeviceService.Current.BreathManeuver.StatusCode != 0x00)
            {
                var model = BreathManeuverErrorDBModel.Create(Services.DeviceService.Current.BreathManeuver);
                ErrorsRepo.Insert(model);

                PlaySounds.PlayFailedSound();
                Services.Navigation.TestErrorView();
            }
            else
            {
                Services.Navigation.QCUserTestCalculationView();
            }
        }

        #endregion


        #region "QC User Test Calculation"

        private Timer UiTimer;

        public void StartUserTestCalculations()
        {
            UiTimer = new Timer(Config.TestResultReadyWait * 1000);
            UiTimer.Elapsed += (sender, e) => CalculationsCompleted();
            UiTimer.Start();
        }

        [ObservableProperty]
        private string _userTestErrorCode = string.Empty;

        private bool CalculationsCompleted()
        {
            UiTimer.Stop();
            UiTimer.Dispose();

            if (Services.DeviceService.Current is { FenomReady: false })
                return false;

            // ToDo: Add to normal test results database?
            var model = BreathManeuverResultDBModel.Create(Services.DeviceService.Current!.BreathManeuver);
            ResultsRepo.Insert(model);

            var str = ResultsRepo.ToString();

            Debug.WriteLine($"Cache.BreathManeuver.StatusCode = {Services.DeviceService.Current.BreathManeuver.StatusCode}");

            if (Services.DeviceService.Current.BreathManeuver.StatusCode != 0x00)
            {
                var errorModel = BreathManeuverErrorDBModel.Create(Services.DeviceService.Current.BreathManeuver);
                ErrorsRepo.Insert(errorModel);

                UserTestErrorCode = errorModel.ErrorCode;

                PlaySounds.PlayFailedSound();
                Services.Navigation.QCUserTestErrorView();
            }
            else
            {
                Services.Navigation.QCUserTestResultView();
            }

            return (Services.DeviceService.Current.FenomReady == false);
        }

        #endregion


        #region "QC User Test Result"


        [ObservableProperty]
        private string _qCUserTestResult = string.Empty;

        public void InitUserTestResults()
        {
            QCUserTestResult = (Services.DeviceService.Current.FenomValue) < 5 ? "< 5" :
                (Services.DeviceService.Current.FenomValue) > 300 ? "> 300" :
                Services.DeviceService.Current.FenomValue.ToString(CultureInfo.InvariantCulture);

            Services.DeviceService.Current.ReadyForTest = false;
        }

        #endregion


        #region "Debug Commands"

        [RelayCommand]
        private void CreateMockDataBase()
        {
            string serialNumber = string.Empty;

            var newDevice = CreateQcDevice(serialNumber);

            // New User Qualified
            var newUser = CreateQcUser(serialNumber, "Jim");
            var newTest = CreateQcTest(serialNumber, newUser.UserName, 20);
            newTest = CreateQcTest(serialNumber, newUser.UserName, 30);
            newTest = CreateQcTest(serialNumber, newUser.UserName, 25);

            // New User Disqualified
            newUser = CreateQcUser(serialNumber, "Vinh");
            newTest = CreateQcTest(serialNumber, newUser.UserName, 20);
            newTest = CreateQcTest(serialNumber, newUser.UserName, 30);
            newTest = CreateQcTest(serialNumber, newUser.UserName, 19);

            // New User Disqualified
            newUser = CreateQcUser(serialNumber, "Bob");
            newTest = CreateQcTest(serialNumber, newUser.UserName, 20);
            newTest = CreateQcTest(serialNumber, newUser.UserName, 30);
            newTest = CreateQcTest(serialNumber, newUser.UserName, 32);
        }

        [RelayCommand]
        private void WipeDataBase()
        {
            using (var db = new LiteDatabase(QCUserRecordsPath))
            {
                var usersCollection = db.GetCollection<QCUser>("qcusers");
                usersCollection.DeleteAll();
            }

            using (var db = new LiteDatabase(QCTestRecordsPath))
            {
                var testsCollection = db.GetCollection<QCTest>("qctests");
                testsCollection.DeleteAll();
            }
        }

        #endregion
    }
}
