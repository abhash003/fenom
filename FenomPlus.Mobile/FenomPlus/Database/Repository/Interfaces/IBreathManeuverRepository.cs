﻿
using System.Threading.Tasks;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models.Characteristic;

namespace FenomPlus.Database.Repository.Interfaces
{
    public interface IBreathManeuverRepository
    {
        void Insert(BreathManeuverModel breathManeuver);
        void Insert(BreathManeuver breathManeuver);
    }
}