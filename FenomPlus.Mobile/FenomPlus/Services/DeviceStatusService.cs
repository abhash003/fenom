using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;
using TinyIoC;
using Xamarin.Forms;

namespace FenomPlus.Services
{
    public partial class DeviceStatusService : ObservableObject
    {

        public static TinyIoCContainer Container => TinyIoCContainer.Current;


        private readonly Timer DeviceStatusTimer;
        private readonly ICacheService CacheService;

        public DeviceStatusService()
        {
            CacheService = Container.Resolve<ICacheService>();

            DeviceStatusTimer = new Timer(1000);
            DeviceStatusTimer.Elapsed += DeviceStatusTimerOnElapsed;
        }

        private void DeviceStatusTimerOnElapsed(object sender, ElapsedEventArgs e)
        {

        }


    }
}
