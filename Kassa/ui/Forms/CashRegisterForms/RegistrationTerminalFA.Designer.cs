namespace Registrator
{
    partial class RegistrationTerminalFA
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistrationTerminalFA));
            this.textBoxRNM = new MaterialSkin.Controls.MaterialTextBox2();
            this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel7 = new MaterialSkin.Controls.MaterialLabel();
            this.labelCreateXML = new MaterialSkin.Controls.MaterialLabel();
            this.labelRegistrationKKT = new MaterialSkin.Controls.MaterialLabel();
            this.labelGetFD = new MaterialSkin.Controls.MaterialLabel();
            this.labelCreateAkt = new MaterialSkin.Controls.MaterialLabel();
            this.labelCodeChecking = new MaterialSkin.Controls.MaterialLabel();
            this.labelStatusStart = new MaterialSkin.Controls.MaterialLabel();
            this.labelRegistrationFinish = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel11 = new MaterialSkin.Controls.MaterialLabel();
            this.verticalProgressBar1 = new Registrator.VerticalProgressBar();
            this.buttonCompleteRegistration = new MaterialSkin.Controls.MaterialButton();
            this.ladelStatusInputNetworkSetting = new MaterialSkin.Controls.MaterialLabel();
            this.buttonRegistrationKKT = new MaterialSkin.Controls.MaterialButton();
            this.buttonCreateAkt = new MaterialSkin.Controls.MaterialButton();
            this.buttonXMLCreate = new MaterialSkin.Controls.MaterialButton();
            this.MultiLineTextBoxResponseDocument = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            this.SuspendLayout();
            // 
            // textBoxRNM
            // 
            this.textBoxRNM.AnimateReadOnly = false;
            this.textBoxRNM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.textBoxRNM.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.textBoxRNM.Depth = 0;
            this.textBoxRNM.Enabled = false;
            this.textBoxRNM.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxRNM.HideSelection = true;
            this.textBoxRNM.Hint = "Регистрационный номер ККТ";
            this.textBoxRNM.LeadingIcon = null;
            this.textBoxRNM.Location = new System.Drawing.Point(317, 274);
            this.textBoxRNM.MaxLength = 32767;
            this.textBoxRNM.MouseState = MaterialSkin.MouseState.OUT;
            this.textBoxRNM.Name = "textBoxRNM";
            this.textBoxRNM.PasswordChar = '\0';
            this.textBoxRNM.PrefixSuffixText = null;
            this.textBoxRNM.ReadOnly = false;
            this.textBoxRNM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxRNM.SelectedText = "";
            this.textBoxRNM.SelectionLength = 0;
            this.textBoxRNM.SelectionStart = 0;
            this.textBoxRNM.ShortcutsEnabled = true;
            this.textBoxRNM.Size = new System.Drawing.Size(363, 48);
            this.textBoxRNM.TabIndex = 157;
            this.textBoxRNM.TabStop = false;
            this.textBoxRNM.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.textBoxRNM.TrailingIcon = null;
            this.textBoxRNM.UseSystemPasswordChar = false;
            this.textBoxRNM.TextChanged += new System.EventHandler(this.textBoxRNM_Changed);
            // 
            // materialLabel5
            // 
            this.materialLabel5.AutoSize = true;
            this.materialLabel5.BackColor = System.Drawing.Color.Lime;
            this.materialLabel5.Depth = 0;
            this.materialLabel5.Enabled = false;
            this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel5.Location = new System.Drawing.Point(118, 555);
            this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel5.Name = "materialLabel5";
            this.materialLabel5.Size = new System.Drawing.Size(199, 19);
            this.materialLabel5.TabIndex = 160;
            this.materialLabel5.Text = "Проверка кода активации";
            this.materialLabel5.UseAccent = true;
            // 
            // materialLabel6
            // 
            this.materialLabel6.AutoSize = true;
            this.materialLabel6.Depth = 0;
            this.materialLabel6.Enabled = false;
            this.materialLabel6.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel6.Location = new System.Drawing.Point(118, 604);
            this.materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel6.Name = "materialLabel6";
            this.materialLabel6.Size = new System.Drawing.Size(182, 19);
            this.materialLabel6.TabIndex = 161;
            this.materialLabel6.Text = "Регистрация завершена";
            // 
            // materialLabel7
            // 
            this.materialLabel7.AutoSize = true;
            this.materialLabel7.Depth = 0;
            this.materialLabel7.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel7.Location = new System.Drawing.Point(118, 97);
            this.materialLabel7.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel7.Name = "materialLabel7";
            this.materialLabel7.Size = new System.Drawing.Size(153, 19);
            this.materialLabel7.TabIndex = 162;
            this.materialLabel7.Text = "Регистрация начата";
            // 
            // labelCreateXML
            // 
            this.labelCreateXML.AutoSize = true;
            this.labelCreateXML.Depth = 0;
            this.labelCreateXML.Enabled = false;
            this.labelCreateXML.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.labelCreateXML.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.labelCreateXML.HighEmphasis = true;
            this.labelCreateXML.Location = new System.Drawing.Point(77, 207);
            this.labelCreateXML.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelCreateXML.Name = "labelCreateXML";
            this.labelCreateXML.Size = new System.Drawing.Size(27, 41);
            this.labelCreateXML.TabIndex = 163;
            this.labelCreateXML.Text = "⭕";
            // 
            // labelRegistrationKKT
            // 
            this.labelRegistrationKKT.AutoSize = true;
            this.labelRegistrationKKT.Depth = 0;
            this.labelRegistrationKKT.Enabled = false;
            this.labelRegistrationKKT.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.labelRegistrationKKT.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.labelRegistrationKKT.HighEmphasis = true;
            this.labelRegistrationKKT.Location = new System.Drawing.Point(77, 281);
            this.labelRegistrationKKT.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelRegistrationKKT.Name = "labelRegistrationKKT";
            this.labelRegistrationKKT.Size = new System.Drawing.Size(27, 41);
            this.labelRegistrationKKT.TabIndex = 164;
            this.labelRegistrationKKT.Text = "⭕";
            // 
            // labelGetFD
            // 
            this.labelGetFD.AutoSize = true;
            this.labelGetFD.Depth = 0;
            this.labelGetFD.Enabled = false;
            this.labelGetFD.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.labelGetFD.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.labelGetFD.HighEmphasis = true;
            this.labelGetFD.Location = new System.Drawing.Point(78, 374);
            this.labelGetFD.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelGetFD.Name = "labelGetFD";
            this.labelGetFD.Size = new System.Drawing.Size(27, 41);
            this.labelGetFD.TabIndex = 165;
            this.labelGetFD.Text = "⭕";
            // 
            // labelCreateAkt
            // 
            this.labelCreateAkt.AutoSize = true;
            this.labelCreateAkt.Depth = 0;
            this.labelCreateAkt.Enabled = false;
            this.labelCreateAkt.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.labelCreateAkt.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.labelCreateAkt.HighEmphasis = true;
            this.labelCreateAkt.Location = new System.Drawing.Point(78, 484);
            this.labelCreateAkt.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelCreateAkt.Name = "labelCreateAkt";
            this.labelCreateAkt.Size = new System.Drawing.Size(27, 41);
            this.labelCreateAkt.TabIndex = 166;
            this.labelCreateAkt.Text = "⭕";
            // 
            // labelCodeChecking
            // 
            this.labelCodeChecking.AutoSize = true;
            this.labelCodeChecking.Depth = 0;
            this.labelCodeChecking.Enabled = false;
            this.labelCodeChecking.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.labelCodeChecking.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.labelCodeChecking.HighEmphasis = true;
            this.labelCodeChecking.Location = new System.Drawing.Point(78, 543);
            this.labelCodeChecking.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelCodeChecking.Name = "labelCodeChecking";
            this.labelCodeChecking.Size = new System.Drawing.Size(27, 41);
            this.labelCodeChecking.TabIndex = 167;
            this.labelCodeChecking.Text = "⭕";
            this.labelCodeChecking.Click += new System.EventHandler(this.labelCodeChecking_Click);
            // 
            // labelStatusStart
            // 
            this.labelStatusStart.AutoSize = true;
            this.labelStatusStart.Depth = 0;
            this.labelStatusStart.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.labelStatusStart.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.labelStatusStart.HighEmphasis = true;
            this.labelStatusStart.Location = new System.Drawing.Point(78, 84);
            this.labelStatusStart.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelStatusStart.Name = "labelStatusStart";
            this.labelStatusStart.Size = new System.Drawing.Size(24, 41);
            this.labelStatusStart.TabIndex = 168;
            this.labelStatusStart.Text = "🗸";
            // 
            // labelRegistrationFinish
            // 
            this.labelRegistrationFinish.AutoSize = true;
            this.labelRegistrationFinish.Depth = 0;
            this.labelRegistrationFinish.Enabled = false;
            this.labelRegistrationFinish.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.labelRegistrationFinish.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.labelRegistrationFinish.HighEmphasis = true;
            this.labelRegistrationFinish.Location = new System.Drawing.Point(78, 591);
            this.labelRegistrationFinish.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelRegistrationFinish.Name = "labelRegistrationFinish";
            this.labelRegistrationFinish.Size = new System.Drawing.Size(27, 41);
            this.labelRegistrationFinish.TabIndex = 169;
            this.labelRegistrationFinish.Text = "⭕";
            // 
            // materialLabel11
            // 
            this.materialLabel11.AutoSize = true;
            this.materialLabel11.Depth = 0;
            this.materialLabel11.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel11.Location = new System.Drawing.Point(117, 153);
            this.materialLabel11.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel11.Name = "materialLabel11";
            this.materialLabel11.Size = new System.Drawing.Size(197, 19);
            this.materialLabel11.TabIndex = 170;
            this.materialLabel11.Text = "Настройки DHCP введены";
            // 
            // verticalProgressBar1
            // 
            this.verticalProgressBar1.Location = new System.Drawing.Point(32, 94);
            this.verticalProgressBar1.Maximum = 100;
            this.verticalProgressBar1.Minimum = 0;
            this.verticalProgressBar1.Name = "verticalProgressBar1";
            this.verticalProgressBar1.ProgressColor = System.Drawing.Color.Transparent;
            this.verticalProgressBar1.Size = new System.Drawing.Size(10, 534);
            this.verticalProgressBar1.TabIndex = 159;
            this.verticalProgressBar1.Text = "verticalProgressBar1";
            this.verticalProgressBar1.Value = 0;
            // 
            // buttonCompleteRegistration
            // 
            this.buttonCompleteRegistration.AutoSize = false;
            this.buttonCompleteRegistration.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCompleteRegistration.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonCompleteRegistration.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.buttonCompleteRegistration.Depth = 0;
            this.buttonCompleteRegistration.HighEmphasis = true;
            this.buttonCompleteRegistration.Icon = null;
            this.buttonCompleteRegistration.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCompleteRegistration.Location = new System.Drawing.Point(448, 589);
            this.buttonCompleteRegistration.Margin = new System.Windows.Forms.Padding(14, 4, 4, 4);
            this.buttonCompleteRegistration.MouseState = MaterialSkin.MouseState.HOVER;
            this.buttonCompleteRegistration.Name = "buttonCompleteRegistration";
            this.buttonCompleteRegistration.NoAccentTextColor = System.Drawing.Color.Empty;
            this.buttonCompleteRegistration.Padding = new System.Windows.Forms.Padding(2);
            this.buttonCompleteRegistration.Size = new System.Drawing.Size(351, 47);
            this.buttonCompleteRegistration.TabIndex = 178;
            this.buttonCompleteRegistration.Text = "Прервать регистрацию";
            this.buttonCompleteRegistration.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.buttonCompleteRegistration.UseAccentColor = false;
            this.buttonCompleteRegistration.UseVisualStyleBackColor = true;
            this.buttonCompleteRegistration.Click += new System.EventHandler(this.buttonCompleteRegistration_Click);
            // 
            // ladelStatusInputNetworkSetting
            // 
            this.ladelStatusInputNetworkSetting.AutoSize = true;
            this.ladelStatusInputNetworkSetting.Depth = 0;
            this.ladelStatusInputNetworkSetting.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.ladelStatusInputNetworkSetting.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.ladelStatusInputNetworkSetting.HighEmphasis = true;
            this.ladelStatusInputNetworkSetting.Image = global::Registrator.Properties.Resources.circle;
            this.ladelStatusInputNetworkSetting.Location = new System.Drawing.Point(77, 140);
            this.ladelStatusInputNetworkSetting.MouseState = MaterialSkin.MouseState.HOVER;
            this.ladelStatusInputNetworkSetting.Name = "ladelStatusInputNetworkSetting";
            this.ladelStatusInputNetworkSetting.Size = new System.Drawing.Size(24, 41);
            this.ladelStatusInputNetworkSetting.TabIndex = 171;
            this.ladelStatusInputNetworkSetting.Text = "🗸";
            // 
            // buttonRegistrationKKT
            // 
            this.buttonRegistrationKKT.AutoSize = false;
            this.buttonRegistrationKKT.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonRegistrationKKT.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRegistrationKKT.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.buttonRegistrationKKT.Depth = 0;
            this.buttonRegistrationKKT.Enabled = false;
            this.buttonRegistrationKKT.HighEmphasis = true;
            this.buttonRegistrationKKT.Icon = ((System.Drawing.Image)(resources.GetObject("buttonRegistrationKKT.Icon")));
            this.buttonRegistrationKKT.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRegistrationKKT.Location = new System.Drawing.Point(120, 275);
            this.buttonRegistrationKKT.Margin = new System.Windows.Forms.Padding(14, 4, 4, 4);
            this.buttonRegistrationKKT.MouseState = MaterialSkin.MouseState.HOVER;
            this.buttonRegistrationKKT.Name = "buttonRegistrationKKT";
            this.buttonRegistrationKKT.NoAccentTextColor = System.Drawing.Color.Empty;
            this.buttonRegistrationKKT.Padding = new System.Windows.Forms.Padding(2);
            this.buttonRegistrationKKT.Size = new System.Drawing.Size(168, 47);
            this.buttonRegistrationKKT.TabIndex = 38;
            this.buttonRegistrationKKT.Text = "Регистрация Терминал-ФА";
            this.buttonRegistrationKKT.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.buttonRegistrationKKT.UseAccentColor = false;
            this.buttonRegistrationKKT.UseVisualStyleBackColor = true;
            this.buttonRegistrationKKT.Click += new System.EventHandler(this.buttonRegistrationKKT_Click);
            // 
            // buttonCreateAkt
            // 
            this.buttonCreateAkt.AutoSize = false;
            this.buttonCreateAkt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCreateAkt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonCreateAkt.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.buttonCreateAkt.Depth = 0;
            this.buttonCreateAkt.Enabled = false;
            this.buttonCreateAkt.HighEmphasis = true;
            this.buttonCreateAkt.Icon = ((System.Drawing.Image)(resources.GetObject("buttonCreateAkt.Icon")));
            this.buttonCreateAkt.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCreateAkt.Location = new System.Drawing.Point(121, 480);
            this.buttonCreateAkt.Margin = new System.Windows.Forms.Padding(14, 4, 4, 4);
            this.buttonCreateAkt.MouseState = MaterialSkin.MouseState.HOVER;
            this.buttonCreateAkt.Name = "buttonCreateAkt";
            this.buttonCreateAkt.NoAccentTextColor = System.Drawing.Color.Empty;
            this.buttonCreateAkt.Padding = new System.Windows.Forms.Padding(2);
            this.buttonCreateAkt.Size = new System.Drawing.Size(168, 47);
            this.buttonCreateAkt.TabIndex = 37;
            this.buttonCreateAkt.Text = "Акт ввода в эксплуатацию";
            this.buttonCreateAkt.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.buttonCreateAkt.UseAccentColor = false;
            this.buttonCreateAkt.UseVisualStyleBackColor = true;
            this.buttonCreateAkt.Click += new System.EventHandler(this.buttonCreateAkt_Click);
            // 
            // buttonXMLCreate
            // 
            this.buttonXMLCreate.AutoSize = false;
            this.buttonXMLCreate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonXMLCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonXMLCreate.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.buttonXMLCreate.Depth = 0;
            this.buttonXMLCreate.HighEmphasis = true;
            this.buttonXMLCreate.Icon = ((System.Drawing.Image)(resources.GetObject("buttonXMLCreate.Icon")));
            this.buttonXMLCreate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonXMLCreate.Location = new System.Drawing.Point(121, 201);
            this.buttonXMLCreate.Margin = new System.Windows.Forms.Padding(14, 4, 4, 4);
            this.buttonXMLCreate.MouseState = MaterialSkin.MouseState.HOVER;
            this.buttonXMLCreate.Name = "buttonXMLCreate";
            this.buttonXMLCreate.NoAccentTextColor = System.Drawing.Color.Empty;
            this.buttonXMLCreate.Padding = new System.Windows.Forms.Padding(2);
            this.buttonXMLCreate.Size = new System.Drawing.Size(168, 47);
            this.buttonXMLCreate.TabIndex = 36;
            this.buttonXMLCreate.Text = "Файл регистрации";
            this.buttonXMLCreate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.buttonXMLCreate.UseAccentColor = false;
            this.buttonXMLCreate.UseVisualStyleBackColor = true;
            this.buttonXMLCreate.Click += new System.EventHandler(this.buttonXMLCreate_Click);
            // 
            // MultiLineTextBoxResponseDocument
            // 
            this.MultiLineTextBoxResponseDocument.AnimateReadOnly = false;
            this.MultiLineTextBoxResponseDocument.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MultiLineTextBoxResponseDocument.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.MultiLineTextBoxResponseDocument.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.MultiLineTextBoxResponseDocument.Depth = 0;
            this.MultiLineTextBoxResponseDocument.HideSelection = true;
            this.MultiLineTextBoxResponseDocument.Location = new System.Drawing.Point(121, 337);
            this.MultiLineTextBoxResponseDocument.MaxLength = 32767;
            this.MultiLineTextBoxResponseDocument.MouseState = MaterialSkin.MouseState.OUT;
            this.MultiLineTextBoxResponseDocument.Name = "MultiLineTextBoxResponseDocument";
            this.MultiLineTextBoxResponseDocument.PasswordChar = '\0';
            this.MultiLineTextBoxResponseDocument.ReadOnly = false;
            this.MultiLineTextBoxResponseDocument.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.MultiLineTextBoxResponseDocument.SelectedText = "";
            this.MultiLineTextBoxResponseDocument.SelectionLength = 0;
            this.MultiLineTextBoxResponseDocument.SelectionStart = 0;
            this.MultiLineTextBoxResponseDocument.ShortcutsEnabled = true;
            this.MultiLineTextBoxResponseDocument.Size = new System.Drawing.Size(463, 121);
            this.MultiLineTextBoxResponseDocument.TabIndex = 220;
            this.MultiLineTextBoxResponseDocument.TabStop = false;
            this.MultiLineTextBoxResponseDocument.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.MultiLineTextBoxResponseDocument.UseSystemPasswordChar = false;
            this.MultiLineTextBoxResponseDocument.Visible = false;
            // 
            // RegistrationTerminalFA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 680);
            this.Controls.Add(this.MultiLineTextBoxResponseDocument);
            this.Controls.Add(this.buttonCompleteRegistration);
            this.Controls.Add(this.ladelStatusInputNetworkSetting);
            this.Controls.Add(this.materialLabel11);
            this.Controls.Add(this.labelRegistrationFinish);
            this.Controls.Add(this.labelStatusStart);
            this.Controls.Add(this.labelCodeChecking);
            this.Controls.Add(this.labelCreateAkt);
            this.Controls.Add(this.labelGetFD);
            this.Controls.Add(this.labelRegistrationKKT);
            this.Controls.Add(this.labelCreateXML);
            this.Controls.Add(this.materialLabel7);
            this.Controls.Add(this.materialLabel6);
            this.Controls.Add(this.materialLabel5);
            this.Controls.Add(this.verticalProgressBar1);
            this.Controls.Add(this.textBoxRNM);
            this.Controls.Add(this.buttonRegistrationKKT);
            this.Controls.Add(this.buttonCreateAkt);
            this.Controls.Add(this.buttonXMLCreate);
            this.Name = "RegistrationTerminalFA";
            this.Text = "Регистрация Терминал-ФА";
            this.Load += new System.EventHandler(this.RegistrationTerminalFA_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialButton buttonXMLCreate;
        private MaterialSkin.Controls.MaterialButton buttonCreateAkt;
        private MaterialSkin.Controls.MaterialButton buttonRegistrationKKT;
        private MaterialSkin.Controls.MaterialTextBox2 textBoxRNM;
        private VerticalProgressBar verticalProgressBar1;
        private MaterialSkin.Controls.MaterialLabel materialLabel5;
        private MaterialSkin.Controls.MaterialLabel materialLabel6;
        private MaterialSkin.Controls.MaterialLabel materialLabel7;
        private MaterialSkin.Controls.MaterialLabel labelCreateXML;
        private MaterialSkin.Controls.MaterialLabel labelRegistrationKKT;
        private MaterialSkin.Controls.MaterialLabel labelGetFD;
        private MaterialSkin.Controls.MaterialLabel labelCreateAkt;
        private MaterialSkin.Controls.MaterialLabel labelCodeChecking;
        private MaterialSkin.Controls.MaterialLabel labelStatusStart;
        private MaterialSkin.Controls.MaterialLabel labelRegistrationFinish;
        private MaterialSkin.Controls.MaterialLabel ladelStatusInputNetworkSetting;
        private MaterialSkin.Controls.MaterialLabel materialLabel11;
        private MaterialSkin.Controls.MaterialButton buttonCompleteRegistration;
        private MaterialSkin.Controls.MaterialMultiLineTextBox2 MultiLineTextBoxResponseDocument;
    }
}