using KitCashProtocol;
using MaterialSkin.Controls;
using Registrator.repo.models;
using Registrator.repo;
using Registrator.services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Registrator.services.kkt;
using Registrator.ui.components;
using System.Runtime.CompilerServices;
using Kassa;
using Registrator.Properties;

namespace Registrator.ui.Forms.CashRegisterForms
{
    public partial class ReplamentFN : MaterialForm
    {
        public DataKKT dataRegistrationKKT { get; set; }
        public string[] statusRegistrationKKT { get; set; }

        private SettingsProgram settings;
        TerminalFA CashRegistor = new TerminalFA();
        CreatorStatementsRegistrationKKT creatorStatements = new CreatorStatementsRegistrationKKT();
        private readonly KktResponseParser responseParser;
        private System.Windows.Forms.Timer responsePollingTimer;
        private LoadingSpinner spinner;
        Shift Shift = new Shift();

        bool statusConnection = false;
        public int valueProgressBar = 0;
        public ReplamentFN(SettingsProgram _setting)
        {
            InitializeComponent();
            InitializePollingTimer();
            InitializeSpinner();

            settings = _setting;
            responseParser = new KktResponseParser(statusConnection, settings);
        }
        private void InitializePollingTimer()
        {
            responsePollingTimer = new System.Windows.Forms.Timer();
            responsePollingTimer.Interval = 2000; // 2 секунды
            responsePollingTimer.Tick += ResponsePollingTimer_Tick;
        }
        private void InitializeSpinner()
        {
            spinner = new LoadingSpinner
            {
                Location = new System.Drawing.Point(126, 326),
                SpinnerColor = Color.FromArgb(63, 81, 181)
            };

            this.Controls.Add(spinner);
            spinner.Visible = false;
        }

        private void ReplamentFN_Load(object sender, EventArgs e)
        {
            verticalProgressBar1.ProgressColor = Color.FromArgb(63, 81, 181); // Indigo500
            verticalProgressBar1.Value += 25;

            if (Shift.CheckStatus(statusConnection, settings.PortName) == true)
            {
                if (Shift.Close(statusConnection, settings.PortName) == true)
                {
                    labelCloseShift.Text = "🗸";
                }
                else
                {
                    MaterialMessageBox.Show("Не удалось закрыть смену", "Ошибка");
                }
            }
            else
            {
                labelCloseShift.Text = "🗸";
            }
            labelCloseShift.Enabled = true;
            buttonClosedFN.Enabled = true;
            labelCloseFN.Enabled = true;
        }
        private void buttonClosed_Click(object sender, EventArgs e)
        {
            labelGetFDCloseFN.Enabled = true;
            labelCloseFN.Text = "🗸";

            spinner.Visible = true;
            spinner.Start();
            responsePollingTimer.Start();
        }

        private void ResponsePollingTimer_Tick(object sender, EventArgs e)
        {
            // Вызываем метод проверки ответа
            int numberDocumentReport = 1;
            (bool success, DocumentByNumber document) = responseParser.ParseResponseDocumentByNumber(numberDocumentReport);
            string[] lines = new string[]
            {
                "Тип документа: " + document.Type,
                "Получен ответ ОФД: " + document.AnswerOFD,
                "Дата / время: " + document.DateTime.ToString("dd.MM.yyyy HH:mm"),
                "Номер ФД: " + document.Number,
                "Фискальный признак: " + document.FiscalSign
            };

            if (MultiLineTextBoxResponseDocumentCloseFN.Text == "")
            {
                MultiLineTextBoxResponseDocumentCloseFN.Text = string.Join(Environment.NewLine, lines);
                // Проверяем условие завершения
                if (document.AnswerOFD == "да")
                {
                    spinner.Stop();
                    responsePollingTimer.Stop();
                    MultiLineTextBoxResponseDocumentCloseFN.Visible = true;

                    labelGetFDCloseFN.Text = "🗸";
                    verticalProgressBar1.Value += 15;

                    materialLabel2.Enabled = true;
                    labelConnectedNewFN.Enabled = true;
                }
            }

            else
            {
                MultiLineTextBoxResponseDocumentOpenFN.Text = string.Join(Environment.NewLine, lines);
                // Проверяем условие завершения
                if (document.AnswerOFD == "да")
                {
                    spinner.Stop();
                    responsePollingTimer.Stop();
                    MultiLineTextBoxResponseDocumentOpenFN.Visible = true;

                    labelGetReportRegistrationOpenFN.Text = "🗸";
                    verticalProgressBar1.Value = 15;

                    labelCreateAkt.Enabled = true;
                    buttonCreateAkt.Enabled = true;
                }
            }
        }
        private void labelConnectedNewFN_Click(object sender, EventArgs e)
        {
            labelConnectedNewFN.Text = "🗸";
            buttonReregistrationNewFN.Enabled = true;
            labelReregistrationNewFN.Enabled = true;
        }

        private void buttonReregistrationNewFN_Click(object sender, EventArgs e)
        {
            spinner = new LoadingSpinner
            {
                Location = new System.Drawing.Point(126, 571),
                SpinnerColor = Color.FromArgb(63, 81, 181)
            };
        }

        private void buttonCreateAkt_Click(object sender, EventArgs e)
        {
            var newakt = new CreateAkt("Akt-without.docx");

            var items = new Dictionary<string, string>
                {
                    {"<Model_KKT>", dataRegistrationKKT.ModelKKT },
                    {"<ZN_KKT>", dataRegistrationKKT.ZN_KKT },
                    {"<RNM>", dataRegistrationKKT.RNM },
                    {"<N_FN>", dataRegistrationKKT.NumberFN },
                    {"<dataTime>", dataRegistrationKKT.DataTimeFD},
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
            labelStatusCheckTechRun.Enabled = true;
            materialLabel4.Enabled = true;
        }
        private void labelStatusCheckTechRun_Click(object sender, EventArgs e)
        {
            labelStatusCheckTechRun.Text = "🗸";
            labelRegistrationFinish.Text = "🗸";
            labelRegistrationFinish.Enabled = true;
            materialLabel6.Enabled = true;
        }

        private void buttonCompleteReplamentFN_Click(object sender, EventArgs e)
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
