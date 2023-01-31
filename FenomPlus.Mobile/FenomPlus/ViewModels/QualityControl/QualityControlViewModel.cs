using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Controls;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;
using FenomPlus.SDK.Core.Utils;
using FenomPlus.Services.DeviceService.Concrete;
using FenomPlus.ViewModels.QualityControl;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Plugin.BLE.Abstractions;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class QualityControlViewModel : BaseViewModel
    {

        //QcButtonViewModels[0] is the Negative Control, the other elements are users
        public List<QcButtonViewModel> QcButtonViewModels = new List<QcButtonViewModel>();

        public QCSettingsButtonViewModel ImageButtonViewModel = new QCSettingsButtonViewModel();

        private string CurrentDeviceSerialNumber = string.Empty;

        public string CurrentSerialNumber => $"Device Serial Number ({CurrentDeviceSerialNumber})";

        public QCUser CurrentQcUser;

        [ObservableProperty]
        private string _newUserName = string.Empty;

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

            InitializeUserButtonViewModels();
        }

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

        private void InitializeUserButtonViewModels()
        {
            Debug.Assert(QCNegativeControl != null); // Should always be assigned by this point

            QcButtonViewModels.Clear();

            //QcButtonViewModels[0] is the Negative Control, the other elements are users
            var negativeControl = new QcButtonViewModel(QCNegativeControl)
            {
                Assigned = true,
                UserName = QCNegativeControl.UserName,
                CurrentStatus = QCNegativeControl.CurrentStatus,
                ExpiresDate = QCNegativeControl.ExpiresDate, //QCNegativeControl.ExpiresDate.ToString(Constants.PrettyDateFormatString, CultureInfo.CurrentCulture);
                NextTestDate = QCNegativeControl.NextTestDate //.ToString(Constants.PrettyHoursFormatString, CultureInfo.CurrentCulture);
            };

            QcButtonViewModels.Add(negativeControl);

            // Now assign users from the database
            for (int i = 0; i < 6; i++)
            {
                if (i <= QCUsers.Count - 1)
                {
                    var buttonViewModel = new QcButtonViewModel(QCUsers[i]);
                    QcButtonViewModels.Add(buttonViewModel);
                }
                else
                {
                    var buttonViewModel = new QcButtonViewModel(null);
                    QcButtonViewModels.Add(buttonViewModel);
                }
            }
        }

        [RelayCommand]
        private void UpdateNegativeControl()
        {
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
            if (QcButtonViewModels[1].Assigned)
            {
                // Open and run new breath test for this user
            }
            else
            {
                // Open user name dialog and create user, then open breath test view
                string userName = await Services.Dialogs.UserNamePromptAsync();

                if (!string.IsNullOrEmpty(userName))
                {
                    QcButtonViewModels[1].Assigned = true;
                    QcButtonViewModels[1].UserName = userName;

                    // Open and run new breath test for this user
                }
            }
        }

        [RelayCommand]
        private void UpdateUser2()
        {
            //QCUsers[1]
            //QcUser2ViewModel
        }

        [RelayCommand]
        private void UpdateUser3()
        {
            //QCUsers[2]
            //QcUser3ViewModel
        }

        [RelayCommand]
        private void UpdateUser4()
        {
            //QCUsers[3]
            //QcUser4ViewModel
        }

        [RelayCommand]
        private void UpdateUser5()
        {
            //QCUsers[4]
            //QcUser5ViewModel
        }

        [RelayCommand]
        private void UpdateUser6()
        {
            //QCUsers[5]
            //QcUser6ViewModel
        }

        [RelayCommand]
        private void ShowQCSettings()
        {

        }

        [ObservableProperty]
        private string _gaugeStatus;

        [ObservableProperty]
        private int _gaugeSeconds;

        [ObservableProperty]
        private float _gaugeData;

        partial void OnGaugeDataChanged(float value)
        {
            PlaySounds.PlaySound(GaugeData);
        }

        private void DoBreathTest()
        {
            Services.DeviceService.Current.BreathFlowChanged += Cache_BreathFlowChanged;

            //TestType = "10-second Test";
            //TestTime = 10;

            GaugeData = Services.DeviceService.Current.BreathFlow = 0;
            GaugeSeconds = 10;
            GaugeStatus = "Start Blowing";


            Services.DeviceService.Current.BreathFlowChanged -= Cache_BreathFlowChanged;
        }

        private async void Cache_BreathFlowChanged(object sender, EventArgs e)
        {
            GaugeData = Services.DeviceService.Current.BreathFlow;
            GaugeSeconds = Services.DeviceService.Current.BreathManeuver.TimeRemaining;

            if (GaugeSeconds <= 0)
            {
                if (Services.DeviceService.Current != null && Services.DeviceService.Current is BleDevice)
                {
                    await Services.DeviceService.Current.StopTest();
                }
                await Services.Navigation.StopExhalingView();
                return;
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

    }
}
