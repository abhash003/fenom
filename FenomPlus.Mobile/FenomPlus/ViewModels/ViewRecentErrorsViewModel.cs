using System.Collections.Generic;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public class ViewRecentErrorsViewModel : BaseViewModel
    {
        public ViewRecentErrorsViewModel()
        {
            DataForGrid = new RangeObservableCollection<BreathManeuverErrorDataModel>();
            UpdateGrid();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateGrid()
        {
            DataForGrid.Clear();
            IEnumerable<BreathManeuverErrorTb> records = ErrorsRepo.SelectAll();
            foreach (BreathManeuverErrorTb record in records)
            {
                AddToGrid(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        public void AddToGrid(BreathManeuverErrorTb record)
        {
            if (record != null)
            {
                DataForGrid.Add(record.ConvertForGrid());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        private RangeObservableCollection<BreathManeuverErrorDataModel> _DataForGrid;
        public RangeObservableCollection<BreathManeuverErrorDataModel> DataForGrid
        {
            get => _DataForGrid;
            set
            {
                _DataForGrid = value;
                OnPropertyChanged("DataForGrid");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
