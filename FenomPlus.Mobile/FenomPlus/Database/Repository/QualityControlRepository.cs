using System;
using System.Collections.Generic;
using FenomPlus.Core.Database.Repository;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Database.Tables;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using LiteDB;

namespace FenomPlus.Database.Repository
{
    public class QualityControlRepository : GenericRepository<QualityControlTable>, IQualityControlRepository
    {
        //private static string TblName = "QualityControlRepository";
        //private ILiteCollection<QualityControlTb> Collection => db.GetCollection<QualityControlTb>(TblName);

        //public QualityControlRepository() : base(TblName)
        //{

        //}

        //public QualityControlRepository(LiteDatabase db) : base(TblName, db)
        //{

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        //public void Delete(QualityControlDBModel model)
        //{
        //    Delete(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        //public void Delete(QualityControlDataModel model)
        //{
        //    Delete(model.Convert());
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name=""></param>
        ///// <returns></returns>
        //public QualityControlTb FindDevice(string serialNumber)
        //{
        //    return this.Collection.FindOne(x => x.SerialNumber.Equals(serialNumber));
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlTb Insert(QualityControlDBModel model)
        //{
        //    return Insert(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlTb Insert(QualityControlDataModel model)
        //{
        //    return Insert(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlTb Update(QualityControlDBModel model)
        //{
        //    return Update(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlTb Update(QualityControlDataModel model)
        //{
        //    return Update(model.Convert());
        //}

        ///// <summary>
        ///// Common Delete
        ///// </summary>
        ///// <param name="model"></param>
        //public void Delete(QualityControlTb model)
        //{
        //    try
        //    {
        //        if (model != null)
        //        {
        //            this.Collection.Delete(model._id);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Services.LogCat.Print(ex);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public void DeleteAll()
        //{
        //    try
        //    {
        //        this.Collection.DeleteAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        Services.LogCat.Print(ex);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public QualityControlTb FindById(BsonValue _id)
        //{
        //    return this.Collection.FindOne(x => x._id == _id);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        
        //public QualityControlTb Insert(QualityControlTb model)
        //{
        //    if (model != null)
        //    {
        //        try
        //        {
        //            if (FindById(model._id) == null)
        //            {
        //                this.Collection.Insert(model);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Services.LogCat.Print(ex);
        //        }
        //    }
        //    return model;
        //}
        
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<QualityControlTb> SelectAll()
        //{
        //    return this.Collection.FindAll();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlTb Update(QualityControlTb model)
        //{
        //    if (model != null)
        //    {
        //        try
        //        {
        //            if (FindById(model._id) != null)
        //            {
        //                this.Collection.Update(model);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Services.LogCat.Print(ex);
        //        }
        //    }
        //    return model;
        //}
    }
}