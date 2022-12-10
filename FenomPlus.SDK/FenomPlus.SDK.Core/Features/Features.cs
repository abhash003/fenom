using System;
using System.Threading.Tasks;
using FenomPlus.Helpers;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.SDK.Core.Ble.PluginBLE
{
    internal partial class BleDevice
    {
        public async Task<bool> DEVICEINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEVICEINFO);
            return await WRITEREQUEST(message,1);
        }

        public async Task<bool> ENVIROMENTALINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_ENVIROMENTALINFO);
            return await WRITEREQUEST(message,1);
        }

        public async Task<bool> ERRORSTATUSINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_ERRORSTATUSINFO);
            return await WRITEREQUEST(message, 1);
        }

        public async Task<bool> DEVICESTATUSINFO()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEVICESTATUSINFO);
            return await WRITEREQUEST(message, 1);
        }

        public async Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_BREATHTEST, (Byte)breathTestEnum);
            return await WRITEREQUEST(message, 1);
        }

        //public async Task<bool> TRAININGMODE()
        //{
        //    MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_TRAININGMODE);
        //    return await WRITEREQUEST(message,1);
        //}

        //public async Task<bool> BREATHMANUEVER()
        //{
        //    MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_BREATHMANUEVER);
        //    return await WRITEREQUEST(message,1);
        //}

        public async Task<bool> DEBUGMSG()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEBUGMSG);
            return await WRITEREQUEST(message,1);
        }

        public async Task<bool> DEBUGMANUEVERTYPE()
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_REQUEST_DATA, ID_SUB.ID_REQUEST_DEBUGMANUEVERTYPE);
            return await WRITEREQUEST(message,1);
        }

        public async Task<bool> MESSAGE(MESSAGE message)
        {
            return await WRITEREQUEST(message,1);
        }

        /// <returns></returns>
        public async Task<bool> SERIALNUMBER(string SerialNumber)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_SERIALNUMBER, SerialNumber);
            return await WRITEREQUEST(message,10);
        }

        public async Task<bool> DATETIME(string date, string time)
        {
            string strDateTime;

            strDateTime = (date + "T" + time);

            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_PROVISIONING_DATA, ID_SUB.ID_PROVISIONING_DATETIME, strDateTime);
            return await WRITEREQUEST(message, (short)strDateTime.Length);
        }
        
        public async Task<bool> CALIBRATION(ID_SUB iD_SUB, double cal1, double cal2, double cal3)
        {
            MESSAGE message = new MESSAGE(ID_MESSAGE.ID_CALIBRATION_DATA, iD_SUB, cal1, cal2, cal3);
            return await WRITEREQUEST(message,24);
        }

        private async Task<bool> WRITEREQUEST(MESSAGE message, Int16 idvar_size)
        {
            byte[] data = new byte[2+2+ idvar_size];
            
            data[0]  = (byte)(message.IDMSG >> 8);
            data[1]  = (byte)(message.IDMSG);
            data[2]  = (byte)(message.IDSUB >> 8);
            data[3]  = (byte)(message.IDSUB);

            Buffer.BlockCopy(message.IDVAR, 0, data, 4, idvar_size);

            IGattCharacteristic Characteristic = await FindCharacteristic(Constants.FeatureWriteCharacteristic);
            if (Characteristic != null)
            {
                await Characteristic.WriteWithoutResponseAsync(data);
                return true;
            }
            return false;
        }
    }
}
