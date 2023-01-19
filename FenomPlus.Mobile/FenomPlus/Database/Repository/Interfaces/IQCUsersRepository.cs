using System.Collections.Generic;
using FenomPlus.Database.Tables;
using FenomPlus.Models;

namespace FenomPlus.Database.Repository.Interfaces
{
    public interface IQCUsersRepository : IGenericRepository<QCUserTable>
    {
        //// inserts
        //QualityControlUsersTb Insert(QualityControlUsersDBModel model);
        //QualityControlUsersTb Insert(QualityControlUsersDataModel model);

        //// delete
        //void Delete(QualityControlUsersDBModel model);
        //void Delete(QualityControlUsersDataModel model);

        //// updates
        //QualityControlUsersTb Update(QualityControlUsersDBModel model);
        //QualityControlUsersTb Update(QualityControlUsersDataModel model);

        //// select
        //QualityControlUsersTb FindUser(string user);

        //IEnumerable<QualityControlUsersTb> SelectAll();
    }
}
