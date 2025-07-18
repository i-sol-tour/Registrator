using KitCashProtocol;
using MaterialSkin.Controls;
using Registrator.Properties;
using Registrator.repo.models;
using Registrator.services;
using Registrator.services.kkt;
using Registrator.ui.components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registrator.ui.Forms.CashRegisterForms
{
    public partial class ClosingFN : MaterialForm
    {
        bool statusConnection = false;
        private SettingsProgram settings;
        Shift Shift = new Shift();
        private Timer responsePollingTimer;
        private LoadingSpinner spinner;

        private readonly KktResponseParser responseParser;
        public ClosingFN(SettingsProgram _setting)
        {
            InitializeComponent();
            InitializePollingTimer();
            InitializeSpinner();

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
                Location = new System.Drawing.Point(126, 355),
                SpinnerColor = Color.FromArgb(63, 81, 181)
            };

            this.Controls.Add(spinner);
            spinner.Visible = false;
        }

        private void ClosingFN_Load(object sender, EventArgs e)
        {
            verticalProgressBar1.ProgressColor = Color.FromArgb(63, 81, 181); // Indigo500
            verticalProgressBar1.Value += 30;

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
            verticalProgressBar1.Value += 15;
            labelCloseFN.Enabled = true;
            buttonCloseFN.Enabled = true;
        }

        private void buttonCloseFN_Click(object sender, EventArgs e)
        {
            labelCloseFN.Text = "🗸";
            verticalProgressBar1.Value += 25;
            labelGetFDCloseFN.Enabled = true;
            textBoxNumberFD.Enabled = true;
        }
        private void textBoxNumberFD_TextChanged(object sender, EventArgs e)
        {
            if (textBoxNumberFD.Text != "")
            {
                // Запускаем опрос ответа
                spinner.Visible = true;
                spinner.Start();
                responsePollingTimer.Start();
            }
        }

        private void ResponsePollingTimer_Tick(object sender, EventArgs e)
        {
            int numberDocumentReport = Convert.ToInt32(textBoxNumberFD.Text);
            (bool success, DocumentByNumber document) = responseParser.ParseResponseDocumentByNumber(numberDocumentReport);
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
                buttonCompleteCloseFN.Text = "Завершить операцию";

                labelGetFDCloseFN.Text = "🗸";
                verticalProgressBar1.Value = 100;
                labelClosingFinish.Enabled = true;
                labelClosingFinish.Text = "🗸";
                labelClosintFinishText.Enabled = true;
            }
        }

        private void buttonCompleteCloseFN_Click(object sender, EventArgs e)
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
