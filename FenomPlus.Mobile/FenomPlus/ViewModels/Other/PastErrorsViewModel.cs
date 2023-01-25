using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace FenomPlus.ViewModels
{
    public partial class PastErrorsViewModel : BaseViewModel
    {
        public ObservableCollection<BreathManeuverErrorDataModel> RecentErrorsData;

        public PastErrorsViewModel()
        {
            RecentErrorsData = new ObservableCollection<BreathManeuverErrorDataModel>();
        }

        [RelayCommand]
        public void RefreshRecentErrors()
        {
            RecentErrorsData.Clear();

            List<BreathManeuverErrorTb> records = ErrorsRepo.SelectAll().ToList();

            var sortedRecords = records.OrderByDescending(c => c.DateError);

            foreach (BreathManeuverErrorTb record in sortedRecords)
            {
                RecentErrorsData.Add(record.ConvertForGrid());
            }

            //InjectMockData();  //For debugging only!
        }

        private void InjectMockData()
        {
            //For debugging only!

            RecentErrorsData.Clear();

            int maxEntries = 160;

            // Create test data & add
            for (int i = 0; i < maxEntries; i++)
            {
                BreathManeuverErrorDataModel record = new BreathManeuverErrorDataModel
                {
                    ErrorCode = $"Error {i*10}",
                    Description = "Error of type {i*10}",
                    Humidity = "40",
                    DateError = DateTime.Now.AddDays(-(maxEntries - i)).ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture),
                    SerialNumber = "F150-00000022",
                    Firmware ="xxxx",
                    Software = "xxxx"
                };

                RecentErrorsData.Add(record.ConvertForGrid());
            }
        }        

        public override void OnAppearing()
        {
            base.OnAppearing();
            RefreshRecentErrors();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
