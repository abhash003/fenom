using System;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using Microsoft.Extensions.Logging;

namespace FenomPlus.Interfaces
{
    public interface ICacheService
    {
        ILoggerFactory Logger { get; set; }
        string QCUsername { get; set; }

        DateTime DeviceExpireDate { get; set; }                
        TestTypeEnum TestType { get; set; }
        
        int BreathFlowTimer { get; set; }        

        float HumanControlResult { get; set; }
    }
}
