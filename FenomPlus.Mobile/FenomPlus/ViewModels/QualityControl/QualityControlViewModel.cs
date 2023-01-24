using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Controls;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;
using FenomPlus.ViewModels.QualityControl;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Plugin.BLE.Abstractions;

namespace FenomPlus.ViewModels
{
    public partial class QualityControlViewModel : BaseViewModel
    {
        public QcButtonViewModel QcNegativeControlViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser1ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser2ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser3ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser4ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser5ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser6ViewModel = new QcButtonViewModel();
        public QCSettingsButtonViewModel ImageButtonViewModel = new QCSettingsButtonViewModel();

        public string CurrentSerialNumber => $"Device Serial Number ({Services?.Cache?.DeviceSerialNumber})";

        public QCUser CurrentQcUser;


        public List<QCUser> AllDevices => GetAllQcDevices();
        public List<QCUser> AllUsers => ReadAllQcUsers(Services?.Cache?.DeviceSerialNumber);
        public List<QCTest> AllTests => ReadAllQcTests(Services?.Cache?.DeviceSerialNumber, CurrentQcUser.UserName);

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
                Services.Dialogs.ShowAlert("You must have a connection to a device to continue.","Device Not Connected", "OK");
                Services.Navigation.DashboardView();
                return;
            }

            WipeDataBase(); // ToDo: Fro debugging only

            // Get currently connected device's status or create a new one
            QCDevice = ReadQcDevice(Services?.Cache?.DeviceSerialNumber);

            // Get currently connected device's negative control or create one
            QCNegativeControl = ReadQcNegativeControl(Services?.Cache?.DeviceSerialNumber);

            // Get all users for this currently connected device
            QCUsers = ReadAllQcUsers(Services?.Cache?.DeviceSerialNumber);

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
                var newTest = new QCTest(Services.Cache.DeviceSerialNumber, userName, DateTime.Now, result);

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
            if (QCNegativeControl != null)
            {
                QcNegativeControlViewModel.Assigned = true;
                QcNegativeControlViewModel.Title = QCNegativeControl.UserName;
                QcNegativeControlViewModel.Status = QCNegativeControl.CurrentStatus;
                QcNegativeControlViewModel.Expires = QCNegativeControl.ExpiresDate.ToString(Constants.PrettyDateFormatString, CultureInfo.CurrentCulture);
                QcNegativeControlViewModel.NextTest = QCNegativeControl.NextTestDate.ToString(Constants.PrettyHoursFormatString, CultureInfo.CurrentCulture);
            }


            if (QCUsers.Count <= 0)
            {
                QcUser1ViewModel.Assigned = false;
                QcUser2ViewModel.Assigned = false;
                QcUser3ViewModel.Assigned = false;
                QcUser4ViewModel.Assigned = false;
                QcUser5ViewModel.Assigned = false;
                QcUser6ViewModel.Assigned = false;
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    var user = QCUsers[i];

                    switch (i)
                    {
                        case 0:
                            if (i < QCUsers.Count - 1)
                            {
                                QcUser1ViewModel.Assigned = true;
                                QcUser1ViewModel.DeviceSerialNumber = user.DeviceSerialNumber;
                                QcUser1ViewModel.Title = user.UserName;
                                QcUser1ViewModel.Status = user.CurrentStatus;
                                QcUser1ViewModel.Expires = user.ExpiresDate.ToString(Constants.PrettyDateFormatString);
                                QcUser1ViewModel.NextTest = user.NextTestDate.ToString(Constants.PrettyHoursFormatString);
                            }
                            else
                            {
                                QcUser1ViewModel.Assigned = false;
                            }

                            break;

                        case 1:
                            if (i < QCUsers.Count - 1)
                            {
                                QcUser2ViewModel.Assigned = true;
                                QcUser2ViewModel.DeviceSerialNumber = user.DeviceSerialNumber;
                                QcUser2ViewModel.Title = user.UserName;
                                QcUser2ViewModel.Status = user.CurrentStatus;
                                QcUser2ViewModel.Expires = user.ExpiresDate.ToString(Constants.PrettyDateFormatString);
                                QcUser2ViewModel.NextTest = user.NextTestDate.ToString(Constants.PrettyHoursFormatString);
                            }
                            else
                            {
                                QcUser2ViewModel.Assigned = false;
                            }


                            break;

                        case 2:
                            if (i < QCUsers.Count - 1)
                            {
                                QcUser3ViewModel.Assigned = true;
                                QcUser3ViewModel.DeviceSerialNumber = user.DeviceSerialNumber;
                                QcUser3ViewModel.Title = user.UserName;
                                QcUser3ViewModel.Status = user.CurrentStatus;
                                QcUser3ViewModel.Expires = user.ExpiresDate.ToString(Constants.PrettyDateFormatString);
                                QcUser3ViewModel.NextTest = user.NextTestDate.ToString(Constants.PrettyHoursFormatString);
                            }
                            else
                            {
                                QcUser3ViewModel.Assigned = false;
                            }

                            break;

                        case 3:
                            if (i < QCUsers.Count - 1)
                            {
                                QcUser4ViewModel.Assigned = true;
                                QcUser4ViewModel.DeviceSerialNumber = user.DeviceSerialNumber;
                                QcUser4ViewModel.Title = user.UserName;
                                QcUser4ViewModel.Status = user.CurrentStatus;
                                QcUser4ViewModel.Expires = user.ExpiresDate.ToString(Constants.PrettyDateFormatString);
                                QcUser4ViewModel.NextTest = user.NextTestDate.ToString(Constants.PrettyHoursFormatString);
                            }
                            else
                            {
                                QcUser4ViewModel.Assigned = false;
                            }


                            break;

                        case 4:
                            if (i < QCUsers.Count - 1)
                            {
                                QcUser5ViewModel.Assigned = true;
                                QcUser5ViewModel.DeviceSerialNumber = user.DeviceSerialNumber;
                                QcUser5ViewModel.Title = user.UserName;
                                QcUser5ViewModel.Status = user.CurrentStatus;
                                QcUser5ViewModel.Expires = user.ExpiresDate.ToString(Constants.PrettyDateFormatString);
                                QcUser5ViewModel.NextTest = user.NextTestDate.ToString(Constants.PrettyHoursFormatString);
                            }
                            else
                            {
                                QcUser5ViewModel.Assigned = false;
                            }


                            break;

                        case 5:
                            if (i < QCUsers.Count - 1)
                            {
                                QcUser6ViewModel.Assigned = true;
                                QcUser6ViewModel.DeviceSerialNumber = user.DeviceSerialNumber;
                                QcUser6ViewModel.Title = user.UserName;
                                QcUser6ViewModel.Status = user.CurrentStatus;
                                QcUser6ViewModel.Expires = user.ExpiresDate.ToString(Constants.PrettyDateFormatString);
                                QcUser6ViewModel.NextTest = user.NextTestDate.ToString(Constants.PrettyHoursFormatString);
                            }
                            else
                            {
                                QcUser6ViewModel.Assigned = false;
                            }


                            break;

                        default: break;
                    }
                }
            }
        }

        private void MockData()
        {


            //var negativeControlTable = new QCNegativeControlTable();

            //var userTable1 = new QCUserTable(serial, "Jim");
            //userTable1.TestResults.Add(new QCResultTable(serial, "10", 20));
            //userTable1.TestStatus
            //    userTable1.TestDate




            //userTable1.TestResults.Add(new QCResultTable(serial, "10", 30));
            //userTable1.TestResults.Add(new QCResultTable(serial, "10", 25));
            //newDevice.Users.Add(userTable1);

            //var userTable2 = new QCUserTable(serial, "Bob");
            //userTable2.TestResults.Add(new QCResultTable(serial, "10", 20));
            //userTable2.TestResults.Add(new QCResultTable(serial, "10", 30));
            //userTable2.TestResults.Add(new QCResultTable(serial, "10", 19));
            //newDevice.Users.Add(userTable2);

            //var userTable3 = new QCUserTable(serial, "Vinh");
            //userTable3.TestResults.Add(new QCResultTable(serial, "10", 20));
            //userTable3.TestResults.Add(new QCResultTable(serial, "10", 30));
            //userTable3.TestResults.Add(new QCResultTable(serial, "10", 31));
            //newDevice.Users.Add(userTable3);
        }
    }
}
