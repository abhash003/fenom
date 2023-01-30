using System;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using Microsoft.Extensions.Logging;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace FenomPlus.Services
{
    public class CacheService : BaseService, ICacheService
    {
        private readonly RingBuffer BreathBuffer;

        public CacheService(IAppServices services) : base(services)
        {
            BreathBuffer = new RingBuffer(Services.Config.RingBufferSample, Services.Config.RingBufferTimeout);

            BreathFlowTimer = Services.Config.BreathFlowTimeout;            

            Logger = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug)
                    .AddFilter("FenomPlus", LogLevel.Debug);
            });
            
        }        

        public ILoggerFactory Logger { get; set; }        

        public DateTime DeviceExpireDate { get; set; }

        public TestTypeEnum TestType { get; set; }

        public int BreathFlowTimer { get; set; }

        public float HumanControlResult { get; set; }

        private static ISettings AppSettings => CrossSettings.Current;

        public bool ReadyForTest { get; set; }        

        public string QCUsername { get; set; }               

        
    }
}
