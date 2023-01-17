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
    public class QCUsersRepository : GenericRepository<QCUsersTable>, IQQCUsersRepository
    {
        //private static string TblName = "QualityControlUsersRepository";
        //private ILiteCollection<QualityControlUsersTb> Collection => db.GetCollection<QualityControlUsersTb>(TblName);

        //public QualityControlUsersRepository() : base(TblName)
        //{

        //}

        //public QualityControlUsersRepository(LiteDatabase db) : base(TblName, db)
        //{

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        //public void Delete(QualityControlUsersDBModel model)
        //{
        //    Delete(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        //public void Delete(QualityControlUsersDataModel model)
        //{
        //    Delete(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public QualityControlUsersTb FindUser(string user)
        //{
        //    return this.Collection.FindOne(x => x.User.Equals(user));
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlUsersTb Insert(QualityControlUsersDBModel model)
        //{
        //    return Insert(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlUsersTb Insert(QualityControlUsersDataModel model)
        //{
        //    return Insert(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlUsersTb Update(QualityControlUsersDBModel model)
        //{
        //    return Update(model.Convert());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlUsersTb Update(QualityControlUsersDataModel model)
        //{
        //    return Update(model.Convert());
        //}

        ///// <summary>
        ///// Common Delete
        ///// </summary>
        ///// <param name="model"></param>
        //public void Delete(QualityControlUsersTb model)
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
        //public QualityControlUsersTb FindById(BsonValue _id)
        //{
        //    return this.Collection.FindOne(x => x._id == _id);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        
        //public QualityControlUsersTb Insert(QualityControlUsersTb model)
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
        //public IEnumerable<QualityControlUsersTb> SelectAll()
        //{
        //    return this.Collection.FindAll();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public QualityControlUsersTb Update(QualityControlUsersTb model)
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