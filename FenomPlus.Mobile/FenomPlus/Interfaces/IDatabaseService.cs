using System;
using FenomPlus.Database.Repository.Interfaces;
using LiteDB;

namespace FenomPlus.Interfaces
{
    public interface IDatabaseService
    {
        string DatabasePath(string dbFile);

        ILiteDatabase DB { get; }
        IBreathManeuverErrorRepository BreathManeuverErrorRepo { get; }
        IBreathManeuverResultRepository BreathManeuverResultRepo { get; }
        //IQualityControlRepository QualityControlRepo { get; }
        //IQualityControlDevicesRepository QualityControlDevicesRepo { get; }
        //IQualityControlUsersRepository QualityControlUsersRepo { get; } 
    }
}
