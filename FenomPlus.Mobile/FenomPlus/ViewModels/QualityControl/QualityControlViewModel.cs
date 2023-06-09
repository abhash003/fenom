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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Syncfusion.SfChart.XForms;
using Color = Xamarin.Forms.Color;
using Xamarin.Forms;
using FenomPlus.Views;
using FenomPlus.Services.DeviceService.Concrete;
using FenomPlus.Enums.ErrorCodes;

namespace FenomPlus.ViewModels
{
    public partial class QualityControlViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _errorCode;

        private readonly string QCDatabasePath;

        //QcButtonViewModels[0] is the Negative Control, the other elements are users
        public List<QcButtonViewModel> QcButtonViewModels = new List<QcButtonViewModel>();

        private QCDevice QCDevice; // For a specific device with unique serial number - only one per device

        private QCUser QCNegativeControl => QcButtonViewModels[0].QCUserModel;


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

        private string _currentDeviceStatus;
        public string CurrentDeviceStatus
        {
            get => _currentDeviceStatus;
            set
            {
                if (_currentDeviceStatus != value)
                {
                    _currentDeviceStatus = value;
                    OnPropertyChanged(nameof(CurrentDeviceStatus));
                    OnPropertyChanged(nameof(DeviceStatusString));
                }
            }
        }
        public string DeviceStatusString => $"Device Status ({CurrentDeviceStatus})";

        public bool RequireQC
        {
            get => QCDevice.RequireQC;
            set
            {
                var device = Services.DeviceService?.Current;
                QCDevice.RequireQC = value;
                if (device != null && value != device.IsQCEnabled())
                {
                    device.ToggleQC(value);
                    // allow time for the message write to device then read back 
                    Task.Delay(TimeSpan.FromMilliseconds(500)).ContinueWith(_=> 
                    {
                        CurrentDeviceStatus = Services.DeviceService?.Current.GetDeviceQCStatus();
                    });
                }
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
            MessagingCenter.Subscribe<BreathManeuver, string>(this, "NOScore", (sender, arg) =>
            {
                if (int.TryParse(arg, out int score) && Services.Cache.TestType == TestTypeEnum.NegativeControl)
                {
                    NegativeControlTestResult = score;
                    NegativeControlTestCompleted(score);
                    // also need to tell the Marigold Flower to stop Animation and disappear
                    if (App.GetCurrentPage() is QCNegativeControlTestView)  // in recurring flowering growing view  
                    {
                        // Only navigate if during startup
                        Services.Navigation.QCNegativeControlResultView();
                    }
                }
            });
            MessagingCenter.Subscribe<BleDevice, byte>(this, "ErrorStatus", (sender, arg) =>
            {
                if (Services.Cache.TestType == TestTypeEnum.NegativeControl && arg != 0x0)
                {
                    if (App.GetCurrentPage() is QCNegativeControlTestView)  // in recurring flowering growing view  
                    {
                        ShowErrorPage(arg);
                    }
                }
            });
            MessagingCenter.Subscribe<StatusViewModel>(this, "DeviceStatusNeedUpdate", (_) =>
            {
                CurrentDeviceStatus = Services.DeviceService?.Current.GetDeviceQCStatus();
            });
        }

        private void ShowErrorPage(byte arg)
        {
            var error = ErrorCodeLookup.Lookup(arg);
            ErrorCode = error.Code;
            ErrorMessage = error.Message;
            Services.Navigation.QCUserTestErrorView();
        }

        public void LoadData()
        {
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
            // CurrentDeviceStatus = UpdateDeviceStatus();

            // Get all users for this currently connected device
            //QcUserList = ReadAllQcUsers();
            UpdateQcUserList();
            for (int i = 0; i< QcUserList.Count; ++i)
            {
                QcButtonViewModels[i+1].QCUserModel = QcUserList[i];
            }
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

        #endregion


        //--------------------------------------------------------------------------------------

        [ObservableProperty]
        private ObservableCollection<QCDevice> _qcDeviceList;

        private ObservableCollection<QCDevice> ReadAllQcDevices()
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var deviceCollection = db.GetCollection<QCDevice>("qcdevices");

                    var devices = deviceCollection.FindAll().ToList();

                    watch.Stop();
                    var ms = watch.ElapsedMilliseconds;

                    return new ObservableCollection<QCDevice>(devices);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ObservableCollection<QCDevice>();
            }
        }


        public void UpdateQcDeviceList()
        {
            QcDeviceList = ReadAllQcDevices();
        }

        //--------------------------------------------------------------------------------------

        [ObservableProperty]
        private ObservableCollection<QCUser> _qcUserList;

        private ObservableCollection<QCUser> ReadAllQcUsers()
        {
            try
            {

                Stopwatch watch = new Stopwatch();
                watch.Start();

                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var userCollection = db.GetCollection<QCUser>("qcusers");

                    if (string.IsNullOrEmpty(CurrentDeviceSerialNumber))
                    {
                        var users = userCollection.Query()
                            .Where(x => x.UserName != QCUser.NegativeControlName)
                            .OrderBy(x => x.UserName)
                            .ToList();

                        watch.Stop();
                        var ms = watch.ElapsedMilliseconds;

                        return new ObservableCollection<QCUser>(users);
                    }
                    else
                    {
                        // Return all users if device not connected (No device serial number)
                        var users = userCollection.Query()
                            .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber &&
                                        x.UserName != QCUser.NegativeControlName)
                            .OrderBy(x => x.UserName)
                            .ToList();

                        watch.Stop();
                        var ms = watch.ElapsedMilliseconds;

                        return new ObservableCollection<QCUser>(users);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<QCUser>(); // Empty
            }
        }

        public void UpdateQcUserList()
        {
            QcUserList = ReadAllQcUsers();
            foreach(QCUser qcUser in QcUserList)
            {
                if (qcUser.CurrentStatus != QCUser.UserDisqualified && qcUser.ExpiresDate > DateTime.Now.AddDays(7))
                {
                    qcUser.CurrentStatus = QCUser.UserDisqualified;
                    DbUpdateQcUser(qcUser);
                }
            }
        }

        //--------------------------------------------------------------------------------------

        [ObservableProperty]
        private ObservableCollection<QCTest> _qcTestList;

        private ObservableCollection<QCTest> ReadAllQcTests()
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.FindAll()
                        .OrderByDescending(x => x.TestDate)
                        .ToList();

                    watch.Stop();
                    var ms = watch.ElapsedMilliseconds;

                    return new ObservableCollection<QCTest>(tests);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new ObservableCollection<QCTest>();
            }
        }

        private ObservableCollection<QCTest> ReadUserQcTests(string name)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    var tests = testCollection.Query()
                        .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == name)
                        .OrderBy(x => x.TestDate)
                        .ToList();

                    watch.Stop();
                    var ms = watch.ElapsedMilliseconds;

                    return new ObservableCollection<QCTest>(tests);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new ObservableCollection<QCTest>();
            }
        }

        public void UpdateQcTestList()
        {
            QcTestList = ReadAllQcTests();
        }


        #region "QCTests CRUD"

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

        private QCTest DbCreateQcTest(string userName, float? testValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(userName) && testValue >= 0);

            try
            {
                bool goodScore = NegativeControlMaxThreshold <= testValue && testValue <= TestThresholdMax;
                string testStatus = goodScore ? QCTest.TestPass : QCTest.TestFail;
                var newTest = new QCTest(CurrentDeviceSerialNumber, userName, DateTime.Now, testValue, testStatus);

                return DbCreateQcTest(newTest)? newTest : null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private QCTest DbCreateNegativeControlTest(int testValue)
        {
            Debug.Assert(testValue >= 0);

            try
            {
                string testStatus;

                testStatus = Math.Abs(testValue) < NegativeControlMaxThreshold ? QCTest.TestPass : QCTest.TestFail;

                var newTest = new QCTest(CurrentDeviceSerialNumber, QCUser.NegativeControlName, DateTime.Now, testValue, testStatus);

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

        private bool DbDeleteQcTest(QCTest test)
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Get all user records for this device
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    testCollection.Delete(test.Id);

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
            DeviceCheckEnum? de = PreNegativeControlChecking();
            if ((de != DeviceCheckEnum.DevicePurging && de != DeviceCheckEnum.Ready) ||
                 de == DeviceCheckEnum.BatteryCriticallyLow)
                return;
            if (QcButtonViewModels[userIndex].Assigned)
            {
                if (!Services.DeviceService.Current.IsQCEnabled())
                {
                    await Services.Dialogs.ShowAlertAsync($"Quality Control is Disabled.", "Quality Control Error", "Close");
                    return;
                }
                switch (SelectedQcUser.CurrentStatus)
                {
                    case QCUser.UserDisqualified:
                        await Services.Dialogs.ShowAlertAsync($"'{SelectedQcUser.UserName}' is currently disqualified and no further tests are allowed.", "User Disqualified", "OK");
                        return;

                    case QCUser.UserConditionallyQualified:
                        {
                            (DateTime? lastTestDate, DateTime? last2ndTestDate) = GetLast2TestDate(SelectedQcUser.UserName);
                            if (DateTime.Now < lastTestDate?.AddHours(16))
                            {
                                await Services.Dialogs.ShowAlertAsync("At least 16 hours must pass from your last Qualifying test before another test can be performed.", "More Time Required", "OK");
                                return;
                            }
                            else if (DateTime.Now > last2ndTestDate?.AddHours(192))
                            {
                                SelectedQcUser.CurrentStatus = QCUser.UserDisqualified;
                                DbUpdateQcUser(SelectedQcUser);
                                // Update the GUI 
                                QcButtonViewModels[userIndex].QCUserModel = QcUserList[userIndex-1] = SelectedQcUser;
                                await Services.Dialogs.ShowAlertAsync($"More than 192 hours has passed since your last qualifying test. You are now disqualified.", "User Disqualified", "OK");
                                return;
                            }
                        }
                        break;
                    case QCUser.UserQualified:
                        {
                            (DateTime? lastTestDate, _) = GetLast2TestDate(SelectedQcUser.UserName);
                            if (DateTime.Now < lastTestDate?.AddHours(16))
                            {
                                await Services.Dialogs.ShowAlertAsync("At least 16 hours must pass before another test can be performed.", "More Time Required", "OK");
                                return;
                            }
                        }
                        break;
                }
                if (de == DeviceCheckEnum.DevicePurging)
                {
                    await Services.Dialogs.NotifyDevicePurgingAsync(Services.DeviceService.Current.DeviceReadyCountDown);
                    if (Services.Dialogs.PurgeCancelRequest)
                        return;
                }
                await StartNegativeControlTest();
            }
            else
            {
                if (de == DeviceCheckEnum.DevicePurging)
                {
                    await Services.Dialogs.NotifyDevicePurgingAsync(Services.DeviceService.Current.DeviceReadyCountDown);
                    if (Services.Dialogs.PurgeCancelRequest)
                        return;
                }
                // Open user name dialog and create user, then open breath test view
                string userName = await Services.Dialogs.UserNamePromptAsync();
                if (userName == "cancel")
                    return;

                if (!string.IsNullOrEmpty(userName))
                {
                    if (QcUserList.Any(user => userName.ToLower() == user.UserName.ToLower()))
                    {
                        await Services.Dialogs.ShowAlertAsync($"User name [{userName}] already exists.", "Conflicting User Name", "OK");
                    }
                    else
                    {
                        QcButtonViewModels[userIndex].QCUserModel = DbCreateQcUser(userName);
                        await StartNegativeControlTest();
                    }
                }
            }
        }

        [RelayCommand]
        private async Task UpdateUser1Async()
        {
            SelectedUserIndex = 1;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task UpdateUser2Async()
        {
            SelectedUserIndex = 2;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task UpdateUser3Async()
        {
            SelectedUserIndex = 3;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task UpdateUser4Async()
        {
            SelectedUserIndex = 4;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task UpdateUser5Async()
        {
            SelectedUserIndex = 5;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task UpdateUser6Async()
        {
            SelectedUserIndex = 6;
            await UpdateUserAsync(SelectedUserIndex);
        }

        [RelayCommand]
        private async Task ShowQCSettings()
        {
            await Services.Navigation.QCSettingsView();
        }

        [RelayCommand]
        public async Task ExitToQC()
        {
            if (string.IsNullOrEmpty(CurrentDeviceSerialNumber))
            {
                await Services.Navigation.DashboardView();
            }
            else
            {
                if (IsDeviceConnected)
                    await Services.Navigation.QualityControlView();
                else 
                    await Services.Navigation.DashboardView();
            }


        }
        [RelayCommand]
        public async Task ExitToQCBM()
        {
            if (string.IsNullOrEmpty(CurrentDeviceSerialNumber))
            {
                await Services.Navigation.DashboardView();
            }
            else
            {
                if (NegativeControlStatus == QCUser.NegativeControlPass)
                {
                    await InitializeBreathGauge();
                    await Services.Navigation.QCUserTestView();
                }
                else
                {
                    await Services.Navigation.QualityControlView();
                }
            }
        }

        #endregion


        #region "Negative Control Test"
        private string _buttonText = "Next";

        [ObservableProperty]
        private string _userMessage = string.Empty;

        public string ButtonText
        {
            get { return _buttonText; }
            set 
            {
                if (value == _buttonText) return;
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }
        private int _negativeControlTestResult = 0; 
        public int NegativeControlTestResult 
        {
            get { return _negativeControlTestResult; }
            set 
            {
                _negativeControlTestResult = value;
                bool goodScore = Math.Abs(value) < NegativeControlMaxThreshold ;
                NegativeControlStatus = goodScore ? QCUser.NegativeControlPass : QCUser.NegativeControlFail;
                ButtonText = goodScore ? "Next" : "Exit";
                OnPropertyChanged(nameof(NegativeControlTestResult));
            }
        }
        private string _negativeControlStatus = QCUser.NegativeControlNone;
        public string NegativeControlStatus 
        {
            get { return _negativeControlStatus; }
            set 
            {
                if (value == _negativeControlStatus) return;
                _negativeControlStatus = value;
                OnPropertyChanged(nameof(NegativeControlStatus));
            }
        }

        private async Task StartNegativeControlTest()
        {
            Services.Cache.TestType = TestTypeEnum.NegativeControl;
            UserMessage = "QC " + SelectedQcUser.UserName;
            await Services.Navigation.QCNegativeControlTestView();
            await Services.DeviceService.Current.StartTest(BreathTestEnum.QualityControl);
        }

        private void NegativeControlTestCompleted(int Score)
        {
            QCTest negativeControlTest = DbCreateNegativeControlTest(Score);
            UpdateNegativeControlStatus();
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

        /// <summary>
        /// Pre Negative Control just check if it in Device Purging before user input name
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public DeviceCheckEnum? PreNegativeControlChecking()
        {
            if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                DeviceCheckEnum deviceStatus = Services.DeviceService.Current.CheckDeviceBeforeTest(true);

                switch (deviceStatus)
                {
                    case DeviceCheckEnum.DevicePurging:
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
                    case DeviceCheckEnum.NoSensorMissing:
                        Services.Dialogs.ShowAlert($"Nitrous Oxide Sensor is missing.  Install a F150 sensor.", "Sensor Error", "Close");
                        break;
                    case DeviceCheckEnum.NoSensorCommunicationFailed:
                        Services.Dialogs.ShowAlert($"Nitrous Oxide Sensor communication failed.", "Sensor Error", "Close");
                        break;
                    case DeviceCheckEnum.QCDisabled:
                        Services.Dialogs.ShowAlert($"Quality Control is Disabled.", "Quality Control Error", "Close");
                        break;
                    case DeviceCheckEnum.ERROR_SYSTEM_NEGATIVE_QC_FAILED:
                        Services.Dialogs.ShowAlert("Negative Control failed, please contact customer service", "Error 129", "Close");
                        break;
                    case DeviceCheckEnum.Unknown:
                        var error = ErrorCodeLookup.Lookup(Services.DeviceService.Current.ErrorStatusInfo.ErrorCode);
                        Services.Dialogs.ShowAlert(((error != null) ? error.Message : "Unknown error"), "Unknown Error", "Close");
                        break;
                }
                return deviceStatus;
            }
            return null;
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
                GaugeSeconds = 10;
                GaugeStatus = "Start Blowing";
            }
        }

        public void Cache_BreathFlowChanged(object sender, EventArgs e)
        {
            _ = Task.Run(async () => 
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
            });
        }

        #endregion


        #region "QC User Stop Test View"

        public void InitUserStopBreathTest()
        {
            if (Services.DeviceService.Current != null)
            {

                Services.DeviceService.Current.BreathFlow = 0;

                PlaySounds.StopAll();
                Task.Delay(TimeSpan.FromMilliseconds(500)).ContinueWith(_=> 
                {
                    if (Services.DeviceService.Current != null && Services.DeviceService.Current.ErrorStatusInfo.ErrorCode != 0x00)
                    {
                        PlaySounds.PlayFailedSound();
                    }
                    else
                    {
                        PlaySounds.PlayStopSoundForSuccess();
                    }
                });

                Task.Delay(Config.StopExhalingReadyWait * 1000).ContinueWith(_ => // the 'STOP' sign showing time is the delay time
                {
                    var code = Services.DeviceService.Current.ErrorStatusInfo.ErrorCode;

                    if (Services.DeviceService.Current.ErrorStatusInfo.ErrorCode != 0x00)
                    {
                        var model = BreathManeuverErrorDBModel.Create(Services.DeviceService.Current.BreathManeuver, Services.DeviceService.Current.ErrorStatusInfo);
                        ErrorsRepo.Insert(model);
                        ShowErrorPage(code);
                    }
                    else
                    {
                        Services.Navigation.QCUserTestCalculationView();
                    }
                });
            }
        }

        #endregion 


        #region "QC User Test Calculation View"

        private Timer CalculationsTimer;

        public void StartUserTestCalculations()
        {
            int milliseconds = Config.TestResultReadyWait * 1000 + 1000; // ToDo: Is adding 1000 necessary?
            CalculationsTimer = new Timer(milliseconds);
            CalculationsTimer.Elapsed += (sender, e) => UserTestCompleted();
            CalculationsTimer.Start();
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

            CalculationsTimer.Stop();
            CalculationsTimer.Dispose();

            //if (Services.DeviceService.Current is { FenomReady: false }) // ToDo: Is this the best way to handle this
            //{
            //    Services.Dialogs.ShowAlert("An error has occurred and breath calculation was not completed.", "Device Not Ready", "OK");
            //    Services.Navigation.QCUserTestResultView();
            //}

            

            if (Services.DeviceService.Current.FenomReady == true)
            {

                QCTest test = DbCreateQcTest(SelectedQcUser.UserName, Services.DeviceService.Current.FenomValue);

                Debug.WriteLine( $"Cache.DeviceStatusInfo.StatusCode = {Services.DeviceService.Current.ErrorStatusInfo.ErrorCode}");

                var code = Services.DeviceService.Current.ErrorStatusInfo.ErrorCode;

                Services.Cache.TestType = Enums.TestTypeEnum.None;
                if (Services.DeviceService.Current.ErrorStatusInfo.ErrorCode != 0x00)
                {
                    // ToDo: How to handle fail here?
                    PlaySounds.PlayFailedSound();

                    // Don't add this test with error to the database
                    ShowErrorPage(code);
                }
                else
                {
                    PlaySounds.PlaySuccessSound();
                    Services.Navigation.QCUserTestResultView();
                }

            }
            else
            {
                Debugger.Break();
                Debug.WriteLine("Device reports its not ready with calculation");

            }
        }

        #endregion


        #region "QC User Test Result View"


        [ObservableProperty]
        private string _qCUserTestResult = string.Empty;

        private string QCUserTestResultString = string.Empty;
        public void InitUserTestResults()
        {
            if (Services.DeviceService.Current == null)
                return;

            float? FenomVal = Services.DeviceService.Current.FenomValue;
            if (NegativeControlMaxThreshold <= FenomVal && FenomVal <= TestThresholdMax)
            {
                QCUserTestResult = FenomVal.ToString();
                QCUserTestResultString = QCTest.TestPass;
            }
            else
            {
                QCUserTestResult = FenomVal < NegativeControlMaxThreshold ? $"<{NegativeControlMaxThreshold}" : $">{TestThresholdMax}";
                QCUserTestResultString = QCTest.TestFail;
                PromptFor2FailedUserTest();
            }

            UpdateUserStatus();
            UpdateDeviceStatus();

            Services.DeviceService.Current.ReadyForTest = false;
        }
        private bool BothQualifiedUser(string userName1, string userName2)
        {
            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    var users = db.GetCollection<QCUser>("qcusers").Query()
                            .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber &&
                                        (x.UserName == userName1 || x.UserName == userName2))
                            .ToList();
                    return users.Count==2 && users[0].CurrentStatus == QCUser.UserQualified 
                                          && users[1].CurrentStatus == QCUser.UserQualified;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private void PromptFor2FailedUserTest()
        {
            var tests = GetNTests("*", 2);
            // current failed test already insert into the db, tests[0],
            // check the most recent last one, i.e.  tests[1]
            if (tests.Count == 2 && tests[1].TestStatus == QCTest.TestFail && tests[0].UserName != tests[1].UserName) 
            {
                // both user should be qualified user
                if (BothQualifiedUser(tests[1].UserName, tests[0].UserName))
                    Services.Dialogs.ShowAlert($"2 consecutive QC test failed, please check User Manual or contact Customer Service", "Quality Control Error", "Close");
            }
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
            newUser1.C1 = 20;
            newUser1.C1Date = DateTime.Now.AddHours(-120);
            newUser1.C2 = 30;
            newUser1.C2Date = DateTime.Now.AddHours(-96);
            newUser1.C3 = 25;
            newUser1.C3Date = DateTime.Now.AddHours(-72);
            DbUpdateQcUser(newUser1);

            var newTest1 = DbCreateQcTest(newUser1.UserName, 20);
            newTest1.TestDate = DateTime.Now.AddHours(-120);
            DbUpdateQcTest(newTest1);
            var newTest2 = DbCreateQcTest(newUser1.UserName, 30);
            newTest2.TestDate = DateTime.Now.AddHours(-96); ;
            DbUpdateQcTest(newTest2);
            var newTest3 = DbCreateQcTest(newUser1.UserName, 25);
            newTest3.TestDate = DateTime.Now.AddHours(-72);
            DbUpdateQcTest(newTest3);

            var newTest4 = DbCreateQcTest(newUser1.UserName, 23);
            newTest4.TestDate = DateTime.Now.AddHours(-48);
            DbUpdateQcTest(newTest4);
            var newTest5 = DbCreateQcTest(newUser1.UserName, 29);
            newTest5.TestDate = DateTime.Now.AddHours(-24); ;
            DbUpdateQcTest(newTest5);
            var newTest6 = DbCreateQcTest(newUser1.UserName, 26);
            newTest6.TestDate = DateTime.Now;
            DbUpdateQcTest(newTest6);

            float? median = GetMedian(newUser1.UserName);
            newUser1.QCT = median;

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

            //--------------------------------

            var newUser5 = DbCreateQcUser("Scott");
            newUser5.CurrentStatus = QCUser.UserQualified;
            newUser5.ExpiresDate = DateTime.Now.AddDays(7);
            newUser5.NextTestDate = DateTime.Now.AddHours(16);
            newUser5.C1 = 20;
            newUser5.C1Date = DateTime.Now.AddHours(-120);
            newUser5.C2 = 30;
            newUser5.C2Date = DateTime.Now.AddHours(-96);
            newUser5.C3 = 25;
            newUser5.C3Date = DateTime.Now.AddHours(-72);
            newUser5.QCT = 25;
            DbUpdateQcUser(newUser5);

            newTest1 = DbCreateQcTest(newUser5.UserName, 20);
            newTest1.TestDate = DateTime.Now.AddHours(-120);
            DbUpdateQcTest(newTest1);
            newTest2 = DbCreateQcTest(newUser5.UserName, 30);
            newTest2.TestDate = DateTime.Now.AddHours(-96); ;
            DbUpdateQcTest(newTest2);
            newTest3 = DbCreateQcTest(newUser5.UserName, 25);
            newTest3.TestDate = DateTime.Now.AddHours(-72);
            DbUpdateQcTest(newTest3);
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

        [RelayCommand]
        private async void DeleteDevice(object parameter)
        {
            int index = (int)parameter;

            if (QcDeviceList[index - 1].DeviceSerialNumber == CurrentDeviceSerialNumber)
            {
                Services.Dialogs.ShowAlert("You cannot delete the current device.", "Current Device", "OK");
                return;
            }

            var result = await Services.Dialogs.ShowConfirmYesNo("Are you sure you wish to delete this device?", "Delete Device");
            
            if (result)
            {
                DbDeleteQcDevice(QcDeviceList[index - 1]);// SfDataGrid apparently is one based on the index
                UpdateQcDeviceList();
            }            
            
        }

        [RelayCommand]
        private async void DeleteUser(object parameter)
        {
            int index = (int)parameter;

            var result = await Services.Dialogs.ShowConfirmYesNo("Are you sure you wish to delete this user?", "Delete User");

            if (result)
            {
                // Delete User and tests
                DbDeleteQcUser(QcUserList[index - 1]); // SfDataGrid apparently is one based on the index
                UpdateQcUserList();
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
        private (float? min, float? max) GetRange(float? a, float? b, float? c)
        {
            float?[] numbers = {a, b, c};
            Array.Sort(numbers);
            return (numbers[0], numbers[2]);
        }

        private float? GetMedian(string UserName)
        {
            List<QCTest> tests = GetNTests(UserName, 3, false); // get first 3 tests
            float?[] numbers = {tests[0].TestValue, tests[1].TestValue,  tests[2].TestValue};
            Array.Sort(numbers);
            return numbers[1];
        }

        private int TimeSpanHours(DateTime t1, DateTime t2)
        {
            TimeSpan timeSpan = t1 - t2;
            return Math.Abs(timeSpan.Hours);
        }

        #endregion


        #region "Determine Status Routines"

        private void UpdateDeviceStatus()
        {
            // Returns: "Valid", "Failed", "Expired"
            if (QCUserTestResultString == QCTest.TestPass) 
            {
                var device = Services.DeviceService?.Current;
                device.ExtendDeviceValidity((short)24);
                Task.Delay(TimeSpan.FromMilliseconds(500)).ContinueWith(_=> 
                {
                    CurrentDeviceStatus = Services.DeviceService?.Current.GetDeviceQCStatus();
                    UpdateDeviceStatusOnDB();
                });
                return;
            }
            UpdateDeviceStatusOnDB();
        }
        private void UpdateDeviceStatusOnDB()
        {
            string deviceStatus = CurrentDeviceStatus;

            if (QCNegativeControl.CurrentStatus == QCUser.NegativeControlNone)
            {
                deviceStatus = QCDevice.DeviceExpired;
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
                deviceStatus = QCDevice.DeviceValid;
            }

            QCDevice.CurrentStatus = CurrentDeviceStatus;
            // No need to set QCDevice.ExpiresDate in case of device
            // No need to set QCDevice.NextTestDate in case of device

            DbUpdateQcDevice(QCDevice);
        }

        private readonly int NegativeControlMaxThreshold = 5;
        private readonly int NegativeControlTimeoutHours = 24;

        private void UpdateNegativeControlStatus()
        {
            // Returns: "Pass", "Fail", "Expired"

            string negativeControlStatus;

            QCTest negativeControlTest = GetLastNegativeControlTest();

            if (negativeControlTest != null)
            {
                int timeSpanHours = TimeSpanHours(DateTime.Now, negativeControlTest.TestDate);

                bool timeSpanOK = timeSpanHours <= NegativeControlTimeoutHours;

                if (timeSpanHours > NegativeControlTimeoutHours)
                {
                    negativeControlStatus = QCUser.NegativeControlExpired;
                }
                else if (Math.Abs((decimal)negativeControlTest.TestValue) >= NegativeControlMaxThreshold)
                {
                    negativeControlStatus = QCUser.NegativeControlFail;
                    // Debug.WriteLine("==== Current Device Status is " + CurrentDeviceStatus);
                    // when NC fails, need to get the Device QC status and update to DB
                    // it takes 7s from app get score to poll device status get fail status
                    Task.Delay(TimeSpan.FromMilliseconds(11000)).ContinueWith(_=> 
                    {
                        CurrentDeviceStatus = Services.DeviceService?.Current.GetDeviceQCStatus();
                        // Debug.WriteLine("===== Current Device Status is " + CurrentDeviceStatus);
                        UpdateDeviceStatusOnDB();
                    });
                }
                else
                {
                    negativeControlStatus = QCUser.NegativeControlPass;
                }

                QCNegativeControl.ExpiresDate = negativeControlTest.TestDate.AddHours(NegativeControlTimeoutHours);
                QCNegativeControl.NextTestDate = QCNegativeControl.ExpiresDate;
            }
            else
            {
                negativeControlStatus = QCUser.NegativeControlNone;
            }

            QCNegativeControl.CurrentStatus = negativeControlStatus;

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
            List<QCTest> tests = GetNTests(SelectedQcUser.UserName);

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

                    testValuesWithinRange = Math.Abs((decimal)(SelectedQcUser.C1 - SelectedQcUser.C2)) <= 10;

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

                    (float? min, float? max) = GetRange(tests[0].TestValue, tests[1].TestValue, tests[2].TestValue);
                    float? median = GetMedian(SelectedQcUser.UserName);
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
                    // For Qualified User, a latest fail QC test will not turn it into Disqualified
                    else if (SelectedQcUser.CurrentStatus == QCUser.UserQualified && tests[0].TestStatus == QCTest.TestFail 
                            && timeSpanQualificationGood && lastTestTimeSpanGood)
                    { 
                        userStatus = QCUser.UserQualified;
                    }
                    else 
                        userStatus = QCUser.UserDisqualified;
                    SelectedQcUser.ShowChartOption = true;
                    break;

                default:
                    if (SelectedQcUser.CurrentStatus == QCUser.UserQualified)
                    {
                        // get median from first 3 records
                        Debug.Assert(SelectedQcUser.QCT > 0);

                        QCTest lastTest = tests[0];

                        // ToDo: Calculate result based on QCT
                        decimal deltaValue = Math.Abs((decimal)(SelectedQcUser.QCT - lastTest.TestValue));

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
            QcUserList = ReadAllQcUsers();
            
            DateTime lastTestDate = DateTime.MinValue;

            foreach (var user in QcUserList)
            {
                var tests = GetNTests(user.UserName).ToList();

                if (tests.Count <= 0)
                    continue;

                if (tests[0].TestDate > lastTestDate)
                    lastTestDate = tests[0].TestDate;
            }

            if (lastTestDate != DateTime.MinValue)
            {
                bool lastTestDateOK = TimeSpanHours(DateTime.Now, lastTestDate) <= 24;

                foreach (var user in QcUserList)
                {
                    if (user.CurrentStatus is QCUser.UserConditionallyQualified or QCUser.UserQualified && lastTestDateOK)
                        return true;
                }
            }

            return false;
        }


        private (DateTime? lastTestDate, DateTime? last2ndTestDate) GetLast2TestDate(string userName)
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

                    if (tests == null) return (null, null);

                    DateTime? lastTestDate = null, last2ndTestDate = null; 
                    if (tests[0].TestStatus == QCTest.TestPass)
                    {
                        last2ndTestDate = lastTestDate = tests[0].TestDate;
                        if (tests.Count>1)
                        {
                            bool pass2Tests = tests[1].TestStatus == QCTest.TestPass;
                            last2ndTestDate = tests[pass2Tests ? 1 : 0].TestDate;
                        }
                    }
                    return (lastTestDate, last2ndTestDate);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return (null, null);
            }
        }

        private List<QCTest> GetNTests(string userName, int count = 3, bool last = true)
        {
            Debug.Assert(!string.IsNullOrEmpty(CurrentDeviceSerialNumber) && !string.IsNullOrEmpty(userName));

            try
            {
                using (var db = new LiteDatabase(QCDatabasePath))
                {
                    // Get all user records for this device and user
                    var testCollection = db.GetCollection<QCTest>("qctests");
                    List<QCTest> tests;
                    if (userName == "*") 
                    {
                        tests = testCollection.Query()
                                .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName != QCUser.NegativeControlName )
                                .OrderByDescending(x => x.TestDate).ToList().Take(count).ToList();
                    }
                    else if (last)
                    {
                        tests = testCollection.Query()
                                .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == userName)
                                .OrderByDescending(x => x.TestDate).ToList().Take(count).ToList();
                    }
                    else
                    {
                        tests = testCollection.Query()
                                .Where(x => x.DeviceSerialNumber == CurrentDeviceSerialNumber && x.UserName == userName)
                                .OrderBy(x => x.TestDate).ToList().Take(count).ToList();
                    }
                    return tests;
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
        private string _chartTitle = string.Empty;

        [ObservableProperty] 
        private double _xMin;

        [ObservableProperty] 
        private double _xMax;

        [ObservableProperty] 
        private double _yMin;

        [ObservableProperty] 
        private double _yMax;

        [ObservableProperty] 
        private ChartSeriesCollection _seriesCollection;

        private ObservableCollection<ChartDataPoint> ChartData { get; set; }

        private ObservableCollection<ChartDataPoint> UpperBoundsData { get; set; }

        private ObservableCollection<ChartDataPoint> LowerBoundsData { get; set; }

        public void InitializeUserDataForChart(QCUser user)
        {
            ChartTitle = $"Test Data for {user.UserName}";

            ChartData = new ObservableCollection<ChartDataPoint>();

            ObservableCollection<QCTest> allUserTests = ReadUserQcTests(user.UserName);

            float? qct = user.QCT;
            XMin = 0;
            XMax = allUserTests.Count + 1;
            YMax = (double)(qct + 10 + 5);
            YMin = (double)(qct - 10 - 5);

            for (int i = 0; i <= allUserTests.Count - 1; i++)
            {
                ChartData.Add(new ChartDataPoint(i+1, (double)allUserTests[i].TestValue));
            }

            UpperBoundsData = new ObservableCollection<ChartDataPoint>
            {
                new ChartDataPoint(0, (double)(qct + 10)),
                new ChartDataPoint(XMax, (double)(qct + 10))
            };

            LowerBoundsData = new ObservableCollection<ChartDataPoint>
            {
                new ChartDataPoint(0, (double)(qct - 10)),
                new ChartDataPoint(XMax, (double)(qct - 10))
            };


            SeriesCollection = new ChartSeriesCollection();

            LineSeries testSeriesLine = new LineSeries()
            {
                ItemsSource = ChartData,
                XBindingPath = "XValue",
                YBindingPath = "YValue",
                Color = Color.LightBlue,
            };

            SeriesCollection.Add(testSeriesLine);

            ScatterSeries testSeriesPoints = new ScatterSeries()
            {
                ItemsSource = ChartData,
                XBindingPath = "XValue",
                YBindingPath = "YValue",
                Color = Color.Blue,
                StrokeColor = Color.Black,
                StrokeWidth = 1,
                ScatterHeight = 8,
                ScatterWidth = 8,
                ShapeType = ChartScatterShapeType.Ellipse
            };

            SeriesCollection.Add(testSeriesPoints);


            //ColumnSeries testSeries = new ColumnSeries()
            //{
            //    ItemsSource = UserTestData,
            //    XBindingPath = "XValue",
            //    YBindingPath = "YValue",
            //    Color = Color.LightBlue,
            //};

            //SeriesCollection.Add(testSeries);

            LineSeries upperBoundsSeries = new LineSeries()
            {
                ItemsSource = UpperBoundsData,
                XBindingPath = "XValue",
                YBindingPath = "YValue",
                Color = Color.Red,
                StrokeDashArray = new double[2] { 3, 3},
                StrokeWidth = 3
            };


            SeriesCollection.Add(upperBoundsSeries);


            LineSeries lowerBoundsSeries = new LineSeries()
            {
                ItemsSource = LowerBoundsData,
                XBindingPath = "XValue",
                YBindingPath = "YValue",
                Color = Color.Red,
                StrokeDashArray = new double[2] { 2, 3 },
                StrokeWidth = 3
            };

            SeriesCollection.Add(lowerBoundsSeries);
        }

        #endregion

    }
}
