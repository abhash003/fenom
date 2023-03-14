using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    // 8 bytes old * 9 bytes new
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class BreathManeuver : BaseCharacteristic
    {
        private const int COMM_TEST_ID_ID                      =       (0x40);
        private const int COMM_TEST_ID_SIZE                    =           2 ;
        private const int COMM_TEST_STATE_ID                   =       (0x41);
        private const int COMM_TEST_STATE_SIZE                 =           1 ;
        private const int COMM_TEST_STATE_TIME_REMAINING_ID    =       (0x42);
        private const int COMM_TEST_STATE_TIME_REMAINING_SIZE  =           1 ;
        private const int COMM_BREATH_FLOW_ID                  =       (0x43);
        private const int COMM_BREATH_FLOW_SIZE                =           4 ;  /* float */
        private const int COMM_FENO_SCORE_ID                   =       (0x44);
        private const int COMM_FENO_SCORE_SIZE                 =           2 ;  /* uint_16 */
        private const int COMM_TEST_ITEMS                      =           5 ;
        private const int COMM_TEST_PAYLOAD_SIZE               =            ( COMM_TEST_ID_SIZE
                                                                            + COMM_TEST_STATE_SIZE
                                                                            + COMM_TEST_STATE_TIME_REMAINING_SIZE
                                                                            + COMM_BREATH_FLOW_SIZE
                                                                            + COMM_FENO_SCORE_SIZE);

        // implemented in firmware
        public byte TimeRemaining;     // 0-done (??) F0 - | FE-FenomReady | FF-ready
        public float BreathFlow;
        public short? NOScore;

        // not implemented yet in firmware
        public short TestNumber;
        public byte TestState;


        public BreathManeuver Decode(byte[] data)
        {
            int totalSize = COMM_TEST_PAYLOAD_SIZE + (COMM_TEST_ITEMS * 2) + 1;

            if (data.Length != totalSize)
                throw new ArgumentException($"Payload size mismatch (expected: {totalSize}, saw: {data.Length})");

            int offset = 0;

            int itemCount = data[offset++];

            if (itemCount != COMM_TEST_ITEMS)
                throw new ArgumentException($"Payload count mismatch (expected: {COMM_TEST_ITEMS}, saw: {itemCount})");

            while (offset < totalSize)
            {
                int type = data[offset++];
                int size = data[offset++];

                switch (type)
                {
                    case COMM_TEST_ID_ID:
                        if (size != COMM_TEST_ID_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_TEST_ID_ID}, saw: {size})");
                        }
                        TestNumber = ToShort(data, offset);
                        break;

                    case COMM_TEST_STATE_ID:
                        if (size != COMM_TEST_STATE_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_TEST_STATE_SIZE}, saw: {size})");
                        }
                        TestState = ToByte(data, offset);
                        break;

                    case COMM_TEST_STATE_TIME_REMAINING_ID:
                        if (size != COMM_TEST_STATE_TIME_REMAINING_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_TEST_STATE_TIME_REMAINING_SIZE}, saw: {size})");
                        }
                        TimeRemaining = ToByte(data, offset);
                        break;

                    case COMM_BREATH_FLOW_ID:
                        if (size != COMM_BREATH_FLOW_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_BREATH_FLOW_SIZE}, saw: {size})");
                        }

                        BreathFlow = ToFloat(data, offset);
                        break;

                    case COMM_FENO_SCORE_ID:
                        if (size != COMM_FENO_SCORE_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_FENO_SCORE_SIZE}, saw: {size})");
                        }
                        // Only read the score when receiving the special code "0xFE" meaning "score ready" in the 'time remaining' field.
                        NOScore = TimeRemaining == 0xFE ?  ToShort(data, offset) : null;
                        break;
                }

                offset += size;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BreathManeuver Create(byte[] data)
        {
            BreathManeuver breathManeuver = new BreathManeuver();
            return breathManeuver.Decode(data);
        }
    }
}
