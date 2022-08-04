namespace laserCraft_Control
{
    using laserCraft_Control.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class frmSettings : Form
    {
        private string language;
        private bool isMetric = true;
        private IContainer components;
        private Label lblUnits;
        private CheckBox cbMetric;
        private CheckBox cbImperial;
        private GroupBox sValue;
        private Label label4;
        private Label label3;
        private NumericUpDown nudSMax;
        private NumericUpDown nudSMin;
        private Label label2;
        private Label label1;
        private ComboBox cboBaudRates;
        private GroupBox groupBox1;
        private ComboBox cboDataBits;
        private Label label10;
        private Label label11;
        private Label label12;
        private ComboBox cboStopBits;
        private Label label13;
        private ComboBox cboParity;
        private Label label5;
        private ComboBox cboHandshaking;
        private Label label6;
        private CheckBox cbEnglish;
        private CheckBox cbVietnamese;
        private GroupBox groupBox2;
        private Label label15;
        private Label label7;
        private Label label8;
        private NumericUpDown nudMaxHeight;
        private NumericUpDown nudMaxWidth;
        private Label label9;
        private Label label14;
        private GroupBox groupBox3;
        private Button btnConfigure;
        private Label label16;
        private ComboBox cbGRBLVersion;
        private Label label17;
        private Label label19;
        private NumericUpDown nudYStep;
        private Label label18;
        private NumericUpDown nudXStep;
        private Label label23;
        private Label label22;
        private Label label20;
        private NumericUpDown nudYMaxRate;
        private Label label21;
        private NumericUpDown nudXMaxRate;
        private GroupBox groupBox4;
        private CheckBox cbIgnoreErrors;
        private Label label26;

        public frmSettings()
        {
            this.InitializeComponent();
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            if (this.cbGRBLVersion.Text == "GRBL v1.1")
            {
                this.MyParent.writeGRBLSettingsV11();
            }
        }

        private void cbEnglish_Click(object sender, EventArgs e)
        {
            this.cbVietnamese.Checked = false;
            Settings.Default.language = "English";
            Settings.Default.Save();
        }

        private void cbIgnoreErrors_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ignoreError = this.cbIgnoreErrors.Checked;
            Settings.Default.Save();
        }

        private void cbImperial_Click(object sender, EventArgs e)
        {
            this.cbMetric.Checked = false;
            Settings.Default.isMetric = false;
            Settings.Default.Save();
        }

        private void cbMetric_Click(object sender, EventArgs e)
        {
            this.cbImperial.Checked = false;
            Settings.Default.isMetric = true;
            Settings.Default.Save();
        }

        private void cbVietnamese_Click(object sender, EventArgs e)
        {
            this.cbEnglish.Checked = false;
            Settings.Default.language = "Vietnamese";
            Settings.Default.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.minS = Convert.ToInt32(this.nudSMin.Value);
            Settings.Default.maxS = Convert.ToInt32(this.nudSMax.Value);
            Settings.Default.baud = this.cboBaudRates.Text;
            Settings.Default.dataBits = this.cboDataBits.Text;
            Settings.Default.stopBits = this.cboStopBits.Text;
            Settings.Default.parity = this.cboParity.Text;
            Settings.Default.handShaking = this.cboHandshaking.Text;
            Settings.Default.maxWorkingX = (double) this.nudMaxWidth.Value;
            Settings.Default.maxWorkingY = (double) this.nudMaxHeight.Value;
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            if (Settings.Default.isMetric)
            {
                this.cbMetric.Checked = true;
                this.cbImperial.Checked = false;
                this.label7.Text = "mm";
                this.label15.Text = "mm";
            }
            else
            {
                this.cbMetric.Checked = false;
                this.cbImperial.Checked = true;
                this.label7.Text = "inches";
                this.label15.Text = "inches";
            }
            if (Settings.Default.language == "English")
            {
                this.cbEnglish.Checked = true;
                this.cbVietnamese.Checked = false;
            }
            else
            {
                this.cbEnglish.Checked = false;
                this.cbVietnamese.Checked = true;
            }
            if (Settings.Default.ignoreError)
            {
                this.cbIgnoreErrors.Checked = true;
            }
            else
            {
                this.cbIgnoreErrors.Checked = false;
            }
            this.nudSMin.Value = Settings.Default.minS;
            this.nudSMax.Value = Settings.Default.maxS;
            this.cboBaudRates.SelectedItem = Settings.Default.baud;
            this.cboDataBits.SelectedItem = Settings.Default.dataBits;
            this.cboStopBits.SelectedItem = Settings.Default.stopBits;
            this.cboParity.SelectedItem = Settings.Default.parity;
            this.cboHandshaking.SelectedItem = Settings.Default.handShaking;
            this.language = Settings.Default.language;
            this.isMetric = Settings.Default.isMetric;
            this.nudMaxWidth.Value = (decimal) Settings.Default.maxWorkingX;
            this.nudMaxHeight.Value = (decimal) Settings.Default.maxWorkingY;
            this.cbGRBLVersion.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.lblUnits = new System.Windows.Forms.Label();
            this.cbMetric = new System.Windows.Forms.CheckBox();
            this.cbImperial = new System.Windows.Forms.CheckBox();
            this.sValue = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudSMax = new System.Windows.Forms.NumericUpDown();
            this.nudSMin = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBaudRates = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboHandshaking = new System.Windows.Forms.ComboBox();
            this.cboParity = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cboStopBits = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cboDataBits = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbEnglish = new System.Windows.Forms.CheckBox();
            this.cbVietnamese = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.nudMaxHeight = new System.Windows.Forms.NumericUpDown();
            this.nudMaxWidth = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.nudYMaxRate = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.nudXMaxRate = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.nudYStep = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.nudXStep = new System.Windows.Forms.NumericUpDown();
            this.btnConfigure = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.cbGRBLVersion = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbIgnoreErrors = new System.Windows.Forms.CheckBox();
            this.label26 = new System.Windows.Forms.Label();
            this.sValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSMin)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxWidth)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYMaxRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXMaxRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXStep)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUnits
            // 
            this.lblUnits.AutoSize = true;
            this.lblUnits.Location = new System.Drawing.Point(12, 23);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(34, 16);
            this.lblUnits.TabIndex = 0;
            this.lblUnits.Text = "Unit:";
            // 
            // cbMetric
            // 
            this.cbMetric.AutoSize = true;
            this.cbMetric.Checked = true;
            this.cbMetric.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMetric.Location = new System.Drawing.Point(83, 22);
            this.cbMetric.Name = "cbMetric";
            this.cbMetric.Size = new System.Drawing.Size(96, 20);
            this.cbMetric.TabIndex = 3;
            this.cbMetric.Text = "Metric (mm)";
            this.cbMetric.UseVisualStyleBackColor = true;
            this.cbMetric.Click += new System.EventHandler(this.cbMetric_Click);
            // 
            // cbImperial
            // 
            this.cbImperial.AutoSize = true;
            this.cbImperial.Location = new System.Drawing.Point(175, 22);
            this.cbImperial.Name = "cbImperial";
            this.cbImperial.Size = new System.Drawing.Size(110, 20);
            this.cbImperial.TabIndex = 4;
            this.cbImperial.Text = "Imperial (inch)";
            this.cbImperial.UseVisualStyleBackColor = true;
            this.cbImperial.Click += new System.EventHandler(this.cbImperial_Click);
            // 
            // sValue
            // 
            this.sValue.Controls.Add(this.label4);
            this.sValue.Controls.Add(this.label3);
            this.sValue.Controls.Add(this.nudSMax);
            this.sValue.Controls.Add(this.nudSMin);
            this.sValue.Controls.Add(this.label2);
            this.sValue.Controls.Add(this.label1);
            this.sValue.Location = new System.Drawing.Point(5, 151);
            this.sValue.Name = "sValue";
            this.sValue.Size = new System.Drawing.Size(378, 90);
            this.sValue.TabIndex = 5;
            this.sValue.TabStop = false;
            this.sValue.Text = "S Value Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(8, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "control the power of the laser beam.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(7, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(354, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "This sets the min and max values to be used for the \"S\" gcode to";
            // 
            // nudSMax
            // 
            this.nudSMax.Location = new System.Drawing.Point(164, 58);
            this.nudSMax.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudSMax.Name = "nudSMax";
            this.nudSMax.Size = new System.Drawing.Size(70, 22);
            this.nudSMax.TabIndex = 4;
            this.nudSMax.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // nudSMin
            // 
            this.nudSMin.Location = new System.Drawing.Point(39, 58);
            this.nudSMin.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudSMin.Name = "nudSMin";
            this.nudSMin.Size = new System.Drawing.Size(70, 22);
            this.nudSMin.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(129, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Max:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Min:";
            // 
            // cboBaudRates
            // 
            this.cboBaudRates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBaudRates.FormattingEnabled = true;
            this.cboBaudRates.Items.AddRange(new object[] {
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cboBaudRates.Location = new System.Drawing.Point(97, 28);
            this.cboBaudRates.Name = "cboBaudRates";
            this.cboBaudRates.Size = new System.Drawing.Size(132, 24);
            this.cboBaudRates.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cboHandshaking);
            this.groupBox1.Controls.Add(this.cboParity);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.cboStopBits);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cboDataBits);
            this.groupBox1.Controls.Add(this.cboBaudRates);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(5, 241);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 116);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 16);
            this.label5.TabIndex = 20;
            this.label5.Text = "Handshaking:";
            // 
            // cboHandshaking
            // 
            this.cboHandshaking.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHandshaking.FormattingEnabled = true;
            this.cboHandshaking.Items.AddRange(new object[] {
            "None",
            "XOnXOff",
            "RequestToSend",
            "RequestToSendXOnXOff"});
            this.cboHandshaking.Location = new System.Drawing.Point(97, 82);
            this.cboHandshaking.Name = "cboHandshaking";
            this.cboHandshaking.Size = new System.Drawing.Size(132, 24);
            this.cboHandshaking.TabIndex = 19;
            // 
            // cboParity
            // 
            this.cboParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParity.FormattingEnabled = true;
            this.cboParity.Items.AddRange(new object[] {
            "None",
            "Even",
            "Mark",
            "Odd",
            "Space"});
            this.cboParity.Location = new System.Drawing.Point(285, 55);
            this.cboParity.Name = "cboParity";
            this.cboParity.Size = new System.Drawing.Size(64, 24);
            this.cboParity.TabIndex = 18;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(235, 61);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 16);
            this.label13.TabIndex = 17;
            this.label13.Text = "Parity:";
            // 
            // cboStopBits
            // 
            this.cboStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStopBits.FormattingEnabled = true;
            this.cboStopBits.Items.AddRange(new object[] {
            "One",
            "OnePointFive",
            "Two"});
            this.cboStopBits.Location = new System.Drawing.Point(97, 55);
            this.cboStopBits.Name = "cboStopBits";
            this.cboStopBits.Size = new System.Drawing.Size(132, 24);
            this.cboStopBits.TabIndex = 16;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 16);
            this.label12.TabIndex = 15;
            this.label12.Text = "Stop Bits:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(235, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 16);
            this.label11.TabIndex = 14;
            this.label11.Text = "Data Bits:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 16);
            this.label10.TabIndex = 13;
            this.label10.Text = "Baud Rate:";
            // 
            // cboDataBits
            // 
            this.cboDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDataBits.FormattingEnabled = true;
            this.cboDataBits.Items.AddRange(new object[] {
            "8",
            "7"});
            this.cboDataBits.Location = new System.Drawing.Point(306, 28);
            this.cboDataBits.Name = "cboDataBits";
            this.cboDataBits.Size = new System.Drawing.Size(43, 24);
            this.cboDataBits.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "Language:";
            // 
            // cbEnglish
            // 
            this.cbEnglish.AutoSize = true;
            this.cbEnglish.Location = new System.Drawing.Point(83, 47);
            this.cbEnglish.Name = "cbEnglish";
            this.cbEnglish.Size = new System.Drawing.Size(71, 20);
            this.cbEnglish.TabIndex = 13;
            this.cbEnglish.Text = "English";
            this.cbEnglish.UseVisualStyleBackColor = true;
            this.cbEnglish.Click += new System.EventHandler(this.cbEnglish_Click);
            // 
            // cbVietnamese
            // 
            this.cbVietnamese.AutoSize = true;
            this.cbVietnamese.Checked = true;
            this.cbVietnamese.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVietnamese.Location = new System.Drawing.Point(175, 48);
            this.cbVietnamese.Name = "cbVietnamese";
            this.cbVietnamese.Size = new System.Drawing.Size(99, 20);
            this.cbVietnamese.TabIndex = 14;
            this.cbVietnamese.Text = "Vietnamese";
            this.cbVietnamese.UseVisualStyleBackColor = true;
            this.cbVietnamese.Click += new System.EventHandler(this.cbVietnamese_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.nudMaxHeight);
            this.groupBox2.Controls.Add(this.nudMaxWidth);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Location = new System.Drawing.Point(5, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(378, 71);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Working Envelop";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(329, 42);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(30, 16);
            this.label15.TabIndex = 7;
            this.label15.Text = "mm";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(141, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "mm";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label8.Location = new System.Drawing.Point(5, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(340, 15);
            this.label8.TabIndex = 5;
            this.label8.Text = "Define the dimensions of the work area of your laser machine";
            // 
            // nudMaxHeight
            // 
            this.nudMaxHeight.DecimalPlaces = 2;
            this.nudMaxHeight.Location = new System.Drawing.Point(265, 39);
            this.nudMaxHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudMaxHeight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudMaxHeight.Name = "nudMaxHeight";
            this.nudMaxHeight.Size = new System.Drawing.Size(64, 22);
            this.nudMaxHeight.TabIndex = 4;
            this.nudMaxHeight.ThousandsSeparator = true;
            this.nudMaxHeight.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // nudMaxWidth
            // 
            this.nudMaxWidth.DecimalPlaces = 2;
            this.nudMaxWidth.Location = new System.Drawing.Point(77, 39);
            this.nudMaxWidth.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudMaxWidth.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudMaxWidth.Name = "nudMaxWidth";
            this.nudMaxWidth.Size = new System.Drawing.Size(64, 22);
            this.nudMaxWidth.TabIndex = 3;
            this.nudMaxWidth.Value = new decimal(new int[] {
            350,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(189, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 16);
            this.label9.TabIndex = 2;
            this.label9.Text = "Max Height:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 42);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 16);
            this.label14.TabIndex = 1;
            this.label14.Text = "Max Width:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.nudYMaxRate);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.nudXMaxRate);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.nudYStep);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.nudXStep);
            this.groupBox3.Controls.Add(this.btnConfigure);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.cbGRBLVersion);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Location = new System.Drawing.Point(5, 355);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(378, 127);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configure Mainboard";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(323, 102);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(55, 16);
            this.label23.TabIndex = 26;
            this.label23.Text = "mm/min";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(322, 77);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(55, 16);
            this.label22.TabIndex = 25;
            this.label22.Text = "mm/min";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(178, 102);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(74, 16);
            this.label20.TabIndex = 24;
            this.label20.Text = "Y max rate:";
            // 
            // nudYMaxRate
            // 
            this.nudYMaxRate.Enabled = false;
            this.nudYMaxRate.Location = new System.Drawing.Point(252, 100);
            this.nudYMaxRate.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudYMaxRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudYMaxRate.Name = "nudYMaxRate";
            this.nudYMaxRate.Size = new System.Drawing.Size(70, 22);
            this.nudYMaxRate.TabIndex = 23;
            this.nudYMaxRate.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(178, 77);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(73, 16);
            this.label21.TabIndex = 22;
            this.label21.Text = "X max rate:";
            // 
            // nudXMaxRate
            // 
            this.nudXMaxRate.Enabled = false;
            this.nudXMaxRate.Location = new System.Drawing.Point(252, 75);
            this.nudXMaxRate.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudXMaxRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudXMaxRate.Name = "nudXMaxRate";
            this.nudXMaxRate.Size = new System.Drawing.Size(70, 22);
            this.nudXMaxRate.TabIndex = 21;
            this.nudXMaxRate.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(11, 102);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(75, 16);
            this.label19.TabIndex = 20;
            this.label19.Text = "Y step/mm:";
            // 
            // nudYStep
            // 
            this.nudYStep.Enabled = false;
            this.nudYStep.Location = new System.Drawing.Point(101, 100);
            this.nudYStep.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudYStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudYStep.Name = "nudYStep";
            this.nudYStep.Size = new System.Drawing.Size(70, 22);
            this.nudYStep.TabIndex = 19;
            this.nudYStep.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 77);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(74, 16);
            this.label18.TabIndex = 18;
            this.label18.Text = "X step/mm:";
            // 
            // nudXStep
            // 
            this.nudXStep.Enabled = false;
            this.nudXStep.Location = new System.Drawing.Point(101, 75);
            this.nudXStep.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudXStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudXStep.Name = "nudXStep";
            this.nudXStep.Size = new System.Drawing.Size(70, 22);
            this.nudXStep.TabIndex = 17;
            this.nudXStep.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // btnConfigure
            // 
            this.btnConfigure.Location = new System.Drawing.Point(181, 44);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Size = new System.Drawing.Size(86, 24);
            this.btnConfigure.TabIndex = 16;
            this.btnConfigure.Text = "Configure";
            this.btnConfigure.UseVisualStyleBackColor = true;
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 50);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 16);
            this.label16.TabIndex = 15;
            this.label16.Text = "Version:";
            // 
            // cbGRBLVersion
            // 
            this.cbGRBLVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGRBLVersion.FormattingEnabled = true;
            this.cbGRBLVersion.Items.AddRange(new object[] {
            "GRBL v1.1"});
            this.cbGRBLVersion.Location = new System.Drawing.Point(67, 45);
            this.cbGRBLVersion.Name = "cbGRBLVersion";
            this.cbGRBLVersion.Size = new System.Drawing.Size(104, 24);
            this.cbGRBLVersion.TabIndex = 11;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label17.Location = new System.Drawing.Point(7, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(222, 15);
            this.label17.TabIndex = 5;
            this.label17.Text = "Automatically configure your mainboard";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbIgnoreErrors);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(5, 484);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(378, 71);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Ignore Errors";
            // 
            // cbIgnoreErrors
            // 
            this.cbIgnoreErrors.AutoSize = true;
            this.cbIgnoreErrors.Location = new System.Drawing.Point(10, 41);
            this.cbIgnoreErrors.Name = "cbIgnoreErrors";
            this.cbIgnoreErrors.Size = new System.Drawing.Size(104, 20);
            this.cbIgnoreErrors.TabIndex = 15;
            this.cbIgnoreErrors.Text = "Ignore Errors";
            this.cbIgnoreErrors.UseVisualStyleBackColor = true;
            this.cbIgnoreErrors.CheckedChanged += new System.EventHandler(this.cbIgnoreErrors_CheckedChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label26.Location = new System.Drawing.Point(5, 18);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(272, 15);
            this.label26.TabIndex = 5;
            this.label26.Text = "Ignore errors, do not stop the machine at runtime";
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 561);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cbVietnamese);
            this.Controls.Add(this.cbEnglish);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.sValue);
            this.Controls.Add(this.cbImperial);
            this.Controls.Add(this.cbMetric);
            this.Controls.Add(this.lblUnits);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_FormClosing);
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.sValue.ResumeLayout(false);
            this.sValue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSMin)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxWidth)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYMaxRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXMaxRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXStep)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public frmMain MyParent { get; set; }
    }
}

