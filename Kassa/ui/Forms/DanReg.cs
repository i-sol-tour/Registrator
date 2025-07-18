using KitCashProtocol;
using MaterialSkin.Controls;
using System;
using System.Configuration;
using System.Data.SQLite;
using System.Windows.Forms;
using Registrator.services;
using Registrator.repo;
using Registrator.models;


namespace Kassa
{
    
    public partial class DanReg : MaterialForm
    {
        string VERSION_FFD = "1.2";
        bool StatusСonnectionKKT = false;
        string portName = "COM3";
        public TerminalFA CashRegister { get; set; }
        private static readonly byte[] START_BYTES = { 0xB6, 0x29 };

        public DanReg(OptionsOFD optionsOFD, OptionsFN optionsFN, bool statusСonnectionKKT, string version_FFD)
        {
            InitializeComponent();

            VERSION_FFD = version_FFD;
            StatusСonnectionKKT = statusСonnectionKKT;
            textBoxNameOFDData.Text = optionsOFD.Name;
            textBoxURLOFDData.Text = optionsOFD.URL;
            textBoxIPOFDData.Text = optionsOFD.IP;
            textBoxTCPOFDData.Text = optionsOFD.TCP;
            textBoxDNSOFDData.Text = optionsOFD.DNS;
            textBoxTcpOismData.Text = optionsOFD.TCP_OISM;
            textBoxUrlOismData.Text = optionsOFD.URL_OISM;

            textBoxNameFNData.Text = optionsFN.Name;
            textBoxUrlFNData.Text = optionsFN.URL;
            textBoxTcpFNData.Text = optionsFN.TCP;

        }
        
        private void butClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butInputParametersOFD_Click(object sender, EventArgs e)
        {

            OptionsOFD optionsOFD = new OptionsOFD
            {
                URL = textBoxURLOFDData.Text,
                IP = textBoxIPOFDData.Text,
                TCP = textBoxTCPOFDData.Text,
                Timeout = textBoxTimeoutOFDData.Text,
                URL_OISM = textBoxUrlOismData.Text,
                TCP_OISM = textBoxTcpOismData.Text
            };
            OptionsFN optionsFN = new OptionsFN
            {
                URL = textBoxUrlFNData.Text,
                TCP = textBoxTcpFNData.Text
            };
            OFDParametersManager parametersOFD = new OFDParametersManager();
            parametersOFD.InputParametersOFD(StatusСonnectionKKT, portName, VERSION_FFD, optionsOFD, optionsFN);
        }

        private void butGetParametersOFD_Click(object sender, EventArgs e)
        {
            OFDParametersManager parametersOFD = new OFDParametersManager();
            parametersOFD.OutputParametersOFD(StatusСonnectionKKT, portName, VERSION_FFD);
        }

        private void buttonCopyNameOFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxNameOFDData.Text);
        }

        private void buttonCopyAdressOFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxURLOFDData.Text);
        }

        private void buttonCopyIPOFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxIPOFDData.Text);
        }

        private void buttonCopyTCPOFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxTCPOFDData.Text);
        }

        private void buttonCopyDNSOFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxDNSOFDData.Text);
        }

        private void buttonCopyTimeoutOFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxTimeoutOFDData.Text);
        }

        private void buttonCopyAdress2OFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxUrlOismData.Text);
        }

        private void buttonCopyPortOFDData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxTcpOismData.Text);
        }

        private void buttonCopyTimeoutOISMData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxTimeoutOismData.Text);
        }

        private void buttonCopyNameFNData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxNameFNData.Text);
        }

        private void buttonCopyAdressFNData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxUrlFNData.Text);
        }

        private void buttonCopyPortFNData_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxTcpFNData.Text);
        }
    }
}
