using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
using KitCashProtocol;
using MaterialSkin;
using MaterialSkin.Controls;
using System.IO.Compression;
using System.Configuration;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.IO.Ports;
using Registrator;
using Registrator.repo;
using Registrator.services;
using System.Text;
using Microsoft.Office.Interop.Access;
using System.IdentityModel.Tokens;
using Microsoft.Office.Interop.Word;
using Registrator.repo.models;
using Label = System.Windows.Forms.Label;
using Registrator.services.kkt;
using Registrator.ui.Forms.CashRegisterForms;
using System.Linq;
using Microsoft.Data.Sqlite;
using Registrator.Properties;
using Registrator.models;
using System.Security.AccessControl;



namespace Kassa
{
    public partial class Form_Start : MaterialForm
    {
        public bool Internet_status = false;
        public int t = 0;
        public long resultConvertDataToInt;
        public string standart_ModelKKT = "Терминал-ФА";
        public string VERSION_CONFIG = "------";
        public bool statusConnectionKKT = false;
        public string VERSION_FFD = "";
        public byte STATUS_SHIFT = 0;
        public byte STATUS_DOCUMENT = 0;
        public byte FN_THERE_IS = 0;
        public string PHASE = "1";

        public string M_FN;
        public bool otherModelFN = false;
        public bool[] Save_parametrs = new bool[39];
        public bool[] arrayCheckingFilledFields = new bool[26];

        SettingsProgram settings = new SettingsProgram();
        OptionsOFD optionsStandartOFD = new OptionsOFD();       
        DataKKT dataKKT = new DataKKT();

        // Заполнение версии программы на всех 4 страницах
        string program_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        TerminalFA CashRegister = new TerminalFA();

        public string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;

        public Form_Start()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Amber50, Accent.Indigo400, TextShade.WHITE);
        }

        private void Form_Start_Load(object sender, EventArgs e)
        {
            var loader = new SettingsLoader();
            settings = loader.GetSettings();

            var repo = new OFDandFN();
            optionsStandartOFD = repo.GetOptionsOFDByName(settings.StandartOFD);

            TextBox_INN_OFD1.Text = optionsStandartOFD.INN;
            TextBox_Email_OFD1.Text = optionsStandartOFD.Email;
            TextBox_INN_OFD2.Text = optionsStandartOFD.INN;
            TextBox_INN_OFD3.Text = optionsStandartOFD.INN;
            TextBox_Email_OFD3.Text = optionsStandartOFD.Email;
            TextBox_adress_OFD3.Text = optionsStandartOFD.URL;
            TextBox_IP_OFD3.Text = optionsStandartOFD.IP;
            TextBox_TCP_OFD3.Text = optionsStandartOFD.TCP;
            TextBox_DNS_OFD3.Text = optionsStandartOFD.DNS;
            TextBox_adress2_OFD3.Text = optionsStandartOFD.URL_OISM;
            TextBox_port_OFD3.Text = optionsStandartOFD.TCP_OISM;


            var fnOptions = repo.GetOptionsFNByName("Инвента");
            TextBox_adress_FN3.Text = fnOptions.URL;
            TextBox_port_FN3.Text = fnOptions.TCP;

            var namesOfd = repo.GetNamesOfd();

            ComboBox_Name_OFD1.Items.Clear();
            ComboBox_Name_OFD2.Items.Clear();
            ComboBox_Name_OFD3.Items.Clear();
            ComboBox_Name_OFD4.Items.Clear();

            foreach (var name in namesOfd)
            {
                ComboBox_Name_OFD1.Items.Add(name);
                ComboBox_Name_OFD2.Items.Add(name);
                ComboBox_Name_OFD3.Items.Add(name);
                ComboBox_Name_OFD4.Items.Add(name);
            }

            ComboBox_Name_OFD1.SelectedItem = settings.StandartOFD;
            ComboBox_Name_OFD2.SelectedItem = settings.StandartOFD;
            ComboBox_Name_OFD3.SelectedItem = settings.StandartOFD;
            ComboBox_Name_OFD4.SelectedItem = settings.StandartOFD;

            var modelFNs = repo.GetModelFNs();

            ComboBox_Model_FN1.Items.Clear();
            ComboBox_Model_FN4.Items.Clear();

            foreach (var model in modelFNs)
            {
                ComboBox_Model_FN1.Items.Add(model);
                ComboBox_Model_FN4.Items.Add(model);
            }

            ComboBox_Model_FN1.SelectedItem = settings.StandartModelFN;
            ComboBox_Model_FN4.SelectedItem = settings.StandartModelFN;

            labelVers1.Text = program_version;
            labelVers2.Text = program_version;
            labelVers3.Text = program_version;
            labelVers4.Text = program_version;


            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                Save_parametrs[i] = true;
            }
            label_save_status.Text = "";
            label_image_save_status.Text = "";
            arrayCheckingFilledFields[0] = true; // заполенние в массиве для проверки заполненных полей поля Модель ККТ

            arrayCheckingFilledFields[9] = true;
            arrayCheckingFilledFields[12] = true;
            arrayCheckingFilledFields[14] = true;
            arrayCheckingFilledFields[15] = true;
            arrayCheckingFilledFields[16] = true;
            arrayCheckingFilledFields[17] = true;
            arrayCheckingFilledFields[18] = true;
        }

        private void OFD_TextChanged(object sender, EventArgs e) // заполнение полей ИНН ОФД и почта отправителя
        {
            var repo = new OFDandFN();
            optionsStandartOFD = repo.GetOptionsOFDByName(ComboBox_Name_OFD1.Text);
            TextBox_INN_OFD1.Text = optionsStandartOFD.INN;
            TextBox_Email_OFD1.Text = optionsStandartOFD.Email;
            Save_parametrs[14] = false;
        }

        // __________________________ Проверки на изменения перед перез закрытием и на правильность ввода данных _____________
        private void Model_KKT_Changet(object sender, EventArgs e) // Проверка Модели ККТ
        {
            Save_parametrs[0] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";

            if (TextBox_Model_KKT.Text !="")
            {
                arrayCheckingFilledFields[0] = true;
            }
            else
            {
                arrayCheckingFilledFields[0] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            if (TextBox_Model_KKT.Text != "Терминал-ФА")
            {
                buttonXML.Enabled = false;
                buttonRegistrationKKT.Enabled = false;
            }
            else
            {
                buttonXML.Enabled = true;
                buttonRegistrationKKT.Enabled = true;
            }
        }
        private void ZN_KKT_TextChanged(object sender, EventArgs e) // Проверка ЗН ККТ, заполнение номера автомата
        {
            TextBox_Number_automatic.Text = TextBox_ZN_KKT.Text.Substring(Math.Max(0, TextBox_ZN_KKT.Text.Length - 6));
            Save_parametrs[1] = false;
            TextBox_ZN_KKT.Text = TextBox_ZN_KKT.Text.Replace(" ", "");

            if (TextBox_ZN_KKT.Text != "")
            {
                arrayCheckingFilledFields[1] = true;
            }
            else
            {
                arrayCheckingFilledFields[1] = false;
            }

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);
        }
        private void ZN_KKT_Leave(object sender, EventArgs e) // проверки ЗН ККТ
        {
            if ((TextBox_ZN_KKT.Text.Length != 12) && (TextBox_Model_KKT.Text == "Терминал-ФА") && TextBox_ZN_KKT.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан Заводской номер ККТ. Номер должен содержать 12 символов","Ошибка");
            }
            if (TextBox_ZN_KKT.Text.Length != 0)
            {
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_ZN_KKT.Text); }
                catch { MaterialMessageBox.Show("В поле Заводской номер ККТ допускается ввод только цифр","Ошибка"); }
            }
        }
        private void Number_automatic_Changet(object sender, EventArgs e) // Проверка Номера автомата
        {
            Save_parametrs[2] = false;

            if (TextBox_Number_automatic.Text != "")
            {
                arrayCheckingFilledFields[2] = true;
            }
            else
            {
                arrayCheckingFilledFields[2] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Model_FN1_Changet(object sender, EventArgs e) // Проверка Модели ФН
        {
            Save_parametrs[3] = false;
            if (ComboBox_Model_FN1.Text.Length >= 4)
            {
                if ((CheckBox_Podakziz.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) != "15"))
                {
                    MaterialMessageBox.Show("Некорретный выбор модели ФН. С Подакцизными товарами можно работать только на ФН 15 месяцев", "Ошибка");
                }
                if ((Checkbox_OSN.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) != "15"))
                {
                    MaterialMessageBox.Show("Некорретный выбор модели ФН. С системой налогоообложения ОСН можно работать только на ФН 15 месяцев","Ошибка");
                }
            }
            
            if (ComboBox_Model_FN1.Text != "")
            {
                arrayCheckingFilledFields[3] = true;
            }
            else
            {
                arrayCheckingFilledFields[3] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ZN_FN_Changet(object sender, EventArgs e) // ЗН ФН + автоподстановка модели ФН по номеру ФН
        {

            Save_parametrs[4] = false;
            if (TextBox_ZN_FN.Text.Length > 7)
            {
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "73814408") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 0; } // Ин36-4
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72804405") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 1; } // Ин36-3
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "73804408") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 7; } // Ин15-4
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72814407") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 2; } // Ин15-3
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "99604403") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 10; } // Ин15-1
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72824405") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 5; } // Эв15-3
                //if (TextBox_ZN_FN.Text.Substring(0, 8) == "") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = "Эв36-3"; }
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72844405") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 3; }  // Ав15-3
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72854405" && TextBox_ZN_FN.Text.Substring(0, 8) == "72854407") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 4; } // Ав36-3
            }


            if (TextBox_ZN_FN.Text != "")
            {
                arrayCheckingFilledFields[4] = true;
            }
            else
            {
                arrayCheckingFilledFields[4] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ZN_FN_Leave(object sender, EventArgs e) // Проверка ЗН ФН
        {
            if (TextBox_ZN_FN.Text.Length != 16 && TextBox_ZN_FN.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан заводской номер ФН. Номер должен содержать 16 символов","Ошибка");
            }
            if (TextBox_ZN_FN.Text.Length != 0)
            {
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_ZN_FN.Text); }
                catch { MaterialMessageBox.Show("В поле Заводской номер ФН допускается ввод только цифр","Ошибка"); }
            }
        }
        private void ID_Changet(object sender, EventArgs e) //автоудаление пробелов в ID Клиента
        {
            TextBox_ID_client.Text = TextBox_ID_client.Text.Replace(" ", "");
            Save_parametrs[5] = false;

            if (TextBox_ID_client.Text != "")
            {
                arrayCheckingFilledFields[5] = true;
            }
            else
            {
                arrayCheckingFilledFields[5] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ID_Leave(object sender, EventArgs e) // Проверка ID
        {
            if (TextBox_ID_client.Text.Length != 0)
            {
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_ID_client.Text); }
                catch { MaterialMessageBox.Show("В поле ID клиента допускается ввод только цифр", "Ошибка"); }
            }
        }
        private void NameOr_TextChanged(object sender, EventArgs e) // открытие поля КПП если ЮЛ и ввод имя руководителя
        {
            string[] n = TextBox_Name_organization.Text.Split(' ');
            string NOrganization = n[0];
            if (NOrganization != "ИП" && NOrganization.Length > 2)
            {
                TextBox_KPP_organization.Visible = true; // открытие поля КПП
                TextBox_KPP_organization.Visible = true; // открытие поля КПП
            }
            else if (NOrganization == "ИП" && TextBox_Name_organization.Text.Length > 2)
            {
                TextBox_KPP_organization.Visible = false;
                TextBox_Director_org.Text = TextBox_Name_organization.Text.Substring(Math.Max(0, 3)); // ввод имя руководителя
                TextBox_Cashier.Text = TextBox_Name_organization.Text.Substring(Math.Max(0, 3));
            }
            Save_parametrs[6] = false;

            if (TextBox_Name_organization.Text != "")
            {
                arrayCheckingFilledFields[6] = true;
            }
            else
            {
                arrayCheckingFilledFields[6] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Director_org_Changet(object sender, EventArgs e) // ФИО Руководителя
        {
            Save_parametrs[7] = false;

            if (TextBox_Director_org.Text != "")
            {
                arrayCheckingFilledFields[7] = true;
            }
            else
            {
                arrayCheckingFilledFields[7] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Director_org_Leave(object sender, EventArgs e)
        {
            //dataRegistrationKKT.DirectorOrganization.ToUpper();
            string[] directorOrganization_array = TextBox_Director_org.Text.ToUpper().Split(' ');// Получение ФИО
            // Обработка массива ФИО в зависимости от количества элементов
            if (directorOrganization_array.Length < 2)
            {
                MaterialMessageBox.Show("Некорректное ФИО директора. ФИО должно содержать минимум 2 слова (Фамилия, Имя)", "Ошибка");
            }
            else if (directorOrganization_array.Length > 3)
            {
                MaterialMessageBox.Show("ФИО директора содержит больше 3 слов. Фамилия - первое слово, Имя - второе, остальная часть будет считаться отчеством", "Уведомление");
            }
        }
        private void INNOr_TextChanged(object sender, EventArgs e) // ИНН Организации
        {
            TextBox_INN_organization.Text = TextBox_INN_organization.Text.Replace(" ", "");
            Save_parametrs[8] = false;

            if (TextBox_INN_organization.Text != "")
            {
                arrayCheckingFilledFields[8] = true;
            }
            else
            {
                arrayCheckingFilledFields[8] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void INNOr_Leave(object sender, EventArgs e) // Проверка ИНН Организации
        {
            if (TextBox_INN_organization.Text.Length != 10 && TextBox_INN_organization.Text.Length != 12 && TextBox_INN_organization.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан ИНН организации. ИНН должен состоять из 10 (ЮЛ) или 12 (ИП) символов", "Ошибка");
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_INN_organization.Text); }
                catch { MaterialMessageBox.Show("В поле ИНН Организации допускается ввод только цифр", "Ошибка"); }
            }
        }
        private void KPP_organization_Chenged(object sender, EventArgs e) // КПП Организации
        {
            TextBox_KPP_organization.Text = TextBox_KPP_organization.Text.Replace(" ", "");
            Save_parametrs[9] = false;

            if (TextBox_KPP_organization.Visible == false)
            {
                arrayCheckingFilledFields[9] = true;
            }
            else
            {
                if (TextBox_KPP_organization.Text != "")
                {
                    arrayCheckingFilledFields[9] = true;
                }
                else
                {
                    arrayCheckingFilledFields[9] = false;
                }
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void KPP_organization_Leave(object sender, EventArgs e) // Проверка КПП Организации
        {
            if (TextBox_KPP_organization.Text.Length != 9 && TextBox_KPP_organization.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан КПП организации. КПП должен состоять из 9 символов", "Ошибка");
            }
            try { resultConvertDataToInt = Convert.ToInt64(TextBox_KPP_organization.Text); }
            catch { MaterialMessageBox.Show("В поле КПП Организации допускается ввод только цифр", "Ошибка"); }
        }
        private void Cashier_Changet(object sender, EventArgs e) // Кассир
        {
            Save_parametrs[38] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void TelephonNumber_Enter(object sender, EventArgs e) //номер телефона
        {
            if (TextBox_Telephon_number.Text != null)
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    TextBox_Telephon_number.SelectionStart = 0; // Перемещаем курсор в начало текста
                    TextBox_Telephon_number.SelectionLength = 0; // Обеспечиваем отсутствие выделения
                });
            }
        }
        private void Phone_Changet(object sender, EventArgs e) //автоудаление символом из Номера телефона
        {
            if (TextBox_Telephon_number.Text.Length >= 11)
            {
                Mask mask = new Mask();
                string formatted_number = mask.MaskPhoneNumber_Changet(TextBox_Telephon_number.Text);
                TextBox_Telephon_number.Text = formatted_number;
            }
            Save_parametrs[10] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Phone_Leave(object sender, EventArgs e) // Маска телефона
        {
            Mask mask = new Mask();
            string formatted_number = mask.MaskPhoneNumber_Leave(TextBox_Telephon_number.Text);
            TextBox_Telephon_number.Text = formatted_number;
        }
        private void Email_Changet(object sender, EventArgs e) // Email организации
        {
            Save_parametrs[11] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Adress_Changet(object sender, EventArgs e) // Адрес расчетов
        {
            Save_parametrs[12] = false;

            if (TextBox_adressSale.Text != "")
            {
                arrayCheckingFilledFields[12] = true;
            }
            else
            {
                arrayCheckingFilledFields[12] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Place_Changet(object sender, EventArgs e) // Место расчетов
        {
            Save_parametrs[13] = false;

            if (TextBox_PlaceSale.Text != "")
            {
                arrayCheckingFilledFields[13] = true;
            }
            else
            {
                arrayCheckingFilledFields[13] = false;
            }
            if (TextBox_PlaceSale.Text != "")
            {
                buttonInsertValue.Visible = false;
            }
            else
            {
                buttonInsertValue.Visible = true;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void PlaseSale_Enter(object sender, EventArgs e) // Место расчетов
        {
            if (TextBox_PlaceSale.Text == "")
            {
                buttonInsertValue.FlatAppearance.BorderSize = 0;
                buttonInsertValue.Visible = true;
            }
        }
        private async void PlaseSale_Leave(object sender, EventArgs e) // Место расчетов
        {
            await Delay.ExecuteWithDelay(50);
            buttonInsertValue.Visible = false;
        }
        private async void buttonInsertValue_Click(object sender, EventArgs e) //кнопка подстановки места расчетов
        {
            TextBox_PlaceSale.Text = "Сервис аренды кассовой техники";
            await Delay.ExecuteWithDelay(5);
            buttonInsertValue.Visible = false;
        }
        private void INN_OFD_Changet(object sender, EventArgs e) // ИНН ОФД
        {
            Save_parametrs[14] = false;

            if (TextBox_INN_OFD1.Text != "")
            {
                arrayCheckingFilledFields[14] = true;
            }
            else
            {
                arrayCheckingFilledFields[14] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void INN_OFD_Leave(object sender, EventArgs e) // Проверка ИНН ОФД
        {
            if (TextBox_INN_OFD1.Text.Length != 10 && TextBox_INN_OFD1.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан ИНН ОФД. ИНН должен состоять из 10 символов", "Ошибка");
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_INN_OFD1.Text); }
                catch
                {
                    MaterialMessageBox.Show("В поле ИНН ОФД допускается ввод только цифр", "Ошибка");
                }
            }
        }
        private void Name_OFD_Textbox_Changet(object sender, EventArgs e) // ОФД в TextBox
        {
            Save_parametrs[15] = false;

            if (ComboBox_Name_OFD1.Text != "")
            {
                arrayCheckingFilledFields[15] = true;
            }
            else
            {
                arrayCheckingFilledFields[15] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Email_OFD_Changet(object sender, EventArgs e) // Email ОФД
        {
            Save_parametrs[16] = false;

            if (TextBox_Email_OFD1.Text != "")
            {
                arrayCheckingFilledFields[16] = true;
            }
            else
            {
                arrayCheckingFilledFields[16] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void RNM_Changed(object sender, EventArgs e) //автоудаление пробелов в РНМ
        {
            TextBox_RNM1.Text = TextBox_RNM1.Text.Replace(" ", "");
            Save_parametrs[17] = false;

            if (TextBox_RNM1.Text != "")
            {
                arrayCheckingFilledFields[17] = true;
            }
            else
            {
                arrayCheckingFilledFields[17] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void RNM_Leave(object sender, EventArgs e) // Проверка РНМ
        {
            if (TextBox_RNM1.Text.Length != 16 && TextBox_RNM1.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан РНМ. РНМ должен состоять из 16 символов", "Ошибка");
            }
            if (TextBox_RNM1.Text.Length != 0)
            {
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_RNM1.Text); }
                catch { MaterialMessageBox.Show("В поле РНМ допускается ввод только цифр", "Ошибка"); }
            }
        }
        private void Number_FD_Changed(object sender, EventArgs e) // Номер ФД
        {
            if (TextBox_Number_FD.Text.Length != 0)
            {
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_Number_FD.Text); }
                catch { MaterialMessageBox.Show("В поле Номер ФД допускается ввод только цифр", "Ошибка"); }
            }
            Save_parametrs[18] = false;

            if (TextBox_Number_FD.Text != "")
            {
                arrayCheckingFilledFields[18] = true;
            }
            else
            {
                arrayCheckingFilledFields[18] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
            
        }
        private void Datetime_Changed(object sender, EventArgs e) // Дата и время ФД
        {
            Save_parametrs[19] = false;

            if (TextBox_Datetime_FD.Text != "")
            {
                arrayCheckingFilledFields[19] = true;
            }
            else
            {
                arrayCheckingFilledFields[19] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Datetime_Leave(object sender, EventArgs e) // Проверка Даты и времени ФД
        {
            Mask mask = new Mask();
            mask.MaskDateTime(TextBox_Datetime_FD.Text);
        }
        private void Datetime_Enter(object sender, EventArgs e) // Активное поле даты и времени
        {
            if (TextBox_Datetime_FD.Text != null)
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    TextBox_Datetime_FD.SelectionStart = 0; // Перемещаем курсор в начало текста
                    TextBox_Datetime_FD.SelectionLength = 0; // Обеспечиваем отсутствие выделения
                });
            }
        }
        private void FP_FD_Changed(object sender, EventArgs e) // ФП ФД
        {
            Save_parametrs[20] = false;

            if (TextBox_FP_FD.Text != "")
            {
                arrayCheckingFilledFields[20] = true;
            }
            else
            {
                arrayCheckingFilledFields[20] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void FP_FD_Leave(object sender, EventArgs e) // ФП ФД
        {
            if (TextBox_FP_FD.Text.Length != 10 && TextBox_FP_FD.Text.Length != 9 && TextBox_FP_FD.Text.Length != 8 && TextBox_FP_FD.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан Фискальный признак документа. ФП должен состоять из 8-10 символов", "Ошибка");
            }
            if (TextBox_FP_FD.Text.Length != 0)
            {
                try { resultConvertDataToInt = Convert.ToInt64(TextBox_FP_FD.Text); }
                catch { MaterialMessageBox.Show("В поле Фискальный признак допускается ввод только цифр", "Ошибка"); }
            }
        }
        // _________________________________________________________ Перечень СНО
        private void SNO_OSN_Checked(object sender, EventArgs e)
        {
            Save_parametrs[21] = false;
            if (Checkbox_OSN.Checked == true && ComboBox_Model_FN1.Text.Substring(2, 2) != "15" && CheckBox_Podakziz.Checked == false)
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения ОСН можно работать только на ФН 15 месяцев", "Ошибка");
            }

            if (Checkbox_OSN.Checked == true)
            {
                arrayCheckingFilledFields[21] = true;
            }
            else
            {
                arrayCheckingFilledFields[21] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_USN_Dohod_Checked(object sender, EventArgs e)
        {
            Save_parametrs[22] = false;
            if ((Checkbox_USN_Dohod.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения УСН можно работать только на ФН 36 месяцев или добавьте работу с подакцизными товарами", "Ошибка");
            }

            if (Checkbox_USN_Dohod.Checked == true)
            {
                arrayCheckingFilledFields[22] = true;
            }
            else
            {
                arrayCheckingFilledFields[22] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_USN_Dohod_rashod_Checked(object sender, EventArgs e)
        {
            Save_parametrs[23] = false;
            if ((Checkbox_USN_Dohod_rashod.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения УСН можно работать только на ФН 36 месяцев или добавьте работу с подакцизными товарами", "Ошибка");
            }

            if (Checkbox_USN_Dohod_rashod.Checked == true)
            {
                arrayCheckingFilledFields[23] = true;
            }
            else
            {
                arrayCheckingFilledFields[23] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_Patent_Checked(object sender, EventArgs e)
        {
            Save_parametrs[24] = false;
            if ((Checkbox_Patent.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения ПАТЕНТ можно работать только на ФН 36 месяцев или добавьте работу с подакцизными товарами", "Ошибка");
            }

            if (Checkbox_Patent.Checked == true)
            {
                arrayCheckingFilledFields[24] = true;
            }
            else
            {
                arrayCheckingFilledFields[24] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_ESHN_Checked(object sender, EventArgs e)
        {
            Save_parametrs[25] = false;
            if ((Checkbox_ESHN.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения ЕСХН можно работать только на ФН 36 месяцев или добавьте работу с подакцизными товарами", "Ошибка");
            }

            if (Checkbox_ESHN.Checked == true)
            {
                arrayCheckingFilledFields[25] = true;
            }
            else
            {
                arrayCheckingFilledFields[25] = false;
            }

            CheckingFilledFields Checked = new CheckingFilledFields();
            buttonXML.Enabled = Checked.CheckingFilledFields_FileRegistration(arrayCheckingFilledFields);
            buttonAkt.Enabled = Checked.CheckingFilledFields_CreationAkt(arrayCheckingFilledFields);
            buttonRegistrationKKT.Enabled = Checked.CheckingFilledFields_RegistrationKKT(arrayCheckingFilledFields);

            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        // _________________________________________________________ Перечень режимов работы
        private void Podakziz_Checked(object sender, EventArgs e)
        {
            Save_parametrs[26] = false;
            if ((CheckBox_Podakziz.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) != "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор режима работы. Для работы с подакцизными товарами требуется ФН 15 месяцев", "Ошибка");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Mark_Checked(object sender, EventArgs e)
        {
            Save_parametrs[27] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Azart_play_Checked(object sender, EventArgs e)
        {
            Save_parametrs[28] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Lotereya_Checked(object sender, EventArgs e)
        {
            Save_parametrs[29] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Printer_v_avtomate_Checked(object sender, EventArgs e)
        {
            Save_parametrs[30] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Bank_agent_Checked(object sender, EventArgs e)
        {
            Save_parametrs[31] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Plat_agent_Checked(object sender, EventArgs e)
        {
            Save_parametrs[32] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Shifr_Checked(object sender, EventArgs e)
        {
            Save_parametrs[33] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Avtonom_Checked(object sender, EventArgs e)
        {
            Save_parametrs[34] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Domen_Changed(object sender, EventArgs e)
        {
            Save_parametrs[35] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Internet_Checked(object sender, EventArgs e) // проверка одновременной развозной торговли и интернет и выпадающий адрес Интернет
        {
            if ((CheckBox_Delivery.Checked == true) && (CheckBox_Internet.Checked == true))
            {
                MaterialMessageBox.Show("Запрещено отмечать в параметрах регистрации одновременно развозную торговлю и применение ККТ в сети Интернет. Измените выбор параметров","Оповещение");
                CheckBox_Internet.Checked = false;
            }
            Save_parametrs[36] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Delivery_Checked(object sender, EventArgs e) // проверка одновременной развозной торговли и интернет
        {
            if ((CheckBox_Delivery.Checked == true) && (CheckBox_Internet.Checked == true))
            {
                MaterialMessageBox.Show("Запрещено отмечать в параметрах регистрации одновременно развозную торговлю и применение ККТ в сети Интернет. Измените выбор параметров","Оповещение");
                CheckBox_Delivery.Checked = false;
            }

            if ((CheckBox_Delivery.Checked == true) && (CheckBox_Internet.Checked == false) && (TextBox_PlaceSale.Text.Contains("Курьер") == false))
            {
                string text_place_calculations = TextBox_PlaceSale.Text + "; Курьер"; // добавление к месту расчетов "Курьер" при отметке развозной торговли
                TextBox_PlaceSale.Text = text_place_calculations;
            }
            Save_parametrs[37] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void buttonParOFD_Click(object sender, EventArgs e) // кнопка Параметры ОФД
        {
            var repo = new OFDandFN();
            var optionsOFD = repo.GetOptionsOFDByName(ComboBox_Name_OFD1.Text);
            var optionsOFN = repo.GetOptionsFNByName(ComboBox_Model_FN1.Text);

            DanReg f = new DanReg(optionsOFD, optionsOFN, statusConnectionKKT, VERSION_FFD);
            f.Show();
        }
        private void butGetParametersOFD_Click(object sender, EventArgs e) // кнопка Считать параметры ОФД
        {
            OFDParametersManager parametersOFD = new OFDParametersManager();
            parametersOFD.OutputParametersOFD(statusConnectionKKT, settings.PortName, VERSION_FFD);
        }
        private void butSave_Click(object sender, EventArgs e) // кнопка Сохранить
        {
            DataKKTManager();
            Save s = new Save();
            s.SaveData(dataKKT, settings);

            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                Save_parametrs[i] = true;
            }
            label_save_status.Text = "Сохранено";
            label_image_save_status.Text = "🗸";
        }
        private void butLoading_Click(object sender, EventArgs e)// кнопка Открыть
        {
            int local_close = 1;
            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                if (Save_parametrs[i] == false)
                {
                    local_close *= 0;
                }
            }
            if (local_close == 0)
            {
                DialogResult result = MaterialMessageBox.Show("Уверены что хотите открыть файл? Несохраненные данный на форме исчезнут", "Уведомление", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Clear_form();
                    local_close = 1;
                }
            }
            if (local_close == 1)
            {

                string FileLine = "";
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (Path.GetExtension(ofd.FileName).ToUpper().ToLower().Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                        {
                            FileLine = System.IO.File.ReadAllText(ofd.FileName);
                        }
                    }
                }
                if (FileLine.Length > 4) {
                    string[] dataFileLine = FileLine.Split('#');
                    string v = dataFileLine[dataFileLine.Length - 3].Trim();
                    string vv = dataFileLine[dataFileLine.Length - 2].Trim();
                    if (dataFileLine[dataFileLine.Length - 3].Trim() == "Версия файла" && dataFileLine[dataFileLine.Length - 2].Trim() == "2.1.3.0")
                    {
                        for (int i = 0; i < (dataFileLine.Length - 1); i = i + 2)
                        {
                            string key = dataFileLine[i].Trim();
                            string value = dataFileLine[i + 1].Trim();

                            switch (key)
                            {
                                case "ЗН ККТ":
                                    TextBox_ZN_KKT.Text = value;
                                    break;
                                case "Модель ККТ":
                                    TextBox_Model_KKT.Text = value;
                                    break;
                                case "Номер автомата":
                                    TextBox_Model_KKT.Text = value;
                                    break;
                                case "Номер ФН":
                                    TextBox_ZN_FN.Text = value;
                                    break;
                                case "Модель ФН":
                                    ComboBox_Model_FN1.Text = value;
                                    break;
                                case "ID клиента":
                                    TextBox_ID_client.Text = value;
                                    break;
                                case "Наименование организации":
                                    TextBox_Name_organization.Text = value;
                                    break;
                                case "Руководитель организации":
                                    TextBox_Director_org.Text = value;
                                    break;
                                case "ФИО уполномоченного лица":
                                    TextBox_Cashier.Text = value;
                                    break;
                                case "ИНН организации":
                                    TextBox_INN_organization.Text = value;
                                    break;
                                case "КПП организации":
                                    TextBox_KPP_organization.Text = value;
                                    break;
                                case "СНО: ОСН":
                                    if (value == "Да")
                                    {
                                        Checkbox_OSN.Checked = true;
                                    }
                                    break;
                                case "УСН Доход":
                                    if (value == "Да")
                                    {
                                        Checkbox_USN_Dohod.Checked = true;
                                    }
                                    break;
                                case "УСН Доход - расход":
                                    if (value == "Да")
                                    {
                                        Checkbox_USN_Dohod_rashod.Checked = true;
                                    }
                                    break;
                                case "Патент":
                                    if (value == "Да")
                                    {
                                        Checkbox_Patent.Checked = true;
                                    }
                                    break;
                                case "ЕСХН":
                                    if (value == "Да")
                                    {
                                        Checkbox_ESHN.Checked = true;
                                    }
                                    break;
                                case "Телефон":
                                    TextBox_Telephon_number.Text = value;
                                    break;
                                case "Почта":
                                    TextBox_Email_organization.Text = value;
                                    break;
                                case "Адрес расчетов":
                                    TextBox_adressSale.Text = value;
                                    break;
                                case "Место расчетов":
                                    TextBox_PlaceSale.Text = value;
                                    break;
                                case "ОФД":
                                    ComboBox_Name_OFD1.Text = value;
                                    break;
                                case "РНМ":
                                    TextBox_RNM1.Text = value;
                                    break;
                                case "Дата, время":
                                    TextBox_Datetime_FD.Text = value;
                                    break;
                                case "Номер ФД":
                                    TextBox_Number_FD.Text = value;
                                    break;
                                case "ФП":
                                    TextBox_FP_FD.Text = value;
                                    break;
                                case "Признак проведения лотереи":
                                    if (value == "Да")
                                    {
                                        CheckBox_Lotereya.Checked = true;
                                    }
                                    break;
                                case "Признак проведения азартных игр":
                                    if (value == "Да")
                                    {
                                        CheckBox_Azart_play.Checked = true;
                                    }
                                    break;
                                case "Признак деятельности платежного агента":
                                    if (value == "Да")
                                    {
                                        CheckBox_Plat_agent.Checked = true;
                                    }
                                    break;
                                case "Применение только в Интернет":
                                    if (value == "Да")
                                    {
                                        CheckBox_Internet.Checked = true;
                                    }
                                    break;
                                case "Применение в сфере услуг":
                                    if (value == "Да")
                                    {
                                        CheckBox_Delivery.Checked = true;
                                    }
                                    break;
                                case "Признак работы с подакцизными товарами":
                                    if (value == "Да")
                                    {
                                        CheckBox_Podakziz.Checked = true;
                                    }
                                    break;
                                case "Признак работы с маркированными товарами":
                                    if (value == "Да")
                                    {
                                        CheckBox_Mark.Checked = true;
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        TextBox_ZN_KKT.Text = dataFileLine[1].Trim();
                        TextBox_Number_automatic.Text = dataFileLine[5].Trim();
                        TextBox_Model_KKT.Text = dataFileLine[3].Trim();
                        TextBox_ZN_FN.Text = dataFileLine[7].Trim();
                        ComboBox_Model_FN1.Text = dataFileLine[9].Trim();
                        TextBox_ID_client.Text = dataFileLine[11].Trim();
                        TextBox_Name_organization.Text = dataFileLine[13].Trim();
                        TextBox_Director_org.Text = dataFileLine[15].Trim();
                        TextBox_INN_organization.Text = dataFileLine[17].Trim();

                        if (dataFileLine[19].Trim() == "ОСН") { Checkbox_OSN.Checked = true; } // СНО
                        if (dataFileLine[20].Trim() == "УСН Доход") { Checkbox_USN_Dohod.Checked = true; }
                        if (dataFileLine[21].Trim() == "УСН Доход - расход") { Checkbox_USN_Dohod_rashod.Checked = true; }
                        if (dataFileLine[22].Trim() == "Патент") { Checkbox_Patent.Checked = true; }
                        if (dataFileLine[23].Trim() == "ЕСХН") { Checkbox_ESHN.Checked = true; }
                        TextBox_Telephon_number.Text = dataFileLine[25].Trim();
                        TextBox_Email_organization.Text = dataFileLine[27].Trim();
                        TextBox_adressSale.Text = dataFileLine[29].Trim();
                        TextBox_PlaceSale.Text = dataFileLine[31].Trim();
                        ComboBox_Name_OFD1.Text = dataFileLine[33].Trim();
                        TextBox_RNM1.Text = dataFileLine[39].Trim();
                        string dataTime = dataFileLine[41].Trim() + dataFileLine[43].Trim(); //объединение даты и времени
                        TextBox_Datetime_FD.Text = dataTime;
                        TextBox_Number_FD.Text = dataFileLine[45].Trim();
                        TextBox_FP_FD.Text = dataFileLine[47].Trim();


                        if (dataFileLine[53].Trim() == "1") { CheckBox_Lotereya.Checked = true; }
                        if (dataFileLine[55].Trim() == "1") { CheckBox_Azart_play.Checked = true; }
                        if (dataFileLine[59].Trim() == "1") { CheckBox_Plat_agent.Checked = true; }
                        if (dataFileLine[63].Trim() == "1") { CheckBox_Internet.Checked = true; }
                        if (dataFileLine[65].Trim() == "1") { CheckBox_Delivery.Checked = true; }
                        if (dataFileLine[67].Trim() == "1") { CheckBox_Podakziz.Checked = true; }
                        if (dataFileLine[69].Trim() == "1") { CheckBox_Mark.Checked = true; }
                        TextBox_KPP_organization.Text = dataFileLine[71].Trim(); //КПП организации раннее забыл подставить
                        if (dataFileLine.Length > 71)
                        { if (dataFileLine[71] == program_version) { } }
                    }

                    TextBox_RNM1.Enabled = true;
                    TextBox_Number_FD.Enabled = true;
                    TextBox_Datetime_FD.Enabled = true;
                    TextBox_FP_FD.Enabled = true;

                    for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
                    {
                        Save_parametrs[i] = true;
                    }
                    label_save_status.Text = "Сохранено";
                    label_image_save_status.Text = "🗸";
                }
            }
        }
        private void butReaddata_Click(object sender, EventArgs e) //кнопка Считать данные
        {
            RegistrationReportKKT dataKKTService = new RegistrationReportKKT();
            var result = dataKKTService.ReadingRegistrationReportKKT(statusConnectionKKT, settings, switch_DHCP_KKT1.Checked);

            DataKKT dataKKT = result.Item1;
            KKTParameters kktParameters = result.Item2;
            FNStatusParsed statusFN = result.Item3;
            TerminalFAStatus status_KKT = result.Item4;

            label_datatime.Text = kktParameters.DateTimeKKTSetting;

            VERSION_CONFIG = kktParameters.VersionConfig;
            label_vers_config.Text = VERSION_CONFIG;

            switch_DHCP_KKT1.Checked = kktParameters.StatusNetworkSetting;

            if (status_KKT.FNThereis == 1) // если ФН подключен
            {
                label_status_shift.Text = statusFN.StatusShift;
                label_status_document.Text = statusFN.Document;

                if (statusFN.Phase == "ФН не зарегистрирован")
                {
                    label_status_FN.Text = statusFN.Phase;

                    buttonRegistrationKKT.Enabled = true;
                    buttonRe_registrationKKT.Enabled = false;
                    buttonReplacementFN.Enabled = false;
                    buttonCloseFN.Enabled = false;

                    TextBox_RNM1.Enabled = false;
                    TextBox_Number_FD.Enabled = false;
                    TextBox_Datetime_FD.Enabled = false;
                    TextBox_FP_FD.Enabled = false;
                }
                else
                {
                    TextBox_RNM1.Text = dataKKT.RNM;
                    TextBox_INN_organization.Text = dataKKT.INNOrganization;
                    TextBox_Number_FD.Text = dataKKT.NumberFD;
                    TextBox_Datetime_FD.Text = dataKKT.DataTimeFD;
                    TextBox_FP_FD.Text = dataKKT.FP;
                    TextBox_INN_OFD1.Text = dataKKT.INNOFD;
                    Checkbox_OSN.Checked = dataKKT.SNO_OSN;
                    Checkbox_USN_Dohod.Checked = dataKKT.SNO_USN_D;
                    Checkbox_USN_Dohod_rashod.Checked = dataKKT.SNO_USN_D_R;
                    Checkbox_Patent.Checked = dataKKT.SNO_PATENT;
                    Checkbox_ESHN.Checked = dataKKT.SNO_ESHN;

                    CheckBox_Internet.Checked = dataKKT.PrInternet;
                    CheckBox_Podakziz.Checked = dataKKT.PrAkxiz;
                    CheckBox_Mark.Checked = dataKKT.PrMark;
                    CheckBox_Delivery.Checked = dataKKT.PrDelivery;
                    CheckBox_Azart_play.Checked = dataKKT.PrAzart;
                    CheckBox_Lotereya.Checked = dataKKT.PrLotereya;


                    TextBox_Name_organization.Text = dataKKT.NameOrganization;
                    TextBox_adressSale.Text = dataKKT.AddressPayment;
                    TextBox_PlaceSale.Text = dataKKT.PlacePayment;
                    TextBox_Cashier.Text = dataKKT.NameCashier;
                    ComboBox_Name_OFD1.Text = dataKKT.NameOFD;
                    TextBox_Email_OFD1.Text = dataKKT.EmailOFD;
                    label_vers_FFD.Text = dataKKT.VersionFFD;
                    VERSION_FFD = dataKKT.VersionFFD;
                }

                if (statusFN.Phase == "ФН зарегистрирован")
                {
                    label_status_FN.Text = statusFN.Phase;

                    buttonRegistrationKKT.Enabled = false;
                    buttonRe_registrationKKT.Enabled = true;
                    buttonReplacementFN.Enabled = true;
                    buttonCloseFN.Enabled = true;

                    TextBox_RNM1.Enabled = true;
                    TextBox_Number_FD.Enabled = true;
                    TextBox_Datetime_FD.Enabled = true;
                    TextBox_FP_FD.Enabled = true;
                }
                else if (statusFN.Phase == "ФН закрыт, идет передача в ОФД")
                {
                    label_status_FN.Text = statusFN.Phase;

                    buttonRegistrationKKT.Enabled = false;
                    buttonRe_registrationKKT.Enabled = false;
                    buttonReplacementFN.Enabled = false;
                    buttonCloseFN.Enabled = false;

                    TextBox_RNM1.Enabled = true;
                    TextBox_Number_FD.Enabled = true;
                    TextBox_Datetime_FD.Enabled = true;
                    TextBox_FP_FD.Enabled = true;
                }
                else if (statusFN.Phase == "ФН закрыт, передача в ОФД заверешена")
                {
                    label_status_FN.Text = statusFN.Phase;

                    buttonRegistrationKKT.Enabled = false;
                    buttonRe_registrationKKT.Enabled = false;
                    buttonCloseFN.Enabled = false;
                    buttonReplacementFN.Enabled = false;

                    TextBox_RNM1.Enabled = true;
                    TextBox_Number_FD.Enabled = true;
                    TextBox_Datetime_FD.Enabled = true;
                    TextBox_FP_FD.Enabled = true;
                }
            }
            else
            {
                VERSION_FFD = kktParameters.VersionFFD;
            }
            label_vers_FFD.Text = VERSION_FFD;
            TextBox_ZN_KKT.Text = dataKKT.ZN_KKT;
            TextBox_ZN_FN.Text = dataKKT.NumberFN;
        }
        private void Clean_Click(object sender, EventArgs e) // Книпка Очистить поля
        {
            DialogResult result = MaterialMessageBox.Show("Уверены что хотите очистить поля?", "Уведомление", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Clear_form();
                dataKKT = null;
            }
        }
        private void buttonXML_Click(object sender, EventArgs e) // кнопка Файл регистрации
        {
            string dataTime = TextBox_Datetime_FD.Text;
            if (dataTime[2] == '.' && dataTime[5] == '.' && dataTime[10] == ' ' && dataTime[13] == ':')
            {
                string ZN_KKT = TextBox_ZN_KKT.Text;
                string M_KKT = TextBox_Model_KKT.Text;
                string N_FN = TextBox_ZN_FN.Text;
                string M_FN = ComboBox_Model_FN1.Text.Replace(" ", "");
                string NameOrganization = TextBox_Name_organization.Text;

                string[] n = NameOrganization.Split(' ');
                string NOrganization = n[0];
                if (NOrganization == "ООО")
                {
                    NameOrganization = "ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ " + NameOrganization.Substring(4);
                }
                if (NOrganization == "АО")
                {
                    NameOrganization = "АКЦИОНЕРНОЕ ОБЩЕСТВО " + NameOrganization.Substring(3);
                }
                string Director_org = TextBox_Director_org.Text.ToUpper(); //Конвертация в заглавные буквы ФИО директора
                string INN_Organization = TextBox_INN_organization.Text;
                string Place_ras = TextBox_PlaceSale.Text;
                string OFD = ComboBox_Name_OFD1.Text;
                string INN_OFD = TextBox_INN_OFD1.Text;
                string KPP_Organization = TextBox_KPP_organization.Text;

                string PrAvtonomS = "2"; // сведения регистрации ККТ
                string PrLotereyaS = "2";
                string PrAzartS = "2";
                string PrBankPlatS = "2";
                string PrPlatAgentS = "2";
                string PrAvtomatUstrS = "2";
                string PrInternetS = "2";
                string PrRazvozS = "2";
                string PrAkxizTovarS = "2";
                string PrMarkS = "2";

                if (CheckBox_Azart_play.Checked == true) { PrAzartS = "1"; }
                if (CheckBox_Mark.Checked == true) { PrMarkS = "1"; }
                if (CheckBox_Plat_agent.Checked == true) { PrPlatAgentS = "1"; }
                if (CheckBox_Lotereya.Checked == true) { PrLotereyaS = "1"; }
                if (CheckBox_Internet.Checked == true) { PrInternetS = "1"; }
                if (CheckBox_Delivery.Checked == true) { PrRazvozS = "1"; }
                if (CheckBox_Podakziz.Checked == true) { PrAkxizTovarS = "1"; }

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

                DateTime data = DateTime.Today; //получение даты ПК
                string d = Convert.ToString(data);
                d = d.Substring(0, d.Length - 8);
                string[] fio = Director_org.Split(' ');// Получение ФИО


                string[] dd = d.Split('.');
                Random rnd = new Random();
                int a = rnd.Next();
                string rand = Convert.ToString(a);

                XmlText Imy2T = null;
                XmlText Familiya2T = null;
                XmlText Otcestvo2T = null;

                if (KPP_Organization == "Заполняется только для ЮЛ")
                {
                    KPP_Organization = "";
                }



                string ID_file = "KO_ZVLREGKKT_5018_5018_" + INN_Organization + KPP_Organization + "_" + dd[2] + dd[1] + dd[0] + "_" + rand;


                if (fio.Length == 2 || fio.Length == 3)
                {

                    XmlText VersProgT = xmlDocument.CreateTextNode("1.0");
                    //  XmlText  = xmlDocument.CreateTextNode("");
                    XmlText VersFormT = xmlDocument.CreateTextNode("5.06");
                    XmlText IdFailT = xmlDocument.CreateTextNode(ID_file);
                    XmlText DataDokT = xmlDocument.CreateTextNode(d);
                    XmlText KNDT = xmlDocument.CreateTextNode("1110061");
                    XmlText KodNOT = xmlDocument.CreateTextNode("9965"); //«Система обозначений налоговых органов» 


                    XmlText INNFLT = xmlDocument.CreateTextNode(INN_Organization); //поправить 
                    XmlText ImyT = xmlDocument.CreateTextNode(fio[1]);
                    XmlText FamiliyaT = xmlDocument.CreateTextNode(fio[0]);
                    XmlText OtcestvoT = xmlDocument.CreateTextNode(fio[2]);


                    XmlText KPPT = xmlDocument.CreateTextNode(KPP_Organization);
                    XmlText NaimOrgT = xmlDocument.CreateTextNode(NameOrganization.Replace("\"", "&quot;"));


                    XmlText PrPodpT = xmlDocument.CreateTextNode("1"); //Подписант 
                    
                    Imy2T = xmlDocument.CreateTextNode(fio[1]);
                    Familiya2T = xmlDocument.CreateTextNode(fio[0]);
                    Otcestvo2T = xmlDocument.CreateTextNode(fio[2]);
                    

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


                    XmlText ZavodNomerKKTT = xmlDocument.CreateTextNode(ZN_KKT); //СведРегККТ 
                    XmlText ZavodNomerFNT = xmlDocument.CreateTextNode(N_FN);
                    XmlText ModelKKTT = xmlDocument.CreateTextNode(M_KKT);
                    string mfn = "Шифровальное (криптографическое) средство защиты фискальных данных фискальный накопитель «ФН-1.2 исполнение " + M_FN + "»";
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

                    XmlText INNYLT = xmlDocument.CreateTextNode(INN_OFD); //СведОФД
                    XmlText NaimOrgOFDT = xmlDocument.CreateTextNode(OFD);
                    XmlText NaimMUstT = xmlDocument.CreateTextNode(Place_ras);

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
                    if (NOrganization == "ИП")
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
                    if (NOrganization == "ИП")
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
                    if (NOrganization == "ИП")
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
                    string NameOrganization_save = NameOrganization;
                    if (NameOrganization != "")
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
                    else { return; }
                    Directory.CreateDirectory(adr_file_save + "\\" + ID_file);
                    xmlDocument.Save(adr_file_save + "\\" + ID_file + "\\" + ID_file + ".xml"); //сохранение файла xml
                    
                    ZipFile.CreateFromDirectory(adr_file_save + "\\" + ID_file, adr_file_save + "\\" + NameOrganization_save + ".zip"); //сохранение zip (что упаковываем, куда)
                    if (settings.DeleteXML == true)
                    {
                        Directory.Delete(adr_file_save + "\\" + ID_file, true);
                    }


                    new MaterialSnackBar($"Файл XML создан и сохранен по пути: {adr_file_save}").Show(this);
                }
                else
                {
                    MaterialMessageBox.Show("Неверно введены ФИО руководителя. Программа принимает фамилию, имя или полное ФИО","Ошибка");
                }
            }
            else
            {
                MaterialMessageBox.Show("Ошибка в вводе даты и времени. Введите по формату (дд.мм.гггг чч:мм)","Ошибка");
            }
        }
        private void buttonAkt_Click(object sender, EventArgs e) //кнопка Акт ввода в эксплуатацию
        {
            string DanaT_FD = TextBox_Datetime_FD.Text;
            if (DanaT_FD[2] == '.' && DanaT_FD[5] == '.' && DanaT_FD[10] == ' ' && DanaT_FD[13] == ':')
            {
                string dataTime = DanaT_FD.Substring(0,10);
                string T_FD = DanaT_FD.Substring(Math.Max(0, DanaT_FD.Length - 5));
                
                var newakt = new CreateAkt("Akt.docx");

                var items = new Dictionary<string, string>
                {
                    {"<Model_KKT>", TextBox_Model_KKT.Text },
                    {"<ZN_KKT>", TextBox_ZN_KKT.Text },
                    {"<RNM>", TextBox_RNM1.Text },
                    {"<N_FN>", TextBox_ZN_FN.Text },
                    {"<dataTime>", dataTime },
                    {"<T_FD>", T_FD },
                    {"<N_FD>", TextBox_Number_FD.Text },
                    {"<FP>", TextBox_FP_FD.Text },
                    {"<INN_Organization>", TextBox_INN_organization.Text },
                    {"<INN_OFD>", TextBox_INN_OFD1.Text },
                    {"<NameOrganization>", TextBox_Name_organization.Text },
                    {"<Name_operator>", settings.NameOperator },
                    {"<ID_Client>", TextBox_ID_client.Text },
                };

                newakt.Process(items, settings);
            }
            else
            {
                MaterialMessageBox.Show(
            "Ошибка в вводе даты и времени. Введите по формату (дд.мм.гггг чч:мм)",
            "Ошибка");
            }
        }
        private void butReg_Terminal_FA_Click(object sender, EventArgs e) // кнопка Регистрация Терминал-ФА
        {
            dataKKT.ID = TextBox_ID_client.Text;
            dataKKT.RNM = TextBox_RNM1.Text;
            dataKKT.ZN_KKT = TextBox_ZN_KKT.Text;
            dataKKT.NumberAvtomate = TextBox_Number_automatic.Text;
            dataKKT.NumberFN = TextBox_ZN_FN.Text;
            dataKKT.ModelFN = ComboBox_Model_FN1.Text;
            dataKKT.NameOrganization = TextBox_Name_organization.Text;
            dataKKT.DirectorOrganization = TextBox_Director_org.Text;
            dataKKT.NameCashier = TextBox_Cashier.Text;
            dataKKT.INNOrganization = TextBox_INN_organization.Text;
            dataKKT.KPPOrganization = TextBox_KPP_organization.Text;

            dataKKT.SNO_OSN = Checkbox_OSN.Checked;
            dataKKT.SNO_USN_D = Checkbox_USN_Dohod.Checked;
            dataKKT.SNO_USN_D_R = Checkbox_USN_Dohod_rashod.Checked;
            dataKKT.SNO_PATENT = Checkbox_Patent.Checked;
            dataKKT.SNO_ESHN = Checkbox_ESHN.Checked;

            dataKKT.Telephone = TextBox_Telephon_number.Text;
            dataKKT.EmailOrganization = TextBox_Email_organization.Text;

            dataKKT.AddressPayment = TextBox_adressSale.Text;
            dataKKT.PlacePayment = TextBox_PlaceSale.Text;

            dataKKT.NameOFD = ComboBox_Name_OFD1.Text;
            dataKKT.INNOFD = TextBox_INN_OFD1.Text;

            dataKKT.DataTimeFD = TextBox_Datetime_FD.Text;
            dataKKT.NumberFD = TextBox_Number_FD.Text;
            dataKKT.FP = TextBox_FP_FD.Text;

            dataKKT.ModelKKT = TextBox_Model_KKT.Text;

            dataKKT.PrLotereya = CheckBox_Lotereya.Checked;
            dataKKT.PrAzart = CheckBox_Azart_play.Checked;
            dataKKT.PrPlatAgent = CheckBox_Plat_agent.Checked;
            dataKKT.PrInternet = CheckBox_Internet.Checked;
            dataKKT.PrDelivery = CheckBox_Delivery.Checked;
            dataKKT.PrAkxiz = CheckBox_Podakziz.Checked;
            dataKKT.PrMark = CheckBox_Mark.Checked;

            if (!switch_DHCP_KKT1.Checked)
            {
                var statusNetwork = new NetworkSetting();
                switch_DHCP_KKT1.Checked = statusNetwork.CheckAndInput(statusConnectionKKT, settings.PortName, switch_DHCP_KKT1.Checked);
            }

            if (switch_DHCP_KKT1.Checked == true)
            {
                RegistrationTerminalFA form = new RegistrationTerminalFA(dataKKT, settings);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    dataKKT = form.dataRegistrationKKT;
                    string[] statusRegistrationKKT = form.statusRegistrationKKT;
                }
            }
        }
        private void buttonRe_registrationKKT_Click(object sender, EventArgs e) // кнопка Перерегистрация
        {
            dataKKT.ID = TextBox_ID_client.Text;
            dataKKT.RNM = TextBox_RNM1.Text;
            dataKKT.ZN_KKT = TextBox_ZN_KKT.Text;
            dataKKT.NumberAvtomate = TextBox_Number_automatic.Text;
            dataKKT.NumberFN = TextBox_ZN_FN.Text;
            dataKKT.ModelFN = ComboBox_Model_FN1.Text;
            dataKKT.NameOrganization = TextBox_Name_organization.Text;
            dataKKT.DirectorOrganization = TextBox_Director_org.Text;
            dataKKT.NameCashier = TextBox_Cashier.Text;
            dataKKT.INNOrganization = TextBox_INN_organization.Text;
            dataKKT.KPPOrganization = TextBox_KPP_organization.Text;

            dataKKT.SNO_OSN = Checkbox_OSN.Checked;
            dataKKT.SNO_USN_D = Checkbox_USN_Dohod.Checked;
            dataKKT.SNO_USN_D_R = Checkbox_USN_Dohod_rashod.Checked;
            dataKKT.SNO_PATENT = Checkbox_Patent.Checked;
            dataKKT.SNO_ESHN = Checkbox_ESHN.Checked;

            dataKKT.Telephone = TextBox_Telephon_number.Text;
            dataKKT.EmailOrganization = TextBox_Email_organization.Text;

            dataKKT.AddressPayment = TextBox_adressSale.Text;
            dataKKT.PlacePayment = TextBox_PlaceSale.Text;

            dataKKT.NameOFD = ComboBox_Name_OFD1.Text;
            dataKKT.INNOFD = TextBox_INN_OFD1.Text;

            dataKKT.DataTimeFD = TextBox_Datetime_FD.Text;
            dataKKT.NumberFD = TextBox_Number_FD.Text;
            dataKKT.FP = TextBox_FP_FD.Text;

            dataKKT.ModelKKT = TextBox_Model_KKT.Text;

            dataKKT.PrLotereya = CheckBox_Lotereya.Checked;
            dataKKT.PrAzart = CheckBox_Azart_play.Checked;
            dataKKT.PrPlatAgent = CheckBox_Plat_agent.Checked;
            dataKKT.PrInternet = CheckBox_Internet.Checked;
            dataKKT.PrDelivery = CheckBox_Delivery.Checked;
            dataKKT.PrAkxiz = CheckBox_Podakziz.Checked;
            dataKKT.PrMark = CheckBox_Mark.Checked;

            if (!switch_DHCP_KKT1.Checked)
            {
                var statusNetwork = new NetworkSetting();
                switch_DHCP_KKT1.Checked = statusNetwork.CheckAndInput(statusConnectionKKT, settings.PortName, switch_DHCP_KKT1.Checked);
            }

            if (switch_DHCP_KKT1.Checked == true)
            {
                Re_registrationTerminalFA form = new Re_registrationTerminalFA(settings);
                if (form.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
        private void buttonCloseFN_Click(object sender, EventArgs e) // кнопка Закрытие ФН
        {
            if (!switch_DHCP_KKT1.Checked)
            {
                var statusNetwork = new NetworkSetting();
                switch_DHCP_KKT1.Checked = statusNetwork.CheckAndInput(statusConnectionKKT, settings.PortName, switch_DHCP_KKT1.Checked);
            }

            if (switch_DHCP_KKT1.Checked == true)
            {
                ClosingFN form = new ClosingFN(settings);
                if (form.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
        private void buttonReplacementFN_Click(object sender, EventArgs e) // кнопка Замена ФН
        {
            if (!switch_DHCP_KKT1.Checked)
            {
                var statusNetwork = new NetworkSetting();
                switch_DHCP_KKT1.Checked = statusNetwork.CheckAndInput(statusConnectionKKT, settings.PortName, switch_DHCP_KKT1.Checked);
            }

            if (switch_DHCP_KKT1.Checked == true)
            {
                ReplamentFN form = new ReplamentFN(settings);
                if (form.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
        private void buttonOpenOperationsPanel_Click(object sender, EventArgs e) // кнопка Другие операции
        {
            statusConnectionKKT = CashRegister.OpenConnection(statusConnectionKKT, settings.PortName);
            if (statusConnectionKKT)
            {
                statusConnectionKKT = CashRegister.CloseConnection(statusConnectionKKT);
                OperationsPanel operationsPanel = new OperationsPanel(statusConnectionKKT, VERSION_FFD, settings);
                operationsPanel.ShowDialog();
            }
            else new MaterialSnackBar("Форма \"Другие операции\" не была открыта. ККТ не подключена").Show(this);

            
        }
        private void butInputTimeKKT_Click(object sender, EventArgs e) // кнопка Ввести время с ПК
        {
            TerminalFA CashRegister = new TerminalFA();
            statusConnectionKKT = CashRegister.OpenConnection(statusConnectionKKT, settings.PortName);
            if (statusConnectionKKT == true)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    CashRegister.InputDATETIME(now);
                    label_datatime.Text = now.ToString("dd.MM.yyyy HH:mm");
                    new MaterialSnackBar("Время ККТ и ПК синхронизированы").Show(this);
                }
                finally
                {
                    statusConnectionKKT = CashRegister.CloseConnection(statusConnectionKKT);
                }
            }
        }

        private bool closingProcessed = false; // Флаг для отслеживания состояния закрытия формы

        private void Form1_Closing(object sender, FormClosingEventArgs e) //Сохранение при закрытии формы
        {
            if (closingProcessed)
            {
                // Если уже обработали событие закрытия, выходим
                return;
            }
            int local_close = 1;
            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                if (Save_parametrs[i] == false)
                {
                    local_close *= 0;
                }
            }
            if ((local_close == 0) && (materialTabControl1.SelectedIndex == 0))
            {
                DialogResult result_close = MaterialMessageBox.Show("У вас есть несохраненные данные. Сохранить?", "Уведомление", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result_close == DialogResult.Yes)
                {
                    DataKKTManager();
                    Save s = new Save();
                    s.SaveData(dataKKT, settings);

                    closingProcessed = true;
                    System.Windows.Forms.Application.Exit();
                }
                else if (result_close == DialogResult.No)
                {
                    closingProcessed = true;
                    System.Windows.Forms.Application.Exit();
                }
                else if (result_close == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }        

        //____Страница_2________________________________________________
        private void OFD2_TextChanged(object sender, EventArgs e) // Подстановка ИНН ОФД на второй странице
        {
            var repo = new OFDandFN();
            var optionsOFD = repo.GetOptionsOFDByName(ComboBox_Name_OFD2.Text);
            TextBox_INN_OFD2.Text = optionsOFD.INN;
        }
        private void Date2_Enter(object sender, EventArgs e) // Дата и время
        {
            int index = TextBox_Date2.SelectionStart;

            // Перевод курсора в начало строки без выделения текста
            if (TextBox_Date2.Text != null)
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    TextBox_Date2.SelectionStart = 0;
                    TextBox_Date2.SelectionLength = 0;
                });
            }
        }
        private void buttonAkt2_Click(object sender, EventArgs e) // Кнопка Акт ввода на второй странице
        {
            var file = new CreateAkt("Akt.docx");
            if (TextBox_ID_client2.Text == "")
            {
                file = new CreateAkt("Akt-without.docx");
            }

            string ID_Сlient = TextBox_ID_client2.Text;
            string RNM = TextBox_RNM2.Text;
            string Model_KKT = TextBox_Model_KKT2.Text;
            string ZN_KKT = TextBox_ZN_KKT2.Text;
            string ZN_FN = TextBox_ZN_FN2.Text;

            string NameOrganization = TextBox_NameOrganization2.Text;
            string INN_Organization = TextBox_INNOrganization2.Text;
            string INN_OFD = TextBox_INN_OFD2.Text;

            string DateT_FD = TextBox_Date2.Text;
            string dataTime = DateT_FD.Substring(0, 10);
            string T_FD = DateT_FD.Substring(Math.Max(0, DateT_FD.Length - 5));
            string Number_FD = TextBox_NumberFD2.Text;
            string FP = TextBox_FPDocument2.Text;




            var items = new Dictionary<string, string>
            {
                {"<Model_KKT>", Model_KKT},
                {"<ZN_KKT>", ZN_KKT },
                {"<RNM>", RNM },
                {"<N_FN>", ZN_FN },
                {"<dataTime>", dataTime },
                {"<T_FD>", T_FD },
                {"<N_FD>", Number_FD },
                {"<FP>", FP },
                {"<INN_Organization>", INN_Organization },
                {"<INN_OFD>", INN_OFD },
                {"<NameOrganization>", NameOrganization },
                {"<Name_operator>", settings.NameOperator },
                {"<ID_Client>", ID_Сlient },
            };

            file.Process(items, settings);
        }
        private void butSaveAKT_Click(object sender, EventArgs e) // Кнопка Сохранить на второй странцие
        {
            DataKKT dataKKTAkt = new DataKKT();
            dataKKTAkt.ID = TextBox_ID_client2.Text;
            dataKKTAkt.RNM = TextBox_RNM2.Text;
            dataKKTAkt.ZN_KKT = TextBox_ZN_KKT2.Text;
            dataKKTAkt.NumberAvtomate = " ";
            dataKKTAkt.ZN_KKT = TextBox_ZN_FN2.Text;
            dataKKTAkt.ModelFN = " ";
            dataKKTAkt.NameOrganization = TextBox_NameOrganization2.Text;
            dataKKTAkt.DirectorOrganization = " ";
            dataKKTAkt.NameCashier = " ";
            dataKKTAkt.INNOrganization = TextBox_INNOrganization2.Text;
            dataKKTAkt.KPPOrganization = " ";
            dataKKTAkt.Telephone = " ";
            dataKKTAkt.EmailOrganization = " ";
            dataKKTAkt.AddressPayment = " ";
            dataKKTAkt.PlacePayment = " ";
            dataKKTAkt.NameOFD = ComboBox_Name_OFD2.Text;
            dataKKTAkt.INNOFD = TextBox_INN_OFD2.Text;
            dataKKTAkt.DataTimeFD = TextBox_Date2.Text;
            dataKKTAkt.NumberFD = TextBox_NumberFD2.Text;
            dataKKTAkt.FP = TextBox_FPDocument2.Text;
            dataKKTAkt.ModelKKT = TextBox_Model_KKT2.Text;

            dataKKTAkt.SNO_OSN = false;
            dataKKTAkt.SNO_USN_D = false;
            dataKKTAkt.SNO_USN_D_R = false;
            dataKKTAkt.SNO_PATENT = false;
            dataKKTAkt.SNO_ESHN = false;


            dataKKTAkt.PrLotereya = false;
            dataKKTAkt.PrAzart = false;
            dataKKTAkt.PrPlatAgent = false;
            dataKKTAkt.PrInternet = false;
            dataKKTAkt.PrDelivery = false;
            dataKKTAkt.PrAkxiz = false;
            dataKKTAkt.PrMark = false;

            Save s = new Save();
            s.SaveData(dataKKTAkt, settings);

            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                Save_parametrs[i] = true;
            }
        }
        private void butLoadingAKT_Click(object sender, EventArgs e) // Кнопка Открыть на второй странице
        {
            string str = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (Path.GetExtension(ofd.FileName).ToUpper().ToLower().Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        str = System.IO.File.ReadAllText(ofd.FileName);
                    }
                }


            }
            string[] str_mas = str.Split('#');
            if (str_mas.Length > 47)
            {
                TextBox_ZN_KKT2.Text = str_mas[1].Trim();
                TextBox_Model_KKT2.Text = str_mas[3].Trim();
                TextBox_ZN_FN2.Text = str_mas[7].Trim();
                TextBox_ID_client2.Text = str_mas[11].Trim();
                TextBox_NameOrganization2.Text = str_mas[13].Trim();
                TextBox_INNOrganization2.Text = str_mas[17].Trim();
                ComboBox_Name_OFD2.Text = str_mas[33].Trim();
                TextBox_INN_OFD2.Text = str_mas[35].Trim();
                TextBox_RNM2.Text = str_mas[39].Trim();
                TextBox_Date2.Text = str_mas[41].Trim() + str_mas[43].Trim(); //объединение даты и времени
                TextBox_NumberFD2.Text = str_mas[45].Trim();
                TextBox_FPDocument2.Text = str_mas[47].Trim();
            }
        }
        private void butReaddata2_Click(object sender, EventArgs e) // Кнопка Считать на второй странице
        {
            RegistrationReportKKT dataKKTService = new RegistrationReportKKT();
            var result = dataKKTService.ReadingRegistrationReportKKT(statusConnectionKKT, settings, switch_DHCP_KKT1.Checked);

            DataKKT dataKKTresult = result.Item1;

            TextBox_RNM2.Text = dataKKTresult.RNM;
            TextBox_Model_KKT2.Text = "Терминал-ФА";
            TextBox_ZN_KKT2.Text = dataKKTresult.ZN_KKT;
            TextBox_ZN_FN2.Text = dataKKTresult.NumberFN;

            TextBox_NameOrganization2.Text = dataKKTresult.NameOrganization;
            TextBox_INNOrganization2.Text = dataKKTresult.INNOrganization;
            TextBox_INN_OFD2.Text = dataKKTresult.INNOFD;

            TextBox_Date2.Text = dataKKTresult.DataTimeFD;
            TextBox_NumberFD2.Text = dataKKTresult.NumberFD;
            TextBox_FPDocument2.Text = dataKKTresult.FP;

        }
        private void butCleare2_Click(object sender, EventArgs e) // Кнопка Очистить поля на второй странице
        {
            DialogResult result = MaterialMessageBox.Show("Уверены что хотите очистить поля?", "Подтверждение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                TextBox_ZN_KKT2.Text = null;
                TextBox_Model_KKT2.Text = null;
                TextBox_ZN_FN2.Text = null;
                TextBox_ID_client2.Text = null;
                TextBox_NameOrganization2.Text = null;
                TextBox_INNOrganization2.Text = null;
                TextBox_RNM2.Text = null;
                TextBox_Date2.Text = null; 
                TextBox_NumberFD2.Text = null;
                TextBox_FPDocument2.Text = null;
                ComboBox_Name_OFD2.Text = settings.StandartOFD;
                TextBox_INN_OFD2.Text = optionsStandartOFD.INN;
            }
        }

        //____Страница_3________________________________________________
        private void butSave_OFD_Click(object sender, EventArgs e) // Сохранение парметров ОФД
        {
            if (CheckButton_AddNewOFD.Checked == false)
            {
                DialogResult result = MaterialMessageBox.Show("Уверены что хотите сохранить данные ОФД? Отменить действие будет невозможно", "Подтверждение",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        //Получение выбранного значения из ComboBox
                        string selectedOFDName = ComboBox_Name_OFD3.SelectedItem.ToString();

                        // SQL-запрос для обновления данных
                        string query = @"
                    UPDATE options_OFD 
                    SET 
                        inn_OFD = @inn_OFD, 
                        email_OFD = @email_OFD, 
                        adress_OFD = @adress_OFD, 
                        IP_OFD = @IP_OFD, 
                        TCP_OFD = @TCP_OFD, 
                        DNS_OFD = @DNS_OFD, 
                        port_OFD = @port_OFD,
                        adress_OISM_OFD = @adress_OISM_OFD
                    WHERE name_OFD = @name_OFD";
                        using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                        {// Открытие соединения
                            sqliteConnection.Open();
                            // Создание команды SQL
                            using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                            {
                                // Добавление параметров к запросу
                                sqliteCommand.Parameters.AddWithValue("@inn_OFD", TextBox_INN_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@email_OFD", TextBox_Email_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@adress_OFD", TextBox_adress_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@IP_OFD", TextBox_IP_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@TCP_OFD", TextBox_TCP_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@DNS_OFD", TextBox_DNS_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@port_OFD", TextBox_port_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@adress_OISM_OFD", TextBox_adress2_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@name_OFD", selectedOFDName);


                                // Выполнение запроса
                                int rowsAffected = sqliteCommand.ExecuteNonQuery();

                                //Проверка, были ли обновлены строки
                                if (rowsAffected > 0)
                                {
                                    new MaterialSnackBar("Данные сохранены").Show(this);
                                }
                                else
                                {
                                    MaterialMessageBox.Show("Не удалось обновить данные. Проверьте выбранное имя ОФД", "Ошибка");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MaterialMessageBox.Show("Ошибка: " + ex.Message);
                    }

                }
            }

            // добавление новой записи о ОФД
            else
            {
                DialogResult result2 = MaterialMessageBox.Show("Уверены что хотите добавить ОФД? Отменить действие будет невозможно", "Подтверждение",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result2 == DialogResult.Yes)
                {
                    // Получение нового имени ОФД из TextBox
                    string newNameOFD = TextBox_NewName_OFD3.Text;


                    // SQL-запрос для вставки новой записи
                    string query = @"
        INSERT INTO options_OFD (name_OFD, inn_OFD, email_OFD, adress_OFD, IP_OFD, TCP_OFD, DNS_OFD, port_OFD, adress_OISM_OFD) 
        VALUES (@name_OFD, @inn_OFD, @email_OFD, @adress_OFD, @IP_OFD, @TCP_OFD, @DNS_OFD, @port_OFD, @adress_OISM_OFD)";

                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                    {
                        // Открытие соединения
                        sqliteConnection.Open();

                        using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                        {
                            // Добавление параметров к запросу
                            sqliteCommand.Parameters.AddWithValue("@name_OFD", newNameOFD);
                            sqliteCommand.Parameters.AddWithValue("@inn_OFD", TextBox_INN_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@email_OFD", TextBox_Email_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@adress_OFD", TextBox_adress_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@IP_OFD", TextBox_IP_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@TCP_OFD", TextBox_TCP_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@DNS_OFD", TextBox_DNS_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@port_OFD", TextBox_port_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@adress_OISM_OFD", TextBox_adress2_OFD3.Text);

                            try
                            {
                                // Выполнение запроса
                                int rowsAffected = sqliteCommand.ExecuteNonQuery();

                                // Проверка, была ли добавлена запись
                                if (rowsAffected > 0)
                                {
                                    new MaterialSnackBar("Данные сохранены").Show(this);
                                }
                                else
                                {
                                    MaterialMessageBox.Show("Не удалось добавить новую запись ОФД.", "Ошибка");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Обработка возможных ошибок
                                MaterialMessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
                            }
                        }
                    }
                }
            }
            
        }
        private void butSave_FN_Click(object sender, EventArgs e) // Сохранение КП ФН
        {
            DialogResult result = MaterialMessageBox.Show(
                "Уверены что хотите сохранить данные КП ФН? Отменить действие будет невозможно",
                "Подтверждение",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    //Получение выбранного значения из ComboBox
                    string selectedFNName = ComboBox_Name_FN3.SelectedItem.ToString();

                    // SQL-запрос для обновления данных
                    string query = @"
                    UPDATE options_FN 
                    SET 
                        
                        adress_FN = @adress_FN,
                        port_FN = @port_FN
                    WHERE name_FN = @name_FN";
                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                    {// Открытие соединения
                        sqliteConnection.Open();
                        // Создание команды SQL
                        using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                        {
                            // Добавление параметров к запросу
                            sqliteCommand.Parameters.AddWithValue("@adress_FN", TextBox_adress_FN3.Text);
                            sqliteCommand.Parameters.AddWithValue("@adress_FN", TextBox_port_FN3.Text);
                            sqliteCommand.Parameters.AddWithValue("@name_FN", selectedFNName);


                            // Выполнение запроса
                            int rowsAffected = sqliteCommand.ExecuteNonQuery();

                            //Проверка, были ли обновлены строки
                            if (rowsAffected > 0)
                            {
                                new MaterialSnackBar("Данные сохранены").Show(this);
                            }
                            else
                            {
                                MaterialMessageBox.Show("Не удалось обновить данные. Проверьте выбранное имя ФН", "Ошибка");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Обработка возможных ошибок
                    MaterialMessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
                }
            }
        }
        private void CheckButton_AddNewOFD_Checked(object sender, EventArgs e) // Открытие поля Наименование нового ОФД
        {
            if (CheckButton_AddNewOFD.Checked == true) 
            { 
                TextBox_NewName_OFD3.Visible = true;
                ComboBox_Name_OFD3.Visible = false;
            }
            else { TextBox_NewName_OFD3.Visible = false; ComboBox_Name_OFD3.Visible = true; }
        }
        private void Name_OFD_Changed(object sender, EventArgs e) // Подстановка параметров ОФД в соответствии с ComboBox
        {
            var repo = new OFDandFN();
            var optionsOFD = repo.GetOptionsOFDByName(ComboBox_Name_OFD3.Text);

            TextBox_INN_OFD3.Text = optionsOFD.INN;
            TextBox_Email_OFD3.Text = optionsOFD.Email;
            TextBox_adress_OFD3.Text = optionsOFD.URL;
            TextBox_IP_OFD3.Text = optionsOFD.IP;
            TextBox_TCP_OFD3.Text = optionsOFD.TCP;
            TextBox_DNS_OFD3.Text = optionsOFD.DNS;
            TextBox_adress2_OFD3.Text = optionsOFD.URL_OISM;
            TextBox_port_OFD3.Text = optionsOFD.TCP_OISM;
        }
        private void Name_FN_Changed(object sender, EventArgs e) // Подстановка параметров FN в соответствии с ComboBox
        {
            var repo = new OFDandFN();
            var fnOptions = repo.GetOptionsFNByName(ComboBox_Name_FN3.Text);
            TextBox_adress_FN3.Text = fnOptions.URL;
            TextBox_port_FN3.Text = fnOptions.TCP;
        }    

        //____Страница_4________________________________________________
        private void TabControl_Selected(object sender, TabControlEventArgs e) // событие автозаполнения textBox настроек при активации вкладки
        {
            textBoxAdressFile.Text = settings.AdressFile;
            textBoxNameOperator.Text = settings.NameOperator;
            Switch_Del_xml.Checked = settings.DeleteXML;
            Switch_Print_Akt.Checked = settings.PrintAkt;
            Switch_CreateFolder.Checked = settings.CreateFolder;
            textBoxPortName.Text = settings.PortName;
        }
        private void materialButton1_Click(object sender, EventArgs e) // Кнопка сохранение
        {
            DialogResult result = MaterialMessageBox.Show("Уверены что хотите сохранить данные? Отменить действие будет невозможно", "Подтверждение",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                settings.DeleteXML = Switch_Del_xml.Checked;
                settings.PrintAkt = Switch_Print_Akt.Checked;
                settings.CreateFolder = Switch_CreateFolder.Checked;
                settings.AdressFile = textBoxAdressFile.Text;
                if (settings.AdressFile.Length > 0)
                {
                    if (settings.AdressFile.Substring(settings.AdressFile.Length - 1, 1) == "\\")
                    {
                        settings.AdressFile = settings.AdressFile.Remove(settings.AdressFile.Length - 1);
                    }
                }
                else
                {
                    MaterialMessageBox.Show("Поле \"Папка по умолчанию\" не может быть пустым", "Ошибка");
                    return;
                }

                var repo = new OFDandFN();

                // Обновляем параметры и проверяем успех
                bool success = true;

                success &= repo.UpdateParameter("adr_file", settings.AdressFile);
                success &= repo.UpdateParameter("name_operator", textBoxNameOperator.Text);
                success &= repo.UpdateParameter("del_xml", settings.DeleteXML ? "true" : "false");
                success &= repo.UpdateParameter("print_akt", settings.PrintAkt ? "true" : "false");
                success &= repo.UpdateParameter("create_folder", settings.CreateFolder ? "true" : "false");
                success &= repo.UpdateParameter("port_name", textBoxPortName.Text);
                success &= repo.UpdateParameter("standart_OFD", ComboBox_Name_OFD4.Text);
                success &= repo.UpdateParameter("standart_FN", ComboBox_Model_FN4.Text);

                // Проверка результата
                if (!success)
                {
                    MaterialMessageBox.Show("Не удалось сохранить настройки", "Ошибка");
                }
                else
                {
                    new MaterialSnackBar("Данные успешно сохранены").Show(this);
                }
            }
        }
        private void materialButton2_Click(object sender, EventArgs e) //открытие проводника
        {
            FolderBrowserDialog Browserdialog = new FolderBrowserDialog();
            if (Browserdialog.ShowDialog() == DialogResult.OK)
            {
                textBoxAdressFile.Text = Browserdialog.SelectedPath;
            }
        }
        private void DataKKTManager()
        {
            dataKKT.ID = TextBox_ID_client.Text;
            dataKKT.RNM = TextBox_RNM1.Text;
            dataKKT.ZN_KKT = TextBox_ZN_KKT.Text;
            dataKKT.NumberAvtomate = TextBox_Number_automatic.Text;
            dataKKT.NumberFN = TextBox_ZN_FN.Text;
            dataKKT.ModelFN = ComboBox_Model_FN1.Text;
            dataKKT.NameOrganization = TextBox_Name_organization.Text;
            dataKKT.DirectorOrganization = TextBox_Director_org.Text;
            dataKKT.NameCashier = TextBox_Cashier.Text;
            dataKKT.INNOrganization = TextBox_INN_organization.Text;
            dataKKT.KPPOrganization = TextBox_KPP_organization.Text;

            dataKKT.SNO_OSN = Checkbox_OSN.Checked;
            dataKKT.SNO_USN_D = Checkbox_USN_Dohod.Checked;
            dataKKT.SNO_USN_D_R = Checkbox_USN_Dohod_rashod.Checked;
            dataKKT.SNO_PATENT = Checkbox_Patent.Checked;
            dataKKT.SNO_ESHN = Checkbox_ESHN.Checked;

            dataKKT.Telephone = TextBox_Telephon_number.Text;
            dataKKT.EmailOrganization = TextBox_Email_organization.Text;

            dataKKT.AddressPayment = TextBox_adressSale.Text;
            dataKKT.PlacePayment = TextBox_PlaceSale.Text;

            dataKKT.NameOFD = ComboBox_Name_OFD1.Text;
            dataKKT.INNOFD = TextBox_INN_OFD1.Text;
            dataKKT.EmailOFD = TextBox_Email_OFD1.Text;

            dataKKT.DataTimeFD = TextBox_Datetime_FD.Text;
            dataKKT.NumberFD = TextBox_Number_FD.Text;
            dataKKT.FP = TextBox_FP_FD.Text;

            dataKKT.ModelKKT = TextBox_Model_KKT.Text;

            dataKKT.PrLotereya = CheckBox_Lotereya.Checked;
            dataKKT.PrAzart = CheckBox_Azart_play.Checked;
            dataKKT.PrInternet = CheckBox_Internet.Checked;
            dataKKT.PrDelivery = CheckBox_Delivery.Checked;
            dataKKT.PrAkxiz = CheckBox_Podakziz.Checked;
            dataKKT.PrMark = CheckBox_Mark.Checked;
        }
        private void Clear_form()
        {
            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                Save_parametrs[i] = true;
            }
            label_save_status.Text = "";
            label_image_save_status.Text = "";

            label_vers_FFD.Text = "----";
            label_vers_config.Text = "----";
            label_datatime.Text = "----";

            Checkbox_OSN.Checked = false;
            Checkbox_USN_Dohod.Checked = false;
            Checkbox_USN_Dohod_rashod.Checked = false;
            Checkbox_Patent.Checked = false;
            Checkbox_ESHN.Checked = false;

            TextBox_ID_client.Text = null;
            TextBox_RNM1.Text = null;
            TextBox_ZN_KKT.Text = null;
            TextBox_Number_automatic.Text = null;
            TextBox_ZN_FN.Text = null;
            ComboBox_Model_FN1.Text = settings.StandartModelFN;
            TextBox_Name_organization.Text = null;
            TextBox_Director_org.Text = null;
            TextBox_Cashier.Text = null;
            TextBox_INN_organization.Text = null;
            TextBox_KPP_organization.Text = null;
            TextBox_Telephon_number.Text = null;
            TextBox_Email_organization.Text = null;
            TextBox_adressSale.Text = "440000, г.Пенза, ул. Суворова, стр 92";
            TextBox_PlaceSale.Text = null;
            ComboBox_Name_OFD1.Text = settings.StandartOFD;
            var repo = new OFDandFN();
            optionsStandartOFD = repo.GetOptionsOFDByName(settings.StandartOFD);
            TextBox_INN_OFD1.Text = optionsStandartOFD.INN;
            TextBox_Email_OFD1.Text = optionsStandartOFD.Email;
            TextBox_Datetime_FD.Text = null;
            TextBox_Number_FD.Text = "1";
            TextBox_FP_FD.Text = null;
            TextBox_Model_KKT.Text = "Терминал-ФА";

            CheckBox_Azart_play.Checked = false;
            CheckBox_Mark.Checked = false;
            CheckBox_Plat_agent.Checked = false;
            CheckBox_Lotereya.Checked = false;
            CheckBox_Internet.Checked = false;
            CheckBox_Delivery.Checked = false;
            CheckBox_Podakziz.Checked = false;
        }

        private void buttonCopy1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Model_KKT.Text);
        }

        private void buttonCopy2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_ZN_KKT.Text);
        }

        private void buttonCopy3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Number_automatic.Text);
        }

        private void buttonCopy4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ComboBox_Model_FN1.Text);
        }

        private void buttonCopy5_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_ZN_FN.Text);
        }

        private void buttonCopy6_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_ID_client.Text);
        }

        private void buttonCopy7_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Name_organization.Text);
        }

        private void buttonCopy8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Director_org.Text);
        }

        private void buttonCopy9_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_INN_organization.Text);
        }

        private void buttonCopy10_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_KPP_organization.Text);
        }

        private void buttonCopy11_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Telephon_number.Text);
        }

        private void buttonCopy12_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Email_organization.Text);
        }

        private void buttonCopy13_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_adressSale.Text);
        }

        private void buttonCopy14_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_PlaceSale.Text);
        }

        // Обработка двойного клика при копировании ОФД
        private DateTime _lastClickTime;
        private void buttonCopy15_Click(object sender, EventArgs e)
        {
            if ((DateTime.Now - _lastClickTime).TotalMilliseconds < 500) // 500 мс
            {
                string companyName = OfdNameExtractor.Extract(ComboBox_Name_OFD1.Text);
                Clipboard.SetText(companyName);
                new MaterialSnackBar($"Скопировано: {companyName}").Show(this);
            }
            else
            {
                Clipboard.SetText(ComboBox_Name_OFD1.Text);
            }
            _lastClickTime = DateTime.Now;
            
        }

        private void buttonCopy16_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_INN_OFD1.Text);
        }

        private void buttonCopy17_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Email_OFD1.Text);
        }

        private void buttonCopy18_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_RNM1.Text);
        }

        private void buttonCopy19_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Number_FD.Text);
        }

        private void buttonCopy20_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Datetime_FD.Text);
        }

        private void buttonCopy21_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_FP_FD.Text);
        }
        private void buttonCopeCashier_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Cashier.Text);
        }

        private void switch_DHCP_KKT1_Checked_Click(object sender, EventArgs e)
        {
            bool statusInpitNetworkSetting;
            if (switch_DHCP_KKT1.Checked)
            {
                var networkSetting = new NetworkSetting();
                statusInpitNetworkSetting = networkSetting.CheckAndInput(statusConnectionKKT, settings.PortName, false);
                if (statusInpitNetworkSetting)
                {
                    new MaterialSnackBar("Сетевые настройки переведены на DHCP").Show(this);
                }
                else
                {
                    switch_DHCP_KKT1.Checked = statusInpitNetworkSetting;
                    new MaterialSnackBar("Ошибка при вводе сетевых настроек").Show(this);
                }
            }
            else
            {
                switch_DHCP_KKT1.Checked = true;
                new MaterialSnackBar("Нельзя перевести сетевые настройки на IP").Show(this);
            }
        }

        private void materialButton1_Click_1(object sender, EventArgs e)
        {
            DataKKTManager();
            CashRegister.Registration_12(dataKKT);
        }
    }
} 
