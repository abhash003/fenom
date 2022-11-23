using FenomPlus.Interfaces;

namespace FenomPlus.Services
{
    public class UsbDeviceService : BaseService, IUsbDeviceService
    {
        public UsbDeviceService(IAppServices services) : base(services)
        {
        }
    }
}
