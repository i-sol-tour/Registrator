using KitCashProtocol;
using MaterialSkin.Controls;
using Registrator.repo.models;
using Registrator.services.kkt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Registrator.repo;
using System.Data;

namespace Registrator.services
{
    internal class RegistrationReportKKT
    {
        public (DataKKT, KKTParameters, FNStatusParsed, TerminalFAStatus) ReadingRegistrationReportKKT(bool statusConnectionKKT, SettingsProgram settings, bool statusNetworkSetting)
        {
            TerminalFA CashRegister = new TerminalFA();
            DataKKT dataKKT = new DataKKT();
            KKTParameters kktParameters = new KKTParameters();
            FNStatusParsed FNStatusParsed = new FNStatusParsed();
            TerminalFAStatus status_KKT = new TerminalFAStatus();
            try
            {
                statusConnectionKKT = CashRegister.OpenConnection(statusConnectionKKT, settings.PortName);
                if (statusConnectionKKT == true)
                {
                    string dataTime_KKT = "01.01.2000 00:00";
                    dataTime_KKT = CashRegister.GetDATETIME(); // запрос времени в ККТ
                    kktParameters.DateTimeKKTSetting = dataTime_KKT;
                    DateTime dateTime;
                    DateTime.TryParseExact(dataTime_KKT, "dd.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dateTime);
                    DateTime dateTime_PK = DateTime.Now; // Получаем текущее время на ПК
                    TimeSpan difference = dateTime_PK - dateTime;// Сравниваем разницу во времени      
                    if (Math.Abs(difference.TotalMinutes) > 5)
                    {
                        DialogResult result = MaterialMessageBox.Show("Разница во времени на ККТ и в ПК более 5 минут. Синхронизировать время с ПК?", "Уведомление", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                DateTime now = DateTime.Now;
                                CashRegister.InputDATETIME(now);
                            }
                            catch { MaterialMessageBox.Show("Не удалось синхронизировать время в ККТ"); }
                        }
                    }

                    kktParameters.VersionConfig = CashRegister.GetVersConfig().Replace("rw", "");// запрос версии конфигурации

                    NetworkSetting networkSetting = new NetworkSetting(); // проверка сетевых настроек
                    kktParameters.StatusNetworkSetting = networkSetting.CheckAndInput(statusConnectionKKT, settings.PortName, false);

                    string zn_kkt = "";
                    string zn_fn = "";

                    zn_kkt = CashRegister.GetZN(); // запрос ЗН ККТ

                    try
                    {
                        status_KKT = CashRegister.GetStatusKKT(); // запрос статуса ККТ
                        if (status_KKT.FNThereis == 1) // если ФН подключен
                        {
                            zn_fn = CashRegister.GetFN(); // запрос ЗН ФН

                            KktResponseParser responseParser = new KktResponseParser(statusConnectionKKT, settings);
                            FNStatusParsed = responseParser.ParseResponseStatusFN();


                            if (FNStatusParsed.Phase != "ФН не зарегистрирован")
                            {
                                string zn_kkt_report = "";
                                string zn_fn_report = "";
                                string[,] data = new string[30, 3];
                                data = CashRegister.GetRegistrationReportTLVLast();
                                string report = "";
                                for (int i = 0; i < data.GetLength(0); i++)
                                {
                                    report += $"{data[i, 0]} | {data[i, 1]} {data[i, 2]}\n";
                                    int tag = Convert.ToInt32(data[i, 0]);
                                    string message = Convert.ToString(data[i, 2]);
                                    switch (tag)
                                    {
                                        case 1041:
                                            zn_fn_report = message;
                                            break;
                                        case 1037:
                                            dataKKT.RNM = message;
                                            break;
                                        case 1018:
                                            dataKKT.INNOrganization = message;
                                            break;
                                        case 1040:
                                            dataKKT.NumberFD = message;
                                            break;
                                        case 1012:
                                            dataKKT.DataTimeFD = message;
                                            break;
                                        case 1077:
                                            dataKKT.FP = message;
                                            break;
                                        case 1017:
                                            dataKKT.INNOFD = message;
                                            break;
                                        case 1062:
                                            for (int j = 0; j < message.Length; j++)
                                            {
                                                char bit = message[j];
                                                switch (bit)
                                                {
                                                    case '0':
                                                        dataKKT.SNO_OSN = true;
                                                        break;
                                                    case '1':
                                                        dataKKT.SNO_USN_D = true;
                                                        break;
                                                    case '2':
                                                        dataKKT.SNO_USN_D_R = true;
                                                        break;
                                                    case '4':
                                                        dataKKT.SNO_ESHN = true;
                                                        break;
                                                    case '5':
                                                        dataKKT.SNO_PATENT = true;
                                                        break;
                                                }
                                            }
                                            break;
                                        case 1290:
                                            if (message.Length < 11)
                                            {
                                                throw new ArgumentException("Двоичный код должен содержать как минимум 11 бит.");
                                            }
                                            dataKKT.PrInternet = message[message.Length - 6] == '1'; // 5 бит
                                            dataKKT.PrAkxiz = message[message.Length - 7] == '1'; // 6 бит
                                            dataKKT.PrMark = message[message.Length - 9] == '1'; // 8 бит
                                            dataKKT.PrDelivery = message[message.Length - 10] == '1'; // 9 бит
                                            dataKKT.PrAzart = message[message.Length - 11] == '1'; // 10 бит
                                            dataKKT.PrLotereya = message[message.Length - 12] == '1'; // 11 бит

                                            break;
                                        case 1048:
                                            dataKKT.NameOrganization = message;
                                            break;
                                        case 1009:
                                            dataKKT.AddressPayment = message;
                                            break;
                                        case 1187:
                                            dataKKT.PlacePayment = message;
                                            break;
                                        case 1021:
                                            dataKKT.NameCashier = message;
                                            break;
                                        case 1046:
                                            message.Replace("<", "\"");
                                            message.Replace(">", "\"");
                                            dataKKT.NameOFD = message;
                                            break;
                                        case 1117:
                                            dataKKT.EmailOFD = message;
                                            break;
                                        case 1189:
                                            dataKKT.VersionFFD = message;
                                            break;
                                        case 1013:
                                            zn_kkt_report = message;
                                            break;
                                    }
                                }
                                if (zn_kkt != zn_kkt_report)
                                {
                                    MaterialMessageBox.Show("Заводской номер ККТ в отчете не совпадает с настройками кассы", "Ошибка");
                                }
                                if (zn_fn != zn_fn_report)
                                {
                                    MaterialMessageBox.Show("Номер ФН в отчете не совпадает с настройками кассы", "Ошибка");
                                }
                            }
                        }
                        else
                        {
                            MaterialMessageBox.Show("ФН не подключен", "Сообщение");
                            if (kktParameters.VersionConfig[2] == '3' || kktParameters.VersionConfig == "1.51")
                            {
                                kktParameters.VersionFFD = "1.05";
                            }
                            else
                            {
                                kktParameters.VersionFFD = "1.2";
                            }
                        }
                    }
                    catch
                    {
                        MaterialMessageBox.Show("Ошибка чтения данных ФН", "Ошибка");
                        if (kktParameters.VersionConfig[2] == '3' || kktParameters.VersionConfig == "1.51")
                        {
                            kktParameters.VersionFFD = "1.05";
                        }
                        else
                        {
                            kktParameters.VersionFFD = "1.2";
                        }
                    }
                    dataKKT.ZN_KKT = zn_kkt;
                    dataKKT.NumberFN = zn_fn;
                }
            }
            catch
            {
                MaterialMessageBox.Show("Не удалось считать данные с ККТ", "Ошибка");
            }
            finally
            {
                statusConnectionKKT = CashRegister.CloseConnection(statusConnectionKKT);
            }
            return (dataKKT, kktParameters, FNStatusParsed, status_KKT);
        }
    }
}
