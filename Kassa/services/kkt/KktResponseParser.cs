using KitCashProtocol;
using MaterialSkin.Controls;
using Registrator.repo.models;
using System;

namespace Registrator.services
{
    public class KktResponseParser
    {
        TerminalFA CashRegistor = new TerminalFA();
        bool statusConnectionKKT;
        SettingsProgram settings;

        public KktResponseParser( bool _statusConnectionKKT, SettingsProgram _setting ) 
        {
            settings = _setting;
            statusConnectionKKT = _statusConnectionKKT;
        }

        public (bool Success, repo.models.DocumentByNumber document) ParseResponseDocumentByNumber(int documentNumber) //обработка ответа поиска документа по номеру
        {
            Document reportRegistration = null;
            statusConnectionKKT = CashRegistor.OpenConnection(statusConnectionKKT, settings.PortName);

            try
            {
                reportRegistration = CashRegistor.GetDocumentNumber(documentNumber);
            }
            finally
            {
                statusConnectionKKT = CashRegistor.CloseConnection(statusConnectionKKT);
            }

            if (reportRegistration.Result != ErrorCode.OK)
            {
                MaterialMessageBox.Show($"Ошибка: {reportRegistration.Result}");
                return (false, null);
            }

            // Определение типа документа
            string documentType = ParseDocumentType(reportRegistration.Type);

            // Проверка ответа ОФД
            string answerOFD = reportRegistration.AnswerOFD == 1 ? "да" : "нет";

            // Разбор данных документа
            byte[] data = reportRegistration.DataDocument;
            if (data == null || data.Length < 13)
            {
                MaterialMessageBox.Show("Ошибка: некорректные данные документа");
                return (false, null);
            }

            // Парсинг даты и времени
            DateTime documentDateTime = ParseDateTime(data);

            // Парсинг номера ФД и фискального признака
            uint documentNumberFd = BitConverter.ToUInt32(data, 5);
            uint fiscalSign = ParseFiscalSign(data);

            var document = new DocumentByNumber
            {
                Type = documentType,                      // Уже определено в вашем коде
                AnswerOFD = answerOFD,                    // "да"/"нет"
                DateTime = documentDateTime,
                Number = Convert.ToString(documentNumberFd),  // Номер ФД
                FiscalSign = Convert.ToString(fiscalSign) // Фискальный признак
            };

            return (true, document);
        }
        private string ParseDocumentType(byte type)
        {
            switch (type)
            {
                case 0x02: return "отчет об открытии смены";
                case 0x05: return "отчет о закрытии смены";
                case 0x0B: return "отчёт об изменении параметров регистрации";
                case 0x01: return "отчёт о регистрации";
                case 0x03: return "кассовый чек";
                default: return "отчет о закрытии фискального накопителя";
            }
        }
        private DateTime ParseDateTime(byte[] data)
        {
            int year = 2000 + data[0];
            int month = data[1];
            int day = data[2];
            int hour = data[3];
            int minute = data[4];
            return new DateTime(year, month, day, hour, minute, 0);
        }
        private uint ParseFiscalSign(byte[] data)
        {
            byte[] fiscalSignBytes = { data[9], data[10], data[11], data[12] };
            return BitConverter.ToUInt32(fiscalSignBytes, 0);
        }

        public FNStatusParsed ParseResponseStatusFN () //обработка отчета о состоянии ФН
        {
            FNStatus statusFNfromKKT = new FNStatus();
            statusFNfromKKT = CashRegistor.GetStatusFN();

            FNStatusParsed statusFN = new FNStatusParsed();
            switch (statusFNfromKKT.Phase)
            {
                case 0x00:
                    statusFN.Phase = "ФН не зарегистрирован";
                    break;
                case 0x03:
                    statusFN.Phase = "ФН зарегистрирован";
                    break;
                case 0x07:
                    statusFN.Phase = "ФН закрыт, идет передача в ОФД";
                    break;
                case 0x0F:
                    statusFN.Phase = "ФН закрыт, передача в ОФД заверешена";
                    break;
            }
            if (statusFNfromKKT.Document != 0x00)
            {
                statusFN.Document = "открыт";
            }
            else
            {
                statusFN.Document = "закрыт";
            }
            if (statusFNfromKKT.StatusShift != 0x00)
            {
                statusFN.StatusShift = "открыта";
            }
            else
            {
                statusFN.StatusShift = "закрыта";
            }

            statusFN.NumberLastDocument = Convert.ToInt32(statusFNfromKKT.NumberLastDocument);
            return statusFN;
        }
    }
}
