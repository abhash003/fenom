using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public partial class ViewRecentErrorsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private RangeObservableCollection<BreathManeuverErrorDataModel> _recentErrorsData;

        public ViewRecentErrorsViewModel()
        {
            RecentErrorsData = new RangeObservableCollection<BreathManeuverErrorDataModel>();
            UpdateRecentErrorsData();
        }


        [RelayCommand]
        public void UpdateRecentErrorsData()
        {
            RecentErrorsData.Clear();

            IEnumerable<BreathManeuverErrorTb> records = ErrorsRepo.SelectAll();

            foreach (BreathManeuverErrorTb record in records)
            {
                RecentErrorsData.Add(record.ConvertForGrid());
            }

            InjectMockData();  //For debugging only!
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
                    DateError = DateTime.Now.AddDays(-(maxEntries - i)).ToString(Constants.DateTimeFormatString),
                    SerialNumber = "F150-00000022",
                    Firmware ="xxxx",
                    Software = "xxxx"
                };

                RecentErrorsData.Add(record.ConvertForGrid());
            }
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
