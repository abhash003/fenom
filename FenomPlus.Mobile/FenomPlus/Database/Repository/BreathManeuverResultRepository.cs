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
    public class BreathManeuverResultRepository : GenericRepository<BreathManeuverResultTb>, IBreathManeuverResultRepository
    {
        private static string TblName = "BreathManeuverResultRepository";
        private ILiteCollection<BreathManeuverResultTb> Collection => db.GetCollection<BreathManeuverResultTb>(TblName);

        public BreathManeuverResultRepository() : base(TblName)
        {

        }

        public BreathManeuverResultRepository(LiteDatabase db) : base(TblName, db)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void Delete(BreathManeuverResultDBModel model)
        {
            Delete(model.Convert());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BreathManeuverResultTb Insert(BreathManeuverResultDBModel model)
        {
            return Insert(model.Convert());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BreathManeuverResultTb Update(BreathManeuverResultDBModel model)
        {
            return Update(model.Convert());
        }

        /// <summary>
        /// Common Delete
        /// </summary>
        /// <param name="model"></param>
        public void Delete(BreathManeuverResultTb model)
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
        public BreathManeuverResultTb FindById(BsonValue _id)
        {
            return this.Collection.FindOne(x => x._id == _id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BreathManeuverResultTb Insert(BreathManeuverResultTb model)
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
        public IEnumerable<BreathManeuverResultTb> SelectAll()
        {
            return this.Collection.FindAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BreathManeuverResultTb Update(BreathManeuverResultTb model)
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
    }
}