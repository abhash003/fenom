using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Models;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace FenomPlus.ViewModels
{
    public partial class PastResultsViewModel : BaseViewModel
    {
        public ObservableCollection<BreathManeuverResultDataModel> PastResultsData;

        public PastResultsViewModel()
        {
            PastResultsData = new ObservableCollection<BreathManeuverResultDataModel>();
        }

        [RelayCommand]
        public void RefreshPastResults()
        {
            PastResultsData.Clear();

            IEnumerable<BreathManeuverResultTb> records = ResultsRepo.SelectAll();

            var sortedRecords = records.OrderByDescending(c => c.DateOfTest);

            foreach (BreathManeuverResultTb record in sortedRecords)
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
