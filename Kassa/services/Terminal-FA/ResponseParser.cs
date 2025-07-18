using System.Linq;

namespace KitCashProtocol
{
    static class ResponseParser
    {
        private static readonly byte[] START_BYTES = { 0xB6, 0x29 };

        public static bool IsValid(byte[] response)
        {
            if (response.Length < 7) return false;
            if (!(response[0] == START_BYTES[0] && response[1] == START_BYTES[1])) return false;

            int length = (response[2] << 8) | response[3];
            //int length = response[2];
            //length = length >> 8;
            //length |= response[3];
            if (response.Length - 6 != length) return false;

            byte[] crc = CRC16CCITT.ComputeCheckSumBytes(response.Skip(2).Take(response.Length - 4).ToArray());
            if (!(crc[0] == response[response.Length - 2] && crc[1] == response[response.Length - 1])) return false;

            return true;
        }

        public static byte[] GetData(byte[] response)
        {
            return response.Skip(4).Take(response.Length - 6).ToArray();
        }
    }
}