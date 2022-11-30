using CommunityToolkit.Mvvm.Messaging.Messages;
//using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Interfaces
{
    public interface IFenomDeviceService
    {
        public class DeviceConnectedMessage : ValueChangedMessage<bool>
        {
            public DeviceConnectedMessage(bool isConnected) : base(isConnected)
            {
            }
        }

        public class DeviceConnectionLostMessage : ValueChangedMessage<bool>
        {
            public DeviceConnectionLostMessage(bool isConnected) : base(isConnected)
            {
            }
        }
        public class DeviceDisconnectedMessage : ValueChangedMessage<bool>
        {
            public DeviceDisconnectedMessage(bool isConnected) : base(isConnected)
            {
            }
        }


    }
}
