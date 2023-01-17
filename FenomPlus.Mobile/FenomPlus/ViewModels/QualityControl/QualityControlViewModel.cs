using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using FenomPlus.Controls;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.ViewModels
{
    public class QualityControlViewModel : BaseViewModel
    {
        // ToDo: Load from device
        private Database.Tables.QualityControlTable QCViewTable = new Database.Tables.QualityControlTable();


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

            // Add three dummy users

            var userTable = new QCUsersTable("Jim");
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 20));
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 30));
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 25));
            QCViewTable.Users.Add(userTable);

            userTable = new QCUsersTable("Bob");
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 20));
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 30));
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 19));
            QCViewTable.Users.Add(userTable);

            userTable = new QCUsersTable("Vinh");
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 20));
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 30));
            userTable.TestResults.Add(new QCTestResultsTable(DateTime.Now, serial, "10", 31));
            QCViewTable.Users.Add(userTable);

        }

        private QCUsersTable CurrentQCUser;

        private bool AddUser(string userName)
        {
            if (QCViewTable.Users.Any(user => userName == user.UserName))
            {
                // User already exists
                return false;
            }

            QCUsersTable newUser = new QCUsersTable(userName);
            QCViewTable.Users.Add(newUser);
            CurrentQCUser = newUser;
            return true;
        }

        private bool DeleteUser(string userName)
        {
            for (int i = 0; i < QCViewTable.Users.Count; i++)
            {
                if (userName == QCViewTable.Users[i].UserName)
                {
                    QCViewTable.Users.RemoveAt(i);
                    CurrentQCUser = null;
                    return true;
                }
            }

            return false;
        }


        public QCTestResultsTable LastResult()
        {
            if (CurrentQCUser.TestResults.Count <= 0)
                return null;

            return CurrentQCUser.TestResults[CurrentQCUser.TestResults.Count - 1];
        }

        public List<QCTestResultsTable> LastFourResults()
        {
            if (CurrentQCUser.TestResults.Count <= 0)
                return null;

            List<QCTestResultsTable> results = new List<QCTestResultsTable>();

            for (int i = CurrentQCUser.TestResults.Count - 1; i >= 0; i--)
            {
                results.Add(CurrentQCUser.TestResults[i]);
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
