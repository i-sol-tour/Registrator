using KitCashProtocol;
using MaterialSkin;
using MaterialSkin.Controls;
using Registrator.repo.models;
using Registrator.services;
using Registrator.services.kkt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registrator
{
    public partial class OperationsPanel : MaterialForm
    {
        string VERSION_FFD = "1.2";
        bool statusConnection = false;
        TerminalFA CashRegistor = new TerminalFA();
        Shift Shift = new Shift();
        ActivationCode ActivationCode = new ActivationCode();
        TerminalFA CashRegister = new TerminalFA();

        private readonly KktResponseParser responseParser;
        private SettingsProgram settings;
        public OperationsPanel(bool _statusConnection, string _VERSION_FFD, SettingsProgram _setting)
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Amber50, Accent.Indigo400, TextShade.WHITE);

            VERSION_FFD = _VERSION_FFD;
            settings = _setting;
            statusConnection = _statusConnection;
            responseParser = new KktResponseParser(statusConnection, settings);
        }

        private void OperationsPanel_Load(object sender, EventArgs e)
        {
            if (Shift.CheckStatus(statusConnection, settings.PortName) == true)
            {
                buttonOpenShift.Enabled = false;
                buttonCloseShift.Enabled = true;
            }
            else
            {
                buttonOpenShift.Enabled = true;
                buttonCloseShift.Enabled = false;
            }
        }

        private void buttonOpenShift_Click(object sender, EventArgs e)
        {
            if (Shift.Open(statusConnection, settings.PortName) == true)
            {
                buttonOpenShift.Enabled = false;
                buttonCloseShift.Enabled = true;
                MaterialMessageBox.Show("Смена открыта", "Уведомление");
            }
        }

        private void buttonCloseShift_Click(object sender, EventArgs e)
        {
            if (Shift.Close(statusConnection, settings.PortName) == true)
            {
                buttonOpenShift.Enabled = true;
                buttonCloseShift.Enabled = false;
                MaterialMessageBox.Show("Смена закрыта", "Уведомление");
            }
        }

        private void buttonСheckActivationCode_Click(object sender, EventArgs e)
        {
            if (VERSION_FFD == "1.2")
            {
                if (ActivationCode.Check(statusConnection, settings.PortName) == true)
                {
                    MaterialMessageBox.Show("Код активации введен", "Уведомление");
                }
                else
                {
                    MaterialMessageBox.Show("Требуется ввести код активации", "Уведомление");
                }
            }
            else
            {
                MaterialMessageBox.Show("ФФД 1.2, код активации не требуется", "Уведомление");
            }
        }

        private void buttonDocumentByNumber_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxDocumentNumber.Text, out int documentNumber))
            {
                MaterialMessageBox.Show("Некорректный номер документа", "Ошибка");
                return;
            }

            (bool success, DocumentByNumber document) = responseParser.ParseResponseDocumentByNumber(documentNumber);
            string documentMessenge =   $"- Тип документа:  {document.Type}\n" +
                                        $"- Ответ ОФД:      {document.AnswerOFD}\n" +
                                        $"- Дата/время:     {document.DateTime:dd.MM.yyyy HH:mm}\n" +
                                        $"- Номер ФД:      {document.Number}\n" +
                                        $"- Фискальный признак: {document.FiscalSign}";
            if (!success)
            {
                MaterialMessageBox.Show(documentMessenge, "Ошибка");
                return;
            }

            MaterialMessageBox.Show(documentMessenge, "Информация о документе");
        }
        private void buttonGetRegistrationReportTLV_Click(object sender, EventArgs e)
        {
            if (textBoxRegistrationReportTLVNumber.Text != "" && Convert.ToInt32(textBoxRegistrationReportTLVNumber.Text) < 100 && Convert.ToInt32(textBoxRegistrationReportTLVNumber.Text) > 0)
            {
                int numberRegistrationReport = Convert.ToInt32(textBoxRegistrationReportTLVNumber.Text);

                TerminalFA CashRegister = new TerminalFA();
                statusConnection = CashRegister.OpenConnection(statusConnection, settings.PortName);
                if (statusConnection == true)
                {
                    string[,] data = new string[26, 3];
                    data = CashRegister.GetRegistrationReportTLVNumber(numberRegistrationReport);
                    string report = "";

                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        if (data[i, 0] == "1290")
                        {
                            var bitDescriptions = new Dictionary<int, string>
                            {
                                {5, "Интернет"},
                                {6, "Подакциз"},
                                {8, "Маркировка"},
                                {9, "Развозная торговля"},
                                {10, "Азартные игры"},
                                {11, "Лотерея"}
                            };
                            string cellValue = data[i, 2].ToString();
                            List<string> signs = new List<string>();

                            foreach (var pair in bitDescriptions)
                            {
                                // Вычисляем позицию с конца: (общая длина - 1 - смещение)
                                int positionFromEnd = cellValue.Length - 1 - pair.Key;

                                // Проверяем, что позиция существует и символ равен '1'
                                if (positionFromEnd >= 0 && cellValue[positionFromEnd] == '1')
                                {
                                    signs.Add(pair.Value);
                                }
                            }

                            string result = string.Join(", ", signs);
                            data[i, 2] = result;
                        }
                        report += $"{data[i, 0]} | {data[i, 1]} {data[i, 2]}\n";  
                        
                    }
                    MaterialMessageBox.Show(report, "Отчет о регистрации номер - " + numberRegistrationReport);
                    statusConnection = CashRegister.CloseConnection(statusConnection);
                }
            }
            else
            {
                MaterialMessageBox.Show("Указан некорректный номер ФД", "Ошибка");
            }
        }
        private void buttonInputTimeKKT_Click(object sender, EventArgs e)
        {
            TerminalFA CashRegister = new TerminalFA();
            statusConnection = CashRegister.OpenConnection(statusConnection, settings.PortName);
            if (statusConnection == true)
            {
                try
                {
                    DateTime DateTime_KKT = Convert.ToDateTime(textBoxDatetimeKKT);
                    CashRegister.InputDATETIME(DateTime_KKT);
                }
                catch { MaterialMessageBox.Show("Не удалось ввести время в ККТ"); }
                finally
                {
                    statusConnection = CashRegister.CloseConnection(statusConnection);
                }
            }
        }

        private void texBoxDatetimeKKT_Leave(object sender, EventArgs e)
        {
            Mask mask = new Mask();
            mask.MaskDateTime(textBoxDatetimeKKT.Text);
        }

        private void buttonAutoOpenCloseShift_Click(object sender, EventArgs e)
        {
            var result = MaterialMessageBox.Show(
                    "Смена будет автоматически закрыта, затем открыта. Вы уверены?",
                    "Уведомление",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Shift.Open(statusConnection, settings.PortName);
                    Shift.Close(statusConnection, settings.PortName);
                    MaterialMessageBox.Show("Операция выполнена", "Уведомление");
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
