﻿using System;
using System.Collections.Generic;
using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Database.Tables;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using LiteDB;

namespace FenomPlus.Core.Database.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseTb<T>, new()
    {
        protected IDatabaseService Database => Services?.Database;
        protected IAppServices Services;
        protected ILiteDatabase db;
        private string _TblName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_db"></param>
        public GenericRepository(string tblName, LiteDatabase _db)
        {
            _TblName = tblName;
            db = _db;
            Services = IOC.Services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_services"></param>
        /// <param name="_db"></param>
        public GenericRepository(string tblName, IAppServices _services, LiteDatabase _db)
        {
            _TblName = tblName;
            Services = _services;
            db = _db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_services"></param>
        public GenericRepository(string tblName, IAppServices _services)
        {
            _TblName = tblName;
            Services = _services;
            db = Services.Database.DB;
        }

        /// <summary>
        /// 
        /// </summary>
        public GenericRepository(string tblName)
        {
            _TblName = tblName;
            Services = IOC.Services;
            db = Services.Database.DB;
        }

        /*
        /// <summary>
        /// Common Delete
        /// </summary>
        /// <param name="model"></param>
        public void Delete(T model) 
        {
            try
            {
                if (model != null)
                {
                    this.Collection.Delete(model._id);
                }
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteAll()
        {
            try
            {
                this.Collection.DeleteAll();
            }
            catch (Exception ex)
            {
                Services.LogCat.Print(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(BsonValue _id)
        {
            return this.Collection.FindOne(x => x._id == _id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>       
        public T Insert(T model)
        {
            if (model != null)
            {
                try
                {
                    if (FindById(model._id) == null)
                    {
                        this.Collection.Insert(model);
                    }
                }
                catch (Exception ex)
                {
                    Services.LogCat.Print(ex);
                }
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> SelectAll()
        {
            return this.Collection.FindAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public T Update(T model)
        {
            if (model != null)
            {
                try
                {
                    if (FindById(model._id) != null)
                    {
                        this.Collection.Update(model);
                    }
                }
                catch (Exception ex)
                {
                    Services.LogCat.Print(ex);
                }
            }
            return model;
        }
        */
    }
}