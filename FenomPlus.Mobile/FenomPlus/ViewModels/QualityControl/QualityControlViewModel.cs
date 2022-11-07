using System.Collections.Generic;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.ViewModels
{
    public class QualityControlViewModel : BaseViewModel
    {
        public QualityControlViewModel()
        {
            DataForGrid = new RangeObservableCollection<QualityControlDataModel>();
            UpdateGrid();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateGrid()
        {
            DataForGrid.Clear();
            IEnumerable<QualityControlTb> records = QCRepo.SelectAll();
            foreach (QualityControlTb record in records)
            {
                AddToGrid(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        public void AddToGrid(QualityControlTb record)
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
            UpdateGrid();
            BleHub.StartTest(BreathTestEnum.Stop);
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
        private RangeObservableCollection<QualityControlDataModel> _DataForGrid;
        public RangeObservableCollection<QualityControlDataModel> DataForGrid
        {
            get => _DataForGrid;
            set
            {
                _DataForGrid = value;
                OnPropertyChanged("RecentErrorsData");
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
