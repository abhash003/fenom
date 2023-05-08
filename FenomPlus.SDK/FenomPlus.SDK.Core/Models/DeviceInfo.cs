using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FenomPlus.Services;
using Xamarin.Forms;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DeviceInfo : BaseCharacteristic
    {
        // Device characteristic items
        private const int COMM_PCBA_VERSION_ID                =        (0x10);
        private const int COMM_PCBA_VERSION_SIZE              =            1 ;
        private const int COMM_FIRMWARE_VERSION_MAJOR_ID      =        (0x01);
        private const int COMM_FIRMWARE_VERSION_MAJOR_SIZE    =            1 ;
        private const int COMM_FIRMWARE_VERSION_MINOR_ID      =        (0x02);
        private const int COMM_FIRMWARE_VERSION_MINOR_SIZE    =            1 ;
        private const int COMM_DEVICE_DAYS_REMAINING_ID       =        (0x03);
        private const int COMM_DEVICE_DAYS_REMAINING_SIZE     =            2 ;
        private const int COMM_NO_SENSOR_DAYS_REMAINING_ID    =        (0x04);
        private const int COMM_NO_SENSOR_DAYS_REMAINING_SIZE  =            2 ;
        private const int COMM_DEVICE_SERIAL_NUMBER_ID        =        (0x05);
        private const int COMM_DEVICE_SERIAL_NUMBER_SIZE      =            6 ;
        private const int COMM_QC_VALIDITY_ID =        (0x06);
        private const int COMM_QC_VALIDITY_SIZE =           2;
        private const int COMM_DEVICE_ITEMS = 7;
        private const int COMM_DEVICE_PAYLOAD_SIZE      = ( COMM_PCBA_VERSION_SIZE 
                                                            + COMM_FIRMWARE_VERSION_MAJOR_SIZE    
                                                            + COMM_FIRMWARE_VERSION_MINOR_SIZE    
                                                            + COMM_DEVICE_DAYS_REMAINING_SIZE     
                                                            + COMM_NO_SENSOR_DAYS_REMAINING_SIZE  
                                                            + COMM_DEVICE_SERIAL_NUMBER_SIZE );

        private byte pcbaVersion;
        private byte firmwareVersionMajor;
        private byte firmwareVersionMinor;
        private int deviceDaysRemaining;
        private int noSensorDaysRemaining;
        private byte[] serialNumber;
        private short qcValidity;

        private bool isQcEnabled;

        public byte PcbaVersion { get => pcbaVersion; set => pcbaVersion = value; }
        public byte FirmwareVersionMajor { get => firmwareVersionMajor; set => firmwareVersionMajor = value; }
        public byte FirmwareVersionMinor { get => firmwareVersionMinor; set => firmwareVersionMinor = value; }
        public int DeviceDaysRemaining { get => deviceDaysRemaining; set => deviceDaysRemaining = value; }
        public int NoSensorDaysRemaining { get => noSensorDaysRemaining; set => noSensorDaysRemaining = value; }
        public byte[] SerialNumber { get => serialNumber; set => serialNumber = value; }
        public short QcValidity { get => qcValidity; set => qcValidity = value; }


        public bool IsQcEnabled { get => isQcEnabled; set => isQcEnabled = value; }

        public int SensorExpDateYear
        {
            get
            {
                return DateTime.Now.AddDays(noSensorDaysRemaining).Year;
            }
        }

        public int SensorExpDateMonth
        {
            get
            {
                return DateTime.Now.AddDays(noSensorDaysRemaining).Month;
            }
        }
        public int SensorExpDateDay
        {
            get
            {
                return DateTime.Now.AddDays(noSensorDaysRemaining).Day;
            }
        }

        public DeviceInfo()
        {
            qcValidity = (short)0x0;
        }

        public DeviceInfo Decode(byte[] data)
        {
            int offset = 0;
            isQcEnabled = false;
            int itemCount = data[offset++];

            int totalSize = data.Length; // COMM_DEVICE_PAYLOAD_SIZE + (itemCount * 2) + 1;

            while (offset < totalSize)
            {
                int type = data[offset++];
                int size = data[offset++];

                switch (type)
                {
                    case COMM_PCBA_VERSION_ID:
                        if (size != COMM_PCBA_VERSION_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_PCBA_VERSION_SIZE}, saw: {size})");
                        }
                        pcbaVersion = data[offset];
                        break;

                    case COMM_FIRMWARE_VERSION_MAJOR_ID:
                        if (size != COMM_FIRMWARE_VERSION_MAJOR_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_FIRMWARE_VERSION_MAJOR_SIZE}, saw: {size})");
                        }
                        firmwareVersionMajor = data[offset];
                        break;

                    case COMM_FIRMWARE_VERSION_MINOR_ID:
                        if (size != COMM_FIRMWARE_VERSION_MINOR_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_FIRMWARE_VERSION_MINOR_SIZE}, saw: {size})");
                        }
                        firmwareVersionMinor = data[offset];
                        break;

                    case COMM_DEVICE_DAYS_REMAINING_ID:
                        if (size != COMM_DEVICE_DAYS_REMAINING_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_DEVICE_DAYS_REMAINING_SIZE}, saw: {size})");
                        }
                        deviceDaysRemaining = ToInt16(data, offset);
                        break;

                    case COMM_NO_SENSOR_DAYS_REMAINING_ID:
                        if (size != COMM_NO_SENSOR_DAYS_REMAINING_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_NO_SENSOR_DAYS_REMAINING_SIZE}, saw: {size})");
                        }
                        noSensorDaysRemaining = ToShort(data, offset);
                        break;

                    case COMM_DEVICE_SERIAL_NUMBER_ID:
                        if (size != COMM_DEVICE_SERIAL_NUMBER_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_DEVICE_SERIAL_NUMBER_SIZE}, saw: {size})");
                        }
                        serialNumber = new byte[COMM_DEVICE_SERIAL_NUMBER_SIZE];
                        Array.Copy(data, offset, serialNumber, 0, serialNumber.Length);
                        break;

                    case COMM_QC_VALIDITY_ID:
                        {
                            if (size != COMM_QC_VALIDITY_SIZE)
                            {
                                throw new ArgumentException($"Unexpected payload item size (expected: {COMM_QC_VALIDITY_SIZE}, saw: {size})");
                            }
                            isQcEnabled = true;
                            qcValidity = ToShort(data, offset);
                            break;
                        }

                    default:
                        // log unexpected item and skip it
                        break;
                }

                offset += size;
            }

            return this;
        }
    }
}
