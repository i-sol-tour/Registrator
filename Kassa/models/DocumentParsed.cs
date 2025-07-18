using KitCashProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.repo.models
{
    public class FNStatusParsed
    {
        public ErrorCode Result { get; set; }
        public string Phase { get; set; }
        public string Document { get; set; }
        public string StatusShift { get; set; }
        public int NumberLastDocument { get; set; }
    }

    public class DocumentByNumber
    {
        public string Type { get; set; }
        public string AnswerOFD { get; set; }
        public DateTime DateTime { get; set; }
        public string Number { get; set; }
        public string FiscalSign { get; set; }
    }

    public class DataKKT
    {
        public string ID { get; set; }
        public string RNM { get; set; }
        public string ZN_KKT { get; set; }
        public string NumberAvtomate { get; set; }
        public string NumberFN { get; set; }
        public string ModelFN { get; set; }
        public string NameOrganization { get; set; }
        public string DirectorOrganization { get; set; }
        public string NameCashier { get; set; }
        public string INNOrganization { get; set; }
        public string KPPOrganization { get; set; }
        public bool SNO_OSN { get; set; }
        public bool SNO_USN_D { get; set; }
        public bool SNO_USN_D_R { get; set; }
        public bool SNO_PATENT { get; set; }
        public bool SNO_ESHN { get; set; }
        public string Telephone { get; set; }
        public string EmailOrganization { get; set; }
        public string AddressPayment { get; set; }
        public string PlacePayment { get; set; }
        public string NameOFD { get; set; }
        public string INNOFD { get; set; }
        public string EmailOFD { get; set; }
        public string DataTimeFD { get; set; }
        public string NumberFD { get; set; }
        public string FP { get; set; }
        public string ModelKKT { get; set; }
        public string AdressInternet { get; set; }
        public string VersionFFD { get; set; }

        // Признаки
        public bool PrLotereya { get; set; }
        public bool PrAzart { get; set; }
        public bool PrPlatAgent { get; set; }
        public bool PrInternet { get; set; }
        public bool PrDelivery { get; set; }
        public bool PrAkxiz { get; set; }
        public bool PrMark { get; set; }
    }

    public class KKTParameters
    {
        public string DateTimeKKTSetting { get; set; }
        public string VersionConfig { get; set; }
        public string VersionFFD { get; set; }
        public bool StatusNetworkSetting { get; set; }

    }
}
