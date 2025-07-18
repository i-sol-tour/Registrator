using Aspose.Words;
using Azure;
using MaterialSkin.Controls;
using Microsoft.Office.Interop.Access;
using Registrator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Registrator.repo.models;

namespace KitCashProtocol
{
    public class TerminalFA
    {
        private static SerialPort Port { get; set; }
        private const int READ_TIMEOUT = 10;
        private byte[] TLV { get; set; }
        private ushort TLVPosition { get; set; }
        private TaxType DefaultTaxType { get; set; }
        private static readonly byte[] START_BYTES = { 0xB6, 0x29 };
        public ushort tag;
        public byte[] message;

        public bool OpenConnection(bool statusConnectionKKT, string portName)
        {
            if (statusConnectionKKT == false)
            {
                try
                {
                    DefaultTaxType = TaxType.Unknown;
                    TLV = new byte[1024];
                    TLVPosition = 0;

                    if (Port == null)
                    {
                        try
                        {
                            Port = new SerialPort(portName);
                            Port.BaudRate = 115200;
                            Port.DataBits = 8;
                            Port.Parity = Parity.None;
                            Port.StopBits = StopBits.One;
                            Port.Open();
                        }
                        catch
                        {
                            MaterialMessageBox.Show("Не удалось подключиться к " + portName);
                            Port = null;
                            return false;
                        }
                    }
                    return true;
                }
                catch
                {
                    MaterialMessageBox.Show("Не удалось подключиться к " + portName);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        public bool CloseConnection(bool statusConnectionKKT)
        {
            try
            {
                if (statusConnectionKKT == true)
                {
                    if (Port != null)
                    {
                        if (Port.IsOpen)
                        {
                            Port.Close();
                            Port.Dispose();
                            Port = null; // Обнуление для дальнейшего предотвращения ошибок
                            return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return true;
            }
        }

        public ErrorCode CancelDocument()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.CANCEL_DOCUMENT);
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    return ErrorCode.OK;
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        public ErrorCode Initialize()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.REGISTRATION_PARAMETERS);
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    byte taxes = response[33];

                    if ((taxes & 1) != 0)
                    {
                        DefaultTaxType = TaxType.Common;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 2) != 0)
                    {
                        DefaultTaxType = TaxType.Simplified;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 4) != 0)
                    {
                        DefaultTaxType = TaxType.Simplified2;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 8) != 0)
                    {
                        DefaultTaxType = TaxType.ENVD;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 16) != 0)
                    {
                        DefaultTaxType = TaxType.ESN;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 32) != 0)
                    {
                        DefaultTaxType = TaxType.Patent;
                        return ErrorCode.OK;
                    }

                    throw new Exception();
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        public TerminalFAStatus GetStatusKKT()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_STATUS_KKT);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                TerminalFAStatus result;
                if (response[0] == 0x00)
                {
                    result = new TerminalFAStatus
                    {
                        Result = ErrorCode.OK,
                        FactoryNumber = Encoding.ASCII.GetString(response, 1, 12),
                        CurrentDateTime = new DateTime(response[13] + 2000, response[14], response[15], response[16], response[17], 0),
                        FatalErrors = (response[18] != 0),
                        PrinterStatus = (TerminalFAPrinterStatus)response[19],
                        FNThereis = response[20]
                    };
                }
                else
                {
                    result = new TerminalFAStatus { Result = (ErrorCode)response[1] }; 
                }

                return result;
            }

            return null;
        }

        public FNStatus GetStatusFN()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_STATUS_FN);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                FNStatus result;
                if (response[0] == 0x00)
                {
                    result = new FNStatus
                    {
                        Result = ErrorCode.OK,
                        Phase = response[1],
                        Document = response[2],
                        StatusShift = response[4],
                        NumberLastDocument = response[response.Length - 3]
                    };
                }
                else
                {
                    result = new FNStatus { Result = (ErrorCode)response[1] };
                }
                return result;
            }
            return null;
        }
        public FiscalStorageStatus GetFiscalStorageStatus()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_FISCAL_STORAGE_STATUS);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                FiscalStorageStatus result;
                if (response[0] == 0x00)
                {
                    result = new FiscalStorageStatus
                    {
                        Result = ErrorCode.OK,
                        CurrentDocument = response[2],
                        SessionIsOpen = (response[4] != 0)
                    };
                }
                else
                {
                    result = new FiscalStorageStatus { Result = (ErrorCode)response[1] };
                }

                return result;
            }

            return null;
        }
        public string GetZN()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_ZN);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    return Encoding.GetEncoding(866).GetString(response.Skip(1).ToArray());
                }

                return string.Empty;
            }

            return string.Empty;
        }
        public string GetFN()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_FN);
            Port.Write(command, 0, command.Length);
            //MaterialMessageBox.Show("ЗН ФН команда " + BitConverter.ToString(command)); // проверка команды
            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    return Encoding.GetEncoding(866).GetString(response.Skip(1).ToArray());
                }

                return string.Empty;
            }

            return string.Empty;
        }
        public string GetDATETIME()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_DATATIME);

            try
            {
                //MaterialMessageBox.Show("Время и дата команда " + BitConverter.ToString(command)); // проверка команды
                Port.Write(command, 0, command.Length);
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                return string.Empty;
            }

            byte[] response = ReadResponse();
            //MaterialMessageBox.Show("Время и дата ответ " + BitConverter.ToString(response)); // проверка команды
            if (response != null && response.Length >= 7)
            {

                // Проверяем длину
                byte lengthResponse = response[3];
                if (lengthResponse == 5)
                { 
                    byte year = response[5];
                    byte month = response[6];
                    byte day = response[7];
                    byte hour = response[8];
                    byte minute = response[9];

                    DateTime dateTime;
                    dateTime = new DateTime(year, month, day, hour, minute, 0);
                    try
                    {
                        dateTime = new DateTime(year+2000, month, day, hour, minute, 0);
                        string dateTimeString = dateTime.ToString("dd.MM.yyyy HH:mm");
                        
                        return dateTimeString; 
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        MaterialMessageBox.Show("Ошибка при создании массива dateTime: " + ex.Message);
                    }
                }
                else
                {
                    MaterialMessageBox.Show("Неверная длина данных: " + lengthResponse);
                }

            }
            else
            {
                MaterialMessageBox.Show("Недопустимый ответ или недостаточная длина: " + (response?.Length ?? 0));
            }

            return string.Empty; // Возвращаем пустую строку, если ответ некорректен
        }
        public void InputDATETIME(DateTime DateTimeKKT)
        {
            try {
                byte[] data = { 48, 117, 5, 0, 25, 1, 1, 12, 00 };
                // Обновляем значения в массиве
                int year = DateTimeKKT.Year - 2000;
                data[4] = (byte)year;   // Год
                data[5] = (byte)DateTimeKKT.Month;  // Месяц
                data[6] = (byte)DateTimeKKT.Day;    // День
                data[7] = (byte)DateTimeKKT.Hour;   // Час
                data[8] = (byte)DateTimeKKT.Minute;  // Минута

                byte[] command = CommandGenerator.GetCommand(Command.INPUT_DATATIME, data);
                Port.Write(command, 0, command.Length);
                }
            catch (ArgumentOutOfRangeException ex) { MaterialMessageBox.Show("Не удалось ввести время. Ошибка:" + ex.Message); }
        }
        public string[] GetPARAMETERS_OFD()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_PARAMETERS_OFD);
            try
            {
                Port.Write(command, 0, command.Length);
                //MaterialMessageBox.Show("Kоманда" + BitConverter.ToString(command)); // проверка команды
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
            }

            byte[] response = ReadResponse();
            //MaterialMessageBox.Show("Время и дата ответ" + BitConverter.ToString(response)); // проверка команды
                                                                                            
            string[] data = new string[4];
            int position = 1;

            while (position < response.Length)
            {
                // Читаем тег
                ushort tag = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем длину
                ushort length = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем сообщение в зависимости от длины
                byte[] message = new byte[length];
                Array.Copy(response, position, message, 0, length);
                position += length;

                // Конвертируем байты в строку
                switch (tag)
                {
                    case 30040: // TAG для URL
                        data[0] = Encoding.ASCII.GetString(message);
                        break;
                    case 30005: // TAG для IP
                        data[1] = Encoding.ASCII.GetString(message);
                        break;
                    case 30006: // TAG для первого LE значения
                        data[2] = Convert.ToString(BitConverter.ToInt16(message, 0));
                        break;
                    case 30009: // TAG для второго LE значения
                        data[3] = Convert.ToString(BitConverter.ToInt16(message, 0));
                        break;
                }
            }
            return data;
        }
        public void InputPARAMETERS_OFD(string adress_ofd, string ip_ofd, string port_ofd, string timestopOFD)
        {
            int int_port_ofd = Convert.ToInt32(port_ofd);
            int int_timestopOFD = Convert.ToInt32(timestopOFD);

            // Стартовые байты
            byte[] startBytes = new byte[] { 0xB6, 0x29 };

            // Получаем byte[] для каждого сообщения
            byte[] hexAdressOFD = Encoding.ASCII.GetBytes(adress_ofd);
            byte[] hexIPOFD = Encoding.ASCII.GetBytes(ip_ofd);
            byte[] hexPortOFD = BitConverter.GetBytes((ushort)int_port_ofd).Reverse().ToArray();
            byte[] hexTimestopOFD = BitConverter.GetBytes((ushort)int_timestopOFD).Reverse().ToArray();

            // переворачиваем сообщения по формату LE
            Array.Reverse(hexPortOFD);
            Array.Reverse(hexTimestopOFD);

            // Определяем длину каждого сообщения
            byte[] lengthMessage1 = BitConverter.GetBytes((ushort)hexAdressOFD.Length);
            byte[] lengthMessage2 = BitConverter.GetBytes((ushort)hexIPOFD.Length);
            byte[] lengthMessage3 = BitConverter.GetBytes((ushort)hexPortOFD.Length);
            byte[] lengthMessage4 = BitConverter.GetBytes((ushort)hexTimestopOFD.Length);

            // Тег сообщения
            byte[] tagMessage1 = new byte[] { 0x58, 0x75 };
            byte[] tagMessage2 = new byte[] { 0x35, 0x75 };
            byte[] tagMessage3 = new byte[] { 0x36, 0x75 };
            byte[] tagMessage4 = new byte[] { 0x39, 0x75 };

            // Общая длина сообщения
            int totalLengthValue = tagMessage1.Length + hexAdressOFD.Length + lengthMessage1.Length +
                                   tagMessage2.Length + hexIPOFD.Length + lengthMessage2.Length +
                                   tagMessage1.Length + hexPortOFD.Length + lengthMessage3.Length +
                                   tagMessage1.Length + hexTimestopOFD.Length + lengthMessage4.Length + 1;

            byte[] totalLength = BitConverter.GetBytes((ushort)totalLengthValue);
            Array.Reverse(totalLength);

            // Формируем массив данных
            byte[] data = startBytes
                .Concat(totalLength)
                .Concat(new byte[] { 0x76 }) // Бит команды
                .Concat(tagMessage1)
                .Concat(lengthMessage1)
                .Concat(hexAdressOFD)
                .Concat(tagMessage2)
                .Concat(lengthMessage2)
                .Concat(hexIPOFD)
                .Concat(tagMessage3)
                .Concat(lengthMessage3)
                .Concat(hexPortOFD)
                .Concat(tagMessage4)
                .Concat(lengthMessage4)
                .Concat(hexTimestopOFD)
                .ToArray();

            // Хвост
            byte[] crc = CRC16CCITT.ComputeCheckSumBytes(data.Skip(2).ToArray());

            byte[] command = data
                .Concat(crc)
                .ToArray();

            Port.Write(command, 0, command.Length);
        }
        public string[] GetOISM()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_OISM);
            try
            {
                Port.Write(command, 0, command.Length);
                //MaterialMessageBox.Show("Kоманда" + BitConverter.ToString(command)); // проверка команды
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
            }

            byte[] response = ReadResponse();
            //MaterialMessageBox.Show("Время и дата ответ" + BitConverter.ToString(response)); // проверка команды

            string[] data = new string[4];
            int position = 1;

            while (position < response.Length)
            {
                // Читаем тег
                ushort tag = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем длину
                ushort length = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем сообщение в зависимости от длины
                byte[] message = new byte[length];
                Array.Copy(response, position, message, 0, length);
                position += length;

                // Конвертируем байты в строку
                switch (tag)
                {
                    case 30050:
                        data[0] = Encoding.ASCII.GetString(message);
                        break;
                    case 30051:
                        data[1] = Convert.ToString(BitConverter.ToInt16(message, 0));
                        break;
                    case 30052:
                        data[2] = Convert.ToString(BitConverter.ToInt16(message, 0));
                        break;
                }
            }
            return data;
        }
        public void InputOISM(string adress_OISM, string tsp_OISM, string timestopOFD)
        {
            int int_tsp_OISM = Convert.ToInt32(tsp_OISM);
            int int_timestopOISM = Convert.ToInt32(timestopOFD);

            // Стартовые байты
            byte[] startBytes = new byte[] { 0xB6, 0x29 };

            // Получаем byte[] для каждого сообщения
            byte[] hexAdressOISM = Encoding.ASCII.GetBytes(adress_OISM);
            byte[] hexTspOISM = BitConverter.GetBytes((ushort)int_tsp_OISM).Reverse().ToArray();
            byte[] hexTimestopOISM = BitConverter.GetBytes((ushort)int_timestopOISM).Reverse().ToArray();

            // переворачиваем сообщения по формату LE
            Array.Reverse(hexTspOISM);
            Array.Reverse(hexTimestopOISM);

            // Определяем длину каждого сообщения
            byte[] lengthMessage1 = BitConverter.GetBytes((ushort)hexAdressOISM.Length);
            byte[] lengthMessage2 = BitConverter.GetBytes((ushort)hexTspOISM.Length);
            byte[] lengthMessage3 = BitConverter.GetBytes((ushort)hexTimestopOISM.Length);

            // Тег сообщения
            byte[] tagMessage1 = new byte[] { 0x62, 0x75 };
            byte[] tagMessage2 = new byte[] { 0x63, 0x75 };
            byte[] tagMessage3 = new byte[] { 0x64, 0x75 };

            // Общая длина сообщения
            int totalLengthValue = tagMessage1.Length + hexAdressOISM.Length + lengthMessage1.Length +
                                   tagMessage2.Length + hexTspOISM.Length + lengthMessage2.Length +
                                   tagMessage3.Length + hexTimestopOISM.Length + lengthMessage3.Length + 1;

            byte[] totalLength = BitConverter.GetBytes((ushort)totalLengthValue);
            Array.Reverse(totalLength);

            // Формируем массив данных
            byte[] data = startBytes
                .Concat(totalLength)
                .Concat(new byte[] { 0x82 }) // Бит команды
                .Concat(tagMessage1)
                .Concat(lengthMessage1)
                .Concat(hexAdressOISM)
                .Concat(tagMessage2)
                .Concat(lengthMessage2)
                .Concat(hexTspOISM)
                .Concat(tagMessage3)
                .Concat(lengthMessage3)
                .Concat(hexTimestopOISM)
                .ToArray();

            // Хвост
            byte[] crc = CRC16CCITT.ComputeCheckSumBytes(data.Skip(2).ToArray());
            byte[] tail = new byte[] { 0x1A, 0x56 };

            byte[] command = data
                .Concat(crc)
                .ToArray();

            Port.Write(command, 0, command.Length);
        }
        public string[] GetKeyKM()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_KM);
            try
            {
                Port.Write(command, 0, command.Length);
                //MaterialMessageBox.Show("Kоманда" + BitConverter.ToString(command)); // проверка команды
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
            }

            byte[] response = ReadResponse();
            //MaterialMessageBox.Show("Время и дата ответ" + BitConverter.ToString(response)); // проверка команды

            string[] data = new string[4];
            int position = 1;

            while (position < response.Length)
            {
                // Читаем тег
                ushort tag = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем длину
                ushort length = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем сообщение в зависимости от длины
                byte[] message = new byte[length];
                Array.Copy(response, position, message, 0, length);
                position += length;

                // Конвертируем байты в строку
                switch (tag)
                {
                    case 30060: // TAG для URL
                        data[0] = Encoding.ASCII.GetString(message);
                        break;
                    case 30061: // TAG для первого LE значения
                        data[1] = Convert.ToString(BitConverter.ToInt16(message, 0));
                        break;
                }
            }
            return data;
        }
        public void InputKeyKM(string adress_KM, string tsp_KM)
        {
            int int_tsp_KM = Convert.ToInt32(tsp_KM);

            // Стартовые байты
            byte[] startBytes = new byte[] { 0xB6, 0x29 };

            // Получаем byte[] для каждого сообщения
            byte[] hexAdressKM = Encoding.ASCII.GetBytes(adress_KM);
            byte[] hexTspKM = BitConverter.GetBytes((ushort)int_tsp_KM).Reverse().ToArray();

            // переворачиваем сообщения по формату LE
            Array.Reverse(hexTspKM);

            // Определяем длину каждого сообщения
            byte[] lengthMessage1 = BitConverter.GetBytes((ushort)hexAdressKM.Length);
            byte[] lengthMessage2 = BitConverter.GetBytes((ushort)hexTspKM.Length);

            // Тег сообщения
            byte[] tagMessage1 = new byte[] { 0x6C, 0x75 };
            byte[] tagMessage2 = new byte[] { 0x6D, 0x75 };

            // Общая длина сообщения
            int totalLengthValue = tagMessage1.Length + hexAdressKM.Length + lengthMessage1.Length +
                                   tagMessage2.Length + hexTspKM.Length + lengthMessage2.Length + 1;

            byte[] totalLength = BitConverter.GetBytes((ushort)totalLengthValue);
            Array.Reverse(totalLength);

            // Формируем массив данных
            byte[] data = startBytes
                .Concat(totalLength)
                .Concat(new byte[] { 0x84 }) // Бит команды
                .Concat(tagMessage1)
                .Concat(lengthMessage1)
                .Concat(hexAdressKM)
                .Concat(tagMessage2)
                .Concat(lengthMessage2)
                .Concat(hexTspKM)
                .ToArray();

            // Хвост
            byte[] crc = CRC16CCITT.ComputeCheckSumBytes(data.Skip(2).ToArray());
            byte[] tail = new byte[] { 0x1A, 0x56 };

            byte[] command = data
                .Concat(crc)
                .ToArray();

            Port.Write(command, 0, command.Length);
        }
        public string[,] GetRegistrationReportTLVLast()
        {
            int numberRegistrationReport = 30;
            string[,] data = new string[30, 3];
            byte[] command = null;
            byte[] response = null;

            while (numberRegistrationReport >= 1)
            {
                byte[] byteArray = new byte[] { (byte)numberRegistrationReport };
                command = CommandGenerator.GetCommand(Command.GET_REGISTRATION_REPORT_TLV, byteArray);
                try
                {
                    Port.Write(command, 0, command.Length);
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                }

                response = ReadResponse();
                if (response[0] == 0x00)
                {
                    break;
                }
                numberRegistrationReport--;
            }
            
            if (response[0] == 0x00)
            {
                int position = 1;
                try
                {
                    while (position < response.Length - 2)
                    {
                        // Читаем тег
                        tag = BitConverter.ToUInt16(response, position);
                        position += 2;

                        // Читаем длину
                        ushort length = BitConverter.ToUInt16(response, position);
                        position += 2;

                        // Читаем сообщение в зависимости от длины
                        message = new byte[length];
                        Array.Copy(response, position, message, 0, length);
                        position += length;

                        switch (tag)
                        {
                            case 1209:
                                int code1209 = message[0];
                                string FFD1209 = "Не определена";
                                switch (code1209)
                                {
                                    case 00:
                                        FFD1209 = "ФН неактивизирован";
                                        break;
                                    case 01:
                                        FFD1209 = "1.05";
                                        break;
                                    case 03:
                                        FFD1209 = "1.1";
                                        break;
                                    case 04:
                                        FFD1209 = "1.2";
                                        break;
                                }
                                data[0, 0] = Convert.ToString(tag);
                                data[0, 1] = "Версия ФФД:";
                                data[0, 2] = FFD1209;
                                break;
                            case 1190:
                                int code1190 = message[0];
                                string FFD1190 = "Не определена";
                                switch (code1190)
                                {
                                    case 00:
                                        FFD1190 = "ФН неактивизирован";
                                        break;
                                    case 01:
                                        FFD1190 = "1.05";
                                        break;
                                    case 03:
                                        FFD1190 = "1.1";
                                        break;
                                    case 04:
                                        FFD1190 = "1.2";
                                        break;
                                }
                                data[1, 0] = Convert.ToString(tag);
                                data[1, 1] = "Версия ФФД ФН:";
                                data[1, 2] = FFD1190;
                                break;
                            case 1041:
                                data[2, 0] = Convert.ToString(tag);
                                data[2, 1] = "Номер ФН:";
                                data[2, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1037:
                                data[3, 0] = Convert.ToString(tag);
                                data[3, 1] = "Регистрационный номер ККТ:";
                                data[3, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1018:
                                data[4, 0] = Convert.ToString(tag);
                                data[4, 1] = "ИНН пользователя:";
                                data[4, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1040:
                                data[5, 0] = Convert.ToString(tag);
                                data[5, 1] = "Номер ФД:";
                                data[5, 2] = Convert.ToString(BitConverter.ToInt16(message, 0));
                                break;
                            case 1012:
                                ulong secondsSinceEpoch = BitConverter.ToUInt32(message, 0);
                                DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(secondsSinceEpoch);
                                data[6, 0] = Convert.ToString(tag);
                                data[6, 1] = "Дата и время ФД:";
                                data[6, 2] = Convert.ToString(dateTime);
                                break;
                            case 1077:
                                byte[] bytes = new byte[4];
                                Array.Copy(message, message.Length - 4, bytes, 0, 4);
                                Array.Reverse(bytes);
                                data[7, 0] = Convert.ToString(tag);
                                data[7, 1] = "Фискальный признак документа:";
                                data[7, 2] = Convert.ToString(BitConverter.ToUInt32(bytes, 0));
                                break;
                            case 1017:
                                data[8, 0] = Convert.ToString(tag);
                                data[8, 1] = "ИНН ОФД:";
                                data[8, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1062:
                                string binarySNO = "";
                                int i = 0;
                                foreach (byte b in message)
                                {
                                    if (Convert.ToString(b, 2) == "1")
                                    {
                                        binarySNO += Convert.ToString(i);
                                    }
                                }
                                data[9, 0] = Convert.ToString(tag);
                                data[9, 1] = "Системы налогообложения:";
                                data[9, 2] = binarySNO;
                                break;
                            case 1056:
                                data[10, 0] = Convert.ToString(tag);
                                data[10, 1] = "Признак шифрования:";
                                data[10, 2] = BitConverter.ToString(message);
                                break;
                            case 1002:
                                data[11, 0] = Convert.ToString(tag);
                                data[11, 1] = "Признак автономного режима:";
                                data[11, 2] = BitConverter.ToString(message);
                                break;
                            case 1001:
                                data[12, 0] = Convert.ToString(tag);
                                data[12, 1] = "Признак автоматического режима:";
                                data[12, 2] = BitConverter.ToString(message);
                                break;
                            case 1290: //Признаки применения ККТ
                                string binaryString = string.Empty;
                                foreach (byte b in message)
                                {
                                    binaryString = Convert.ToString(b, 2).PadLeft(8, '0') + binaryString;
                                }
                                data[13, 0] = Convert.ToString(tag);
                                data[13, 1] = "Признаки применения ККТ:";
                                data[13, 2] = binaryString;
                                break;
                            case 1213:
                                ushort number = BitConverter.ToUInt16(message, 0);
                                data[14, 0] = Convert.ToString(tag);
                                data[14, 1] = "Ресурс ключей ФП:";
                                data[14, 2] = number.ToString();
                                break;
                            case 1048:
                                data[15, 0] = Convert.ToString(tag);
                                data[15, 1] = "Наименование пользователя:";
                                data[15, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1009:
                                data[16, 0] = Convert.ToString(tag);
                                data[16, 1] = "Адрес расчетов:";
                                data[16, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1187:
                                data[17, 0] = Convert.ToString(tag);
                                data[17, 1] = "Место расчетов:";
                                data[17, 2] = Encoding.GetEncoding(1251).GetString(message);
                                break;
                            case 1021:
                                data[18, 0] = Convert.ToString(tag);
                                data[18, 1] = "Кассир:";
                                data[18, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1036:
                                data[19, 0] = Convert.ToString(tag);
                                data[19, 1] = "Номер автомата:";
                                data[19, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1046:
                                data[20, 0] = Convert.ToString(tag);
                                data[20, 1] = "Наименование ОФД:";
                                data[20, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1117:
                                data[21, 0] = Convert.ToString(tag);
                                data[21, 1] = "Адрес электронной почты отправителя чека:";
                                data[21, 2] = Encoding.GetEncoding(1251).GetString(message);
                                break;
                            case 1060:
                                data[22, 0] = Convert.ToString(tag);
                                data[22, 1] = "Адрес сайта ФНС:";
                                data[22, 2] = Encoding.GetEncoding(1251).GetString(message);
                                break;
                            case 1189:
                                int code = message[0];
                                string FFD = "Не определена";
                                switch (code)
                                {
                                    case 00:
                                        FFD = "ФН неактивизирован";
                                        break;
                                    case 01:
                                        FFD = "1.05";
                                        break;
                                    case 03:
                                        FFD = "1.1";
                                        break;
                                    case 04:
                                        FFD = "1.2";
                                        break;
                                }
                                data[23, 0] = Convert.ToString(tag);
                                data[23, 1] = "Версия ФФД ККТ:";
                                data[23, 2] = FFD;
                                break;
                            case 1188:
                                data[24, 0] = Convert.ToString(tag);
                                data[24, 1] = "Версия ККТ:";
                                data[24, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1013:
                                data[25, 0] = Convert.ToString(tag);
                                data[25, 1] = "Заводской номер ККТ:";
                                data[25, 2] = Encoding.ASCII.GetString(message);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show("Ошибка: " + ex.Message + "\nTag: " + tag + "\nMessage: " + message);
                }

                return data;
            }
            else
            {
                data[0,0] = Convert.ToString((ErrorCode)response[4]);
                return data;
            }
        }
        public string[,] GetRegistrationReportTLVNumber(int numberRegistrationReport)
        {
            string[,] data = new string[26, 3];
            byte[] command = null;
            byte[] response = null;
            byte[] byteArray = new byte[] { (byte)numberRegistrationReport };
            command = CommandGenerator.GetCommand(Command.GET_REGISTRATION_REPORT_TLV, byteArray);
            try
            {
                Port.Write(command, 0, command.Length);
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
            }

            response = ReadResponse();
                  

            if (response[0] == 0x00)
            {
                int position = 1;
                try
                {
                    while (position < response.Length - 2)
                    {
                        // Читаем тег
                        tag = BitConverter.ToUInt16(response, position);
                        position += 2;

                        // Читаем длину
                        ushort length = BitConverter.ToUInt16(response, position);
                        position += 2;

                        // Читаем сообщение в зависимости от длины
                        message = new byte[length];
                        Array.Copy(response, position, message, 0, length);
                        position += length;

                        switch (tag)
                        {
                            case 1209:
                                int code1209 = message[0];
                                string FFD1209 = "Не определена";
                                switch (code1209)
                                {
                                    case 00:
                                        FFD1209 = "ФН неактивизирован";
                                        break;
                                    case 01:
                                        FFD1209 = "1.05";
                                        break;
                                    case 03:
                                        FFD1209 = "1.1";
                                        break;
                                    case 04:
                                        FFD1209 = "1.2";
                                        break;
                                }
                                data[0, 0] = Convert.ToString(tag);
                                data[0, 1] = "Версия ФФД:";
                                data[0, 2] = FFD1209;
                                break;
                            case 1190:
                                int code1190 = message[0];
                                string FFD1190 = "Не определена";
                                switch (code1190)
                                {
                                    case 00:
                                        FFD1190 = "ФН неактивизирован";
                                        break;
                                    case 01:
                                        FFD1190 = "1.05";
                                        break;
                                    case 03:
                                        FFD1190 = "1.1";
                                        break;
                                    case 04:
                                        FFD1190 = "1.2";
                                        break;
                                }
                                data[1, 0] = Convert.ToString(tag);
                                data[1, 1] = "Версия ФФД ФН:";
                                data[1, 2] = FFD1190;
                                break;
                            case 1041:
                                data[2, 0] = Convert.ToString(tag);
                                data[2, 1] = "Номер ФН:";
                                data[2, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1037:
                                data[3, 0] = Convert.ToString(tag);
                                data[3, 1] = "Регистрационный номер ККТ:";
                                data[3, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1018:
                                data[4, 0] = Convert.ToString(tag);
                                data[4, 1] = "ИНН пользователя:";
                                data[4, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1040:
                                data[5, 0] = Convert.ToString(tag);
                                data[5, 1] = "Номер ФД:";
                                data[5, 2] = Convert.ToString(BitConverter.ToInt16(message, 0));
                                break;
                            case 1012:
                                ulong secondsSinceEpoch = BitConverter.ToUInt32(message, 0);
                                DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(secondsSinceEpoch);
                                data[6, 0] = Convert.ToString(tag);
                                data[6, 1] = "Дата и время ФД:";
                                data[6, 2] = Convert.ToString(dateTime);
                                break;
                            case 1077:
                                byte[] bytes = new byte[4];
                                Array.Copy(message, message.Length - 4, bytes, 0, 4);
                                Array.Reverse(bytes);
                                data[7, 0] = Convert.ToString(tag);
                                data[7, 1] = "Фискальный признак документа:";
                                data[7, 2] = Convert.ToString(BitConverter.ToUInt32(bytes, 0));
                                break;
                            case 1017:
                                data[8, 0] = Convert.ToString(tag);
                                data[8, 1] = "ИНН ОФД:";
                                data[8, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1062:
                                string binarySNO = "";
                                int i = 0;
                                foreach (byte b in message)
                                {
                                    if (Convert.ToString(b, 2) == "1")
                                    {
                                        binarySNO += Convert.ToString(i);
                                    }
                                }
                                data[9, 0] = Convert.ToString(tag);
                                data[9, 1] = "Системы налогообложения:";
                                data[9, 2] = binarySNO;
                                break;
                            case 1056:
                                data[10, 0] = Convert.ToString(tag);
                                data[10, 1] = "Признак шифрования:";
                                data[10, 2] = BitConverter.ToString(message);
                                break;
                            case 1002:
                                data[11, 0] = Convert.ToString(tag);
                                data[11, 1] = "Признак автономного режима:";
                                data[11, 2] = BitConverter.ToString(message);
                                break;
                            case 1001:
                                data[12, 0] = Convert.ToString(tag);
                                data[12, 1] = "Признак автоматического режима:";
                                data[12, 2] = BitConverter.ToString(message);
                                break;
                            case 1290: //Признаки применения ККТ
                                string binaryString = string.Empty;
                                foreach (byte b in message)
                                {
                                    binaryString = Convert.ToString(b, 2).PadLeft(8, '0') + binaryString;
                                }
                                data[13, 0] = Convert.ToString(tag);
                                data[13, 1] = "Признаки применения ККТ:";
                                data[13, 2] = binaryString;
                                break;
                            case 1213:
                                ushort number = BitConverter.ToUInt16(message, 0);
                                data[14, 0] = Convert.ToString(tag);
                                data[14, 1] = "Ресурс ключей ФП:";
                                data[14, 2] = number.ToString();
                                break;
                            case 1048:
                                data[15, 0] = Convert.ToString(tag);
                                data[15, 1] = "Наименование пользователя:";
                                data[15, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1009:
                                data[16, 0] = Convert.ToString(tag);
                                data[16, 1] = "Адрес расчетов:";
                                data[16, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1187:
                                data[17, 0] = Convert.ToString(tag);
                                data[17, 1] = "Место расчетов:";
                                data[17, 2] = Encoding.GetEncoding(1251).GetString(message);
                                break;
                            case 1021:
                                data[18, 0] = Convert.ToString(tag);
                                data[18, 1] = "Кассир:";
                                data[18, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1036:
                                data[19, 0] = Convert.ToString(tag);
                                data[19, 1] = "Номер автомата:";
                                data[19, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1046:
                                data[20, 0] = Convert.ToString(tag);
                                data[20, 1] = "Наименование ОФД:";
                                data[20, 2] = Encoding.GetEncoding(866).GetString(message);
                                break;
                            case 1117:
                                data[21, 0] = Convert.ToString(tag);
                                data[21, 1] = "Адрес электронной почты отправителя чека:";
                                data[21, 2] = Encoding.GetEncoding(1251).GetString(message);
                                break;
                            case 1060:
                                data[22, 0] = Convert.ToString(tag);
                                data[22, 1] = "Адрес сайта ФНС:";
                                data[22, 2] = Encoding.GetEncoding(1251).GetString(message);
                                break;
                            case 1189:
                                int code = message[0];
                                string FFD = "Не определена";
                                switch (code)
                                {
                                    case 00:
                                        FFD = "ФН неактивизирован";
                                        break;
                                    case 01:
                                        FFD = "1.05";
                                        break;
                                    case 03:
                                        FFD = "1.1";
                                        break;
                                    case 04:
                                        FFD = "1.2";
                                        break;
                                }
                                data[23, 0] = Convert.ToString(tag);
                                data[23, 1] = "Версия ФФД ККТ:";
                                data[23, 2] = FFD;
                                break;
                            case 1188:
                                data[24, 0] = Convert.ToString(tag);
                                data[24, 1] = "Версия ККТ:";
                                data[24, 2] = Encoding.ASCII.GetString(message);
                                break;
                            case 1013:
                                data[25, 0] = Convert.ToString(tag);
                                data[25, 1] = "Заводской номер ККТ:";
                                data[25, 2] = Encoding.ASCII.GetString(message);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show("Ошибка: " + ex.Message + "\nTag: " + tag + "\nMessage: " + message);
                }
                
                return data;
            }
            else
            {
                MaterialMessageBox.Show("Отчета с таким номером не существует", "Ошибка");
                return null;
            }
        }
        public string GetVersConfig()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_VERS_CONFIG);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    return Encoding.GetEncoding(866).GetString(response.Skip(1).ToArray());
                }

                return string.Empty;
            }

            return string.Empty;
        }
        public byte[] GetNetworkSettings()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_NetworkSettings);
            try
            {
                Port.Write(command, 0, command.Length);
                //MaterialMessageBox.Show("Kоманда" + BitConverter.ToString(command)); // проверка команды
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
            }

            byte[] response = ReadResponse();
            //MaterialMessageBox.Show("Время и дата ответ" + BitConverter.ToString(response)); // проверка команды

            byte[] data = new byte[4];
            int position = 1;

            while (position < response.Length)
            {
                // Читаем тег
                ushort tag = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем длину
                ushort length = BitConverter.ToUInt16(response, position);
                position += 2;

                // Читаем сообщение в зависимости от длины
                byte[] message = new byte[length];
                Array.Copy(response, position, message, 0, length);
                position += length;

                // Конвертируем байты в строку
                switch (tag)
                {
                    case 30001:
                        data[0] = message[0];
                        break;
                }
            }
            return data;
        }
        public void InputNetworkSettings()
        {
            byte[] valueDynamicAddress = { 0x01 };
            string IP = "192.168.0.99";
            string mask = "255.255.255.0";
            string gateway = "192.168.0.1";

            // Стартовые байты
            byte[] startBytes = new byte[] { 0xB6, 0x29 };

            // Получаем byte[] для каждого сообщения
            byte[] hexIP = Encoding.ASCII.GetBytes(IP);
            byte[] hexMask = Encoding.ASCII.GetBytes(mask);
            byte[] hexGateway = Encoding.ASCII.GetBytes(gateway);

            // Определяем длину каждого сообщения
            byte[] lengthMessage1 = BitConverter.GetBytes((ushort)valueDynamicAddress.Length);
            byte[] lengthMessage2 = BitConverter.GetBytes((ushort)hexIP.Length);
            byte[] lengthMessage3 = BitConverter.GetBytes((ushort)hexMask.Length);
            byte[] lengthMessage4 = BitConverter.GetBytes((ushort)hexGateway.Length);

            // Тег сообщения
            byte[] tagMessage1 = new byte[] { 0x31, 0x75 };
            byte[] tagMessage2 = new byte[] { 0x32, 0x75 };
            byte[] tagMessage3 = new byte[] { 0x33, 0x75 };
            byte[] tagMessage4 = new byte[] { 0x34, 0x75 };

            // Общая длина сообщения
            int totalLengthValue = tagMessage1.Length + valueDynamicAddress.Length + lengthMessage1.Length +
                                   tagMessage2.Length + hexIP.Length + lengthMessage2.Length +
                                   tagMessage3.Length + hexMask.Length + lengthMessage3.Length +
                                   tagMessage4.Length + hexGateway.Length + lengthMessage4.Length + 1;

            byte[] totalLength = BitConverter.GetBytes((ushort)totalLengthValue);
            Array.Reverse(totalLength);

            // Формируем массив данных
            byte[] data = startBytes
                .Concat(totalLength)
                .Concat(new byte[] { 0x74 }) // Бит команды
                .Concat(tagMessage1)
                .Concat(lengthMessage1)
                .Concat(valueDynamicAddress)
                .Concat(tagMessage2)
                .Concat(lengthMessage2)
                .Concat(hexIP)
                .Concat(tagMessage3)
                .Concat(lengthMessage3)
                .Concat(hexMask)
                .Concat(tagMessage4)
                .Concat(lengthMessage4)
                .Concat(hexGateway)
                .ToArray();

            // Хвост
            byte[] crc = CRC16CCITT.ComputeCheckSumBytes(data.Skip(2).ToArray());

            byte[] command = data
                .Concat(crc)
                .ToArray();

            Port.Write(command, 0, command.Length);
        }
        public InfoExchangetStatus GetInfoExchange()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_STATUS_INFO_EXCHANGE);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                InfoExchangetStatus result;
                if (response[0] == 0x00)
                {
                    result = new InfoExchangetStatus
                    {
                        Result = ErrorCode.OK,
                        InformationExchangetStatus = response[1],
                        CountDocument = Convert.ToInt16(response[2]),
                        NumberDocument = Convert.ToInt16(response[3]),
                        DateTimeDocument = Convert.ToDateTime(response[4])
                    };
                }
                else
                {
                    result = new InfoExchangetStatus { Result = (ErrorCode)response[1] };
                }
                return result;
            }
            return null;
        }
        public Document GetDocumentNumber(int numberDocument)
        {
            byte[] numberBytes = BitConverter.GetBytes(numberDocument);
            byte[] command = CommandGenerator.GetCommand(Command.GET_DOCUMENT_NUMBER, numberBytes);
            try
            {
                Port.Write(command, 0, command.Length);
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
            }

            byte[] response = ReadResponse();
            if (response != null)
            {
                Document document;
                if (response[0] == 0x00)
                {
                    document = new Document
                    {
                        Result = ErrorCode.OK,
                        Type = response[1],
                        AnswerOFD = response[2],
                        DataDocument = response.Skip(3).ToArray()
                    };
                }
                else
                {
                    document = new Document { Result = (ErrorCode)response[0] };
                }
                return document;
            }
            else
            {
                return null;
            }
        }
        public bool GetShiftStatus()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_SHIFT_STATUS);
            try
            {
                Port.Write(command, 0, command.Length);
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
            }

            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    if (response[1] == 0x01)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    MaterialMessageBox.Show(Convert.ToString((ErrorCode)response[0]));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool PostShiftOpen()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.GET_SHIFT_OPEN_START);
                Port.Write(command, 0, command.Length);
                command = CommandGenerator.GetCommand(Command.GET_SHIFT_OPEN_DATA);
                Port.Write(command, 0, command.Length);
                command = CommandGenerator.GetCommand(Command.GET_SHIFT_OPEN_FINISH);
                Port.Write(command, 0, command.Length);
                return true;
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                return false;
            }
        }
        public bool PostShiftClose()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.GET_SHIFT_CLOSE_START);
                Port.Write(command, 0, command.Length);
                command = CommandGenerator.GetCommand(Command.GET_SHIFT_CLOSE_FINISH);
                Port.Write(command, 0, command.Length);
                return true;
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                return false;
            }
        }
        public bool InputCheckShort()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.INPUT_CHECK_START);
                Port.Write(command, 0, command.Length);
                command = new byte[]
{
    0xB6, 0x29, 0x00, 0xC9, 0x96, 0x06, 0x04, 0x10, 0x00, 0x91, 0xAE, 0xAA, 0x20, 0xA0, 0xAF, 0xA5,
    0xAB, 0xEC, 0xE1, 0xA8, 0xAD, 0xAE, 0xA2, 0xEB, 0xA9, 0x37, 0x04, 0x02, 0x00, 0x98, 0x3A, 0xBE,
    0x04, 0x01, 0x00, 0x04, 0xBC, 0x04, 0x01, 0x00, 0x01, 0xFF, 0x03, 0x02, 0x00, 0x00, 0x01, 0xAF,
    0x04, 0x01, 0x00, 0x01, 0xD0, 0x07, 0x00, 0x00, 0xCD, 0x04, 0x01, 0x00, 0x00, 0xCF, 0x04, 0x00,
    0x00, 0xCE, 0x04, 0x00, 0x00, 0xA7, 0x04, 0x00, 0x00, 0xC6, 0x04, 0x01, 0x00, 0x00, 0x33, 0x04,
    0x0A, 0x00, 0x39, 0x32, 0x30, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x14, 0x04, 0x18, 0x00,
    0xAF, 0xA5, 0xE0, 0xA5, 0xA2, 0xAE, 0xA4, 0x20, 0xA4, 0xA5, 0xAD, 0xA5, 0xA6, 0xAD, 0xEB, 0xE5,
    0x20, 0xE1, 0xE0, 0xA5, 0xA4, 0xE1, 0xE2, 0xA2, 0x31, 0x04, 0x0A, 0x00, 0x38, 0x38, 0x38, 0x38,
    0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x32, 0x04, 0x0A, 0x00, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39,
    0x39, 0x39, 0x39, 0x39, 0x02, 0x04, 0x00, 0x00, 0xED, 0x03, 0x00, 0x00, 0xF8, 0x03, 0x00, 0x00,
    0x93, 0x04, 0x00, 0x00, 0xC9, 0x04, 0x00, 0x00, 0xCA, 0x04, 0x0C, 0x00, 0x37, 0x37, 0x34, 0x30,
    0x30, 0x30, 0x30, 0x30, 0x37, 0x36, 0x20, 0x20, 0xEC, 0x04, 0x11, 0x00, 0xEE, 0x04, 0x01, 0x00,
    0x00, 0xEF, 0x04, 0x00, 0x00, 0xF0, 0x04, 0x00, 0x00, 0xF1, 0x04, 0x00, 0x00, 0x31, 0xC2
};
                Port.Write(command, 0, command.Length);
                byte [] response = ReadResponse();
                if (response[1] == 0xDD)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                return false;
            }
        }
        public bool Registration_12(DataKKT dataKKT)
        {
            // Стартовые байты
            byte[] startBytes = new byte[] { 0xB6, 0x29 };

            // Получаем byte[] для каждого сообщения
            byte[] hexNameOrganization = Encoding.GetEncoding(866).GetBytes(dataKKT.NameOrganization);
            byte[] hexAddressPayment = Encoding.GetEncoding(866).GetBytes(dataKKT.AddressPayment);
            byte[] hexPlacePayment = Encoding.GetEncoding(866).GetBytes(dataKKT.PlacePayment);
            byte[] hexNameCashier = Encoding.GetEncoding(866).GetBytes(dataKKT.NameCashier);
            byte[] hexINNOFD = Encoding.GetEncoding(866).GetBytes(dataKKT.INNOFD);
            byte[] hexNameOFD = Encoding.GetEncoding(866).GetBytes(dataKKT.NameOFD);
            byte[] hexEmailOFD = Encoding.GetEncoding(866).GetBytes(dataKKT.EmailOFD);

            byte conditionsForUsingKKT = 0x00;
            if (dataKKT.PrAkxiz)
            {
                conditionsForUsingKKT = 0x01;
            }
            if (dataKKT.PrMark)
            {
                conditionsForUsingKKT = 0x04;
            }

            byte operatingMode = 0x00;
            if (dataKKT.PrDelivery)
            {
                operatingMode = 0x03;
            }
            if (dataKKT.PrInternet)
            {
                operatingMode = 0x05;
            }

            // Общая длина сообщения
            int totalLengthValue = hexNameOrganization.Length + hexAddressPayment.Length + hexPlacePayment.Length +
                                   hexNameCashier.Length + hexINNOFD.Length + hexNameOFD.Length +
                                   hexEmailOFD.Length + 2;

            byte[] totalLength = BitConverter.GetBytes((ushort)totalLengthValue);
            Array.Reverse(totalLength);

            // Формируем массив данных
            byte[] data = startBytes
                .Concat(totalLength)
                .Concat(new byte[] { (byte)Command.REGISTRATION_12_DATA })
                .Concat(hexNameOrganization)
                .Concat(hexAddressPayment)
                .Concat(hexPlacePayment)
                .Concat(hexNameCashier)
                .Concat(hexINNOFD)
                .Concat(hexNameOFD)
                .Concat(hexEmailOFD)
                .Concat(new[] { conditionsForUsingKKT, operatingMode })
                .ToArray();

            // Хвост
            byte[] crc = CRC16CCITT.ComputeCheckSumBytes(data.Skip(2).ToArray());

            byte[] command = data
                .Concat(crc)
                .ToArray();

            MaterialMessageBox.Show($"REGISTRATION_12_DATA {BitConverter.ToString(command)}");

            // Массив завершения регистрации
            byte[] hexINNOrganization = Encoding.GetEncoding(866).GetBytes(dataKKT.INNOrganization);
            byte[] hexRNM = Encoding.GetEncoding(866).GetBytes(dataKKT.RNM);

            byte[] dataFinish = hexINNOrganization
            .Concat(hexRNM)
            .ToArray();


            try
            {
                command = CommandGenerator.GetCommand(Command.REGISTRATION_12_START);
                MaterialMessageBox.Show($"REGISTRATION_12_START {BitConverter.ToString(command)}");
                //Port.Write(command, 0, command.Length);
                //command = CommandGenerator.GetCommand(Command.REGISTRATION_12_DATA, data);
                //MaterialMessageBox.Show($"REGISTRATION_12_DATA {BitConverter.ToString(command)}");
                //Port.Write(command, 0, command.Length);
                command = CommandGenerator.GetCommand(Command.REGISTRATION_12_FINISH, dataFinish);
                MaterialMessageBox.Show($"REGISTRATION_12_Finish {BitConverter.ToString(command)}");
                return true;
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                return false;
            }
        }

        public ErrorCode OpenSession()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.BEGIN_OPEN_SESSION, new byte[] { 0x01 });
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    command = CommandGenerator.GetCommand(Command.OPEN_SESSION);
                    Port.Write(command, 0, command.Length);
                    response = ReadResponse();
                    if (response[0] == 0x00)
                    {
                        return ErrorCode.OK;
                    }
                    else
                    {
                        return (ErrorCode)response[1];
                    }
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        public ErrorCode CloseSession()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.BEGIN_CLOSE_SESSION, new byte[] { 0x01 });
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    command = CommandGenerator.GetCommand(Command.CLOSE_SESSION);
                    Port.Write(command, 0, command.Length);
                    response = ReadResponse();
                    if (response[0] == 0x00)
                    {
                        return ErrorCode.OK;
                    }
                    else
                    {
                        return (ErrorCode)response[1];
                    }
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        private byte[] ReadResponse()
        {
            DateTime startTime = DateTime.Now;

            while (DateTime.Now.Subtract(startTime) < TimeSpan.FromSeconds(READ_TIMEOUT))
            {
                Thread.Sleep(250);
                try
                {
                    if (Port.BytesToRead != 0)
                    {
                        byte[] response = new byte[Port.BytesToRead];
                        Port.Read(response, 0, response.Length);
                        if (ResponseParser.IsValid(response))
                        {
                            return ResponseParser.GetData(response);
                        }
                    }
                }
                catch
                {
                    // empty
                }
            }

            return null;
        }

        private void SetSubject(string subjectName, double price)
        {
            ushort code, length;
            DoubleValue dv = ToPrice(price);
            DoubleValue dv2 = ToCount(1);
            code = 1059;
            length = 20;
            length += (ushort)(2 + subjectName.Length + dv.Size + dv2.Size);
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);

            code = 1030;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            byte[] buffer = Encoding.GetEncoding(866).GetBytes(subjectName);
            length = (ushort)buffer.Length;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = buffer[i];

            code = 1079;
	        TLV[TLVPosition++] = (byte)code;
	        TLV[TLVPosition++] = (byte)(code >> 8);
	        length = dv.Size;
	        TLV[TLVPosition++] = (byte)length;
	        TLV[TLVPosition++] = (byte)(length >> 8);
	        for(ushort i = 0; i < length; i++) TLV[TLVPosition++] = dv.Value[i];

            code = 1023;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            length = dv2.Size;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = dv2.Value[i];

            code = 1199;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = 1;
            TLV[TLVPosition++] = 0;
            TLV[TLVPosition++] = 6;

            code = 1214;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = 1;
            TLV[TLVPosition++] = 0;
            TLV[TLVPosition++] = 4;
        }

        private void SetParameterString(ushort code, string value)
        {
            byte[] biffer = Encoding.GetEncoding(866).GetBytes(value);
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            ushort length = (ushort)biffer.Length;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = biffer[i];
        }

        private void SetParameterInt8(ushort code, byte value)
        {
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = 1;
            TLV[TLVPosition++] = 0;
            TLV[TLVPosition++] = value;
        }

        private void SetParameterDV(ushort code, DoubleValue dv)
        {
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            ushort length = dv.Size;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = dv.Value[i];
        }

        private DoubleValue ToPrice(double value)
        {
            DoubleValue dv = new DoubleValue();
	        int number = (int)Math.Truncate(value * 100);

	        int step = 0;
	        byte vl;

	        do
	        {
                vl = (byte)(number >> step * 8);
                dv.Value[step] = vl;
		        step++;
	        }
            while (vl != 0);
            dv.Size = (byte)(step - 1);

            return dv;
        }

        private DoubleValue ToPrice5(double value)
        {
            DoubleValue dv = new DoubleValue();
	        dv.Size = 5;
	        int number = (int)Math.Truncate(value * 100);
	        byte vl;

	        for(int i = 0; i < dv.Size; i++)
	        {
		        vl = (byte)(number >> i * 8);
                dv.Value[i] = vl;
	        }

            return dv;
        }

        private DoubleValue ToCount(double value)
        {
            DoubleValue dv = new DoubleValue();
	        byte position = 0;
	        int number = (int)Math.Truncate(value);
	        while(value - number > 0.000001)
	        {
		        position++;
		        value *= 10;
		        number = (int)Math.Truncate(value);
	        }

	        int step = 0;
	        dv.Value[step++] = position;
	        byte vl;
	        do
	        {
		        vl = (byte)(number >> (step - 1) * 8);
                dv.Value[step] = vl;
		        step++;
	        }
            while (vl != 0);
            dv.Size = (byte)(step - 1);

            return dv;
        }
    }
}