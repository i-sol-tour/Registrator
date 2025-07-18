using System;
using System.ComponentModel;

namespace KitCashProtocol
{
    public enum ErrorCode : byte
    {
        [Description("Успешно")]
        OK = 0,

        [Description("Неверный формат команды")]
        WrongFormat = 0x01,

        [Description("Данная команда требует другого состояния ФН")]
        WrongState = 0x02,

        [Description("Ошибка ФН")]
        FiscalStorageError = 0x03,

        [Description("Ошибка KC")]
        CryptoprocessorError = 0x04,

        [Description("Закончен срок эксплуатации ФН")]
        FiscalStorageLifeTime = 0x05,

        [Description("Архив ФН переполнен")]
        ArchiveIsFull = 0x06,

        [Description("Дата и время операции не соответствуют логике работы ФН")]
        WrongTime = 0x07,

        [Description("Запрошенные данные отсутствуют в Архиве ФН")]
        NoData = 0x08,

        [Description("Параметры команды имеют правильный формат, но их значение не верно")]
        ParametersWrongFormat = 0x09,

        [Description("Превышение размеров TLV данных")]
        TLVDataExceeding = 0x10,

        [Description("Исчерпан ресурс КС. Требуется закрытие фискального режима")]
        CryptoprocessorResourceExhausted = 0x12,

        [Description("Ресурс хранения документов для ОФД исчерпан")]
        OFDResourceExhausted = 0x14,

        [Description("Превышено время ожидания передачи сообщения (30 дней)")]
        WaitingTimeExceeded = 0x15,

        [Description("Продолжительность смены более 24 часов")]
        Session24 = 0x16,

        [Description("Неверная разница во времени между 2 операциями (более 5 минут)")]
        WrongTimeDifference = 0x17,

        [Description("Сообщение от ОФД не может быть принято")]
        MessageCanNotBeAccepted = 0x20,

        [Description("Неверная структура команды, либо неверная контрольная сумма")]
        WrongCommand = 0x25,

        [Description("Неизвестная команда")]
        UnknownCommand = 0x26,

        [Description("Неверная длина параметров команды")]
        WrongLength = 0x27,

        [Description("Неверный формат или значение параметров команды")]
        WrongParametersFormat = 0x28,

        [Description("Нет связи с ФН")]
        FiscalStorageNoConnect = 0x30,

        [Description("Неверные дата/время в ККТ")]
        WrongDateTime = 0x31,

        [Description("Переданы не все необходимые данные")]
        NotFullData = 0x32,

        [Description("РНМ сформирован неверно, проверка на данной ККТ не прошла")]
        WrongRN = 0x33,

        [Description("Данные уже были переданы ранее")]
        AlreadyTransferred = 0x34,

        [Description("Аппаратный сбой ККТ")]
        HardwareError = 0x35,

        [Description("Неверно указан признак расчета, возможные значения: приход, расход, возврат прихода, возврат расхода")]
        WrongCalculationSign = 0x36,

        [Description("Указанный налог не может быть применен")]
        WrongTax = 0x37,

        [Description("Данные необходимы только для платежного агента (указано при регистрации)")]
        DataForAhentOnly = 0x38,

        [Description("Итоговая сумма оплаты не равна стоимости предметов расчета")]
        WrongSum = 0x39,

        [Description("Некорректный статус печатающего устройства")]
        WrongStatePrinter = 0x40,

        [Description("Ошибка сохранения настроек")]
        SavingSettingsError = 0x50,

        [Description("Передано некорректное значение времени")]
        WrongTimeValue = 0x51,

        [Description("В чеке не должны присутствовать иные предметы расчета помимо предмета расчета с признаком способа расчета Оплата кредита")]
        OtherCalculationSubject = 0x52,

        [Description("Переданы не все необходимые данные для агента")]
        FewDataForAgent = 0x53,

        [Description("Итоговая сумма чека не равна сумме оплаты всеми видами")]
        WrongSum2 = 0x54,

        [Description("Неверно указан признак расчета для чека коррекции, возможные значения: приход, расход")]
        WrongCalculationSignCorrection = 0x55,

        [Description("Неверная структура переданных данных для агента")]
        WrongDataForAgent = 0x56,

        [Description("Не указан режим налогообложения")]
        NoTaxMode = 0x57,

        [Description("Данная ставка НДС недопустима для агента. Агент не является плательщиком НДС")]
        WrongTaxForAgent = 0x58,

        [Description("Некорректно указано значение тэга Признак платежного агента")]
        WrongTaxValueSign = 0x59,

        [Description("Номер блока прошивки указан некорректно")]
        WrongBlockFirmware = 0x60,

        [Description("Присутствуют неотправленные в ОФД документы")]
        NotSendedDocuments = 0xE0,

        [Description("Подключенный ФН не соответствует данным регистрации ККТ")]
        WrongRegistrationData = 0xF3,

        [Description("Неизвестная ошибка")]
        UnknownError = 0xFF
    }

    public enum TerminalFAPrinterStatus
    {
        OK = 0,
        NoDevice = 1,
        NoPaper = 2,
        PaperJammed = 3,
        OpenBox = 5,
        CutterError = 6,
        HardwareError = 7
    }

    public class TerminalFAStatus
    {
        public ErrorCode Result { get; set; }
        public string FactoryNumber { get; set; }
        public DateTime CurrentDateTime { get; set; }
        public bool FatalErrors { get; set; }
        public TerminalFAPrinterStatus PrinterStatus { get; set; }
        public byte FNThereis { get; set; }
    }
    public class FNStatus
    {
        public ErrorCode Result { get; set; }
        public byte Phase { get; set; }
        public byte Document { get; set; }
        public byte StatusShift { get; set; }
        public int NumberLastDocument { get; set; }

    }
    public class Document
    {
        public ErrorCode Result { get; set; }
        public byte Type { get; set; }
        public byte AnswerOFD { get; set; }
        public byte[] DataDocument { get; set; }
    }
    
    public class InfoExchangetStatus
    {
        public ErrorCode Result { get; set; }
        public byte InformationExchangetStatus { get; set; }
        public int CountDocument { get; set; }
        public int NumberDocument { get; set; }
        public DateTime DateTimeDocument { get; set; }
    }
    public class FiscalStorageStatus
    {
        public ErrorCode Result { get; set; }
        public byte CurrentDocument { get; set; }
        public bool SessionIsOpen { get; set; }
    }
}