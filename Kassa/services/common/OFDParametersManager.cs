using KitCashProtocol;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registrator.repo;
using Registrator.models;

namespace Registrator.services
{
    public class OFDParametersManager
    {
        public TerminalFA CashRegister { get; set; }
        public OptionsOFD optionsOFD { get; set; }
        public void OutputParametersOFD(bool StatusСonnectionKKT, string portName, string versionFFD)
        {
            TerminalFA CashRegister = new TerminalFA();
            if (StatusСonnectionKKT == false)
            {
                StatusСonnectionKKT = CashRegister.OpenConnection(StatusСonnectionKKT, portName);
            }
            try
            {
                if (StatusСonnectionKKT == true)
                {
                    string[] parametersOFD = CashRegister.GetPARAMETERS_OFD();
                    string message = "--------------------------------------------------------------------\n" +
                                         " Адрес ОФД                 | " + parametersOFD[0] + "\n" +
                                         "--------------------------------------------------------------------\n" +
                                         " IP ОФД                          | " + parametersOFD[1] + "\n" +
                                         "--------------------------------------------------------------------\n" +
                                         " Порт                               | " + parametersOFD[2] + "\n" +
                                         "--------------------------------------------------------------------\n" +
                                         " Интервал таймера | " + parametersOFD[3] + "\n" +
                                         "--------------------------------------------------------------------";
                    if (versionFFD == "")
                    {

                    }
                    if (versionFFD == "1.2")
                    {
                        string[] parametersOISM = CashRegister.GetOISM();
                        string[] parametersKeyKM = CashRegister.GetKeyKM();
                        message +=       " Адрес ОИСМ                 | " + parametersOISM[0] + "\n" +
                                         "--------------------------------------------------------------------\n" +
                                         " Порт                               | " + parametersOISM[1] + "\n" +
                                         "--------------------------------------------------------------------\n" +
                                         " Интервал таймера | " + parametersOISM[3] + "\n" +
                                         "--------------------------------------------------------------------\n" +
                                         " Адрес ключа КМ        | " + parametersKeyKM[0] + "\n" +
                                         "--------------------------------------------------------------------\n" +
                                         " Порт                                | " + parametersKeyKM[1] + "\n" +
                                         "--------------------------------------------------------------------";
                    }
                    MaterialMessageBox.Show(message, "Параметры ОФД");
                }
            }
            catch
            {
                MaterialMessageBox.Show("Не удалось получить параметры ОФД","Ошибка");
            }
            finally
            {
                if (StatusСonnectionKKT == true)
                {
                    StatusСonnectionKKT = CashRegister.CloseConnection(StatusСonnectionKKT);
                }
            }
        }
        
        public void InputParametersOFD(bool StatusСonnectionKKT, string portName, string versionFFD, OptionsOFD optionsOFD, OptionsFN optionsFN)
        {
            TerminalFA CashRegister = new TerminalFA();
            if (StatusСonnectionKKT == false)
            {
                StatusСonnectionKKT = CashRegister.OpenConnection(StatusСonnectionKKT, portName);
            }
            try
            {
                if (StatusСonnectionKKT == true)
                {
                    CashRegister.InputPARAMETERS_OFD(optionsOFD.URL, optionsOFD.IP, optionsOFD.TCP, optionsOFD.Timeout);
                    if (versionFFD == "1.2")
                    {
                        CashRegister.InputOISM(optionsOFD.URL_OISM, optionsOFD.TCP_OISM, optionsOFD.Timeout);
                        CashRegister.InputKeyKM(optionsFN.URL, optionsFN.TCP);
                    }
                    MaterialMessageBox.Show("Параметры ОФД успешно введены. Перезагрузите кассу.", "Уведомление");
                }
            }
            catch
            {
                MaterialMessageBox.Show("Не удалось ввести параметры ОФД", "Ошибка");
            }
            finally
            {
                StatusСonnectionKKT = CashRegister.CloseConnection(StatusСonnectionKKT);
            }
        }
    }
}
