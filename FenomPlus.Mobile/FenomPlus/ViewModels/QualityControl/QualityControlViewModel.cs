using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Controls;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services.DeviceService.Enums;
using FenomPlus.ViewModels.QualityControl.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace FenomPlus.ViewModels
{
    public partial class QualityControlViewModel : BaseViewModel
    {
        private readonly string QCDatabasePath;

        //QcButtonViewModels[0] is the Negative Control, the other elements are users
        public List<QcButtonViewModel> QcButtonViewModels = new List<QcButtonViewModel>();

        private QCDevice QCDevice; // For a specific device with unique serial number - only one per device

        private QCUser QCNegativeControl => QcButtonViewModels[0].QCUserModel; 

        private List<QCUser> QCUsers; // Users assigned to a device with the device's serial number

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

        private string _currentDeviceStatus = string.Empty;
        public string CurrentDeviceStatus
        {
            get => _currentDeviceStatus;
            set
            {
                _currentDeviceStatus = value;
                OnPropertyChanged(nameof(CurrentDeviceStatus));
                OnPropertyChanged(nameof(DeviceStatusString));
            }
        }

        public string DeviceStatusString => $"Device Status ({CurrentDeviceStatus})";

        public bool RequireQC
        {
            get => Services.Config.RunRequiresQC;
            set
            {
                Services.Config.RunRequiresQC = value;
                OnPropertyChanged(nameof(RequireQC));
            }
        }

        private int _selectedUserIndex = -1;
        public int SelectedUserIndex
        {
            get => _selectedUserIndex;
            set
            {
                _selectedUserIndex = value;
                OnPropertyChanged(nameof(SelectedQcUser));
                OnPropertyChanged(nameof(SelectedUserName));
                OnPropertyChanged(nameof(SelectedCurrentStatus));
                OnPropertyChanged(nameof(SelectedExplanation));
                OnPropertyChanged(nameof(SelectedTests));
                OnPropertyChanged(nameof(SelectedTests));
            }
        }

        public QCUser SelectedQcUser
        {
            get
            {
                if (SelectedUserIndex >= 0 && SelectedUserIndex <= QcButtonViewModels.Count - 1)
                {
                    return QcButtonViewModels[SelectedUserIndex].QCUserModel;
                }

                return null;
            }
        }
        public string SelectedUserName => SelectedQcUser.UserName;
        public string SelectedCurrentStatus => SelectedQcUser.CurrentStatus;
        public string SelectedExplanation => SelectedQcUser.Explanation;
        public List<QCTest> SelectedTests => DbReadQcTests(SelectedUserName);



        public QualityControlViewModel()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            QCDatabasePath = Path.Combine(localFolder, @"QCDatabase.db");


            // Todo: Debugging only
            //DeleteDataBase();

            //int range1 = GetRange(20, 30);
            //int range2 = GetRange(30, 20);
            //(int min, int max, int median) = GetRangeAndMedian(20, 30, 25);


        }

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
            Debug.Assert(!string.IsNullOrEmpty(CurrentDeviceSerialNumber));

            QcButtonViewModels.Clear();
            for (int i = 0; i <= 6; i++) // Negative Control + 6 users
            {
                // Will force to get new data so its always current
                QcButtonViewModels.Add(new QcButtonViewModel());
            }

            // Get currently connected device's status or create a new one
            QCDevice = DbReadQcDevice();

            if (QCDevice == null)
            {
                QCDevice = DbCreateQcDevice();
            }

            // Get currently connected device's negative control or create one
            QcButtonViewModels[0].QCUserModel = DbReadQcNegativeControl();

            if (QcButtonViewModels[0].QCUserModel == null)
            {
                QcButtonViewModels[0].QCUserModel = DbCreateQcNegativeControl();
            }

            // don't call until after you get the Negative Control
            CurrentDeviceStatus = UpdateDeviceStatus();

            // Get all users for this currently connected device
            QCUsers = ReadAllQcUsers();

            if (QCUsers.Count > 0)
            {
                QcButtonViewModels[1].QCUserModel = QCUsers[0];
            }
            //else
            //{
            //    QcButtonViewModels[1].QCUserModel = new QCUser(CurrentDeviceSerialNumber, string.Empty);
            //}

            if (QCUsers.Count > 1)
            {
                QcButtonViewModels[2].QCUserModel = QCUsers[1];
            }
            //else
            //{
            //    QcButtonViewModels[2].QCUserModel = new QCUser(CurrentDeviceSerialNumber, string.Empty);
            //}

            if (QCUsers.Count > 2)
            {
                QcButtonViewModels[3].QCUserModel = QCUsers[2];
            }
            //else
            //{
            //    QcButtonViewModels[3].QCUserModel = new QCUser(CurrentDeviceSerialNumber, string.Empty);
            //}

            if (QCUsers.Count > 3)
            {
                QcButtonViewModels[4].QCUserModel = QCUsers[3];
            }
            //else
            //{
            //    QcButtonViewModels[4].QCUserModel = new QCUser(CurrentDeviceSerialNumber, string.Empty);
            //}

            if (QCUsers.Count > 4)
            {
                QcButtonViewModels[5].QCUserModel = QCUsers[4];
            }
            //else
            //{
            //    QcButtonViewModels[5].QCUserModel = new QCUser(CurrentDeviceSerialNumber, string.Empty);
            //}

            if (QCUsers.Count > 5)
            {
                QcButtonViewModels[6].QCUserModel = QCUsers[5];
            }
            //else
            //{
            //    QcButtonViewModels[6].QCUserModel = new QCUser(CurrentDeviceSerialNumber, string.Empty);
            //}
        }


        #region "QC Device CRUD"

        // Overload helpful in testing
        private bool DbCreateQcDevice(QCDevice qcDevice)
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var deviceCollection = db.GetCollection<QCDevice>("qcdevices");
                    deviceCollection.Insert(qcDevice);
                    deviceCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private QCDevice DbCreateQcDevice()
        {
            try
            {
                var newDevice = new QCDevice(CurrentDeviceSerialNumber);

                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var deviceCollection = db.GetCollection<QCDevice>("qcdevices");
                    deviceCollection.Insert(newDevice);
                    deviceCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }

                return newDevice;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private QCDevice DbReadQcDevice()
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var deviceCollection = db.GetCollection<QCDevice>("qcdevices");

                    // Only ONE in the collection
                    var device = deviceCollection.FindOne(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber);

                    return device;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private bool DbUpdateQcDevice(QCDevice device)
        {
            Debug.Assert(device != null);

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var deviceCollection = db.GetCollection<QCDevice>("qcdevices");
                    deviceCollection.Update(device);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private bool DbDeleteQcDevice(QCDevice device)
        {
            Debug.Assert(device != null);

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Delete all users for this device including device and negative control
                    var deviceCollection = db.GetCollection<QCDevice>("qcdevices");
                    deviceCollection.Delete(device.Id);

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

        // Overload helpful in testing
        private bool DbCreateQcNegativeControl(QCUser negativeControl)
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Insert(negativeControl);
                    userCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        private QCUser DbCreateQcNegativeControl()
        {
            Debug.Assert(!string.IsNullOrEmpty(CurrentDeviceSerialNumber));

            try
            {
                var newNegativeControl = new QCUser(CurrentDeviceSerialNumber, QCUser.NegativeControlName);

                if (DbCreateQcNegativeControl(newNegativeControl))
                {
                    return newNegativeControl;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private QCUser DbReadQcNegativeControl()
        {
            // Only ONE in the collection
            using (var db = new LiteDatabase(QCDatabasePath))
            {
                try
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    var qcNegativeControl = userCollection.FindOne(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == QCUser.NegativeControlName);
                    return qcNegativeControl;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return null;
                }
            }
        }

        private bool DbUpdateQcNegativeControl(QCUser negativeControl)
        {
            if (negativeControl == null)
                return false;

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
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

        private bool DbDeleteQcNegativeControl(QCUser negativeControl)
        {
            if (negativeControl == null)
                return false;

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
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

        // Overload helpful in testing
        private bool DbCreateQcUser(QCUser newUser)
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");
                    userCollection.Insert(newUser);
                    userCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        private QCUser DbCreateQcUser(string userName)
        {
            Debug.Assert(!string.IsNullOrEmpty(CurrentDeviceSerialNumber) && !string.IsNullOrEmpty(userName));

            try
            {
                var newQcUser = new QCUser(CurrentDeviceSerialNumber, userName);

                if (DbCreateQcUser(newQcUser))
                {
                    return newQcUser;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private QCUser DbReadQcUser(string userName)
        {
            Debug.Assert(!string.IsNullOrEmpty(userName));

            using (var db = new LiteDatabase(QCDatabasePath))
            {
                try
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var user = userCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == userName)
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

        private bool DbUpdateQcUser(QCUser user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
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

        private bool DbDeleteQcUser(QCUser user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Delete all tests for this user
                    var testsCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testsCollection.Query()
                        .Where(x => x.DeviceSerialNumber == user.DeviceSerialNumber && x.UserName == user.UserName)
                        .ToList();

                    foreach (var t in tests)
                    {
                        testsCollection.Delete(t.Id);
                    }

                    // Delete user
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

        private List<QCUser> ReadAllQcUsers()
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var users = userCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName != QCUser.NegativeControlName)
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

        private readonly int TestThresholdMin = 5;
        private readonly int TestThresholdMax = 40;

        // Overload helpful in testing
        private bool DbCreateQcTest(QCTest test)
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    testCollection.Insert(test);
                    testCollection.EnsureIndex(x => x.DeviceSerialNumber);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        private QCTest DbCreateQcTest(string userName, int testValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(userName) && testValue >= 0);

            try
            {
                string testStatus;

                if (testValue >= TestThresholdMin && testValue <= TestThresholdMax)
                {
                    testStatus = QCTest.TestPass;
                }
                else
                {
                    testStatus = QCTest.TestFail;
                }

                var newTest = new QCTest(CurrentDeviceSerialNumber, userName, DateTime.Now, testValue, testStatus);

                if (DbCreateQcTest(newTest))
                {
                    return newTest;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private List<QCTest> DbReadQcTests(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return new List<QCTest>(); // Empty

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == userName)
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
        private bool DbUpdateQcTest(QCTest test)
        {
            if (test == null)
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
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

        private bool DbDeleteAllQcTests(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == userName)
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

        #endregion


        #region "Main Screen Commands"

        [ObservableProperty]
        private string _newUserName = string.Empty;

        [RelayCommand]
        private async Task UpdateNegativeControl()
        {
            SelectedUserIndex = 0;

            if (QcButtonViewModels[SelectedUserIndex].Assigned)
            {
                // Open and run automatic test for negative control
                await StartNegativeControlTest();
            }
            else
            {
                Debugger.Break(); // Should never happen
            }
        }

        private async Task UpdateUserAsync(int userIndex)
        {
            if (QcButtonViewModels[userIndex].Assigned)
            {
                if (SelectedQcUser.CurrentStatus == QCUser.UserDisqualified)
                {
                    Services.Dialogs.ShowAlert($"'{SelectedQcUser.UserName}' is currently disqualified and no further tests are allowed.", "User Disqualified", "OK");
                    return;
                }

                // Open and run new breath test for this user
                await StartUserBreathTest();
            }
            else
            {

                    // Open user name dialog and create user, then open breath test view
                    string userName = await Services.Dialogs.UserNamePromptAsync();

                    if (userName == "cancel")
                        return;

                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (QCUsers.Any(user => userName.ToLower() == user.UserName.ToLower()))
                        {
                            await Services.Dialogs.ShowAlertAsync($"User name [{userName}] already exists.", "Conflicting User Name", "OK");
                        }
                        else
                        {
                            QcButtonViewModels[userIndex].QCUserModel = DbCreateQcUser(userName);

                            // Open and run new breath test for this user
                            await StartUserBreathTest();
                        }
                    }
            }
        }

        private async Task ChartUserAsync()
        {
            Services.Navigation.ShowQCChartPopup(this);
        }

        [RelayCommand]
        private async Task UpdateUser1Async()
        {
            SelectedUserIndex = 1;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task ChartUser1Async()
        {
            SelectedUserIndex = 1;
            await ChartUserAsync();
        }

        [RelayCommand]
        private async Task UpdateUser2Async()
        {
            SelectedUserIndex = 2;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task ChartUser2Async()
        {
            SelectedUserIndex = 2;
            await ChartUserAsync();
        }


        [RelayCommand]
        private async Task UpdateUser3Async()
        {
            SelectedUserIndex = 3;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task ChartUser3Async()
        {
            SelectedUserIndex = 3;
            await ChartUserAsync();
        }


        [RelayCommand]
        private async Task UpdateUser4Async()
        {
            SelectedUserIndex = 4;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task ChartUser4Async()
        {
            SelectedUserIndex = 4;
            await ChartUserAsync();
        }

        [RelayCommand]
        private async Task UpdateUser5Async()
        {
            SelectedUserIndex = 5;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task ChartUser5Async()
        {
            SelectedUserIndex = 5;
            await ChartUserAsync();
        }

        [RelayCommand]
        private async Task UpdateUser6Async()
        {
            SelectedUserIndex = 6;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task ChartUser6Async()
        {
            SelectedUserIndex = 6;
            await ChartUserAsync();
        }

        [RelayCommand]
        private async Task ShowQCSettings()
        {
            await Services.Navigation.QCSettingsView();
        }

        [RelayCommand]
        public async Task ExitToQC()
        {
            await Services.Navigation.QualityControlView();
        }

        #endregion


        #region "Negative Control Test"

        [ObservableProperty]
        private int _negativeControlTestResult = 0;

        [ObservableProperty]
        private string _negativeControlTestResultString = "(0 ppb)";

        [ObservableProperty]
        private string _negativeControlStatus;

        private async Task StartNegativeControlTest()
        {
            UiTimer = new Timer(Config.TestResultReadyWait * 1000);
            UiTimer.Elapsed += (sender, e) => NegativeControlTestCompleted();
            UiTimer.Start();

            await Services.Navigation.QCNegativeControlTestView();
        }

        private void NegativeControlTestCompleted()
        {
            // Notes:

            // After each negative control test...
            //  1. Determine negative control test status (time > 16h and less than 24h and value <= 5)
            //  2. Update negative control in database
            //  3. Update device status in database

            UiTimer.Stop();
            UiTimer.Dispose();

            int negativeControlTestResult = 5; // ToDo: Temporary until we have real value from hardware
            NegativeControlTestResultString = $"({negativeControlTestResult} ppb)";

            // Update NegativeControl in DB
            QCTest negativeControlTest = DbCreateQcTest(QCUser.NegativeControlName, negativeControlTestResult);

            // Update Negative Control Result View
            NegativeControlStatus = negativeControlTest.TestStatus;

            // Update device status in main screen
            UpdateNegativeControlStatus();
            UpdateDeviceStatus(); // Also calls UpdateNegativeControlStatus

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
            PlaySounds.PlaySuccessSound();

            Task.Delay(Config.StopExhalingReadyWait);

            if (Services.DeviceService.Current != null && Services.DeviceService.Current.BreathManeuver.StatusCode != 0x00)
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
            UiTimer.Elapsed += (sender, e) => UserTestCompleted();
            UiTimer.Start();
        }

        [ObservableProperty]
        private string _userTestErrorCode = string.Empty;

        private void UserTestCompleted()
        {
            Debug.Assert(Services.DeviceService.Current != null);

            // Notes:

            // After each user test...
            //  1. Add test to database (Pass or Fail)
            //  2. First 3: determine test status by value and comparing any previous tests (including time and date checks), update User table in database
            //  3. After 3 compare value to QTX and set test status, update User table in database
            //  4. Update device status in database

            UiTimer.Stop();
            UiTimer.Dispose();

            if (Services.DeviceService.Current is { FenomReady: false }) // ToDo: Is this the best way to handle this
                return;


            if (Services.DeviceService.Current != null && Services.DeviceService.Current.BreathManeuver.StatusCode != 0x00)
            {
                // ToDo: How to handle fail here?
                PlaySounds.PlayFailedSound();

                // Don't add this test with error to the database
                Services.Navigation.QCUserTestErrorView();
            }
            else
            {
                QCTest test = DbCreateQcTest(QCUser.NegativeControlName, Services.DeviceService.Current.FenomValue);

                UpdateUserStatus();
                UpdateNegativeControlStatus();
                UpdateDeviceStatus();

                Services.Navigation.QCUserTestResultView();
            }
        }

        #endregion


        #region "QC User Test Result View"


        [ObservableProperty]
        private string _qCUserTestResult = string.Empty;

        public void InitUserTestResults()
        {
            if (Services.DeviceService.Current == null)
                return;

            QCUserTestResult = (Services.DeviceService.Current.FenomValue) < 5 ? "< 5" :
                (Services.DeviceService.Current.FenomValue) > 40 ? "> 40" :
                Services.DeviceService.Current.FenomValue.ToString(CultureInfo.InvariantCulture);

            Services.DeviceService.Current.ReadyForTest = false;
        }

        #endregion


        #region "Debug Commands"

        [RelayCommand]
        private void CreateMockDataBase1()
        {
            DeleteDataBase();

            var newDevice = DbCreateQcDevice();

            var negativeControl = DbCreateQcNegativeControl();

            // New User Qualified
            var newUser1 = DbCreateQcUser("Jim");
            newUser1.CurrentStatus = QCUser.UserQualified;
            newUser1.ExpiresDate = DateTime.Now.AddDays(7);
            newUser1.NextTestDate = DateTime.Now.AddHours(16);
            DbUpdateQcUser(newUser1);

            var newTest = DbCreateQcTest(newUser1.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = DbCreateQcTest(newUser1.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = DbCreateQcTest(newUser1.UserName, 25);
            newTest.TestDate = DateTime.Now.AddHours(16);
        }

        [RelayCommand]
        private void CreateMockDataBase2()
        {
            DeleteDataBase();

            var newDevice = DbCreateQcDevice();

            var negativeControl = DbCreateQcNegativeControl();

            // New User Qualified
            var newUser1 = DbCreateQcUser("Jim");
            newUser1.CurrentStatus = QCUser.UserQualified;
            newUser1.ExpiresDate = DateTime.Now.AddDays(7);
            newUser1.NextTestDate = DateTime.Now.AddHours(16);
            DbUpdateQcUser(newUser1);

            var newTest = DbCreateQcTest(newUser1.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = DbCreateQcTest(newUser1.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = DbCreateQcTest(newUser1.UserName, 25);
            newTest.TestDate = DateTime.Now.AddHours(16);

            // New User Disqualified
            var newUser2 = DbCreateQcUser("Vinh");
            newUser2.CurrentStatus = QCUser.UserDisqualified;
            newUser2.ExpiresDate = DateTime.Now.AddDays(7);
            newUser2.NextTestDate = DateTime.Now.AddHours(16);
            DbUpdateQcUser(newUser2);

            newTest = DbCreateQcTest(newUser2.UserName, 20);
            newTest.TestDate = DateTime.Now.AddHours(-16);
            newTest = DbCreateQcTest(newUser2.UserName, 30);
            newTest.TestDate = DateTime.Now;
            newTest = DbCreateQcTest(newUser2.UserName, 19);
            newTest.TestDate = DateTime.Now.AddHours(16);
        }

        [RelayCommand]
        private void CreateMockDataBase3()
        {
            DeleteDataBase();

            var newDevice = DbCreateQcDevice();

            var negativeControl = DbCreateQcNegativeControl();

            // New User Qualified
            var newUser1 = DbCreateQcUser("Jim");
            newUser1.CurrentStatus = QCUser.UserQualified;
            newUser1.ExpiresDate = DateTime.Now.AddDays(7);
            newUser1.NextTestDate = DateTime.Now.AddHours(16);
            DbUpdateQcUser(newUser1);

            var newTest1 = DbCreateQcTest(newUser1.UserName, 20);
            newTest1.TestDate = DateTime.Now.AddHours(-16);
            DbUpdateQcTest(newTest1);
            var newTest2 = DbCreateQcTest(newUser1.UserName, 30);
            newTest2.TestDate = DateTime.Now;
            DbUpdateQcTest(newTest2);
            var newTest3 = DbCreateQcTest(newUser1.UserName, 25);
            newTest3.TestDate = DateTime.Now.AddHours(16);
            DbUpdateQcTest(newTest3);

            // New User Disqualified
            var newUser2 = DbCreateQcUser("Vinh");
            newUser2.CurrentStatus = QCUser.UserDisqualified;
            newUser2.ExpiresDate = DateTime.Now.AddDays(7);
            newUser2.NextTestDate = DateTime.Now.AddHours(16);
            DbUpdateQcUser(newUser2);

            newTest1 = DbCreateQcTest(newUser2.UserName, 20);
            newTest1.TestDate = DateTime.Now.AddHours(-16);
            DbUpdateQcTest(newTest1);
            newTest2 = DbCreateQcTest(newUser2.UserName, 30);
            newTest2.TestDate = DateTime.Now;
            DbUpdateQcTest(newTest2);
            newTest3 = DbCreateQcTest(newUser2.UserName, 19);
            newTest3.TestDate = DateTime.Now.AddHours(16);
            DbUpdateQcTest(newTest3);

            // New User Disqualified
            var newUser3 = DbCreateQcUser("Bob");
            newUser3.CurrentStatus = QCUser.UserDisqualified;
            newUser3.ExpiresDate = DateTime.Now.AddDays(7);
            newUser3.NextTestDate = DateTime.Now.AddHours(16);
            DbUpdateQcUser(newUser3);

            newTest1 = DbCreateQcTest(newUser3.UserName, 20);
            newTest1.TestDate = DateTime.Now.AddHours(-16);
            DbUpdateQcTest(newTest1);
            newTest2 = DbCreateQcTest(newUser3.UserName, 30);
            newTest2.TestDate = DateTime.Now;
            DbUpdateQcTest(newTest2);
            newTest3 = DbCreateQcTest(newUser3.UserName, 32);
            newTest3.TestDate = DateTime.Now.AddHours(16);
            DbUpdateQcTest(newTest3);

            // New User Disqualified
            var newUser4 = DbCreateQcUser("New");
            newUser4.CurrentStatus = QCUser.UserConditionallyQualified;
            newUser4.ExpiresDate = DateTime.Now.AddDays(7);
            newUser4.NextTestDate = DateTime.Now.AddHours(16);
            DbUpdateQcUser(newUser4);

            newTest1 = DbCreateQcTest(newUser4.UserName, 20);
            newTest1.TestDate = DateTime.Now.AddHours(-16);
            DbUpdateQcTest(newTest1);
        }

        [RelayCommand]
        private void DeleteDataBase()
        {
            try
            {
                File.Delete(QCDatabasePath);
                File.Delete(QCDatabasePath);
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        [ObservableProperty]
        private ObservableCollection<QCDevice> _allQcDevices;

        [RelayCommand]
        public void GetAllQcDevices()
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var deviceCollection = db.GetCollection<QCDevice>("qcdevices");

                    var devices = deviceCollection.FindAll().ToList();

                    AllQcDevices = new ObservableCollection<QCDevice>(devices);

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        [RelayCommand]
        private void DeleteDevice(object parameter)
        {
            int index = (int)parameter;

            if (AllQcDevices[index - 1].DeviceSerialNumber == CurrentDeviceSerialNumber)
            {
                Services.Dialogs.ShowAlert("You cannot delete the current device.", "Current Device", "OK");
                return;
            }

            if (Services.Dialogs.ShowConfirmYesNo("Are you sure you wish to delete this device?", "Delete Device").Result)
            {
                // Delete device, user and tests also
                DbDeleteQcDevice(AllQcDevices[index - 1]);// SfDataGrid apparently is one based on the index
                GetAllQcDevices();
            }
        }

        [ObservableProperty]
        private ObservableCollection<QCUser> _allQcUsers;

        [RelayCommand]
        private void GetAllQcUsers()
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    var user = userCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName != QCUser.NegativeControlName)
                        .ToList();

                    AllQcUsers = new ObservableCollection<QCUser>(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        [RelayCommand]
        private void DeleteUser(object parameter)
        {
            int index = (int)parameter;

            if (Services.Dialogs.ShowConfirmYesNo("Are you sure you wish to delete this device?", "Delete Device").Result)
            {
                // Delete User and tests
                DbDeleteQcUser(AllQcUsers[index - 1]); // SfDataGrid apparently is one based on the index
                GetAllQcUsers();
            }
        }

        [ObservableProperty]
        private ObservableCollection<QCTest> _allQcTests;

        [RelayCommand]
        private void GetAllQcTests()
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var testCollection = db.GetCollection<QCTest>("qctests");

                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber)
                        .ToList();

                    AllQcTests = new ObservableCollection<QCTest>(tests);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private QCTest GetLastNegativeControlTest()
        {
            Debug.Assert(!string.IsNullOrEmpty(CurrentDeviceSerialNumber));

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var testsCollection = db.GetCollection<QCTest>("qctests");

                    var tests = testsCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == QCUser.NegativeControlName)
                        .OrderByDescending(x => x.TestDate)
                        .ToList();

                    return tests[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        #endregion


        #region "Helper Routines"

        private int GetRange(int a, int b)
        {
            return Math.Abs(a - b);
        }

        private (int min, int max, int median) GetRangeAndMedian(int a, int b, int c)
        {
            int[] numbers = { a, b, c };
            Array.Sort(numbers);

            return (numbers[0], numbers[2], numbers[1]);
        }

        private int TimeSpanHours(DateTime t1, DateTime t2)
        {
            TimeSpan timeSpan = t1 - t2;
            return Math.Abs(timeSpan.Hours);
        }

        #endregion



        #region "Determine Status Routines"

        private string UpdateDeviceStatus()
        {
            // Returns: "Pass", "Fail", "Expired"

            string deviceStatus;

            if (QCNegativeControl.CurrentStatus == QCUser.NegativeControlNone)
            {
                deviceStatus = QCDevice.DeviceInsufficientData;
            }
            else if (QCNegativeControl.CurrentStatus == QCUser.NegativeControlExpired)
            {
                deviceStatus = QCDevice.DeviceExpired;
            }
            else if (!AnyUserQualified())
            {
                deviceStatus = QCDevice.DeviceFail;
            }
            else
            {
                deviceStatus = QCDevice.DevicePass;
            }

            QCDevice.CurrentStatus = deviceStatus;
            // No need to set QCDevice.ExpiresDate in case of device
            // No need to set QCDevice.NextTestDate in case of device

            DbUpdateQcDevice(QCDevice);

            return
                deviceStatus;
        }


        private readonly int NegativeControlMaxThreshold = 5;
        private readonly int NegativeControlTimeoutHours = 24;

        private void UpdateNegativeControlStatus()
        {
            // Returns: "Pass", "Fail", "Expired"

            QCTest negativeControlTest = GetLastNegativeControlTest();

            int timeSpanHours = TimeSpanHours(DateTime.Now, negativeControlTest.TestDate);

            bool timeSpanOK = timeSpanHours <= NegativeControlTimeoutHours;

            string negativeControlStatus;

            if (timeSpanHours > NegativeControlTimeoutHours)
            {
                negativeControlStatus = QCUser.NegativeControlExpired;
            }
            else if (negativeControlTest.TestValue >= NegativeControlMaxThreshold)
            {
                negativeControlStatus = QCUser.NegativeControlFail;
            }
            else
            {
                negativeControlStatus = QCUser.NegativeControlPass;
            }

            QCNegativeControl.CurrentStatus = negativeControlStatus;
            QCNegativeControl.ExpiresDate = negativeControlTest.TestDate.AddHours(NegativeControlTimeoutHours);
            QCNegativeControl.NextTestDate = QCNegativeControl.ExpiresDate;
            DbUpdateQcNegativeControl(QCNegativeControl);
        }

        private readonly int UserTimeoutMaxHours = 24;
        private readonly int UserTimeoutMinHours = 16;

        private void UpdateUserStatus()
        {
            // Status for User...
            //•	Conditionally Qualified
            //     - Fewer than 4 tests have been performed by the QC User.
            //     - All tests within the Qualification Period are “Pass”.
            //•	Qualified
            //     - Latest test is within the expected range for the QC User.
            //•	Disqualified
            //      - Latest test is outside the expected range for the QC User.
            //•	None
            //      - QC User test is required.

            bool testValuesWithinRange;

            string userStatus = QCUser.UserNone;

            // Returned in descending order so element[0] is the latest test
            List<QCTest> tests = GetLast3Tests(SelectedQcUser.UserName);

            switch (tests.Count)
            {
                case 0:
                    userStatus = QCUser.UserConditionallyQualified;
                    break;

                case 1:
                    SelectedQcUser.C1 = tests[0].TestValue; // Latest test

                    if (tests[0].TestStatus == QCTest.TestPass)
                    {
                        userStatus = QCUser.UserConditionallyQualified;
                    }
                    else
                    {
                        Services.Dialogs.ShowAlert($"'{SelectedQcUser.UserName}' is disqualified and user cannot become a control.", "User Disqualified", "OK");
                        userStatus = QCUser.UserDisqualified;
                    }
                    break;

                case 2:
                    SelectedQcUser.C2 = tests[0].TestValue; // Latest test

                    var testTimeSpanHours = TimeSpanHours(tests[0].TestDate, tests[1].TestDate);
                    var testTimeSpanGood = testTimeSpanHours <= UserTimeoutMaxHours; 

                    testValuesWithinRange = Math.Abs(SelectedQcUser.C1 - SelectedQcUser.C2) <= 10;

                    if (tests[0].TestStatus == QCTest.TestPass && tests[1].TestStatus == QCTest.TestPass && testTimeSpanGood && testValuesWithinRange) 
                    {
                        userStatus = QCUser.UserConditionallyQualified;
                    }
                    else
                    {
                        Services.Dialogs.ShowAlert($"'{SelectedQcUser.UserName}' is disqualified and user cannot become a control.", "User Disqualified", "OK");
                        userStatus = QCUser.UserDisqualified;
                    }
                    break;

                case 3:
                    SelectedQcUser.C3 = tests[0].TestValue; // Latest test

                    (int min, int max, int median) = GetRangeAndMedian(tests[0].TestValue, tests[1].TestValue, tests[2].TestValue);

                    SelectedQcUser.QCT = median;

                    bool allTestsPassed = tests[0].TestStatus == QCTest.TestPass &&
                                          tests[1].TestStatus == QCTest.TestPass &&
                                          tests[2].TestStatus == QCTest.TestPass;

                    testValuesWithinRange = (max - min) <= 10;

                    var timeSpanQualificationHours = TimeSpanHours(tests[0].TestDate, tests[2].TestDate);
                    var timeSpanQualificationGood = timeSpanQualificationHours < (7 * 24);

                    var lastTestTimeSpanHours = TimeSpanHours(DateTime.Now, tests[0].TestDate);
                    var lastTestTimeSpanGood = lastTestTimeSpanHours <= UserTimeoutMaxHours;

                    if (allTestsPassed && timeSpanQualificationGood && lastTestTimeSpanGood && testValuesWithinRange)
                    {
                        userStatus = QCUser.UserQualified;
                    }
                    else
                    {
                        userStatus = QCUser.UserDisqualified;
                    }
                    SelectedQcUser.ShowChartOption = true;
                    break;

                default:
                    if (SelectedQcUser.CurrentStatus == QCUser.UserQualified)
                    {
                        Debug.Assert(SelectedQcUser.QCT > 0);

                        QCTest lastTest = tests[0];

                        // ToDo: Calculate result based on QCT
                        int deltaValue = Math.Abs(SelectedQcUser.QCT - lastTest.TestValue);

                        userStatus = deltaValue <= 10 ? QCUser.UserQualified : QCUser.UserDisqualified;
                        SelectedQcUser.ShowChartOption = true;
                    }
                    break;
            }

            // Update the user in the database
            SelectedQcUser.CurrentStatus = userStatus;
            SelectedQcUser.ExpiresDate = tests[0].TestDate.AddHours(UserTimeoutMaxHours);
            SelectedQcUser.NextTestDate = tests[0].TestDate.AddHours(UserTimeoutMinHours);
            DbUpdateQcUser(SelectedQcUser);
        }

        private void RefreshStatusBasedOnExpirationDates()
        {
            //DateTime currentDatetime = DateTime.Now;

            //QCUsers = ReadAllQcUsers();

            //foreach (var user in QCUsers)
            //{
            //    // User will never expire once qualified
            //    if (user.CurrentStatus == QCUser.UserQualified)
            //        continue;

            //    if (currentDatetime > user.ExpiresDate)
            //    {
            //        user.CurrentStatus = QCUser.
            //    }


            //}
        }

        private bool AnyUserQualified()
        {
            // Refresh list
            QCUsers = ReadAllQcUsers();

            DateTime lastTestDate = DateTime.MinValue;

            foreach (var user in QCUsers)
            {
                var tests = GetLast3Tests(user.UserName).ToList();

                if (tests[0].TestDate > lastTestDate)
                    lastTestDate = tests[0].TestDate;
            }

            bool lastTestDateOK = TimeSpanHours(DateTime.Now, lastTestDate) <= 24;

            foreach (var user in QCUsers)
            {
                if (user.CurrentStatus is QCUser.UserConditionallyQualified or QCUser.UserQualified && lastTestDateOK)
                    return true;
            }

            return false;
        }

        private List<QCTest> GetLast3Tests(string userName)
        {
            Debug.Assert(!string.IsNullOrEmpty(CurrentDeviceSerialNumber) && !string.IsNullOrEmpty(userName));

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Get all user records for this device and user
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == userName)
                        .OrderByDescending(x => x.TestDate).ToList();

                    var last3 = new List<QCTest>();

                    if (tests.Count >= 1)
                    {
                        last3.Add(tests[0]);
                    }

                    if (tests.Count >= 2)
                    {
                        last3.Add(tests[1]);
                    }

                    if (tests.Count >= 3)
                    {
                        last3.Add(tests[2]);
                    }

                    return last3;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        #endregion


        #region "Chart Routines"

        [ObservableProperty]
        private List<XYData> _userTestData;

        [ObservableProperty]
        private List<XYData> _upperBoundsData;

        [ObservableProperty]
        private List<XYData> _lowerBoundsData;

        public void InitializeUserDataForChart()
        {
            UserTestData = new List<XYData>();
            UpperBoundsData = new List<XYData>();
            LowerBoundsData = new List<XYData>();

            List<QCTest> allUserTests = DbReadAllQcTests(SelectedQcUser.UserName);


            int dataCount = allUserTests.Count - 1;
            for (int i = 0; i <= dataCount; i++)
            {
                UserTestData.Add(new XYData(i+1, allUserTests[i].TestValue));
            }

            int qct = SelectedQcUser.QCT;

            UpperBoundsData = new List<XYData>
            {
                new XYData(0, qct + 10),
                new XYData(dataCount, qct + 10)
            };

            LowerBoundsData = new List<XYData>
            {
                new XYData(0, qct - 10),
                new XYData(dataCount, qct - 10)
            };
        }

        private List<QCTest> DbReadAllQcTests(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return new List<QCTest>(); // Empty

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == userName)
                        .OrderBy(x => x.TestDate)
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

    public class XYData
    {
        public int X;
        public int Y;

        public XYData(int x, int y)
        {
            x = x;
            Y = y;
        }
    }

}
