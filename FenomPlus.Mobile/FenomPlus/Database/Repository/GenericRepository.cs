using System;
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

        public GenericRepository(string tblName, LiteDatabase _db)
        {
            _TblName = tblName;
            db = _db;
            Services = IOC.Services;
        }

        public GenericRepository(string tblName, IAppServices _services, LiteDatabase _db)
        {
            _TblName = tblName;
            Services = _services;
            db = _db;
        }

        public GenericRepository(string tblName, IAppServices _services)
        {
            _TblName = tblName;
            Services = _services;
            db = Services.Database.DB;
        }

        public GenericRepository(string tblName)
        {
            _TblName = tblName;
            Services = IOC.Services;
            db = Services.Database.DB;
        }
    }
}