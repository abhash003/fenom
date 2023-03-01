using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DeviceStatusInfo : BaseCharacteristic
    {
        private const int COMM_STATUS_CODE_ID = (0x10);
        private const int COMM_STATUS_CODE_SIZE = 1;
        private const int COMM_STATUS_MAX_ITEMS = 4;
        private const int COMM_STATUS_PAYLOAD_SIZE = (COMM_STATUS_MAX_ITEMS * COMM_STATUS_CODE_SIZE);

        public byte StatusCode;

        public DeviceStatusInfo Decode(byte[] data)
        {
            int totalSize = COMM_STATUS_PAYLOAD_SIZE + (COMM_STATUS_MAX_ITEMS * 2) + 1;

            if (data.Length != totalSize)
                throw new ArgumentException($"Payload size mismatch (expected: {totalSize}, saw: {data.Length})");

            int offset = 0;

            int itemCount = data[offset++];

            if (itemCount >= COMM_STATUS_MAX_ITEMS)
                throw new ArgumentException($"Payload count mismatch (expected: {COMM_STATUS_MAX_ITEMS}, saw: {itemCount})");

            while (offset < totalSize)
            {
                int type = data[offset++];
                int size = data[offset++];

                switch (type)
                {
                    case COMM_STATUS_CODE_ID:
                        if (size != COMM_STATUS_CODE_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_STATUS_CODE_SIZE}, saw: {size})");
                        }
                        StatusCode = ToByte(data, offset);
                        break;

                    default:
                        offset += 1;
                        break;
                }

                offset += size;
            }

            return this;
        }


        public static DeviceStatusInfo Create(byte[] data)
        {
            DeviceStatusInfo deviceStatusInfo = new DeviceStatusInfo();
            return deviceStatusInfo.Decode(data);
        }
    }
}
