using System.Collections.Generic;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public class QualityControlDevicesViewModel : BaseViewModel
    {
        public QualityControlDevicesViewModel()
        {
            DataForGrid = new RangeObservableCollection<QualityControlDevicesDataModel>();
            UpdateGrid();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateGrid()
        {
            DataForGrid.Clear();
            IEnumerable<QualityControlDevicesTb> records = QCDevicesRepo.SelectAll();
            foreach (QualityControlDevicesTb record in records)
            {
                AddToGrid(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        public void AddToGrid(QualityControlDevicesTb record)
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
        private RangeObservableCollection<QualityControlDevicesDataModel> _DataForGrid;
        public RangeObservableCollection<QualityControlDevicesDataModel> DataForGrid
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
