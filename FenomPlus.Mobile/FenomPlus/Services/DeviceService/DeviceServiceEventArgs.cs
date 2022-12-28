#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;

#region Namespace Aliases

using FenomPlus.Services.DeviceService.Interfaces;

#endregion




namespace FenomPlus.Services.DeviceService
{
    public class DeviceServiceEventArgs : EventArgs
    {
        public IDevice Device;

        public DeviceServiceEventArgs(IDevice device)
        {
            Device = device;
        }
    }
}
