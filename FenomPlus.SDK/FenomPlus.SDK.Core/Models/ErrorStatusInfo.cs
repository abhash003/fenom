using System;
using System.Runtime.InteropServices;
using FenomPlus.Services.DeviceService.Utils;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ErrorStatusInfo : BaseCharacteristic
    {
        private const int COMM_ERROR_CODE_ID = (0x90);
        private const int COMM_ERROR_CODE_SIZE = 1;
        private const int COMM_ERROR_MAX_ITEMS = 4;
        private const int COMM_ERROR_PAYLOAD_SIZE = (COMM_ERROR_MAX_ITEMS * COMM_ERROR_CODE_SIZE);

        public byte ErrorCode;

        public ErrorStatusInfo Decode(byte[] data)
        {
            int totalSize = COMM_ERROR_PAYLOAD_SIZE + (COMM_ERROR_MAX_ITEMS * 2) + 1;

            if (data.Length != totalSize)
                throw new ArgumentException($"Payload size mismatch (expected: {totalSize}, saw: {data.Length})");

            int offset = 0;

            int itemCount = data[offset++];

            if (itemCount >= COMM_ERROR_MAX_ITEMS)
                throw new ArgumentException($"Payload count mismatch (expected: {COMM_ERROR_MAX_ITEMS}, saw: {itemCount})");

            while (offset < totalSize)
            {
                int type = data[offset++];
                int size = data[offset++];

                switch (type)
                {
                    case COMM_ERROR_CODE_ID:
                        if (size != COMM_ERROR_CODE_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_ERROR_CODE_SIZE}, saw: {size})");
                        }
                        ErrorCode = ToByte(data, offset);
                        break;
                    
                    default:
                        offset += 1;
                        break;
                }

                offset += size;
            }

            return this;
        }

        public static ErrorStatusInfo Create(byte[] data)
        {
            ErrorStatusInfo errorStatusInfo = new ErrorStatusInfo();
            return errorStatusInfo.Decode(data);
        }
    }
}
