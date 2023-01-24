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


        public List<QCDevice> AllDevices => GetAllDevices();
        public List<QCUser> AllUsers => GetAllUsers();
        public List<QCTest> AllTests => GetAllTests();

        private readonly string QCDeviceRecordsPath;
        private readonly string QCUserRecordsPath;
        private readonly string QCTestRecordsPath;

        public QualityControlViewModel()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            QCDeviceRecordsPath = Path.Combine(localFolder, @"QCDeviceRecords.db");
            QCUserRecordsPath = Path.Combine(localFolder, @"QCUserRecords.db");
            QCTestRecordsPath = Path.Combine(localFolder, @"QCTestRecords.db");

            // ToDo: Read from database
            MockData();
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



        // Required for QualityControlView (Hub)
        private QCDevice QCDevice;
        private QCUser QCNegativeControl;
        private List<QCUser> QCUsers;

        private bool CheckDeviceConnection()
        {
            if (Services?.DeviceService?.Current == null)
                return false;

            bool deviceIsConnected = Services.DeviceService.Current.Connected;

            // Don't use Services.BleHub.IsConnected() or it will try to reconnect - we just want current connection status
            return deviceIsConnected;
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

            WipeDataTables();

            // Get currently connected device's status or create a new one
            QCDevice = GetDeviceQCInfo();

            // Get currently connected device's negative control or create one
            QCNegativeControl = GetNegativeControl();

            // Get all users for this currently connected device
            QCUsers = GetAllDeviceUsers();

            InitializeUserButtonViewModels();
        }

        private void WipeDataTables()
        {
            using (var db = new LiteDatabase(QCDeviceRecordsPath))
            {
                var deviceCollection = db.GetCollection<QCDevice>("qcdevices");
                deviceCollection.DeleteAll();
            }

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

        private void DeleteDevice()
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Delete the QC Device record
            using var db = new LiteDatabase(QCDeviceRecordsPath);

            var deviceCollection = db.GetCollection<QCDevice>("qcdevices");

            var qcDevice = deviceCollection.FindOne(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber);

            if (qcDevice != null)
            {
                deviceCollection.Delete(qcDevice.DeviceId);
            }

            // Delete the QC Negative Control Record

            var usersCollection = db.GetCollection<QCUser>("qcusers");

            var qcNegativeControl = usersCollection.Query()
                .Where(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == "Negative Control")
                .ToList();

            // Delete all QC Users for this device
            if (qcNegativeControl != null)
            {
                // ToDo: Implement
                //usersCollection.Delete(qcNegativeControl);
            }

            // Delete all associated tests
            DeleteAllDeviceTests();
        }

        #region "QC Device"

        private QCDevice GetDeviceQCInfo()
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return null;
            }

            // Only ONE in the collection
            QCDevice qcDevice = null;

            using var db = new LiteDatabase(QCDeviceRecordsPath);
            var deviceCollection = db.GetCollection<QCDevice>("qcdevices");

            qcDevice = deviceCollection.FindOne(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber);

            // If it doesn't exist for this device, create one
            //if (qcDevice == null)
            //{
            //    qcDevice = new QCDevice(Services.Cache.DeviceSerialNumber, "Insufficient Data");
            //    deviceCollection.Insert(qcDevice);
            //    deviceCollection.EnsureIndex(x => x.DeviceSerialNumber); // ToDo: Do I need this?
            //}

            return qcDevice;
        }

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

        private void UpdateDeviceQCStatus(string status)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Only ONE in the collection

            using var db = new LiteDatabase(QCDeviceRecordsPath);
            var deviceCollection = db.GetCollection<QCDevice>("qcdevices");

            var qcDevice = deviceCollection.FindOne(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber);

            if (qcDevice != null)
            {
                qcDevice.DeviceStatus = status;
                deviceCollection.Update(qcDevice);
            }
        }

        private List<QCDevice> GetAllDevices()
        {
            using var db = new LiteDatabase(QCDeviceRecordsPath);
            var deviceCollection = db.GetCollection<QCDevice>("qcdevices");

            var devices = deviceCollection.FindAll()
                .OrderBy(x => x.DeviceSerialNumber)
                .ToList();

            return devices;
        }

        #endregion

        #region "QC Negative Control"

        // Gets existing Negative Control or creates one based on device serial number
        private QCUser GetNegativeControl()
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return null;
            }

            QCUser qcNegativeControl = null;

            // Only ONE in the collection
            using (var db = new LiteDatabase(QCUserRecordsPath))
            {
                var usersCollection = db.GetCollection<QCUser>("qcusers");

                var users = usersCollection.Query()
                    .Where(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == "Negative Control")
                    .OrderBy(x => x.UserName)
                    .ToList();

                if (users.Count <= 0)
                {
                    qcNegativeControl = new QCUser(Services.Cache.DeviceSerialNumber, "Negative Control", "None", DateTime.Now, DateTime.MinValue, DateTime.MinValue);
                    usersCollection.Insert(qcNegativeControl);
                    usersCollection.EnsureIndex(x=> x.DeviceSerialNumber);
                }
                else
                {
                    qcNegativeControl = users[0];
                }
            }


            return qcNegativeControl;
        }

        private void UpdateNegativeControl(string status, DateTime expiresDate, DateTime nextTestDate)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Only ONE in the collection

            using var db = new LiteDatabase(QCUserRecordsPath);
            var usersCollection = db.GetCollection<QCUser>("qcusers");

            var qcNegativeControl = usersCollection.FindOne(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == "Negative Control");

            if (qcNegativeControl != null)
            {
                qcNegativeControl.CurrentStatus = status;
                qcNegativeControl.ExpiresDate = expiresDate;
                qcNegativeControl.NextTestDate = nextTestDate;
                usersCollection.Update(qcNegativeControl);
            }
        }

        private void DeleteNegativeControl()
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Only ONE in the collection

            using var db = new LiteDatabase(QCUserRecordsPath);
            var usersCollection = db.GetCollection<QCUser>("qcusers");

            var user = usersCollection.FindOne(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == "Negative Control");
            usersCollection.Delete(user.UserId);
        }

        #endregion

        #region "QC Users"

        private List<QCUser> GetAllDeviceUsers()
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return new List<QCUser>();
            }

            // Multiples in the collection
            List<QCUser> users = null;

            using var db = new LiteDatabase(QCUserRecordsPath);
            var usersCollection = db.GetCollection<QCUser>("qcusers");

            users = usersCollection.Query()
                .Where(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName != "Negative Control")
                .OrderBy(x => x.UserName)
                .ToList();

            //if (users == null)
            //{
            //    users = new List<QCUser>();
            //    usersCollection.Insert(users);
            //    usersCollection.EnsureIndex(x => x.DeviceSerialNumber); // ToDo: Do I need this?
            //}

            //if (users.Count <= 0)
            //{
            //    qcNegativeControl = new QCUser(Services.Cache.DeviceSerialNumber, "Negative Control", "None", DateTime.Now, DateTime.MinValue, DateTime.MinValue);
            //    usersCollection.Insert(qcNegativeControl);
            //}
            //else
            //{
            //    qcNegativeControl = users[0];
            //}

            return users;
        }

        private QCUser AddDeviceUser(string userName)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return null;
            }

            // Only ONE in the collection

            using var db = new LiteDatabase(QCUserRecordsPath);
            var usersCollection = db.GetCollection<QCUser>("qcusers");

            var newUser = new QCUser(Services.Cache.DeviceSerialNumber, userName, "None", DateTime.Now, DateTime.MinValue, DateTime.MinValue);
            usersCollection.Insert(newUser);

            return newUser;
        }

        private void UpdateDeviceUser(string userName, string status, DateTime expiresDate, DateTime nextTestDate)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Only ONE in the collection

            using var db = new LiteDatabase(QCUserRecordsPath);
            var usersCollection = db.GetCollection<QCUser>("qcusers");

            var user = usersCollection.FindOne(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == userName);

            if (user != null)
            {
                user.CurrentStatus = status;
                user.ExpiresDate = expiresDate;
                user.NextTestDate = nextTestDate;
                usersCollection.Update(user);
            }
        }

        private void DeleteDeviceUser(string userName)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Only ONE in the collection

            using var db = new LiteDatabase(QCUserRecordsPath);
            var usersCollection = db.GetCollection<QCUser>("qcusers");

            var user = usersCollection.FindOne(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == userName);
            usersCollection.Delete(user.UserId);
        }

        private List<QCUser> GetAllUsers()
        {
            using var db = new LiteDatabase(QCUserRecordsPath);
            var usersCollection = db.GetCollection<QCUser>("qcusers");

            var users = usersCollection.FindAll()
                .OrderBy(x => x.UserName)
                .ToList();

            return users;
        }

        #endregion

        #region "QCTests"

        private List<QCTest> GetTests(string userName)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return new List<QCTest>();
            }

            // Multiples in the collection
            List<QCTest> tests = null;

            using var db = new LiteDatabase(QCTestRecordsPath);
            var testsCollection = db.GetCollection<QCTest>("qctests");

            if (testsCollection.Count() > 0)
            {
                tests = testsCollection.Query()
                    .Where(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == userName)
                    .OrderBy(x => x.TestDate)
                    .ToList();
            }

            if (tests == null)
            {
                tests = new List<QCTest>();
                testsCollection.Insert(tests);
                testsCollection.EnsureIndex(x => x.DeviceSerialNumber); // ToDo: Do I need this?
            }

            return tests;
        }

        private void AddTest(string userName, string status, int result)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Multiples in the collection

            using var db = new LiteDatabase(QCTestRecordsPath);
            var testsCollection = db.GetCollection<QCTest>("qctests");

            var newTest = new QCTest(Services.Cache.DeviceSerialNumber, userName, DateTime.Now, result);
            testsCollection.Insert(newTest);
        }

        private void DeleteAllUserTests(string userName)
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Multiples in the collection

            using var db = new LiteDatabase(QCTestRecordsPath);
            var testsCollection = db.GetCollection<QCTest>("qctests");

            var tests = testsCollection.Query()
                .Where(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber && x.UserName == userName)
                .ToList();

            // ToDo: Implement
            //testsCollection.DeleteMany(tests);
        }

        private void DeleteAllDeviceTests()
        {
            if (string.IsNullOrEmpty(Services?.Cache?.DeviceSerialNumber))
            {
                // ToDo: alert user
                return;
            }

            // Multiples in the collection

            using var db = new LiteDatabase(QCTestRecordsPath);
            var testsCollection = db.GetCollection<QCTest>("qctests");

            var tests = testsCollection.Query()
                .Where(x => x.DeviceSerialNumber == Services.Cache.DeviceSerialNumber)
                .ToList();

            // ToDo: Implement
            //testsCollection.DeleteMany(tests);
        }

        private List<QCTest> GetAllTests()
        {
            using var db = new LiteDatabase(QCTestRecordsPath);
            var testsCollection = db.GetCollection<QCTest>("qctests");

            var tests = testsCollection.FindAll()
                .OrderBy(x => x.TestDate)
                .ToList();

            return tests;
        }

        #endregion
    }
}
