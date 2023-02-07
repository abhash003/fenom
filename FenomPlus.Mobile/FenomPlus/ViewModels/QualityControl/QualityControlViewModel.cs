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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace FenomPlus.ViewModels
{
    public partial class QualityControlViewModel : BaseViewModel
    {
        //QcButtonViewModels[0] is the Negative Control, the other elements are users
        public List<QcButtonViewModel> QcButtonViewModels = new List<QcButtonViewModel>();

        private readonly string QCUserRecordsPath;
        private readonly string QCTestRecordsPath;

        private QCUser QCDevice; // For a specific device with unique serial number - only one per device
        private List<QCUser> QCUsers; // Users assigned to a device with the device's serial number

        public int SelectedUserIndex = -1;
        private string _currentDeviceSerialNumber = string.Empty;

        public string CurrentDeviceSerialNumber
        {
            get => _currentDeviceSerialNumber;
            set
            {
                _currentDeviceSerialNumber = value;
                OnPropertyChanged(nameof(CurrentDeviceSerialNumber));
                OnPropertyChanged(nameof(SerialNumberString));
            }
        }

        public string SerialNumberString => $"Device Serial Number ({CurrentDeviceSerialNumber})";

        public bool RequireQC
        {
            get => Services.Config.RunRequiresQC;
            set
            {
                Services.Config.RunRequiresQC = value;
                OnPropertyChanged(nameof(RequireQC));
            }
        }

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

        public QualityControlViewModel()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            QCUserRecordsPath = Path.Combine(localFolder, @"QCUserRecords.db");
            QCTestRecordsPath = Path.Combine(localFolder, @"QCTestRecords.db");

            // ToDo: Read from database
            //MockData();

            QcButtonViewModels.Clear();

            for (int i = 0; i <= 6; i++) // Negative Control + 6 users
            {
                QcButtonViewModels.Add(new QcButtonViewModel());
            }
        }

        private QCUser QCNegativeControl => QcButtonViewModels[0].QCUserModel; // For a specific device with unique serial number - only one per device

        private bool CheckDeviceConnection()
        {
            if (Services?.DeviceService?.Current == null)
                return false;

            return Services.DeviceService.Current.Connected; ;
        }

        public void ResetSelectedUserIndex()
        {
            SelectedUserIndex = -1;
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
                Debug.Assert(true);

            //WipeDataBase(); // ToDo: For debugging only

            // Get currently connected device's status or create a new one
            QCDevice = ReadQcDevice(CurrentDeviceSerialNumber);

            if (QCDevice == null)
            {
                CreateQcDevice(CurrentDeviceSerialNumber);
            }

            // Get currently connected device's negative control or create one
            QcButtonViewModels[0].QCUserModel = ReadQcNegativeControl(CurrentDeviceSerialNumber);

            if (QcButtonViewModels[0].QCUserModel == null)
            {
                QcButtonViewModels[0].QCUserModel = CreateQcNegativeControl(CurrentDeviceSerialNumber);
            }

            // Get all users for this currently connected device
            QCUsers = ReadAllQcUsers(CurrentDeviceSerialNumber);

            if (QCUsers.Count > 0)
                QcButtonViewModels[1].QCUserModel = QCUsers[0];

            if (QCUsers.Count > 1)
                QcButtonViewModels[2].QCUserModel = QCUsers[1];

            if (QCUsers.Count > 2)
                QcButtonViewModels[3].QCUserModel = QCUsers[2];

            if (QCUsers.Count > 3)
                QcButtonViewModels[4].QCUserModel = QCUsers[3];

            if (QCUsers.Count > 4)
                QcButtonViewModels[5].QCUserModel = QCUsers[4];

            if (QCUsers.Count > 5)
                QcButtonViewModels[6].QCUserModel = QCUsers[5];
        }


        #region "QC Device CRUD"

        private QCUser CreateQcDevice(string serialNumber)
        {
            Debug.Assert(!string.IsNullOrEmpty(serialNumber));

            try
            {
                var chartData = new List<double>();
                var newDevice = new QCUser(serialNumber, QCUser.DeviceName, QCUser.DeviceInsufficientData, DateTime.Now, DateTime.MinValue, DateTime.MinValue, chartData);

                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Insert(newDevice);
                    userCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }

                return newDevice;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private QCUser ReadQcDevice(string serialNumber)
        {
            Debug.Assert(!string.IsNullOrEmpty(serialNumber));

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    // Only ONE in the collection
                    var device = userCollection.FindOne(x => x.DeviceSerialNumber == serialNumber && x.UserName == QCUser.DeviceName);

                    return device;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private bool UpdateQcDevice(QCUser device)
        {
            Debug.Assert(device != null);

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Update(device);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private bool DeleteQcDevice(QCUser device)
        {
            Debug.Assert(device != null);

            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    // Delete all users for this device including device and negative control
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Delete(device.Id);

                    // Delete all tests for this device
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion


        #region "QC Negative Control CRUD"

        private QCUser CreateQcNegativeControl(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
                return null;

            try
            {
                var chartData = new List<double>();
                var newNegativeControl = new QCUser(serialNumber, QCUser.NegativeControlName, QCUser.NegativeControlNone, DateTime.Now, DateTime.MinValue, DateTime.MinValue, chartData);

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
                return null;

            // Only ONE in the collection
            using (var db = new LiteDatabase(QCUserRecordsPath))
            {
                try
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    var qcNegativeControl = userCollection.FindOne(x => x.DeviceSerialNumber == serialNumber && x.UserName == QCUser.NegativeControlName);
                    return qcNegativeControl;
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
                return false;

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
                return false;

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

        #endregion


        #region "QC Users CRUD"

        private QCUser CreateQcUser(string serialNumber, string userName)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName))
                return null;

            try
            {
                var chartData = new List<double>();
                var newQcUser = new QCUser(serialNumber, userName, QCUser.UserNone, DateTime.Now, DateTime.MinValue, DateTime.MinValue, chartData);

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
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName != QCUser.DeviceName && x.UserName != QCUser.NegativeControlName)
                        .OrderBy(x => x.UserName)
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
                return null;

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
        private bool UpdateQcTest(QCTest test)
        {
            if (test == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCTestRecordsPath))
                {
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    testCollection.Update(test);

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

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
                        .OrderBy(x => x.UserName)
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

        [ObservableProperty]
        private string _newUserName = string.Empty;

        [RelayCommand]
        private async Task UpdateNegativeControl()
        {
            SelectedUserIndex = 0;

            //QCNegativeControl
            //QcNegativeControlViewModel

            // Open QC Negative control view and do automatic breath test


            if (QcButtonViewModels[0].Assigned)
            {
                // Open and run automatic test for negative control
                await StartNegativeControlTest();
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
                    // Not yet, user may cancel after seeing breath gauge
                    //    QcButtonViewModels[1].UserName = userName;
                    //    QcButtonViewModels[1].Assigned = true;

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

        #region "Negative Control Test"

        private Timer TestTimer;

        [ObservableProperty] 
        private int _negativeControlTestResult = 0;

        private async Task StartNegativeControlTest()
        {
            TestTimer = new Timer(Config.TestResultReadyWait * 1000);
            TestTimer.Elapsed += (sender, e) => NegativeControlTestCompleted();
            TestTimer.Start();

            await Services.Navigation.QCNegativeControlTestView();
        }

        private void NegativeControlTestCompleted()
        {
            NegativeControlTestResult = 5; // ToDo: Temporary until we have real value from hardware


            // Update NegativeControl
            QCTest test = CreateQcTest(CurrentDeviceSerialNumber, QCUser.NegativeControlName, NegativeControlTestResult);

            string result = AnalyzeNegativeControlResults(CurrentDeviceSerialNumber);

            Services.Navigation.QCNegativeControlResultView();
        }

        #endregion


        #region "QC User Breath Test View"

        [ObservableProperty]
        private int _testTime = 0;

        [ObservableProperty]
        private int _gaugeSeconds = 0;

        [ObservableProperty]
        private float _gaugeData = 0f;

        partial void OnGaugeDataChanged(float value)
        {
            PlaySounds.PlaySound(GaugeData);
        }

        [ObservableProperty]
        private string _gaugeStatus = string.Empty;

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


        #region "QC User Stop Test View"

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


        #region "QC User Test Calculation View"

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


        #region "QC User Test Result View"


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
        private void CreateMockDataBase1()
        {
            WipeDataBase();

            var newDevice = CreateQcDevice(CurrentDeviceSerialNumber);

            var negativeControl = CreateQcNegativeControl(CurrentDeviceSerialNumber);

            // New User Qualified
            var newUser = CreateQcUser(CurrentDeviceSerialNumber, "Jim");

            var newTest1 = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest1.TestDate = DateTime.Now.AddHours(-16);
            UpdateQcTest(newTest1);

            var newTest2 = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest2.TestDate = DateTime.Now;
            UpdateQcTest(newTest2);

            var newTest3 = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 25);
            newTest3.TestDate = DateTime.Now.AddHours(16);
            UpdateQcTest(newTest3);
        }

        [RelayCommand]
        private void CreateMockDataBase2()
        {
            WipeDataBase();

            var newDevice = CreateQcDevice(CurrentDeviceSerialNumber);

            var negativeControl = CreateQcNegativeControl(CurrentDeviceSerialNumber);

            // New User Qualified
            var newUser = CreateQcUser(CurrentDeviceSerialNumber, "Jim");
            var newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 25);
            newTest.TestDate = DateTime.Now.AddHours(16);

            // New User Disqualified
            newUser = CreateQcUser(CurrentDeviceSerialNumber, "Vinh");
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 19);
            newTest.TestDate = DateTime.Now.AddHours(16);
        }

        [RelayCommand]
        private void CreateMockDataBase3()
        {
            WipeDataBase();

            var newDevice = CreateQcDevice(CurrentDeviceSerialNumber);

            var negativeControl = CreateQcNegativeControl(CurrentDeviceSerialNumber);

            // New User Qualified
            var newUser = CreateQcUser(CurrentDeviceSerialNumber, "Jim");
            var newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 25);
            newTest.TestDate = DateTime.Now.AddHours(16);

            // New User Disqualified
            newUser = CreateQcUser(CurrentDeviceSerialNumber, "Vinh");
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 19);
            newTest.TestDate = DateTime.Now.AddHours(16);

            // New User Disqualified
            newUser = CreateQcUser(CurrentDeviceSerialNumber, "Bob");
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 32);
            newTest.TestDate = DateTime.Now.AddHours(16);
        }

        [RelayCommand]
        private void CreateMockDataBaseDates()
        {
            WipeDataBase();

            var newDevice = CreateQcDevice(CurrentDeviceSerialNumber);

            var negativeControl = CreateQcNegativeControl(CurrentDeviceSerialNumber);

            // New User Qualified
            var newUser = CreateQcUser(CurrentDeviceSerialNumber, "Jim");
            var newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 25);
            newTest.TestDate = DateTime.Now.AddHours(16);

            // New User Disqualified
            newUser = CreateQcUser(CurrentDeviceSerialNumber, "Vinh");
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 19);
            newTest.TestDate = DateTime.Now.AddHours(16);

            // New User Disqualified
            newUser = CreateQcUser(CurrentDeviceSerialNumber, "Bob");
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = CreateQcTest(CurrentDeviceSerialNumber, newUser.UserName, 32);
            newTest.TestDate = DateTime.Now.AddHours(16);
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

        private ObservableCollection<QCUser> AllQcDevices;

        public void UpdateQcDevicesList()
        {
            try
            {
                using (var db = new LiteDatabase(QCUserRecordsPath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var devices = userCollection.Query()
                        .Where(x => x.UserName == QCUser.DeviceName)
                        .ToList();

                    AllQcDevices = new ObservableCollection<QCUser>(devices);

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        #endregion

        [RelayCommand]
        public async Task ExitToQC()
        {
            await Services.Navigation.QualityControlView();
        }


        #region "Determine Status Routines"

        private string AnalyzeDeviceStatus(string serialNumber)
        {
            // Temporary!!!!!
            return string.Empty;
        }

        private string AnalyzeNegativeControlResults(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
                return string.Empty;

            try
            {
                using (var db = new LiteDatabase(QCTestRecordsPath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == QCUser.NegativeControlName)
                        .OrderByDescending(x => x.TestDate).ToList();

                    var last3Tests = tests.TakeLast(3);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return string.Empty;
            }

            // Temporary!!!!!!
            return string.Empty;
        }

        private string AnalyzeUserTests(string serialNumber, string userName)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(userName))
                return string.Empty;

            try
            {
                using (var db = new LiteDatabase(QCTestRecordsPath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == serialNumber && x.UserName == userName)
                        .OrderByDescending(x => x.TestDate).ToList();

                    var last3Tests = tests.TakeLast(3);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return string.Empty;
            }

            // Temporary!!!!!!
            return string.Empty;
        }

        #endregion
    }


    // Extension method to get the last 3 items in an array
    public static class Extensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int n)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)} must be 0 or greater");

            LinkedList<T> temp = new LinkedList<T>();

            foreach (var value in collection)
            {
                temp.AddLast(value);
                if (temp.Count > n)
                    temp.RemoveFirst();
            }

            return temp;
        }
    }
}
