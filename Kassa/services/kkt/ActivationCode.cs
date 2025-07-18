using KitCashProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.services.kkt
{
    internal class ActivationCode
    {
        public bool Check(bool statusConnection, string portName) 
        {
            TerminalFA CashRegister = new TerminalFA();
            statusConnection = CashRegister.OpenConnection(statusConnection, portName);

            if (CashRegister.GetShiftStatus() == false) // если смена открыта
            {
                CashRegister.PostShiftOpen();
            }

            bool isCheckShort = CashRegister.InputCheckShort();
            CashRegister.CancelDocument();
            CashRegister.CloseConnection(statusConnection);
            return isCheckShort;
        }
    }
}
