using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator
{
    internal class CheckingFilledFields
    {
        public bool CheckingFilledFields_FileRegistration(bool [] ParameterArray)
        {
            for (int i = 0; i < 18; i++)
            {
                if (i != 10 && i != 11)
                {
                    if (ParameterArray[i] == false)
                    {
                        return false;
                    }
                }
            }
            int counterSNO = 0;
            for (int i = 20; i < 26; i++)
            {
                if (ParameterArray[i] == true)
                {
                    counterSNO++;
                }
            }
            if (counterSNO == 0)
            {
                return false;
            }

            return true;
        }
        public bool CheckingFilledFields_RegistrationKKT(bool[] ParameterArray)
        {
            for (int i = 0; i < 19; i++)
            {
                if (i != 10 && i != 11)
                {
                    if (ParameterArray[i] == false)
                    {
                        return false;
                    }
                }
            }

            for (int i = 14; i < 17; i++)
            {
                if (ParameterArray[i] == false)
                {
                    return false;
                }
            }

            int counterSNO = 0;
            for (int i = 20; i < 26; i++)
            {
                if (ParameterArray[i] == true)
                {
                    counterSNO++;
                }
            }
            if (counterSNO == 0)
            {
                return false;
            }
            

            return true;
        }
        public bool CheckingFilledFields_CreationAkt(bool[] ParameterArray)
        {
            for (int i = 0; i < 9; i++)
            {
                if (ParameterArray[i] == false)
                {
                    return false;
                }
            }

            for (int i = 14; i < 17; i++)
            {
                if (ParameterArray[i] == false)
                {
                    return false;
                }
            }

            for (int i = 17; i < 20; i++)
            {
                if (ParameterArray[i] == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
