using System;
using FenomPlus.Interfaces;
using FenomPlus.Services;

namespace FenomPlus.Services
{
    public class UsbDeviceService : BaseService, IUsbDeviceService
    {
        public UsbDeviceService(IAppServices services) : base(services)
        {
        }
    }
}
