using Kassa;
using KitCashProtocol;
using MaterialSkin;
using MaterialSkin.Controls;
using Registrator;
using Registrator.repo;
using Registrator.repo.models;
using Registrator.services;
using Registrator.ui.components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace Registrator
{
    public partial class RegistrationTerminalFA : MaterialForm
    {
        public DataKKT dataRegistrationKKT { get; set; }
        public string[] statusRegistrationKKT { get; set; }

        private SettingsProgram settings;
        TerminalFA CashRegistor = new TerminalFA();
        CreatorStatementsRegistrationKKT creatorStatements = new CreatorStatementsRegistrationKKT();
        private readonly KktResponseParser responseParser;
        private Timer responsePollingTimer;
        private LoadingSpinner spinner;

        bool statusConnection = false;

        
        public RegistrationTerminalFA(DataKKT _dataRegistrationKKT,SettingsProgram _setting)
        {
            InitializeComponent();
            InitializePollingTimer();
            InitializeSpinner();

            dataRegistrationKKT = _dataRegistrationKKT;
            settings = _setting;
            responseParser = new KktResponseParser(statusConnection, settings);
        }
        private void InitializePollingTimer()
        {
            responsePollingTimer = new Timer();
            responsePollingTimer.Interval = 2000; // 2 секунды
            responsePollingTimer.Tick += ResponsePollingTimer_Tick;
        }
        private void InitializeSpinner()
        {
            spinner = new LoadingSpinner
            {
                Location = new Point(118, 364),
                SpinnerColor = Color.FromArgb(63, 81, 181)
            };

            this.Controls.Add(spinner);
            spinner.Visible = false;
        }

        private void RegistrationTerminalFA_Load(object sender, EventArgs e)
        {
            verticalProgressBar1.ProgressColor = Color.FromArgb(63, 81, 181); // Indigo500
            verticalProgressBar1.Value +=25; 
            statusRegistrationKKT = new string[6];
            labelCreateXML.Enabled = true;
        }

        private void buttonXMLCreate_Click(object sender, EventArgs e)
        {
            bool resultCreateXmlDocument = creatorStatements.CreateXmlDocument(dataRegistrationKKT, settings);
            if (resultCreateXmlDocument) 
            {
                labelCreateXML.Text = "🗸";
                labelRegistrationKKT.Enabled = true;
                verticalProgressBar1.Value +=15;
                textBoxRNM.Enabled = true;
                statusRegistrationKKT[0] = "Сформировано заявление";
            }
            
        }

        private void textBoxRNM_Changed(object sender, EventArgs e)
        {
            textBoxRNM.Text = textBoxRNM.Text.Replace(" ", "");
            try { long resultConvertDataToInt = Convert.ToInt64(textBoxRNM.Text); }
            catch { MaterialMessageBox.Show("В поле РНМ допускается ввод только цифр"); }
            if (textBoxRNM.Text.Length == 16 && textBoxRNM.Text.All(char.IsDigit)) 
            {
                dataRegistrationKKT.RNM = textBoxRNM.Text;
                buttonRegistrationKKT.Enabled = true;
                statusRegistrationKKT[1] = "РНМ получен";
            }
            else if (textBoxRNM.Text.Length >= 16) 
            {
                buttonRegistrationKKT.Enabled = false;
                MaterialMessageBox.Show("Введен некорректный РНМ. Номер должен состоять только из цифр и содержать 16 символов.", "Ошибка");
            }
            else
            {
                buttonRegistrationKKT.Enabled = false;
            }
        }

        private void buttonRegistrationKKT_Click(object sender, EventArgs e)
        {
            var message = new StringBuilder();
            message.AppendLine("=== Данные кассового аппарата (KKT) ===");
            message.AppendLine($"Наимнование пользователя: {dataRegistrationKKT.NameOrganization}");
            message.AppendLine($"Адрес расчетов: {dataRegistrationKKT.AddressPayment}");
            message.AppendLine($"Место оплаты: {dataRegistrationKKT.PlacePayment}");
            message.AppendLine($"ФИО уполномоченного лица: {dataRegistrationKKT.NameCashier}");
            message.AppendLine($"ИНН ОФД: {dataRegistrationKKT.INNOFD}");
            message.AppendLine($"ОФД: {dataRegistrationKKT.NameOFD}");
            message.AppendLine($"Номер автомата: {dataRegistrationKKT.NumberAvtomate}");
            message.AppendLine($"Email ОФД: {dataRegistrationKKT.EmailOFD}");

            // Признаки (только true)
            var activePr = new List<string>();
            if (dataRegistrationKKT.PrLotereya) activePr.Add("Лотерея");
            if (dataRegistrationKKT.PrAzart) activePr.Add("Азартные игры");
            if (dataRegistrationKKT.PrPlatAgent) activePr.Add("Платежный агент");
            if (dataRegistrationKKT.PrInternet) activePr.Add("Интернет");
            if (dataRegistrationKKT.PrDelivery) activePr.Add("Развозная торговля");
            if (dataRegistrationKKT.PrAkxiz) activePr.Add("Акцизный товар");
            if (dataRegistrationKKT.PrMark) activePr.Add("Маркировка товаров");
            message.AppendLine(activePr.Any() ? string.Join("\n", activePr) : "Нет активных признаков");

            message.AppendLine($"ИНН: {dataRegistrationKKT.INNOrganization}");
            message.AppendLine($"РНМ: {dataRegistrationKKT.RNM}");
            
            // Системы налогообложения (СНО)
            var snoList = new List<string>();
            if (dataRegistrationKKT.SNO_OSN) snoList.Add("ОСН");
            if (dataRegistrationKKT.SNO_USN_D) snoList.Add("УСН (Доходы)");
            if (dataRegistrationKKT.SNO_USN_D_R) snoList.Add("УСН (Доходы - Расходы)");
            if (dataRegistrationKKT.SNO_PATENT) snoList.Add("Патент");
            if (dataRegistrationKKT.SNO_ESHN) snoList.Add("ЕСХН");
            message.AppendLine($"СНО: {(snoList.Any() ? string.Join(", ", snoList) : "Не указано")}");

            MaterialMessageBox.Show(message.ToString(), "Данные ККТ");

            labelRegistrationKKT.Text = "🗸";
            labelGetFD.Enabled = true;
            verticalProgressBar1.Value += 15;
            MultiLineTextBoxResponseDocument.Enabled = true;
            statusRegistrationKKT[2] = "Проведена регистрация ККТ";

        }

        private void ResponsePollingTimer_Tick(object sender, EventArgs e)
        {
            (bool success, DocumentByNumber document) = responseParser.ParseResponseDocumentByNumber(1);
            string[] lines = new string[]
            {
                "Тип документа: " + document.Type,
                "Получен ответ ОФД: " + document.AnswerOFD,
                "Дата / время: " + document.DateTime.ToString("dd.MM.yyyy HH:mm"),
                "Номер ФД: " + document.Number,
                "Фискальный признак: " + document.FiscalSign
            };

            // Объединяем их в одну строку с переносами
            MultiLineTextBoxResponseDocument.Text = string.Join(Environment.NewLine, lines);

            // Проверяем условие завершения
            if (document.AnswerOFD == "да")
            {
                spinner.Stop();
                responsePollingTimer.Stop();

                MultiLineTextBoxResponseDocument.Visible = true;

                labelGetFD.Text = "🗸";
                labelCreateAkt.Enabled = true;
                verticalProgressBar1.Value += 22;
                buttonCreateAkt.Enabled = true;
            }
        }

        private void buttonCreateAkt_Click(object sender, EventArgs e)
        {
            var newakt = new CreateAkt("Akt.docx");

            var items = new Dictionary<string, string>
            {
                {"<Model_KKT>", dataRegistrationKKT.ModelKKT },
                {"<ZN_KKT>", dataRegistrationKKT.ZN_KKT },
                {"<RNM>", dataRegistrationKKT.RNM },
                {"<N_FN>", dataRegistrationKKT.NumberFN },
                {"<dataTime>", dataRegistrationKKT.DataTimeFD.Substring(0,10)},
                {"<T_FD>", dataRegistrationKKT.DataTimeFD.Substring(Math.Max(0, dataRegistrationKKT.DataTimeFD.Length - 5)) },
                {"<N_FD>", dataRegistrationKKT.NumberFD },
                {"<FP>", dataRegistrationKKT.FP },
                {"<INN_Organization>", dataRegistrationKKT.INNOrganization },
                {"<INN_OFD>", dataRegistrationKKT.INNOFD },
                {"<NameOrganization>", dataRegistrationKKT.NameOrganization },
                {"<Name_operator>", settings.NameOperator },
                {"<ID_Client>", dataRegistrationKKT.ID },
            };

            newakt.Process(items, settings);

            labelCreateAkt.Text = "🗸";
            verticalProgressBar1.Value += 12;
            statusRegistrationKKT[5] = "Создан акт ввода в эксплуатацию";
            materialLabel5.Enabled = true;
            labelCodeChecking.Enabled = true;
        }

        private void labelCodeChecking_Click(object sender, EventArgs e)
        {
            labelCodeChecking.Text = "🗸";
            verticalProgressBar1.Value = 100;
            materialLabel6.Enabled = true;
            labelRegistrationFinish.Enabled = true;
            labelRegistrationFinish.Text = "🗸";
            buttonCompleteRegistration.Text = "Завершить регистрацию";
        }
        private void buttonCompleteRegistration_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                e.Cancel = true; // Отменяем закрытие, если нажата Cancel
                return;
            }

            base.OnFormClosing(e);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
        }


    }
}
