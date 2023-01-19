using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Controls;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.ViewModels
{
    public partial class QualityControlViewModel : BaseViewModel
    {
        // ToDo: Load from device
        private readonly List<QCDeviceTable> QCDevices = new List<QCDeviceTable>();

        private readonly QualityControlTable QualityControlTable = new QualityControlTable();

        public QcButtonViewModel NegativeControlViewModel = new QcButtonViewModel();

        public QcButtonViewModel QcUser1ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser2ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser3ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser4ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser5ViewModel = new QcButtonViewModel();
        public QcButtonViewModel QcUser6ViewModel = new QcButtonViewModel();
        public ImageButtonViewModel ImageButtonViewModel = new ImageButtonViewModel();



        public QualityControlViewModel()
        {
            // ToDo: Read from database
            MockData();

            NegativeControlViewModel.Header = "Negative Control";
            QcUser1ViewModel.Header = "QC User 1";
            QcUser2ViewModel.Header = "QC User 2";
            QcUser3ViewModel.Header = "QC User 3";
            QcUser4ViewModel.Header = "QC User 4";
            QcUser5ViewModel.Header = "QC User 5";
            QcUser6ViewModel.Header = "QC User 6";

            ImageButtonViewModel.ImageName = "TutStep2";

            NegativeControlViewModel.Assigned = true; // Always
            QcUser1ViewModel.Assigned = true;
            QcUser2ViewModel.Assigned = true;
            QcUser3ViewModel.Assigned = true;
            QcUser4ViewModel.Assigned = false;
            QcUser5ViewModel.Assigned = false;
            QcUser6ViewModel.Assigned = false;
        }

        private void MockData()
        {
            string serial = "820022";

            var newDevice = AddDevice(serial);

            newDevice.NegativeControl = new QCNegativeControlTable();

            var userTable1 = new QCUserTable("Jim");
            userTable1.TestResults.Add(new QCResultTable(serial, "10", 20));
            userTable1.TestResults.Add(new QCResultTable(serial, "10", 30));
            userTable1.TestResults.Add(new QCResultTable(serial, "10", 25));
            newDevice.Users.Add(userTable1);

            var userTable2 = new QCUserTable("Bob");
            userTable2.TestResults.Add(new QCResultTable(serial, "10", 20));
            userTable2.TestResults.Add(new QCResultTable(serial, "10", 30));
            userTable2.TestResults.Add(new QCResultTable(serial, "10", 19));
            newDevice.Users.Add(userTable2);

            var userTable3 = new QCUserTable("Vinh");
            userTable3.TestResults.Add(new QCResultTable(serial, "10", 20));
            userTable3.TestResults.Add(new QCResultTable(serial, "10", 30));
            userTable3.TestResults.Add(new QCResultTable(serial, "10", 31));
            newDevice.Users.Add(userTable3);
        }


        public QCDeviceTable AddDevice(string serialNumber)
        {
            if (QCDevices == null)
                return null;

            var device = new QCDeviceTable(serialNumber);
            QCDevices.Add(device);
            return device;
        }

        public bool DeleteDevice(string serialNumber)
        {
            if (QCDevices == null)
                return false;

            for (int i = 0; i < QCDevices.Count - 1; i++)
            {
                if (QCDevices[i].SerialNumber == serialNumber)
                {
                    QCDevices.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private QCUserTable CurrentQcUser;

        private bool AddUser(string userName)
        {
            if (QualityControlTable.Users.Any(user => userName == user.Name))
            {
                // User already exists
                return false;
            }

            QCUserTable newUser = new QCUserTable(userName);
            QualityControlTable.Users.Add(newUser);
            CurrentQcUser = newUser;
            return true;
        }

        private bool DeleteUser(string userName)
        {
            for (int i = 0; i < QualityControlTable.Users.Count; i++)
            {
                if (userName == QualityControlTable.Users[i].Name)
                {
                    QualityControlTable.Users.RemoveAt(i);
                    CurrentQcUser = null;
                    return true;
                }
            }

            return false;
        }


        public QCResultTable LastResult()
        {
            if (CurrentQcUser.TestResults.Count <= 0)
                return null;

            return CurrentQcUser.TestResults[CurrentQcUser.TestResults.Count - 1];
        }

        public List<QCResultTable> LastFourResults()
        {
            if (CurrentQcUser.TestResults.Count <= 0)
                return null;

            List<QCResultTable> results = new List<QCResultTable>();

            for (int i = CurrentQcUser.TestResults.Count - 1; i >= 0; i--)
            {
                results.Add(CurrentQcUser.TestResults[i]);
            }

            return results;
        }




        public override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        private void LoadData()
        {
            // ToDo: finish
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
