using KitCashProtocol;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.services.kkt
{
    internal class NetworkSetting
    {
        TerminalFA CashRegister = new TerminalFA();
        public bool CheckAndInput(bool statusConnectionKKT, string portName, bool statusNetworkSetting)
        {
            if (statusNetworkSetting == false)
            {
                bool local_statusConnectionKKT = statusConnectionKKT;
                try
                {
                    statusConnectionKKT = CashRegister.OpenConnection(statusConnectionKKT, portName);
                    if (statusConnectionKKT == true)
                    {
                        byte[] networkSetting = CashRegister.GetNetworkSettings();
                        if (networkSetting[0] == 0)
                        {
                            CashRegister.InputNetworkSettings();
                            MaterialMessageBox.Show("Сетевый настройки переключены на DHCP. Перезагрузите кассу", "Уведомление");
                        }
                        statusNetworkSetting = true;
                    }
                    else
                        MaterialMessageBox.Show("Не удалось подключиться к ККТ", "Ошибка");
                }
                catch
                {
                    MaterialMessageBox.Show("Не удалось ввести сетевые настройки", "Ошибка");
                }
                finally
                {
                    // Если статус подключения изначально пришел false, то мы его тоже закрываем
                    if (local_statusConnectionKKT == false)
                        CashRegister.CloseConnection(statusConnectionKKT);
                }
            }
            return statusNetworkSetting;
        }
    }
}
