using System.Collections.Generic;
using System.Linq;

namespace KitCashProtocol
{
    static class CommandGenerator
    {
        private static readonly byte[] START_BYTES = { 0xB6, 0x29 };

        public static byte[] GetCommand(Command command)
        {
            byte[] data = new byte[0];
            return GetCommand(command, data);
        }

        public static byte[] GetCommand(Command command, byte[] data)
        {
            List<byte> result = new List<byte>();
            result.AddRange(START_BYTES);
            int length = data.Length + 1;
            byte byte1 = (byte)(length & 0xff);
            length = length >> 8;
            byte byte2 = (byte)(length & 0xff);
            result.Add(byte2);
            result.Add(byte1);
            result.Add((byte)command);
            if (data.Length != 0) result.AddRange(data);
            byte[] crc = CRC16CCITT.ComputeCheckSumBytes(result.Skip(2).ToArray());
            result.AddRange(crc);

            return result.ToArray();
        }

        // Функция для конвертации текста в формат hex
        public static string TextToHex(string text)
        {
            return string.Concat(text.Select(c => ((int)c).ToString("x2")));
        }

        // Функция для конвертации числа в формат hex
        public static string NumberToHex(int number)
        {
            return number.ToString("x4");
        }
    }
}