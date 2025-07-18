using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.repo.models
{
    public class SettingsProgram
    {
        public string AdressFile { get; set; }
        public bool DeleteXML { get; set; }
        public bool PrintAkt { get; set; }
        public bool CreateFolder { get; set; }
        public string NameOperator { get; set; }
        public string StandartModelFN { get; set; }
        public string StandartOFD { get; set; }
        public string PortName { get; set; }
    }
}
