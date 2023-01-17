using System.Collections.Generic;
using FenomPlus.Database.Tables;
using FenomPlus.Models;

namespace FenomPlus.Database.Repository.Interfaces
{
    public interface IQualityControlRepository : IGenericRepository<QCViewRepository>
    {
        //// inserts
        //QualityControlTb Insert(QualityControlDBModel model);
        //QualityControlTb Insert(QualityControlDataModel model);

        //// delete
        //void Delete(QualityControlDBModel model);
        //void Delete(QualityControlDataModel model);

        //// updates
        //QualityControlTb Update(QualityControlDBModel model);
        //QualityControlTb Update(QualityControlDataModel model);

        //// select
        //QualityControlTb FindDevice(string serialNumber);

        //IEnumerable<QualityControlTb> SelectAll();
    }
}
