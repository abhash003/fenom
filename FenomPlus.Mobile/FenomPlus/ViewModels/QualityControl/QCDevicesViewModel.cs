using System.Collections.Generic;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public class QCDevicesViewModel : BaseViewModel
    {
        public QCDevicesViewModel()
        {
            //DataForGrid = new RangeObservableCollection<QualityControlDevicesDataModel>();
            //UpdateGrid();
        }

        /// <summary>
        /// 
        /// </summary>
        //public void UpdateGrid()
        //{
        //    //DataForGrid.Clear();
        //    //IEnumerable<QCDeviceTable> records = QCDevicesRepo.SelectAll();
        //    //foreach (QCDeviceTable record in records)
        //    //{
        //    //    AddToGrid(record);
        //    //}
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        //public void AddToGrid(QCDeviceTable record)
        //{
        //    //if (record != null)
        //    //{
        //    //    DataForGrid.Add(record.ConvertForGrid());
        //    //}
        //}

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            //UpdateGrid();
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
        //private RangeObservableCollection<QualityControlDevicesDataModel> _DataForGrid;
        //public RangeObservableCollection<QualityControlDevicesDataModel> DataForGrid
        //{
        //    get => _DataForGrid;
        //    set
        //    {
        //        _DataForGrid = value;
        //        OnPropertyChanged("RecentErrorsData");
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
