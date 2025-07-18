using KitCashProtocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.services.kkt
{
    internal class Shift
    {
        public bool Open(bool statusConnection, string portName)
        {
            TerminalFA CashRegister = new TerminalFA();
            statusConnection = CashRegister.OpenConnection(statusConnection, portName);
            bool shiftStatus = CashRegister.PostShiftOpen();
            CashRegister.CloseConnection(statusConnection);
            return shiftStatus;
        }
        public bool Close(bool statusConnection, string portName)
        {
            TerminalFA CashRegister = new TerminalFA();
            statusConnection = CashRegister.OpenConnection(statusConnection, portName);
            bool shiftStatus = CashRegister.PostShiftClose();
            CashRegister.CloseConnection(statusConnection);
            return shiftStatus;
            
        }
        public bool CheckStatus(bool statusConnection, string portName)
        {
            TerminalFA CashRegister = new TerminalFA();
            statusConnection = CashRegister.OpenConnection(statusConnection, portName);
            bool shiftStatus = CashRegister.GetShiftStatus();
            CashRegister.CloseConnection(statusConnection);
            return shiftStatus;
        }
    }
}
