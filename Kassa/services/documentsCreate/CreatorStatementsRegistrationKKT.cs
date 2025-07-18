using MaterialSkin.Controls;
using Registrator.repo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Registrator.repo.models;

namespace Registrator.services
{
    internal class CreatorStatementsRegistrationKKT
    {
        private DataKKT dataRegistrationKKT;
        private SettingsProgram settings;
        public bool CreateXmlDocument(DataKKT _dataRegistrationKKT, SettingsProgram _setting)
        {
            dataRegistrationKKT = _dataRegistrationKKT;
            settings = _setting;

            // сведения регистрации ККТ
            string PrAvtonomS = "2"; 
            string PrLotereyaS = "2";
            string PrAzartS = "2";
            string PrBankPlatS = "2";
            string PrPlatAgentS = "2";
            string PrAvtomatUstrS = "2";
            string PrInternetS = "2";
            string PrRazvozS = "2";
            string PrAkxizTovarS = "2";
            string PrMarkS = "2";

            if (dataRegistrationKKT.PrAzart == true) { PrAzartS = "1"; }
            if (dataRegistrationKKT.PrMark == true) { PrMarkS = "1"; }
            if (dataRegistrationKKT.PrPlatAgent == true) { PrPlatAgentS = "1"; }
            if (dataRegistrationKKT.PrLotereya == true) { PrLotereyaS = "1"; }
            if (dataRegistrationKKT.PrInternet == true) { PrInternetS = "1"; }
            if (dataRegistrationKKT.PrDelivery == true) { PrRazvozS = "1"; }
            if (dataRegistrationKKT.PrAkxiz == true) { PrAkxizTovarS = "1"; }

            string[] splitNameOrganization = dataRegistrationKKT.NameOrganization.Split(' ');
            string statusNameOrganization = splitNameOrganization[0];

            DateTime dateNow = DateTime.Today;
            string datestring = Convert.ToString(dateNow);
            datestring = datestring.Substring(0, datestring.Length - 8);

            //dataRegistrationKKT.DirectorOrganization.ToUpper();
            string[] directorOrganization_array = dataRegistrationKKT.DirectorOrganization.ToUpper().Split(' ');// Получение ФИО
            // Обработка массива ФИО в зависимости от количества элементов
            if (directorOrganization_array.Length == 2)
            {
                // Если только 2 элемента (фамилия и имя), добавляем пустое отчество
                Array.Resize(ref directorOrganization_array, 3);
                directorOrganization_array[2] = "";
            }
            else if (directorOrganization_array.Length > 3)
            {
                // Если больше 3 элементов, объединяем все после 2-го в отчество
                string patronymic = string.Join(" ", directorOrganization_array.Skip(2));
                directorOrganization_array = new string[] { directorOrganization_array[0], directorOrganization_array[1], patronymic };
            }

            string[] dateSplit = datestring.Split('.');
            Random rnd = new Random();
            int a = rnd.Next();
            string rand = Convert.ToString(a);

            XmlText Imy2T = null;
            XmlText Familiya2T = null;
            XmlText Otcestvo2T = null;

            string ID_file =
                "KO_ZVLREGKKT_5018_5018_" +
                dataRegistrationKKT.INNOrganization +
                dataRegistrationKKT.KPPOrganization +
                "_" +
                dateSplit[2] +
                dateSplit[1] +
                dateSplit[0] +
                "_" + rand;

            
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load("XML_FNS.xml");
            XmlElement Fail = xmlDocument.DocumentElement;

            //XmlElement Fail = xmlDocument.CreateElement("Файл");
            XmlAttribute VersProg = xmlDocument.CreateAttribute("ВерсПрог");
            XmlAttribute VersForm = xmlDocument.CreateAttribute("ВерсФорм");
            XmlAttribute IdFail = xmlDocument.CreateAttribute("ИдФайл");

            XmlElement Document = xmlDocument.CreateElement("Документ");

            XmlAttribute DataDoc = xmlDocument.CreateAttribute("ДатаДок");
            XmlAttribute KND = xmlDocument.CreateAttribute("КНД");
            XmlAttribute KodNO = xmlDocument.CreateAttribute("КодНО");

            XmlElement SvNP = xmlDocument.CreateElement("СвНП");

            //if (NOrganization == "ИП")
            //{

            XmlElement NPFL = xmlDocument.CreateElement("НПФЛ");

            XmlAttribute INNFL = xmlDocument.CreateAttribute("ИННФЛ");

            XmlElement FIO = xmlDocument.CreateElement("ФИО");

            XmlAttribute Imy = xmlDocument.CreateAttribute("Имя");
            XmlAttribute Otcestvo = xmlDocument.CreateAttribute("Отчество");
            XmlAttribute Familiya = xmlDocument.CreateAttribute("Фамилия");
            //}
            //else
            //{
            XmlElement NPYUL = xmlDocument.CreateElement("НПЮЛ");
            XmlAttribute INNYUL = xmlDocument.CreateAttribute("ИННЮЛ");
            XmlAttribute KPP = xmlDocument.CreateAttribute("КПП");
            XmlAttribute NaimOrg = xmlDocument.CreateAttribute("НаимОрг");
            //}
            //</НПФЛ>
            //</СвНП>
            XmlElement Podpisant = xmlDocument.CreateElement("Подписант");

            XmlAttribute PrPodp = xmlDocument.CreateAttribute("ПрПодп");

            XmlElement FIO2 = xmlDocument.CreateElement("ФИО");

            XmlAttribute Imy2 = xmlDocument.CreateAttribute("Имя");
            XmlAttribute Otcestvo2 = xmlDocument.CreateAttribute("Отчество");
            XmlAttribute Familiya2 = xmlDocument.CreateAttribute("Фамилия");

            //XmlElement SvPred = xmlDocument.CreateElement("СвПред");

            //XmlAttribute NaimDoc = xmlDocument.CreateAttribute("НаимДок");

            //</Подписант>

            XmlElement ZayavRegKKT = xmlDocument.CreateElement("ЗаявРегККТ");

            //XmlAttribute RegNomerKKT = xmlDocument.CreateAttribute("РегНомерККТ");
            XmlAttribute VidDoc = xmlDocument.CreateAttribute("ВидДок");
            XmlAttribute KodNOMUst = xmlDocument.CreateAttribute("КодНОМУст");
            //XmlAttribute PrAvtonomRezim = xmlDocument.CreateAttribute("ПрАвтономРежим");
            //XmlAttribute PrZamFN = xmlDocument.CreateAttribute("ПрЗамФН");
            //XmlAttribute PrIzmAvtUstr = xmlDocument.CreateAttribute("ПрИзмАвтУстр");
            //XmlAttribute PrIzmAdrMU = xmlDocument.CreateAttribute("ПрИзмАдрМУ");
            //XmlAttribute PrIzmNaimNP = xmlDocument.CreateAttribute("ПрИзмНаимНП");
            //XmlAttribute PrIniePricini = xmlDocument.CreateAttribute("ПрИныеПричины");
            //XmlAttribute PrSmenOFD = xmlDocument.CreateAttribute("ПрСменОФД");
            //XmlAttribute PrElectrRezim = xmlDocument.CreateAttribute("ПрЭлектрРежим");

            XmlElement SvedRegKKT = xmlDocument.CreateElement("СведРегККТ");

            XmlAttribute ZavodNomerKKT = xmlDocument.CreateAttribute("ЗаводНомерККТ");
            XmlAttribute ZavodNomerFN = xmlDocument.CreateAttribute("ЗаводНомерФН");
            XmlAttribute ModelKKT = xmlDocument.CreateAttribute("МоделККТ");
            XmlAttribute ModelFN = xmlDocument.CreateAttribute("МоделФН");
            XmlAttribute PrAvtomatUstr = xmlDocument.CreateAttribute("ПрАвтоматУстр");
            XmlAttribute PrAvtonom = xmlDocument.CreateAttribute("ПрАвтоном");
            XmlAttribute PrAzart = xmlDocument.CreateAttribute("ПрАзарт");
            XmlAttribute PrAkxizTovar = xmlDocument.CreateAttribute("ПрАкцизТовар");
            XmlAttribute PrBankPlat = xmlDocument.CreateAttribute("ПрБанкПлат");
            XmlAttribute PrBlank = xmlDocument.CreateAttribute("ПрБланк");
            XmlAttribute PrIgorZaved = xmlDocument.CreateAttribute("ПрИгорнЗавед");
            XmlAttribute PrInternet = xmlDocument.CreateAttribute("ПрИнтернет");
            XmlAttribute PrLotereya = xmlDocument.CreateAttribute("ПрЛотерея");
            XmlAttribute PrPlatAgent = xmlDocument.CreateAttribute("ПрПлатАгент");
            XmlAttribute PrRazvozRaznos = xmlDocument.CreateAttribute("ПрРазвозРазнос");
            XmlAttribute PrRascMark = xmlDocument.CreateAttribute("ПрРасчМарк");

            XmlElement SvedOFD = xmlDocument.CreateElement("СведОФД");

            XmlAttribute INNUYL = xmlDocument.CreateAttribute("ИННЮЛ");
            XmlAttribute NaimOrgOFD = xmlDocument.CreateAttribute("НаимОрг");

            XmlElement SvedAdrMUst = xmlDocument.CreateElement("СведАдрМУст");

            XmlAttribute NaimMUst = xmlDocument.CreateAttribute("НаимМУст");

            XmlElement AdrMUstKKT = xmlDocument.CreateElement("АдрМУстККТ");

            XmlElement AdrFIAS = xmlDocument.CreateElement("АдрФИАС");

            XmlAttribute IdNom = xmlDocument.CreateAttribute("ИдНом");
            XmlAttribute Index = xmlDocument.CreateAttribute("Индекс");

            XmlElement Region = xmlDocument.CreateElement("Регион");
            XmlElement MunixipRayon = xmlDocument.CreateElement("МуниципРайон");

            XmlAttribute VidKod = xmlDocument.CreateAttribute("ВидКод");
            XmlAttribute Naim = xmlDocument.CreateAttribute("Наим");

            XmlElement NaselenPunkt = xmlDocument.CreateElement("НаселенПункт");

            XmlAttribute Vid = xmlDocument.CreateAttribute("Вид");
            XmlAttribute Naim2 = xmlDocument.CreateAttribute("Наим");

            XmlElement ElUlDorSeti = xmlDocument.CreateElement("ЭлУлДорСети");

            XmlAttribute Naim3 = xmlDocument.CreateAttribute("Наим");
            XmlAttribute Tip = xmlDocument.CreateAttribute("Тип");

            XmlElement Zdanie = xmlDocument.CreateElement("Здание");

            XmlAttribute Nomer = xmlDocument.CreateAttribute("Номер");
            XmlAttribute Tip2 = xmlDocument.CreateAttribute("Тип");

            XmlElement userElem = xmlDocument.CreateElement("Здание");
            XmlAttribute Name = xmlDocument.CreateAttribute("Тип");
            //</АдрФИАС>
            //</АдрМУстККТ>
            //</СведАдрМУст>
            //</СведРегККТ>
            //<ЗаявРегККТ>
            //</Документ>
            //</Файл>

            

            XmlText VersProgT = xmlDocument.CreateTextNode("1.0");
            //  XmlText  = xmlDocument.CreateTextNode("");
            XmlText VersFormT = xmlDocument.CreateTextNode("5.06");
            XmlText IdFailT = xmlDocument.CreateTextNode(ID_file);
            XmlText DataDokT = xmlDocument.CreateTextNode(datestring);
            XmlText KNDT = xmlDocument.CreateTextNode("1110061");
            XmlText KodNOT = xmlDocument.CreateTextNode("9965"); //«Система обозначений налоговых органов» 


            XmlText INNFLT = xmlDocument.CreateTextNode(dataRegistrationKKT.INNOrganization); //поправить 
            XmlText ImyT = xmlDocument.CreateTextNode(directorOrganization_array[1]);
            XmlText FamiliyaT = xmlDocument.CreateTextNode(directorOrganization_array[0]);
            XmlText OtcestvoT = xmlDocument.CreateTextNode(directorOrganization_array[2]);


            XmlText KPPT = xmlDocument.CreateTextNode(dataRegistrationKKT.KPPOrganization);
            XmlText NaimOrgT = xmlDocument.CreateTextNode(dataRegistrationKKT.NameOrganization.Replace("\"", "&quot;"));


            XmlText PrPodpT = xmlDocument.CreateTextNode("1"); //Подписант 

            Imy2T = xmlDocument.CreateTextNode(directorOrganization_array[1]);
            Familiya2T = xmlDocument.CreateTextNode(directorOrganization_array[0]);
            Otcestvo2T = xmlDocument.CreateTextNode(directorOrganization_array[2]);


            //XmlText NaimDocT = xmlDocument.CreateTextNode("Свидетельство");

            //XmlText RegNomerKKTT = xmlDocument.CreateTextNode(RNM);//ЗаявРегККТ 
            XmlText VidDocT = xmlDocument.CreateTextNode("1"); // 1-регистрация / 2-перерегистрация
            XmlText KodNOMUstT = xmlDocument.CreateTextNode("5800");
            //XmlText PrAvtonomRezimT = xmlDocument.CreateTextNode("2"); //обязателен при <ВидДок>=2           
            //XmlText PrZamFNT = xmlDocument.CreateTextNode("2");        //обязателен при <ВидДок>=2  
            //XmlText PrIzmAvtUstrT = xmlDocument.CreateTextNode("2");   //обязателен при <ВидДок>=2  
            //XmlText PrIzmAdrMUT = xmlDocument.CreateTextNode("2");     //обязателен при <ВидДок>=2 
            //XmlText PrIzmNaimNPT = xmlDocument.CreateTextNode("2");    //обязателен при <ВидДок>=2 
            //XmlText PrIniePriciniT = xmlDocument.CreateTextNode("2");  //обязателен при <ВидДок>=2
            //XmlText PrSmenOFDT = xmlDocument.CreateTextNode("2");      //обязателен при <ВидДок>=2 
            //XmlText PrElektrRezimT = xmlDocument.CreateTextNode("2");  // не заполняется при <ВидДок>=1  < ПрЭлектрРежим >≠< ПрАвтономРежим > при < ПрЭлектрРежим >= 1


            XmlText ZavodNomerKKTT = xmlDocument.CreateTextNode(dataRegistrationKKT.ZN_KKT); //СведРегККТ 
            XmlText ZavodNomerFNT = xmlDocument.CreateTextNode(dataRegistrationKKT.NumberFN);
            XmlText ModelKKTT = xmlDocument.CreateTextNode(dataRegistrationKKT.ModelKKT);
            string mfn = "Шифровальное (криптографическое) средство защиты фискальных данных фискальный накопитель «ФН-1.2 исполнение " + dataRegistrationKKT.ModelFN + "»";
            XmlText ModelFNT = xmlDocument.CreateTextNode(mfn);
            XmlText PrAvtomatUstrT = xmlDocument.CreateTextNode(PrAvtomatUstrS);
            XmlText PrAvtonomT = xmlDocument.CreateTextNode(PrAvtonomS);
            XmlText PrAzartT = xmlDocument.CreateTextNode(PrAzartS);
            XmlText PrAkxizTovarT = xmlDocument.CreateTextNode(PrAkxizTovarS);
            XmlText PrBankPlatT = xmlDocument.CreateTextNode(PrBankPlatS);
            XmlText PrIgorZavedT = xmlDocument.CreateTextNode("2"); //нет данных
            XmlText PrInternetT = xmlDocument.CreateTextNode(PrInternetS);
            XmlText PrLotereyaT = xmlDocument.CreateTextNode(PrLotereyaS);
            XmlText PrPlatAgentT = xmlDocument.CreateTextNode(PrPlatAgentS);
            XmlText PrRazvozRaznosT = xmlDocument.CreateTextNode(PrRazvozS);
            XmlText PrRascMarkT = xmlDocument.CreateTextNode(PrMarkS);

            XmlText INNYLT = xmlDocument.CreateTextNode(dataRegistrationKKT.INNOFD); //СведОФД
            XmlText NaimOrgOFDT = xmlDocument.CreateTextNode(dataRegistrationKKT.NameOFD);
            XmlText NaimMUstT = xmlDocument.CreateTextNode(dataRegistrationKKT.PlacePayment);

            XmlText IdNomT = xmlDocument.CreateTextNode("307e942a-83b6-4f99-8a94-9996b5a1b953"); //АдрФИАС 
            XmlText IndexT = xmlDocument.CreateTextNode("440000");
            XmlText RegionT = xmlDocument.CreateTextNode("58");
            XmlText VidKodT = xmlDocument.CreateTextNode("2");
            XmlText NaimT = xmlDocument.CreateTextNode("город Пенза");
            XmlText VidT = xmlDocument.CreateTextNode("г");
            XmlText Naim2T = xmlDocument.CreateTextNode("Пенза");
            XmlText Naim3T = xmlDocument.CreateTextNode("Суворова");
            XmlText TipT = xmlDocument.CreateTextNode("ул");
            XmlText NomerT = xmlDocument.CreateTextNode("92");
            XmlText Tip2T = xmlDocument.CreateTextNode("стр.");


            Imy.AppendChild(ImyT);
            Otcestvo.AppendChild(OtcestvoT);
            Familiya.AppendChild(FamiliyaT);

            Imy2.AppendChild(Imy2T);
            Otcestvo2.AppendChild(Otcestvo2T);
            Familiya2.AppendChild(Familiya2T);

            //NaimDoc.AppendChild(NaimDocT);

            ZavodNomerKKT.AppendChild(ZavodNomerKKTT); //Атрибуты <СведРегККТ>
            ZavodNomerFN.AppendChild(ZavodNomerFNT);
            ModelKKT.AppendChild(ModelKKTT);
            ModelFN.AppendChild(ModelFNT);
            PrAvtomatUstr.AppendChild(PrAvtomatUstrT);
            PrAvtonom.AppendChild(PrAvtonomT);
            PrAzart.AppendChild(PrAzartT);
            PrAkxizTovar.AppendChild(PrAkxizTovarT);
            PrBankPlat.AppendChild(PrBankPlatT);
            PrIgorZaved.AppendChild(PrIgorZavedT);
            PrInternet.AppendChild(PrInternetT);
            PrLotereya.AppendChild(PrLotereyaT);
            PrPlatAgent.AppendChild(PrPlatAgentT);
            PrRazvozRaznos.AppendChild(PrRazvozRaznosT);
            PrRascMark.AppendChild(PrRascMarkT);

            INNUYL.AppendChild(INNYLT);
            NaimOrg.AppendChild(NaimOrgT);

            NaimMUst.AppendChild(NaimMUstT);

            IdNom.AppendChild(IdNomT);
            Index.AppendChild(IndexT);
            VidKod.AppendChild(VidKodT);
            Naim.AppendChild(NaimT);
            Vid.AppendChild(VidT);
            Naim2.AppendChild(Naim2T);
            Naim3.AppendChild(Naim3T);
            Tip.AppendChild(TipT);
            Nomer.AppendChild(NomerT);
            Tip2.AppendChild(Tip2T);
            //-----------------------------------------------------
            Region.AppendChild(RegionT);
            MunixipRayon.Attributes.Append(VidKod);
            MunixipRayon.Attributes.Append(Naim);
            NaselenPunkt.Attributes.Append(Vid);
            NaselenPunkt.Attributes.Append(Naim2);
            ElUlDorSeti.Attributes.Append(Naim3);
            ElUlDorSeti.Attributes.Append(Tip);
            Zdanie.Attributes.Append(Nomer);
            Zdanie.Attributes.Append(Tip2);
            //-----------------------------------------------------
            AdrFIAS.Attributes.Append(IdNom);
            AdrFIAS.Attributes.Append(Index);
            AdrFIAS.AppendChild(Region);
            AdrFIAS.AppendChild(MunixipRayon);
            AdrFIAS.AppendChild(NaselenPunkt);
            AdrFIAS.AppendChild(ElUlDorSeti);
            AdrFIAS.AppendChild(Zdanie);

            AdrMUstKKT.AppendChild(AdrFIAS);
            NaimOrgOFD.AppendChild(NaimOrgOFDT);
            //-----------------------------------------------------
            if (statusNameOrganization == "ИП")
            {
                FIO.Attributes.Append(Imy);
                FIO.Attributes.Append(Otcestvo);
                FIO.Attributes.Append(Familiya);
            }
            else
            {
                NaimOrg.AppendChild(NaimOrgT);
                KPP.AppendChild(KPPT);
                INNYUL.AppendChild(INNFLT);
            }

            SvedOFD.Attributes.Append(INNUYL);
            SvedOFD.Attributes.Append(NaimOrgOFD);
            SvedAdrMUst.Attributes.Append(NaimMUst);
            SvedAdrMUst.AppendChild(AdrMUstKKT);
            //-----------------------------------------------------
            if (statusNameOrganization == "ИП")
            {
                NPFL.AppendChild(FIO);
                INNFL.AppendChild(INNFLT);
                NPFL.Attributes.Append(INNFL);
            }
            else
            {
                NPYUL.Attributes.Append(NaimOrg);
                NPYUL.Attributes.Append(INNYUL);
                NPYUL.Attributes.Append(KPP);
            }

            FIO2.Attributes.Append(Imy2);
            FIO2.Attributes.Append(Otcestvo2);
            FIO2.Attributes.Append(Familiya2);
            //NaimDoc.AppendChild(NaimDocT);
            //SvPred.Attributes.Append(NaimDoc);
            SvedRegKKT.AppendChild(SvedOFD);
            SvedRegKKT.AppendChild(SvedAdrMUst);

            SvedRegKKT.Attributes.Append(ZavodNomerKKT); //Атрибуты <СведРегККТ>
            SvedRegKKT.Attributes.Append(ZavodNomerFN);
            SvedRegKKT.Attributes.Append(ModelKKT);
            SvedRegKKT.Attributes.Append(ModelFN);
            SvedRegKKT.Attributes.Append(PrAvtomatUstr);
            SvedRegKKT.Attributes.Append(PrAvtonom);
            SvedRegKKT.Attributes.Append(PrAzart);
            SvedRegKKT.Attributes.Append(PrAkxizTovar);
            SvedRegKKT.Attributes.Append(PrBankPlat);
            SvedRegKKT.Attributes.Append(PrIgorZaved);
            SvedRegKKT.Attributes.Append(PrInternet);
            SvedRegKKT.Attributes.Append(PrLotereya);
            SvedRegKKT.Attributes.Append(PrPlatAgent);
            SvedRegKKT.Attributes.Append(PrRazvozRaznos);
            SvedRegKKT.Attributes.Append(PrRascMark);

            VidDoc.AppendChild(VidDocT); //Атрибуты <ЗаявРегККТ>
            KodNOMUst.AppendChild(KodNOMUstT);
            //RegNomerKKT.AppendChild(RegNomerKKTT); //включается только при регистрации!!!

            //PrAvtonomRezim.AppendChild(PrAvtonomRezimT);
            //PrZamFN.AppendChild(PrZamFNT);
            //PrIzmAvtUstr.AppendChild(PrIzmAvtUstrT);
            //PrIzmAdrMU.AppendChild(PrIzmAdrMUT);
            //PrIzmNaimNP.AppendChild(PrIzmNaimNPT);
            //PrIniePricini.AppendChild(PrIniePriciniT);
            //PrSmenOFD.AppendChild(PrSmenOFDT);
            //PrElectrRezim.AppendChild(PrElektrRezimT);

            //-----------------------------------------------------
            if (statusNameOrganization == "ИП")
            {
                SvNP.AppendChild(NPFL);
            }
            else
            {
                SvNP.AppendChild(NPYUL);
            }
            PrPodp.AppendChild(PrPodpT);
            Podpisant.Attributes.Append(PrPodp);
            Podpisant.AppendChild(FIO2);
            //Podpisant.AppendChild(SvPred);
            ZayavRegKKT.AppendChild(SvedRegKKT);
            //ZayavRegKKT.Attributes.Append(RegNomerKKT);
            ZayavRegKKT.Attributes.Append(VidDoc);
            ZayavRegKKT.Attributes.Append(KodNOMUst);
            //ZayavRegKKT.Attributes.Append(PrAvtonomRezim);
            //ZayavRegKKT.Attributes.Append(PrZamFN);
            //ZayavRegKKT.Attributes.Append(PrIzmAvtUstr);
            //ZayavRegKKT.Attributes.Append(PrIzmAdrMU);
            //ZayavRegKKT.Attributes.Append(PrIzmNaimNP);
            //ZayavRegKKT.Attributes.Append(PrIniePricini);
            //ZayavRegKKT.Attributes.Append(PrSmenOFD);
            //ZayavRegKKT.Attributes.Append(PrElectrRezim);

            DataDoc.AppendChild(DataDokT);
            KND.AppendChild(KNDT);
            KodNO.AppendChild(KodNOT);
            Document.Attributes.Append(DataDoc);
            Document.Attributes.Append(KND);
            Document.Attributes.Append(KodNO);
            //-----------------------------------------------------


            Document.AppendChild(SvNP);
            Document.AppendChild(Podpisant);
            Document.AppendChild(ZayavRegKKT);

            VersProg.AppendChild(VersProgT);
            VersForm.AppendChild(VersFormT);
            IdFail.AppendChild(IdFailT);
            Fail.Attributes.Append(VersProg);
            Fail.Attributes.Append(VersForm);
            Fail.Attributes.Append(IdFail);
            Fail.AppendChild(Document);

            string adr_file_save = null;
            string[] zap_znak = { "\"", "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
            string NameOrganization_save = dataRegistrationKKT.NameOrganization;
            if (dataRegistrationKKT.NameOrganization != "")
            {
                for (int i = 0; i < zap_znak.Length; i++)
                {
                    NameOrganization_save = NameOrganization_save.Replace(zap_znak[i], "");
                }
            }

            FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
            Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
            Browserdialog.SelectedPath = settings.AdressFile;
            if (Browserdialog.ShowDialog() == DialogResult.OK)
            {
                adr_file_save = Browserdialog.SelectedPath;
            }
            else { return false; }
            Directory.CreateDirectory(adr_file_save + "\\" + ID_file);
            xmlDocument.Save(adr_file_save + "\\" + ID_file + "\\" + ID_file + ".xml"); //сохранение файла xml

            string zipFilePath = adr_file_save + "\\" + NameOrganization_save + ".zip";
            string tempZipPath = zipFilePath;
            int counter = 1;

            // Проверяем, существует ли файл, и добавляем индекс, если нужно
            while (File.Exists(tempZipPath))
            {
                tempZipPath = Path.Combine(
                    adr_file_save,
                    $"{Path.GetFileNameWithoutExtension(NameOrganization_save)} ({counter}).zip"
                );
                counter++;
            }

            // Создаем ZIP-архив (что упаковываем, куда)
            ZipFile.CreateFromDirectory(
                sourceDirectoryName: adr_file_save + "\\" + ID_file,
                destinationArchiveFileName: tempZipPath
            ); 
            if (settings.DeleteXML == true)
            {
                Directory.Delete(adr_file_save + "\\" + ID_file, true);
            }

            MaterialMessageBox.Show("Файл XML создан и сохранен", "Сообщение");
            return true;
        }
    }
}
