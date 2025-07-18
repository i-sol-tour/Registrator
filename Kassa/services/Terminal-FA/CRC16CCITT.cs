using System;

namespace KitCashProtocol
{
    static class CRC16CCITT
    {
        private const ushort POLY = 0x1021;
        private static ushort[] Table { get; set; }
        private static ushort InitialValue { get; set; }

        static CRC16CCITT()
        {
            Table = new ushort[256];
            InitialValue = 0xffff;
            ushort temp, a;
            for (int i = 0; i < Table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ POLY);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                Table[i] = temp;
            }
        }

        public static byte[] ComputeCheckSumBytes(byte[] bytes)
        {
            ushort crc = ComputeCheckSum(bytes);
            return BitConverter.GetBytes(crc);
        }

        private static ushort ComputeCheckSum(byte[] bytes)
        {
            ushort crc = InitialValue;
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ Table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }
    }
}