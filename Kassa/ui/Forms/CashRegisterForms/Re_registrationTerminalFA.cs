using KitCashProtocol;
using MaterialSkin;
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
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Registrator.ui.Forms.CashRegisterForms
{
    public partial class Re_registrationTerminalFA : MaterialForm
    {
        bool statusConnection = false;
        private SettingsProgram settings;
        Shift Shift = new Shift();
        private Timer responsePollingTimer;
        private LoadingSpinner spinner;
        private readonly KktResponseParser responseParser;
        
        public Re_registrationTerminalFA(SettingsProgram _setting)
        {
            InitializeComponent();
            InitializePollingTimer();
            InitializeSpinner();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Amber50, Accent.Indigo400, TextShade.WHITE);

            settings = _setting;
            responseParser = new KktResponseParser(statusConnection, settings);

            verticalProgressBar1.ProgressColor = Color.FromArgb(63, 81, 181);
            verticalProgressBar1.Value = 20;
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
                Location = new System.Drawing.Point(104, 574),
                SpinnerColor = Color.FromArgb(63, 81, 181)
            };

            this.Controls.Add(spinner);
            spinner.Visible = false;
        }

        private void Re_registrationTerminalFA_Load(object sender, EventArgs e)
        {
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
            
            verticalProgressBar1.Value += 35;
            labelCloseShift.Enabled = true;
            materialLabel1.Enabled = true;
            Checkbox_1.Enabled = true;
            Checkbox_2.Enabled = true;
            Checkbox_3.Enabled = true;
            Checkbox_4.Enabled = true;
            Checkbox_5.Enabled = true;
            labelReregistration.Enabled = true;

        }
        private void buttonReregistrationKKT_Click(object sender, EventArgs e)
        {
            labelGetFD.Enabled = true;
            labelReregistration.Text = "🗸";
            verticalProgressBar1.Value += 38;

            MultiLineTextBoxResponseDocument.Visible = true;

            // Запускаем опрос ответа
            spinner.Visible = true;
            spinner.Start();
            responsePollingTimer.Start();
        }
        
        private void ResponsePollingTimer_Tick(object sender, EventArgs e)
        {
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

            // Объединяем их в одну строку с переносами
            MultiLineTextBoxResponseDocument.Text = string.Join(Environment.NewLine, lines);

            // Проверяем условие завершения
            if (document.AnswerOFD == "да")
            {
                spinner.Stop();
                responsePollingTimer.Stop();

                // Выполняем финальные действия
                materialLabel6.Enabled = true;
                labelRegistrationFinish.Enabled = true;
                labelRegistrationFinish.Text = "🗸";
                labelGetFD.Text = "🗸";
                verticalProgressBar1.Value = 100;
            }
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

        // Проверка чекбоксов при клике
        private void CheckBox_1_CheckedChanged(object sender, EventArgs e)
        {
            if (Checkbox_1.Checked == true)
                buttonReregistrationKKT.Enabled = true;
            else 
                buttonReregistrationKKT.Enabled = false;
        }

        private void Checkbox_2_CheckedChanged(object sender, EventArgs e)
        {
            if (Checkbox_2.Checked == true)
                buttonReregistrationKKT.Enabled = true;
            else
                buttonReregistrationKKT.Enabled = false;
        }

        private void Checkbox_3_CheckedChanged(object sender, EventArgs e)
        {
            if (Checkbox_3.Checked == true)
                buttonReregistrationKKT.Enabled = true;
            else
                buttonReregistrationKKT.Enabled = false;
        }

        private void Checkbox_4_CheckedChanged(object sender, EventArgs e)
        {
            if (Checkbox_4.Checked == true)
                buttonReregistrationKKT.Enabled = true;
            else
                buttonReregistrationKKT.Enabled = false;
        }

        private void Checkbox_5_CheckedChanged(object sender, EventArgs e)
        {
            if (Checkbox_5.Checked == true)
                buttonReregistrationKKT.Enabled = true;
            else
                buttonReregistrationKKT.Enabled = false;
        }

        
    }
}
