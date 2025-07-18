
namespace KitCashProtocol
{
    enum Command
    {
        GET_STATUS_KKT = 0x01,
        GET_ZN = 0x02,
        GET_MODEL = 0x04,
        GET_FN = 0x05,
        GET_STATUS_FN = 0x08,
        GET_SHIFT_STATUS = 0x20,
        GET_SHIFT_OPEN_START = 0x21,
        GET_SHIFT_OPEN_DATA = 0x2F,
        GET_SHIFT_OPEN_FINISH = 0x22,
        GET_SHIFT_CLOSE_START = 0x29,
        GET_SHIFT_CLOSE_FINISH = 0x2A,
        INPUT_CHECK_START = 0x23,
        INPUT_CHECK_DATA = 0x2B,
        GET_DOCUMENT_NUMBER = 0x30,
        GET_STATUS_INFO_EXCHANGE = 0x50,
        INPUT_DATATIME = 0x72,
        GET_DATATIME = 0x73,
        INPUT_NetworkSettings = 0x74,
        GET_NetworkSettings = 0x75,
        INPUT_PARAMETERS_OFD = 0x76,
        GET_PARAMETERS_OFD = 0x77,
        GET_OISM = 0x83,
        GET_KM = 0x85,
        GET_REGISTRATION_REPORT_TLV = 0x3B,

        REGISTRATION_12_START = 0xE1,
        REGISTRATION_12_DATA = 0xE2,
        REGISTRATION_12_FINISH = 0xE3,

        CLOSE_FN_START = 0x14,
        CLOSE_FN_DATA = 0x17,
        CLOSE_FN_FINISH = 0x28,

        GET_VERS_CONFIG = 0x0B,
        GET_FISCAL_STORAGE_STATUS = 0x08,
        BEGIN_OPEN_SESSION = 0x21,
        OPEN_SESSION = 0x22,
        BEGIN_CLOSE_SESSION = 0x29,
        CLOSE_SESSION = 0x2A,
        BEGIN_CHECK = 0x23,
        CHECK_POSITION = 0x2B,
        AGENT_DATA = 0x2C,
        PAYMENT_DATA = 0x2D,
        CHECK = 0x24,
        PRINT = 0x61,
        CUT = 0x62,
        
        REGISTRATION_PARAMETERS = 0x0A,
        CANCEL_DOCUMENT = 0x10
    }
}