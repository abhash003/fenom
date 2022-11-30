using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public partial class PastResultsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private List<BreathManeuverResultDataModel> _pastResultsData;

        public PastResultsViewModel()
        {
            PastResultsData = new List<BreathManeuverResultDataModel>();
        }

        [RelayCommand]
        public void UpdatePastResultsData()
        {
            PastResultsData.Clear();

            IEnumerable<BreathManeuverResultTb> records = ResultsRepo.SelectAll();

            foreach (BreathManeuverResultTb record in records)
            {
                PastResultsData.Add(record.ConvertForGrid());
            }

            //InjectMockData(); //For debugging only!
        }

        private void InjectMockData()
        {
            //For debugging only!

            PastResultsData.Clear();

            int maxEntries = 160;

            // Create test data & add
            for (int i = 0; i < maxEntries; i++)
            {
                BreathManeuverResultTb record = new BreathManeuverResultTb
                {
                    SerialNumber = "F150-00000022",
                    TestType = i % 2 == 0 ? "Standard" : "Short",
                    DateOfTest = DateTime.Now.AddDays(-(maxEntries - i)).ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture),
                    QCStatus = "?"
                };

                Random random = new Random();
                double minNumber = 25;
                double maxNumber = 45;
                record.TestResult = (random.NextDouble() * (maxNumber - minNumber) + minNumber).ToString("N0"); ;

                PastResultsData.Add(record.ConvertForGrid());
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            UpdatePastResultsData();
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
