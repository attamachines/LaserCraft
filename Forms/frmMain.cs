using laserCraft_Control.Properties;
using netDxf;
using netDxf.Entities;
using Ruler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace laserCraft_Control
{
    public class frmMain : Form
    {
        private DxfDocument dxf = new DxfDocument();
        private double canvasTop;
        private double canvasLeft;
        private double canvasBottom;
        private double canvasRight;
        private double canvasWidth;
        private double canvasHeight;
        private double pixelPerDXFUnit;
        private bool isFileLoaded;
        private double scale;
        private List<string> cutfileLines = new List<string>();
        private Bitmap originalImage;
        private Bitmap adjustedImage;
        private Bitmap displayedImage;
        private Bitmap gcodeImage;
        private string line;
        private int sValue;
        private int minS;
        private int maxS;
        private int laserIntensity;
        private int feedRate;
        private int pixelDelay;
        private float engraveResolution;
        private float coordX;
        private float coordY;
        private float lastX;
        private float lastY;
        private List<string> fileLines = new List<string>();
        private List<string> manualfileLines = new List<string>();
        public const int SYSTEM_STATUS_DISCONNECTED = 0;
        public const int SYSTEM_STATUS_IDLE = 1;
        public const int SYSTEM_STATUS_STREAMING_ENGRAVE = 2;
        public const int SYSTEM_STATUS_STREAMING_CUT = 3;
        public const int SYSTEM_STATUS_STREAMING_MANUAL = 4;
        public const int SYSTEM_STATUS_WRITING_GRLB_SETTINGS = 5;
        public const int RX_BUFFER_SIZE = 0x7c;
        public int systemStatus;
        private bool isStatusReportByUser;
        private List<string> streamResponse = new List<string>();
        public volatile string streamStatusReport = "";
        public volatile int streamResponseCount;
        public volatile int streamResponseTotal;
        public volatile int streamResponseDisplayed;
        public volatile int streamTimeInMillis;
        public volatile bool isStreamingPaused;
        private IContainer components;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private TabControl tabControl;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label1;
        private Label lblStatus;
        private Label label3;
        private Button btnSave;
        private Button btnPause;
        private Button btnStop;
        private Button btnSend;
        private Button btnEngraveOpen;
        private TabPage tabPage3;
        private GroupBox groupBox1;
        private Button button11;
        private Button button6;
        private Button button8;
        private Button button7;
        private GroupBox groupBox3;
        private NumericUpDown nudPixelDelay;
        private NumericUpDown nudLaserIntensity;
        private NumericUpDown nudFeedrate;
        private CheckBox cbSmartScan;
        private Label label6;
        private Label label5;
        private Label label4;
        private OpenFileDialog openFileDialog1;
        private Panel panel1;
        private PictureBox pictureBox1;
        private GroupBox groupJogging;
        private GroupBox groupGRBLTerminal;
        private Label lblInput;
        private GroupBox groupLaserIntensity;
        private Button btnClear;
        private Button btnConnect;
        private RichTextBox rtbIncoming;
        private ComboBox cboPorts;
        private Button btnFindPorts;
        private RichTextBox rtbOutgoing;
        private Label label14;
        private ComboBox cboResolution;
        private NumericUpDown nudWidth;
        private Label label8;
        private Label label7;
        private GroupBox grbGcode;
        private GroupBox grbOuput;
        private NumericUpDown nudHeight;
        private Label lblMaxHeight;
        private Label lblMaxWidth;
        private Label label16;
        private Label label15;
        private Button btnGenerate;
        private SaveFileDialog saveFileDialog1;
        private Panel panelCanvas;
        private GroupBox groupBox7;
        private Label label12;
        private Label label11;
        private NumericUpDown nudCutHeight;
        private NumericUpDown nudCutWidth;
        private Label label22;
        private Label label23;
        private Label label24;
        private GroupBox groupBox2;
        private Button btnCutSave;
        private Button btnCutGenerate;
        private Button btnCutPause;
        private Button btnCutStop;
        private Button btnCutSend;
        private GroupBox groupBox8;
        private Label label26;
        private Label label25;
        private Label label21;
        private NumericUpDown nudCutRepeat;
        private NumericUpDown nudCutLaserIntensity;
        private NumericUpDown nudCutFeedRate;
        private Label label13;
        private Label label19;
        private Label label20;
        private CheckBox cbCutCheckMode;
        private Ruler.Ruler ruler1;
        private Ruler.Ruler ruler2;
        private Button btnCutOpen;
        private GroupBox groupBox9;
        private Label label27;
        private Label label28;
        private Label label30;
        private Label label31;
        private Label label32;
        private Label lblLayers;
        private Label label37;
        private Label lblLwPolylines;
        private Label lblEllipses;
        private Label lblCircles;
        private Label lblArcs;
        private Label lblLines;
        private OpenFileDialog openFileDialog2;
        private Label label29;
        private Label label33;
        private Label label10;
        private Ruler.Ruler ruler3;
        private Ruler.Ruler ruler4;
        private Label label34;
        private Label label18;
        private Label label17;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Label label35;
        private Button btnLaserIntensityMax;
        private Button btnLaserIntensity4;
        private Button btnLaserIntensity3;
        private Button btnLaserIntensity2;
        private Button btnLaserIntensity1;
        private Button btnLaserOff;
        private Button btnReset;
        private ToolStripMenuItem manualToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private DataGridView dataGridView1;
        private CheckBox cbEngraveCheckMode;
        private DataGridView dataGridView2;
        private GroupBox groupGcodeProgramming;
        private Button btnLoadGcode;
        private Button btnManualPause;
        private Button btnManualStop;
        private Button btnManualSend;
        private DataGridView dataGridView3;
        private CheckBox cbManualCheckMode;
        private OpenFileDialog openFileDialog3;
        private Button btnArrow1;
        private Button btnArrow8;
        private Button btnArrow7;
        private Button btnArrow6;
        private Button btnArrow5;
        private Button btnArrow4;
        private Button btnArrow3;
        private Button btnArrow2;
        private Label label38;
        private Label label36;
        private GroupBox groupJoggingDistance;
        private GroupBox groupFanControl;
        private Button btnFanOff;
        private Button btnFanOn;
        private System.Windows.Forms.Timer timerCoordinates;
        private DataGridViewTextBoxColumn Block;
        private DataGridViewTextBoxColumn Status;
        public SerialPort serialPort1;
        private System.Windows.Forms.Timer timerUpdateSystemStatus;
        private BackgroundWorker backgroundWorkerStreaming;
        private Label lblEngraveTime;
        private ProgressBar pbEngrave;
        private Label lblCutTime;
        private ProgressBar pbCut;
        private Label lblManualTime;
        private ProgressBar pbManual;
        private Button btnKillAlarm;
        private Label lblEngravePercent;
        private Label lblCutPercent;
        private Label lblManualPercent;
        private NumericUpDown numScale;
        private Label label2;
        private GroupBox grCutViewer;
        private GroupBox grG_CodeProcess;
        private GroupBox grG_CodeControl;
        private Label lblRepertTimes;
        private Label label39;
        private Label label9;
        private Label label41;
        private Label label42;
        private Label lblWorkX;
        private Label lblWorkY;
        private PictureBox pictureBox2;
        private PictureBox pictureBox7;
        private PictureBox pictureBox6;
        private PictureBox pictureBox5;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private GroupBox groupBox4;
        private Button btnStopBeam;
        private Button btnPlayBeam;
        private Label label40;
        private NumericUpDown nmuLaserIntensity;
        private Label label43;
        private TabPage tabDesign;
        private GroupBox groupBox6;
        private Label label44;
        private Label label45;
        private ProgressBar progressBar1;
        private DataGridView dataGridView4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private CheckBox checkBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private GroupBox groupBox5;
        private GroupBox groupBox11;
        private GroupBox groupBox10;
        private CusDeginCtrl ctrlDesign;
        private TreeView treeItems;
        private Button button5;
        private ToolStripMenuItem settingsToolStripMenuItem1;
        private ToolStripMenuItem materialTemplatesToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private TrackBar trackBar1;

        public frmMain()
        {
            this.InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }

        private void adjustCutRulers()
        {
            if (this.isFileLoaded)
            {
                if ((this.canvasWidth / this.canvasHeight) > (((double) this.panelCanvas.Width) / ((double) this.panelCanvas.Height)))
                {
                    this.ruler1.pixelsPerUnit = (((float) this.panelCanvas.Width) / ((float) this.nudCutWidth.Value)) * 10f;
                    this.ruler2.pixelsPerUnit = this.ruler1.pixelsPerUnit;
                    this.scale = ((double) this.nudCutWidth.Value) / this.canvasWidth;
                }
                else
                {
                    this.ruler2.pixelsPerUnit = (((float) this.panelCanvas.Height) / ((float) this.nudCutHeight.Value)) * 10f;
                    this.ruler1.pixelsPerUnit = this.ruler2.pixelsPerUnit;
                    this.scale = ((double) this.nudCutHeight.Value) / this.canvasHeight;
                }

                numScale.ValueChanged -= numScale_ValueChanged;
                numScale.Value = (decimal)scale;
                numScale.ValueChanged += numScale_ValueChanged;

                this.ruler1.Invalidate();
                this.ruler2.Invalidate();
                if (Settings.Default.language == "English")
                {
                    this.label29.Text = "1 DXF unit = ";
                }
                else
                {
                    this.label29.Text = "1 đơn vị DXF = ";
                }
                if (this.scale.ToString().Length >= 9)
                {
                    this.label29.Text = this.label29.Text + this.scale.ToString().Substring(0, 8);
                }
                else
                {
                    this.label29.Text = this.label29.Text + this.scale.ToString();
                }
                if (Settings.Default.isMetric)
                {
                    this.label29.Text = this.label29.Text + " mm";
                }
                else
                {
                    this.label29.Text = this.label29.Text + " inch(es)";
                }
            }
        }

        private void adjustRulers()
        {
            if (this.displayedImage != null)
            {
                float num;
                if (Settings.Default.isMetric)
                {
                    num = ((float) this.nudWidth.Value) / 10f;
                }
                else
                {
                    num = (float) this.nudWidth.Value;
                }
                this.ruler3.pixelsPerUnit = ((float) this.displayedImage.Width) / num;
                this.ruler4.pixelsPerUnit = this.ruler3.pixelsPerUnit;
                this.ruler3.Invalidate();
                this.ruler4.Invalidate();
            }
        }

        private void backgroundWorkerStreaming_DoWork(object sender, DoWorkEventArgs e)
        {
            SerialPort port = new SerialPort {
                PortName = this.serialPort1.PortName,
                BaudRate = this.serialPort1.BaudRate,
                DataBits = this.serialPort1.DataBits,
                StopBits = this.serialPort1.StopBits,
                Handshake = this.serialPort1.Handshake,
                Parity = this.serialPort1.Parity
            };
            try
            {
                port.Open();
            }
            catch (Exception exception1)
            {
                this.systemStatus = 0;
                this.setSystemState(0);
                MessageBox.Show(exception1.Message, "Opening serialPort2");
                return;
            }
            List<int> list = new List<int>();
            List<string> fileLines = new List<string>();
            bool flag = false;
            switch (this.systemStatus)
            {
                case 2:
                    fileLines = this.fileLines;
                    if (this.cbEngraveCheckMode.Checked)
                    {
                        flag = true;
                    }
                    break;

                case 3:
                    fileLines = this.cutfileLines;
                    if (this.cbCutCheckMode.Checked)
                    {
                        flag = true;
                    }
                    break;

                case 4:
                    fileLines = this.manualfileLines;
                    if (this.cbManualCheckMode.Checked)
                    {
                        flag = true;
                    }
                    break;
            }
            if (flag)
            {
                try
                {
                    port.Write("$C\n");
                    port.ReadLine();
                }
                catch (Exception exception2)
                {
                    MessageBox.Show(exception2.Message, "Error while setting Check Mode");
                }
            }
            if (fileLines.Count != 0)
            {
                this.streamResponseTotal = fileLines.Count;
                int count = fileLines.Count;
                Stopwatch stopwatch = new Stopwatch();
                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch.Start();
                stopwatch2.Start();
                foreach (string str in fileLines)
                {
                    try
                    {
                        list.Add(str.Length + 1);
                        while ((((IEnumerable<int>) list).Sum() >= 0x7b) || (port.BytesToRead > 0))
                        {
                            if (this.isStreamingPaused)
                            {
                                try
                                {
                                    port.Write("!");
                                    stopwatch2.Restart();
                                    while (stopwatch2.ElapsedMilliseconds < 0x2710L)
                                    {
                                    }
                                    while (this.isStreamingPaused)
                                    {
                                    }
                                    port.Write("~");
                                }
                                catch
                                {
                                    this.systemStatus = 0;
                                    this.setSystemState(0);
                                }
                            }
                            if (!this.backgroundWorkerStreaming.CancellationPending)
                            {
                                goto Label_0264;
                            }
                            try
                            {
                                e.Cancel = true;
                                byte[] buffer = new byte[] { 0x18 };
                                port.Write(buffer, 0, 1);
                                stopwatch2.Restart();
                                while (stopwatch2.ElapsedMilliseconds < 100L)
                                {
                                }
                                port.Close();
                            }
                            catch
                            {
                                this.systemStatus = 0;
                                this.setSystemState(0);
                            }
                            return;
                        Label_0243:
                            if (stopwatch.ElapsedMilliseconds >= 250L)
                            {
                                port.Write("?");
                                stopwatch.Restart();
                            }
                        Label_0264:
                            if (port.BytesToRead <= 0)
                            {
                                goto Label_0243;
                            }
                            string item = port.ReadLine();
                            if (!item.Contains("ok") && !item.Contains("error"))
                            {
                                this.streamStatusReport = item;
                            }
                            else
                            {
                                if (item.Contains("ok"))
                                {
                                    this.streamResponse.Add(item);
                                    this.streamResponseCount++;
                                    list.RemoveAt(0);
                                    continue;
                                }
                                this.streamResponse.Add(item);
                                this.streamResponseCount++;
                                if (!Settings.Default.ignoreError)
                                {
                                    this.backgroundWorkerStreaming.CancelAsync();
                                }
                            }
                        }
                        port.Write(str + "\n");
                        if (stopwatch.ElapsedMilliseconds >= 250L)
                        {
                            port.Write("?");
                            stopwatch.Restart();
                        }
                    }
                    catch (Exception exception3)
                    {
                        MessageBox.Show(exception3.Message, "Error streaming G-code", MessageBoxButtons.OK);
                        return;
                    }
                }
                stopwatch2.Restart();
                while (stopwatch2.ElapsedMilliseconds < 200L)
                {
                }
                stopwatch2.Restart();
                stopwatch.Restart();
                while (this.streamResponseCount < count)
                {
                    if (this.isStreamingPaused)
                    {
                        try
                        {
                            port.Write("!");
                            stopwatch2.Restart();
                            while (stopwatch2.ElapsedMilliseconds < 0x2710L)
                            {
                            }
                            while (this.isStreamingPaused)
                            {
                            }
                            port.Write("~");
                        }
                        catch (Exception exception4)
                        {
                            MessageBox.Show(exception4.Message, "Error 0");
                            this.systemStatus = 0;
                            this.setSystemState(0);
                        }
                    }
                    if (!this.backgroundWorkerStreaming.CancellationPending)
                    {
                        goto Label_0489;
                    }
                    try
                    {
                        e.Cancel = true;
                        byte[] buffer = new byte[] { 0x18 };
                        port.Write(buffer, 0, 1);
                        stopwatch2.Restart();
                        while (stopwatch2.ElapsedMilliseconds < 100L)
                        {
                        }
                        port.Close();
                    }
                    catch
                    {
                        this.systemStatus = 0;
                        this.setSystemState(0);
                    }
                    return;
                Label_0468:
                    if (stopwatch.ElapsedMilliseconds >= 250L)
                    {
                        port.Write("?");
                        stopwatch.Restart();
                    }
                Label_0489:
                    if (port.BytesToRead <= 0)
                    {
                        goto Label_0468;
                    }
                    string item = port.ReadLine();
                    if (!item.Contains("ok") && !item.Contains("error"))
                    {
                        this.streamStatusReport = item;
                    }
                    else if (item.Contains("ok"))
                    {
                        this.streamResponse.Add(item);
                        this.streamResponseCount++;
                    }
                    else
                    {
                        this.streamResponse.Add(item);
                        this.streamResponseCount++;
                        if (!Settings.Default.ignoreError)
                        {
                            this.backgroundWorkerStreaming.CancelAsync();
                        }
                    }
                    if (stopwatch.ElapsedMilliseconds >= 250L)
                    {
                        port.Write("?");
                        stopwatch.Restart();
                    }
                }
                stopwatch2.Restart();
                while (stopwatch2.ElapsedMilliseconds < 500L)
                {
                }
                stopwatch2.Stop();
                if (flag)
                {
                    try
                    {
                        port.Write("$C\n");
                        port.ReadLine();
                    }
                    catch (Exception exception5)
                    {
                        MessageBox.Show(exception5.Message, "Disabling Check Mode");
                    }
                }
                try
                {
                    port.Close();
                }
                catch (Exception exception6)
                {
                    this.systemStatus = 0;
                    this.setSystemState(0);
                    MessageBox.Show(exception6.Message);
                    return;
                }
                Thread.Sleep(50);
            }
        }

        private void backgroundWorkerStreaming_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.serialPort1.Open();
                if (e.Cancelled)
                {
                    string str;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    while (stopwatch.ElapsedMilliseconds < 0x3e8L)
                    {
                    }
                    this.serialPort1.Write("$x\n");
                    for (str = this.serialPort1.ReadLine(); !str.Contains("ok"); str = this.serialPort1.ReadLine())
                    {
                    }
                    this.serialPort1.Write("G92X0Y0\n");
                    for (str = this.serialPort1.ReadLine(); !str.Contains("ok"); str = this.serialPort1.ReadLine())
                    {
                    }
                }
                this.systemStatus = 1;
                this.setSystemState(1);
            }
            catch (Exception exception1)
            {
                this.systemStatus = 0;
                this.setSystemState(0);
                MessageBox.Show(exception1.Message, "RunWorkerCompleted");
            }
        }

        private void btnArrow1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 Y1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow2_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 x-1000 Y1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow2_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow3_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 x-1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow3_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow4_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 x-1000 Y-1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow4_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow5_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 Y-1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow5_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow6_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 x1000 Y-1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow6_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow7_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 x1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow7_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow8_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("$J=G91 x1000 Y1000 F" + this.trackBar1.Value.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnArrow8_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    byte[] buffer = new byte[] { 0x85 };
                    this.serialPort1.Write(buffer, 0, 1);
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.rtbIncoming.Text = "";
        }

        private void btnCutGenerate_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus != 3) && this.isFileLoaded)
            {
                string str2;
                double canvasLeft = this.canvasLeft;
                double canvasBottom = this.canvasBottom;
                this.feedRate = Settings.Default.cutFeedrate;
                this.laserIntensity = Settings.Default.cutLaserIntensity;
                this.minS = Settings.Default.minS;
                this.maxS = Settings.Default.maxS;
                this.cutfileLines.Clear();
                this.dataGridView2.Columns.Clear();
                if (Settings.Default.language == "English")
                {
                    this.btnCutGenerate.Text = "Generating...";
                }
                else
                {
                    this.btnCutGenerate.Text = "Đang sinh m\x00e3...";
                }
                this.btnCutGenerate.Enabled = false;
                this.line = "(Code generated by laserCraft Control)";
                this.cutfileLines.Add(this.line);
                this.line = "(" + DateTime.Now.ToString("MMM/dd/yyy HH:mm:ss)");
                this.cutfileLines.Add(this.line);
                this.line = "M5";
                this.cutfileLines.Add(this.line);
                this.line = "G90"; // dat vi tri tuyet doi
                this.cutfileLines.Add(this.line);
                this.line = "G92 X0.000 Y0.000"; // dat vi tri
                this.cutfileLines.Add(this.line);

                if (Settings.Default.isMetric)
                {
                    this.line = "G21";
                }
                else
                {
                    this.line = "G20";
                }

                this.cutfileLines.Add(this.line);
                this.line = "M3 S"; // set toc cuong do laser
                this.line = this.line + Settings.Default.minS.ToString();
                this.cutfileLines.Add(this.line);
                this.line = "M8"; // bat quat lam mat
                this.cutfileLines.Add(this.line);

                if (this.dxf.Lines.Count > 0)
                {
                    foreach (netDxf.Entities.Line line in this.dxf.Lines)
                    {
                        Vector3 vector = new Vector3();
                        Vector3 vector2 = new Vector3();
                        vector.X = Math.Round((double) ((line.StartPoint.X - canvasLeft) * this.scale), 4);
                        vector.Y = Math.Round((double) ((line.StartPoint.Y - canvasBottom) * this.scale), 4);
                        vector2.X = Math.Round((double) ((line.EndPoint.X - canvasLeft) * this.scale), 4);
                        vector2.Y = Math.Round((double) ((line.EndPoint.Y - canvasBottom) * this.scale), 4);
                        string[] textArray1 = new string[] { "G0 X", vector.X.ToString(), " Y", vector.Y.ToString(), " S", this.minS.ToString() };
                        this.line = string.Concat(textArray1);
                        this.cutfileLines.Add(this.line);
                        string[] textArray2 = new string[] { "G1 X", vector2.X.ToString(), " Y", vector2.Y.ToString(), " F" };
                        this.line = string.Concat(textArray2);
                        this.line = this.line + this.nudCutFeedRate.Value.ToString();
                        this.line = this.line + " S";
                        this.line = this.line + this.mapCutSValue(Convert.ToInt32(this.nudCutLaserIntensity.Value), this.minS, this.maxS).ToString();
                        this.cutfileLines.Add(this.line);
                    }
                }
                if (this.dxf.LwPolylines.Count > 0)
                {
                    foreach (LwPolyline polyline in this.dxf.LwPolylines)
                    {
                        this.generateLwPolylineGcode(polyline);
                    }
                }
                if (this.dxf.Circles.Count > 0)
                {
                    foreach (Circle circle in this.dxf.Circles)
                    {
                        double num9 = Math.Round((double) (((circle.Center.X - circle.Radius) - canvasLeft) * this.scale), 4);
                        double num10 = Math.Round((double) ((circle.Center.Y - canvasBottom) * this.scale), 4);
                        double num11 = Math.Round((double) (circle.Radius * this.scale), 4);
                        string[] textArray3 = new string[] { "G0 X", num9.ToString(), " Y", num10.ToString(), " S", this.minS.ToString() };
                        this.line = string.Concat(textArray3);
                        this.cutfileLines.Add(this.line);
                        string[] textArray4 = new string[] { "G2 X", num9.ToString(), " Y", num10.ToString(), " I", num11.ToString(), " J0" };
                        this.line = string.Concat(textArray4);
                        string[] textArray5 = new string[] { this.line, " F", this.nudCutFeedRate.Value.ToString(), " S", this.mapCutSValue(Convert.ToInt32(this.nudCutLaserIntensity.Value), this.minS, this.maxS).ToString() };
                        this.line = string.Concat(textArray5);
                        this.cutfileLines.Add(this.line);
                    }
                }
                if (this.dxf.Ellipses.Count > 0)
                {
                    foreach (netDxf.Entities.Ellipse ellipse in this.dxf.Ellipses)
                    {
                        int num12;
                        double num13;
                        if (ellipse.IsFullEllipse)
                        {
                            num13 = 360.0;
                        }
                        else
                        {
                            num13 = ellipse.EndAngle - ellipse.StartAngle;
                            if (num13 < 0.0)
                            {
                                num13 += 360.0;
                            }
                        }
                        double num14 = ((((ellipse.MinorAxis + ellipse.MajorAxis) / 2.0) * this.scale) * num13) / 360.0;
                        if (num14 > Settings.Default.maxWorkingX)
                        {
                            num12 = 300;
                        }
                        else
                        {
                            num12 = Convert.ToInt32((double) (20.0 + ((num14 / Settings.Default.maxWorkingX) * 300.0)));
                        }
                        LwPolyline plElement = ellipse.ToPolyline(num12);
                        this.generateLwPolylineGcode(plElement);
                    }
                }
                if (this.dxf.Arcs.Count > 0)
                {
                    foreach (netDxf.Entities.Arc arc in this.dxf.Arcs)
                    {
                        Vector3 vector4 = new Vector3();
                        Vector3 vector5 = new Vector3();
                        double num15 = Math.Round((double) ((arc.Center.X - canvasLeft) * this.scale), 4);
                        double num16 = Math.Round((double) ((arc.Center.Y - canvasBottom) * this.scale), 4);
                        double num17 = Math.Round((double) (arc.Radius * this.scale), 4);
                        vector4.X = Math.Round((double) (num15 + (num17 * Math.Cos(this.degreesToRadians(arc.StartAngle)))), 4);
                        vector4.Y = Math.Round((double) (num16 + (num17 * Math.Sin(this.degreesToRadians(arc.StartAngle)))), 4);
                        vector5.X = Math.Round((double) (num15 + (num17 * Math.Cos(this.degreesToRadians(arc.EndAngle)))), 4);
                        vector5.Y = Math.Round((double) (num16 + (num17 * Math.Sin(this.degreesToRadians(arc.EndAngle)))), 4);
                        string[] textArray6 = new string[] { "G0 X", vector4.X.ToString(), " Y", vector4.Y.ToString(), " S", this.minS.ToString() };
                        this.line = string.Concat(textArray6);
                        this.cutfileLines.Add(this.line);
                        double num18 = num15 - vector4.X;
                        double num19 = num16 - vector4.Y;
                        string[] textArray7 = new string[] { "G3 X", vector5.X.ToString(), " Y", vector5.Y.ToString(), " I", num18.ToString(), " J", num19.ToString() };
                        this.line = string.Concat(textArray7);
                        string[] textArray8 = new string[] { this.line, " F", this.nudCutFeedRate.Value.ToString(), " S", this.mapCutSValue(Convert.ToInt32(this.nudCutLaserIntensity.Value), this.minS, this.maxS).ToString() };
                        this.line = string.Concat(textArray8);
                        this.cutfileLines.Add(this.line);
                    }
                }

                int num3 = 8;
                if ((((this.dxf.Circles.Count == 1)
                    && (this.dxf.Lines.Count == 0))
                    && ((this.dxf.LwPolylines.Count == 0) 
                    && (this.dxf.Ellipses.Count == 0)))
                    && (this.dxf.Arcs.Count == 0))
                {
                    num3 = 9;
                }
                else if ((((this.dxf.Ellipses.Count == 1) 
                    && (this.dxf.Lines.Count == 0)) 
                    && ((this.dxf.LwPolylines.Count == 0) 
                    && (this.dxf.Circles.Count == 0))) 
                    && (this.dxf.Arcs.Count == 0))
                {
                    num3 = 9;
                }
                else if ((((this.dxf.LwPolylines.Count == 1) 
                    && this.dxf.LwPolylines[0].IsClosed) 
                    && ((this.dxf.Lines.Count == 0) 
                    && (this.dxf.Ellipses.Count == 0))) 
                    && ((this.dxf.Circles.Count == 0) 
                    && (this.dxf.Arcs.Count == 0)))
                {
                    num3 = 9;
                }

                int num4 = this.cutfileLines.Count<string>() - 1;
                int num5 = Convert.ToInt32(this.nudCutRepeat.Value);

                for (int i = 1; i < num5; i++)
                {
                    for (int j = num3; j <= num4; j++)
                    {
                        string item = this.cutfileLines[j];
                        this.cutfileLines.Add(item);
                    }
                }

                this.line = "M5";
                this.cutfileLines.Add(this.line);
                this.line = "M9";
                this.cutfileLines.Add(this.line);
                this.line = "G0 X0.000 Y0.000";
                this.cutfileLines.Add(this.line);
                if (Settings.Default.language == "English")
                {
                    this.btnCutGenerate.Text = "Generate G-code";
                }
                else
                {
                    this.btnCutGenerate.Text = "Sinh m\x00e3 lệnh";
                }
                this.btnCutGenerate.Enabled = true;
                if (this.cutfileLines.Count > 0)
                {
                    this.btnCutSend.Enabled = true;
                    this.btnCutStop.Enabled = true;
                    this.btnCutPause.Enabled = true;
                    this.btnCutSave.Enabled = true;
                }
                List<StringValue> fileLines1 = new List<StringValue>();
                this.cutfileLines.ForEach(delegate (string ln) {
                    StringValue item = new StringValue(ln);
                    fileLines1.Add(item);
                });
                this.dataGridView2.DataSource = fileLines1;
                this.dataGridView2.Columns[0].HeaderText = "Block";
                this.dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView2.Columns[0].FillWeight = 77f;
                if (Settings.Default.language == "English")
                {
                    str2 = "Status";
                }
                else
                {
                    str2 = "Trạng th\x00e1i";
                }
                this.dataGridView2.Columns.Add("Status", str2);
                this.dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView2.Columns[1].FillWeight = 23f;
            }
        }

        private void btnCutOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if ((this.openFileDialog2.ShowDialog() == DialogResult.Cancel) || !File.Exists(this.openFileDialog2.FileName))
                {
                    return;
                }
                this.Refresh();
                this.dxf = DxfDocument.Load(this.openFileDialog2.FileName);
                this.panelCanvas.Invalidate();
                this.isFileLoaded = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error opening file: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            this.lblLines.Text = this.dxf.Lines.Count.ToString();
            this.lblArcs.Text = this.dxf.Arcs.Count.ToString();
            this.lblCircles.Text = this.dxf.Circles.Count.ToString();
            this.lblEllipses.Text = this.dxf.Ellipses.Count.ToString();
            this.lblLwPolylines.Text = this.dxf.LwPolylines.Count.ToString();
            this.lblLayers.Text = this.dxf.Layers.Count.ToString();
            this.getCanvasDimension();
            this.canvasWidth = this.canvasRight - this.canvasLeft;
            this.canvasHeight = this.canvasTop - this.canvasBottom;
            if ((this.canvasWidth / this.canvasHeight) > (Settings.Default.maxWorkingX / Settings.Default.maxWorkingY))
            {
                this.nudCutWidth.Maximum = (decimal) Settings.Default.maxWorkingX;
                this.nudCutWidth.Value = this.nudCutWidth.Maximum;
                double num3 = this.canvasWidth / this.canvasHeight;
                decimal num4 = this.nudCutWidth.Value / ((decimal) num3);
                this.nudCutHeight.Maximum = num4;
                this.nudCutHeight.Value = this.nudCutHeight.Maximum;
            }
            else
            {
                this.nudCutHeight.Maximum = (decimal) Settings.Default.maxWorkingY;
                this.nudCutHeight.Value = this.nudCutHeight.Maximum;
                double num5 = this.canvasWidth / this.canvasHeight;
                decimal num6 = this.nudCutHeight.Value * ((decimal) num5);
                this.nudCutWidth.Maximum = num6;
                this.nudCutWidth.Value = this.nudCutWidth.Maximum;
            }
            this.visualize();
            this.adjustCutRulers();
            this.groupBox2.Enabled = true;
            this.groupBox7.Enabled = true;
        }

        private void btnCutPause_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 3) && !this.isStreamingPaused)
            {
                this.timerUpdateSystemStatus.Stop();
                this.isStreamingPaused = true;
                this.btnCutPause.Enabled = false;
                this.btnCutStop.Enabled = false;
                this.lblStatus.Text = "Paused";
            }
        }

        private void btnCutSave_Click(object sender, EventArgs e)
        {
            if ((this.dataGridView2.Rows.Count != 0) && (this.saveFileDialog1.ShowDialog() != DialogResult.Cancel))
            {
                File.WriteAllLines(this.saveFileDialog1.FileName, this.cutfileLines);
            }
        }

        private void btnCutSend_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 1) || (this.systemStatus == 3))
            {
                if (this.systemStatus == 3)
                {
                    if (this.isStreamingPaused)
                    {
                        this.isStreamingPaused = false;
                        this.btnCutPause.Enabled = true;
                        this.btnCutStop.Enabled = true;
                        this.timerUpdateSystemStatus.Start();
                    }
                }
                else if (this.dataGridView2.Rows.Count != 0)
                {
                    if (!this.serialPort1.IsOpen)
                    {
                        this.systemStatus = 0;
                        this.setSystemState(0);
                    }
                    else
                    {
                        this.systemStatus = 3;
                        this.setSystemState(3);
                        try
                        {
                            this.serialPort1.Close();
                        }
                        catch (Exception exception1)
                        {
                            MessageBox.Show(exception1.Message, "Error closing port");
                        }
                        Thread.Sleep(100);
                        this.backgroundWorkerStreaming.RunWorkerAsync();
                    }
                }
            }
        }

        private void btnCutStop_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 3) && this.backgroundWorkerStreaming.IsBusy)
            {
                this.backgroundWorkerStreaming.CancelAsync();
            }
        }

        private void btnFanOff_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("M9\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnFanOn_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("M8\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus != 2) && (this.adjustedImage != null))
            {
                string str;
                this.feedRate = Settings.Default.feedRate;
                this.pixelDelay = Settings.Default.pixelDelay;
                this.laserIntensity = Settings.Default.laserIntensity;
                this.engraveResolution = float.Parse(this.cboResolution.Text, CultureInfo.InvariantCulture.NumberFormat);
                this.minS = Settings.Default.minS;
                this.maxS = Settings.Default.maxS;
                this.getGcodeImage();
                int width = this.gcodeImage.Width;
                int height = this.gcodeImage.Height;
                this.fileLines.Clear();
                this.dataGridView1.Columns.Clear();
                if (Settings.Default.language == "English")
                {
                    this.btnGenerate.Text = "Generating...";
                }
                else
                {
                    this.btnGenerate.Text = "Đang sinh m\x00e3...";
                }
                this.btnGenerate.Enabled = false;
                this.line = "(Code generated by laserCraft Control)";
                this.fileLines.Add(this.line);
                this.line = "(" + DateTime.Now.ToString("MMM/dd/yyy HH:mm:ss)");
                this.fileLines.Add(this.line);
                this.line = "M5";
                this.fileLines.Add(this.line);
                this.line = "G90";
                this.fileLines.Add(this.line);
                this.line = "G92X0.000Y0.000";
                this.fileLines.Add(this.line);
                if (Settings.Default.isMetric)
                {
                    this.line = "G21";
                }
                else
                {
                    this.line = "G20";
                }
                this.fileLines.Add(this.line);
                this.line = "G1F" + this.nudFeedrate.Value.ToString();
                this.fileLines.Add(this.line);
                this.line = "M3";
                this.fileLines.Add(this.line);
                this.line = "M8";
                this.fileLines.Add(this.line);
                this.lastX = -1f;
                this.lastY = -1f;
                bool flag = this.cbSmartScan.Checked;
                int num = 0;
                for (int i = this.gcodeImage.Height - 1; i >= 0; i--)
                {
                    this.coordY = this.engraveResolution * num;
                    if ((num % 2) == 0)
                    {
                        for (int j = 0; j < this.gcodeImage.Width; j++)
                        {
                            this.coordX = this.engraveResolution * j;
                            Color pixel = this.gcodeImage.GetPixel(j, i);
                            this.sValue = 0xff - pixel.R;
                            if (!((this.sValue == 0) & flag))
                            {
                                this.sValue = this.mapSValue(this.sValue, this.minS, this.maxS);
                                this.genGcodeLine();
                                this.lastX = this.coordX;
                                this.lastY = this.coordY;
                            }
                        }
                    }
                    else
                    {
                        for (int j = this.gcodeImage.Width - 1; j >= 0; j--)
                        {
                            this.coordX = this.engraveResolution * j;
                            Color pixel = this.gcodeImage.GetPixel(j, i);
                            this.sValue = 0xff - pixel.R;
                            if (!((this.sValue == 0) & flag))
                            {
                                this.sValue = this.mapSValue(this.sValue, this.minS, this.maxS);
                                this.genGcodeLine();
                                this.lastX = this.coordX;
                                this.lastY = this.coordY;
                            }
                        }
                    }
                    num++;
                }
                this.line = "M5";
                this.fileLines.Add(this.line);
                this.line = "M9";
                this.fileLines.Add(this.line);
                this.line = "G0X0.000Y0.000";
                this.fileLines.Add(this.line);
                this.gcodeImage.Dispose();
                if (Settings.Default.language == "English")
                {
                    this.btnGenerate.Text = "Generate G-code";
                }
                else
                {
                    this.btnGenerate.Text = "Sinh m\x00e3 lệnh";
                }
                this.btnGenerate.Enabled = true;
                if (this.fileLines.Count > 0)
                {
                    this.btnSend.Enabled = true;
                    this.btnStop.Enabled = true;
                    this.btnPause.Enabled = true;
                    this.btnSave.Enabled = true;
                }
                List<StringValue> fileLines1 = new List<StringValue>();
                this.fileLines.ForEach(delegate (string ln) {
                    StringValue item = new StringValue(ln);
                    fileLines1.Add(item);
                });
                this.dataGridView1.DataSource = fileLines1;
                this.dataGridView1.Columns[0].HeaderText = "Block";
                this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView1.Columns[0].FillWeight = 60f;
                if (Settings.Default.language == "English")
                {
                    str = "Status";
                }
                else
                {
                    str = "Trạng th\x00e1i";
                }
                this.dataGridView1.Columns.Add("Status", str);
                this.dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView1.Columns[1].FillWeight = 40f;
            }
        }

        private void btnKillAlarm_Click(object sender, EventArgs e)
        {
            string str;
            string str2;
            if (this.systemStatus != 1)
            {
                if (Settings.Default.language == "English")
                {
                    str = "Please wait until system is idle.";
                    str2 = "Kill alarm";
                }
                else
                {
                    str = "Vui l\x00f2ng đợi đến khi hệ thống ở trạng th\x00e1i chờ";
                    str2 = "X\x00f3a Alarm";
                }
                MessageBox.Show(str, str2, MessageBoxButtons.OK);
            }
            else
            {
                if (Settings.Default.language == "English")
                {
                    str = "Are you sure you want to kill alarm?";
                    str2 = "Kill Alarm";
                }
                else
                {
                    str = "Bạn muốn x\x00f3a Alarm?";
                    str2 = "X\x00f3a Alarm";
                }
                if (MessageBox.Show(str, str2, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        this.serialPort1.Write("$x\n");
                    }
                    catch (Exception exception1)
                    {
                        MessageBox.Show(exception1.Message);
                    }
                }
            }
        }

        private void btnLaserIntensity1_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                int num = Settings.Default.maxS / 15;
                try
                {
                    this.serialPort1.Write("G1M3F1000S" + num.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnLaserIntensity2_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                int num = Settings.Default.maxS / 6;
                try
                {
                    this.serialPort1.Write("G1M3F1000S" + num.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnLaserIntensity3_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                int num = Settings.Default.maxS / 4;
                try
                {
                    this.serialPort1.Write("G1M3F1000S" + num.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnLaserIntensity4_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                int num = Settings.Default.maxS / 2;
                try
                {
                    this.serialPort1.Write("G1M3F1000S" + num.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnLaserIntensityMax_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                int maxS = Settings.Default.maxS;
                try
                {
                    this.serialPort1.Write("G1M3F1000S" + maxS.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnLaserOff_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                try
                {
                    this.serialPort1.Write("G1M3S0F1000\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnLoadGcode_Click(object sender, EventArgs e)
        {
            try
            {
                if ((this.openFileDialog3.ShowDialog() == DialogResult.Cancel) || !File.Exists(this.openFileDialog3.FileName))
                {
                    return;
                }
                this.Refresh();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error opening file: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            List<StringValue> list = new List<StringValue>();
            this.manualfileLines.Clear();
            foreach (string str in File.ReadLines(this.openFileDialog3.FileName, Encoding.UTF8))
            {
                this.manualfileLines.Add(str);
                StringValue item = new StringValue(str);
                list.Add(item);
            }
            this.dataGridView3.Columns.Clear();
            this.dataGridView3.DataSource = list;
            this.dataGridView3.Columns[0].HeaderText = "G-code";
            this.dataGridView3.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView3.Columns[0].FillWeight = 70f;
            this.dataGridView3.Columns.Add("Status", "Status");
            this.dataGridView3.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView3.Columns[1].FillWeight = 30f;
            this.dataGridView3.Columns[1].ReadOnly = true;
        }

        private void btnManualPause_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 4) && !this.isStreamingPaused)
            {
                this.timerUpdateSystemStatus.Stop();
                this.isStreamingPaused = true;
                this.btnManualPause.Enabled = false;
                this.btnManualStop.Enabled = false;
                this.lblStatus.Text = "Paused";
            }
        }

        private void btnManualSend_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 1) || (this.systemStatus == 4))
            {
                if (this.systemStatus == 4)
                {
                    if (this.isStreamingPaused)
                    {
                        this.isStreamingPaused = false;
                        this.btnManualPause.Enabled = true;
                        this.btnManualStop.Enabled = true;
                        this.timerUpdateSystemStatus.Start();
                    }
                }
                else if (this.dataGridView3.Rows.Count != 0)
                {
                    if (!this.serialPort1.IsOpen)
                    {
                        this.systemStatus = 0;
                        this.setSystemState(0);
                    }
                    else
                    {
                        this.systemStatus = 4;
                        this.setSystemState(4);
                        try
                        {
                            this.serialPort1.Close();
                        }
                        catch (Exception exception1)
                        {
                            MessageBox.Show(exception1.Message, "Error closing port");
                        }
                        Thread.Sleep(100);
                        this.backgroundWorkerStreaming.RunWorkerAsync();
                    }
                }
            }
        }

        private void btnManualStop_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 4) && this.backgroundWorkerStreaming.IsBusy)
            {
                this.backgroundWorkerStreaming.CancelAsync();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 2) && !this.isStreamingPaused)
            {
                this.timerUpdateSystemStatus.Stop();
                this.isStreamingPaused = true;
                this.btnPause.Enabled = false;
                this.btnStop.Enabled = false;
                this.lblStatus.Text = "Paused";
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string str;
            string str2;
            if (this.systemStatus != 1)
            {
                if (Settings.Default.language == "English")
                {
                    str = "Cannot reset GRBL unless system is idle.";
                    str2 = "Reset GRBL";
                }
                else
                {
                    str = "Chỉ c\x00f3 thể khởi động lại m\x00e1y trong trạng th\x00e1i Chờ lệnh.";
                    str2 = "Khởi động lại";
                }
                MessageBox.Show(str, str2, MessageBoxButtons.OK);
            }
            else
            {
                if (Settings.Default.language == "English")
                {
                    str = "Are you sure you want to reset GRBL?";
                    str2 = "Reset GRBL";
                }
                else
                {
                    str = "Bạn muốn khởi động lại m\x00e1y laser?";
                    str2 = "Khởi động lại";
                }
                if (MessageBox.Show(str, str2, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        byte[] buffer = new byte[] { 0x18 };
                        this.serialPort1.Write(buffer, 0, 1);
                    }
                    catch (Exception exception1)
                    {
                        MessageBox.Show(exception1.Message);
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if ((this.dataGridView1.Rows.Count != 0) && (this.saveFileDialog1.ShowDialog() != DialogResult.Cancel))
            {
                File.WriteAllLines(this.saveFileDialog1.FileName, this.fileLines);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 1) || (this.systemStatus == 2))
            {
                if (this.systemStatus == 2)
                {
                    if (this.isStreamingPaused)
                    {
                        this.isStreamingPaused = false;
                        this.btnPause.Enabled = true;
                        this.btnStop.Enabled = true;
                        this.timerUpdateSystemStatus.Start();
                    }
                }
                else if (this.dataGridView1.Rows.Count != 0)
                {
                    if (!this.serialPort1.IsOpen)
                    {
                        this.systemStatus = 0;
                        this.setSystemState(0);
                    }
                    else
                    {
                        this.systemStatus = 2;
                        this.setSystemState(2);
                        try
                        {
                            this.serialPort1.Close();
                        }
                        catch (Exception exception1)
                        {
                            MessageBox.Show(exception1.Message, "Error closing port");
                        }
                        Thread.Sleep(100);
                        this.backgroundWorkerStreaming.RunWorkerAsync();
                    }
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if ((this.systemStatus == 2) && this.backgroundWorkerStreaming.IsBusy)
            {
                this.backgroundWorkerStreaming.CancelAsync();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if ((this.openFileDialog1.ShowDialog() == DialogResult.Cancel) || !File.Exists(this.openFileDialog1.FileName))
                {
                    return;
                }
                this.Refresh();
                this.originalImage = new Bitmap(System.Drawing.Image.FromFile(this.openFileDialog1.FileName));
                this.originalImage = this.imgGrayscale(this.originalImage);
                this.adjustedImage = new Bitmap(this.originalImage);
                this.updatePictureBox();
                this.grbOuput.Enabled = true;
                this.grbGcode.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error opening file: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            this.initiateOutputSize();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (this.adjustedImage != null)
            {
                this.adjustedImage = this.imgInvert(this.adjustedImage);
                this.originalImage = this.imgInvert(this.originalImage);
                this.updatePictureBox();
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if ((this.btnConnect.Text == "Connect") || (this.btnConnect.Text == "Kết nối"))
            {
                this.serialPort1.PortName = Convert.ToString(this.cboPorts.Text);
                this.serialPort1.BaudRate = Convert.ToInt32(Settings.Default.baud);
                this.serialPort1.DataBits = Convert.ToInt32(Settings.Default.dataBits);
                this.serialPort1.StopBits = (StopBits) Enum.Parse(typeof(StopBits), Settings.Default.stopBits);
                this.serialPort1.Handshake = (Handshake) Enum.Parse(typeof(Handshake), Settings.Default.handShaking);
                this.serialPort1.Parity = (Parity) Enum.Parse(typeof(Parity), Settings.Default.parity);
                try
                {
                    this.serialPort1.Open();
                    this.systemStatus = 1;
                    this.setSystemState(1);
                    if (Settings.Default.language == "English")
                    {
                        this.lblStatus.Text = "Wrong Port";
                    }
                    else
                    {
                        this.lblStatus.Text = "Sai cổng USB";
                    }
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
            else if ((this.btnConnect.Text == "Disconnect") || (this.btnConnect.Text == "Ngắt kết nối"))
            {
                try
                {
                    this.systemStatus = 0;
                    this.setSystemState(0);
                    this.serialPort1.Close();
                }
                catch (IOException exception2)
                {
                    MessageBox.Show(exception2.Message);
                }
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            string[] array = null;
            int index = 0;
            this.cboPorts.Items.Clear();
            this.cboPorts.Text = "";
            this.btnConnect.Enabled = false;
            array = SerialPort.GetPortNames();
            if (array.GetUpperBound(0) >= 0)
            {
                while (index <= array.GetUpperBound(0))
                {
                    this.cboPorts.Items.Add(array[index]);
                    index++;
                }
                Array.Sort<string>(array);
                this.cboPorts.Text = array[0];
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.adjustedImage != null)
            {
                this.adjustedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                this.updatePictureBox();
                this.initiateOutputSize();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.adjustedImage != null)
            {
                this.adjustedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                this.updatePictureBox();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.adjustedImage != null)
            {
                this.adjustedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
                this.updatePictureBox();
            }
        }

        private void calculateOutputSize(bool widthFirst)
        {
            if (this.adjustedImage != null)
            {
                float num = float.Parse(this.adjustedImage.Width.ToString(), CultureInfo.InvariantCulture.NumberFormat) * float.Parse(this.cboResolution.Text, CultureInfo.InvariantCulture.NumberFormat);
                float num2 = float.Parse(this.adjustedImage.Height.ToString(), CultureInfo.InvariantCulture.NumberFormat) * float.Parse(this.cboResolution.Text, CultureInfo.InvariantCulture.NumberFormat);
                this.lblMaxWidth.Text = num.ToString();
                this.lblMaxHeight.Text = num2.ToString();
                float num3 = float.Parse(this.adjustedImage.Width.ToString(), CultureInfo.InvariantCulture.NumberFormat) / float.Parse(this.adjustedImage.Height.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                if (widthFirst)
                {
                    decimal num5 = (decimal) (float.Parse(this.nudWidth.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat) / num3);
                    if (num5 < this.nudHeight.Minimum)
                    {
                        MessageBox.Show("Invalid input value", "Invalid value", MessageBoxButtons.OK);
                        this.nudHeight.Value = (decimal) num2;
                        this.nudWidth.Value = (decimal) num;
                        return;
                    }
                    this.nudHeight.Value = num5;
                }
                else
                {
                    decimal num7 = (decimal) (float.Parse(this.nudHeight.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat) * num3);
                    if (num7 < this.nudWidth.Minimum)
                    {
                        MessageBox.Show("Invalid input value", "Invalid value", MessageBoxButtons.OK);
                        this.nudWidth.Value = (decimal) num;
                        this.nudHeight.Value = (decimal) num2;
                        return;
                    }
                    this.nudWidth.Value = num7;
                }
                if ((this.nudWidth.Value > ((decimal) num)) || (this.nudHeight.Value > ((decimal) num2)))
                {
                    this.nudWidth.Value = (decimal) num;
                    this.nudHeight.Value = (decimal) num2;
                }
                this.adjustRulers();
            }
        }

        private void cboPorts_TextChanged(object sender, EventArgs e)
        {
            if (this.cboPorts.Text != "")
            {
                this.btnConnect.Enabled = true;
            }
            else
            {
                this.btnConnect.Enabled = false;
            }
        }

        private void cboResolution_TextChanged(object sender, EventArgs e)
        {
            this.calculateOutputSize(true);
        }

        private double degreesToRadians(double d) => 
            (d * 0.017453292519943295);

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private int flipY(int Y)
        {
            int num = this.panelCanvas.Height / 2;
            int num2 = Y + (2 * (num - Y));
            if (num2 == this.panelCanvas.Height)
            {
                num2--;
            }
            return num2;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.localize();
            this.cboResolution.SelectedIndex = 3;
            this.nudFeedrate.Value = Settings.Default.feedRate;
            this.nudPixelDelay.Value = Settings.Default.pixelDelay;
            this.nudLaserIntensity.Value = Settings.Default.laserIntensity;
            this.nudCutFeedRate.Value = Settings.Default.cutFeedrate;
            this.nudCutLaserIntensity.Value = Settings.Default.cutLaserIntensity;
            this.nudCutRepeat.Value = Settings.Default.cutRepeat;
            this.systemStatus = 0;
            this.setSystemState(0);
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (base.WindowState != FormWindowState.Minimized)
            {
                this.updatePictureBox();
                this.adjustRulers();
                this.visualize();
                this.adjustCutRulers();
            }
        }

        private void generateLwPolylineGcode(LwPolyline plElement)
        {
            double canvasLeft = this.canvasLeft;
            double canvasBottom = this.canvasBottom;
            double num3 = Math.Round((double) ((plElement.Vertexes[0].Position.X - canvasLeft) * this.scale), 4);
            double num4 = Math.Round((double) ((plElement.Vertexes[0].Position.Y - canvasBottom) * this.scale), 4);
            string[] textArray1 = new string[] { "G0 X", num3.ToString(), " Y", num4.ToString(), " S", this.minS.ToString() };
            this.line = string.Concat(textArray1);
            this.cutfileLines.Add(this.line);
            this.line = "G1 F";
            this.line = this.line + this.nudCutFeedRate.Value.ToString();
            this.line = this.line + " S";
            this.line = this.line + this.mapCutSValue(Convert.ToInt32(this.nudCutLaserIntensity.Value), this.minS, this.maxS).ToString();
            this.cutfileLines.Add(this.line);
            for (int i = 1; i < plElement.Vertexes.Count; i++)
            {
                this.line = "X" + Math.Round((double) ((plElement.Vertexes[i].Position.X - canvasLeft) * this.scale), 4).ToString() + " Y" + Math.Round((double) ((plElement.Vertexes[i].Position.Y - canvasBottom) * this.scale), 4).ToString();
                this.cutfileLines.Add(this.line);
            }
            if (plElement.IsClosed)
            {
                this.line = "X" + Math.Round((double) ((plElement.Vertexes[0].Position.X - canvasLeft) * this.scale), 4).ToString() + " Y" + Math.Round((double) ((plElement.Vertexes[0].Position.Y - canvasBottom) * this.scale), 4).ToString();
                this.cutfileLines.Add(this.line);
            }
        }

        private void genGcodeLine()
        {
            this.line = "";
            if (this.coordX != this.lastX)
            {
                object[] args = new object[] { this.coordX };
                string str = string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0:0.###}", args);
                this.line = this.line + "X" + str;
            }
            if (this.coordY != this.lastY)
            {
                object[] args = new object[] { this.coordY };
                string str2 = string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0:0.###}", args);
                this.line = this.line + "Y" + str2;
            }
            this.line = this.line + "S0";
            this.fileLines.Add(this.line);
            this.line = "";
            float num = ((float) this.pixelDelay) / 1000f;
            string[] textArray1 = new string[] { this.line, "G4P", num.ToString(), "S", this.sValue.ToString() };
            this.line = string.Concat(textArray1);
            this.fileLines.Add(this.line);
            this.line = "";
        }

        private void getCanvasDimension()
        {
            this.canvasTop = -1000000000.0;
            this.canvasLeft = 1000000000.0;
            this.canvasBottom = 1000000000.0;
            this.canvasRight = -1000000000.0;
            if (this.dxf.Lines.Count > 0)
            {
                foreach (netDxf.Entities.Line line in this.dxf.Lines)
                {
                    if (this.canvasLeft > line.StartPoint.X)
                    {
                        this.canvasLeft = line.StartPoint.X;
                    }
                    if (this.canvasLeft > line.EndPoint.X)
                    {
                        this.canvasLeft = line.EndPoint.X;
                    }
                    if (this.canvasRight < line.StartPoint.X)
                    {
                        this.canvasRight = line.StartPoint.X;
                    }
                    if (this.canvasRight < line.EndPoint.X)
                    {
                        this.canvasRight = line.EndPoint.X;
                    }
                    if (this.canvasTop < line.StartPoint.Y)
                    {
                        this.canvasTop = line.StartPoint.Y;
                    }
                    if (this.canvasTop < line.EndPoint.Y)
                    {
                        this.canvasTop = line.EndPoint.Y;
                    }
                    if (this.canvasBottom > line.StartPoint.Y)
                    {
                        this.canvasBottom = line.StartPoint.Y;
                    }
                    if (this.canvasBottom > line.EndPoint.Y)
                    {
                        this.canvasBottom = line.EndPoint.Y;
                    }
                }
            }
            if (this.dxf.LwPolylines.Count > 0)
            {
                using (IEnumerator<LwPolyline> enumerator2 = this.dxf.LwPolylines.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        foreach (LwPolylineVertex vertex in enumerator2.Current.Vertexes)
                        {
                            if (this.canvasLeft > vertex.Position.X)
                            {
                                this.canvasLeft = vertex.Position.X;
                            }
                            if (this.canvasRight < vertex.Position.X)
                            {
                                this.canvasRight = vertex.Position.X;
                            }
                            if (this.canvasTop < vertex.Position.Y)
                            {
                                this.canvasTop = vertex.Position.Y;
                            }
                            if (this.canvasBottom > vertex.Position.Y)
                            {
                                this.canvasBottom = vertex.Position.Y;
                            }
                        }
                    }
                }
            }
            if (this.dxf.Circles.Count > 0)
            {
                foreach (Circle circle in this.dxf.Circles)
                {
                    double num = circle.Center.X - circle.Radius;
                    double num2 = circle.Center.X + circle.Radius;
                    double num3 = circle.Center.Y + circle.Radius;
                    double num4 = circle.Center.Y - circle.Radius;
                    if (this.canvasLeft > num)
                    {
                        this.canvasLeft = num;
                    }
                    if (this.canvasRight < num2)
                    {
                        this.canvasRight = num2;
                    }
                    if (this.canvasTop < num3)
                    {
                        this.canvasTop = num3;
                    }
                    if (this.canvasBottom > num4)
                    {
                        this.canvasBottom = num4;
                    }
                }
            }
            if (this.dxf.Arcs.Count > 0)
            {
                using (IEnumerator<netDxf.Entities.Arc> enumerator5 = this.dxf.Arcs.GetEnumerator())
                {
                    while (enumerator5.MoveNext())
                    {
                        foreach (LwPolylineVertex vertex2 in enumerator5.Current.ToPolyline(200).Vertexes)
                        {
                            if (this.canvasLeft > vertex2.Position.X)
                            {
                                this.canvasLeft = vertex2.Position.X;
                            }
                            if (this.canvasRight < vertex2.Position.X)
                            {
                                this.canvasRight = vertex2.Position.X;
                            }
                            if (this.canvasTop < vertex2.Position.Y)
                            {
                                this.canvasTop = vertex2.Position.Y;
                            }
                            if (this.canvasBottom > vertex2.Position.Y)
                            {
                                this.canvasBottom = vertex2.Position.Y;
                            }
                        }
                    }
                }
            }
            if (this.dxf.Ellipses.Count > 0)
            {
                using (IEnumerator<netDxf.Entities.Ellipse> enumerator6 = this.dxf.Ellipses.GetEnumerator())
                {
                    while (enumerator6.MoveNext())
                    {
                        foreach (LwPolylineVertex vertex3 in enumerator6.Current.ToPolyline(200).Vertexes)
                        {
                            if (this.canvasLeft > vertex3.Position.X)
                            {
                                this.canvasLeft = vertex3.Position.X;
                            }
                            if (this.canvasRight < vertex3.Position.X)
                            {
                                this.canvasRight = vertex3.Position.X;
                            }
                            if (this.canvasTop < vertex3.Position.Y)
                            {
                                this.canvasTop = vertex3.Position.Y;
                            }
                            if (this.canvasBottom > vertex3.Position.Y)
                            {
                                this.canvasBottom = vertex3.Position.Y;
                            }
                        }
                    }
                }
            }
        }

        private void getGcodeImage()
        {
            if ((((float) this.nudWidth.Value) == float.Parse(this.lblMaxWidth.Text.ToString())) && (((float) this.nudHeight.Value) == float.Parse(this.lblMaxHeight.Text.ToString())))
            {
                this.gcodeImage = new Bitmap(this.adjustedImage);
            }
            else
            {
                int width = Convert.ToInt32((float) (float.Parse(this.nudWidth.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat) / float.Parse(this.cboResolution.Text, CultureInfo.InvariantCulture.NumberFormat))) + 1;
                int height = Convert.ToInt32((float) (float.Parse(this.nudHeight.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat) / float.Parse(this.cboResolution.Text, CultureInfo.InvariantCulture.NumberFormat))) + 1;
                this.gcodeImage = new Bitmap(this.adjustedImage, new Size(width, height));
            }
        }

        private Bitmap imgGrayscale(Bitmap original)
        {
            this.Refresh();
            Bitmap image = new Bitmap(original.Width, original.Height);
            Graphics graphics = Graphics.FromImage(image);
            float[][] newColorMatrix = new float[5][];
            newColorMatrix[0] = new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f };
            newColorMatrix[1] = new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f };
            newColorMatrix[2] = new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f };
            float[] singleArray1 = new float[5];
            singleArray1[3] = 1f;
            newColorMatrix[3] = singleArray1;
            float[] singleArray2 = new float[5];
            singleArray2[4] = 1f;
            newColorMatrix[4] = singleArray2;
            ColorMatrix matrix = new ColorMatrix(newColorMatrix);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(matrix);
            graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttr);
            graphics.Dispose();
            this.Refresh();
            return image;
        }

        private Bitmap imgInvert(Bitmap original)
        {
            this.Refresh();
            Bitmap image = new Bitmap(original.Width, original.Height);
            Graphics graphics = Graphics.FromImage(image);
            float[][] newColorMatrix = new float[5][];
            float[] singleArray1 = new float[5];
            singleArray1[0] = -1f;
            newColorMatrix[0] = singleArray1;
            float[] singleArray2 = new float[5];
            singleArray2[1] = -1f;
            newColorMatrix[1] = singleArray2;
            float[] singleArray3 = new float[5];
            singleArray3[2] = -1f;
            newColorMatrix[2] = singleArray3;
            float[] singleArray4 = new float[5];
            singleArray4[3] = 1f;
            newColorMatrix[3] = singleArray4;
            newColorMatrix[4] = new float[] { 1f, 1f, 1f, 0f, 1f };
            ColorMatrix matrix = new ColorMatrix(newColorMatrix);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(matrix);
            graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttr);
            graphics.Dispose();
            this.Refresh();
            return image;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ruler3 = new Ruler.Ruler();
            this.ruler4 = new Ruler.Ruler();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grbGcode = new System.Windows.Forms.GroupBox();
            this.lblEngravePercent = new System.Windows.Forms.Label();
            this.lblEngraveTime = new System.Windows.Forms.Label();
            this.pbEngrave = new System.Windows.Forms.ProgressBar();
            this.cbEngraveCheckMode = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.grbOuput = new System.Windows.Forms.GroupBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblMaxHeight = new System.Windows.Forms.Label();
            this.lblMaxWidth = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cboResolution = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.nudPixelDelay = new System.Windows.Forms.NumericUpDown();
            this.nudLaserIntensity = new System.Windows.Forms.NumericUpDown();
            this.nudFeedrate = new System.Windows.Forms.NumericUpDown();
            this.cbSmartScan = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button11 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.grCutViewer = new System.Windows.Forms.GroupBox();
            this.ruler2 = new Ruler.Ruler();
            this.ruler1 = new Ruler.Ruler();
            this.panelCanvas = new System.Windows.Forms.Panel();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.lblLayers = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.lblLwPolylines = new System.Windows.Forms.Label();
            this.lblEllipses = new System.Windows.Forms.Label();
            this.lblCircles = new System.Windows.Forms.Label();
            this.lblArcs = new System.Windows.Forms.Label();
            this.lblLines = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.nudCutRepeat = new System.Windows.Forms.NumericUpDown();
            this.nudCutLaserIntensity = new System.Windows.Forms.NumericUpDown();
            this.nudCutFeedRate = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.numScale = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.nudCutHeight = new System.Windows.Forms.NumericUpDown();
            this.nudCutWidth = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grG_CodeProcess = new System.Windows.Forms.GroupBox();
            this.lblRepertTimes = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.pbCut = new System.Windows.Forms.ProgressBar();
            this.lblCutPercent = new System.Windows.Forms.Label();
            this.lblCutTime = new System.Windows.Forms.Label();
            this.grG_CodeControl = new System.Windows.Forms.GroupBox();
            this.cbCutCheckMode = new System.Windows.Forms.CheckBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabDesign = new System.Windows.Forms.TabPage();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.treeItems = new System.Windows.Forms.TreeView();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupFanControl = new System.Windows.Forms.GroupBox();
            this.btnFanOff = new System.Windows.Forms.Button();
            this.btnFanOn = new System.Windows.Forms.Button();
            this.groupGcodeProgramming = new System.Windows.Forms.GroupBox();
            this.lblManualPercent = new System.Windows.Forms.Label();
            this.lblManualTime = new System.Windows.Forms.Label();
            this.pbManual = new System.Windows.Forms.ProgressBar();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.Block = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbManualCheckMode = new System.Windows.Forms.CheckBox();
            this.groupGRBLTerminal = new System.Windows.Forms.GroupBox();
            this.rtbOutgoing = new System.Windows.Forms.RichTextBox();
            this.rtbIncoming = new System.Windows.Forms.RichTextBox();
            this.lblInput = new System.Windows.Forms.Label();
            this.groupLaserIntensity = new System.Windows.Forms.GroupBox();
            this.label40 = new System.Windows.Forms.Label();
            this.nmuLaserIntensity = new System.Windows.Forms.NumericUpDown();
            this.label43 = new System.Windows.Forms.Label();
            this.groupJogging = new System.Windows.Forms.GroupBox();
            this.groupJoggingDistance = new System.Windows.Forms.GroupBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label38 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.cboPorts = new System.Windows.Forms.ComboBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog3 = new System.Windows.Forms.OpenFileDialog();
            this.timerCoordinates = new System.Windows.Forms.Timer(this.components);
            this.timerUpdateSystemStatus = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorkerStreaming = new System.ComponentModel.BackgroundWorker();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.lblWorkX = new System.Windows.Forms.Label();
            this.lblWorkY = new System.Windows.Forms.Label();
            this.btnKillAlarm = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnFindPorts = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnEngraveOpen = new System.Windows.Forms.Button();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnCutOpen = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.btnCutGenerate = new System.Windows.Forms.Button();
            this.btnCutSend = new System.Windows.Forms.Button();
            this.btnCutStop = new System.Windows.Forms.Button();
            this.btnCutPause = new System.Windows.Forms.Button();
            this.btnCutSave = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnLoadGcode = new System.Windows.Forms.Button();
            this.btnManualPause = new System.Windows.Forms.Button();
            this.btnManualStop = new System.Windows.Forms.Button();
            this.btnManualSend = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnStopBeam = new System.Windows.Forms.Button();
            this.btnPlayBeam = new System.Windows.Forms.Button();
            this.btnLaserIntensityMax = new System.Windows.Forms.Button();
            this.btnLaserIntensity4 = new System.Windows.Forms.Button();
            this.btnLaserIntensity3 = new System.Windows.Forms.Button();
            this.btnLaserIntensity2 = new System.Windows.Forms.Button();
            this.btnLaserIntensity1 = new System.Windows.Forms.Button();
            this.btnLaserOff = new System.Windows.Forms.Button();
            this.btnArrow8 = new System.Windows.Forms.Button();
            this.btnArrow7 = new System.Windows.Forms.Button();
            this.btnArrow6 = new System.Windows.Forms.Button();
            this.btnArrow5 = new System.Windows.Forms.Button();
            this.btnArrow4 = new System.Windows.Forms.Button();
            this.btnArrow3 = new System.Windows.Forms.Button();
            this.btnArrow2 = new System.Windows.Forms.Button();
            this.btnArrow1 = new System.Windows.Forms.Button();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.materialTemplatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctrlDesign = new laserCraft_Control.CusDeginCtrl();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grbGcode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.grbOuput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPixelDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFeedrate)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grCutViewer.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutRepeat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutLaserIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutFeedRate)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.grG_CodeProcess.SuspendLayout();
            this.grG_CodeControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabDesign.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupFanControl.SuspendLayout();
            this.groupGcodeProgramming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.groupGRBLTerminal.SuspendLayout();
            this.groupLaserIntensity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmuLaserIntensity)).BeginInit();
            this.groupJogging.SuspendLayout();
            this.groupJoggingDistance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1904, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.materialTemplatesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(61, 21);
            this.fileToolStripMenuItem.Text = "&System";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(51, 21);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            this.manualToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.manualToolStripMenuItem.Text = "User Guide";
            this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabDesign);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 67);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1904, 1079);
            this.tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.grbGcode);
            this.tabPage1.Controls.Add(this.grbOuput);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1896, 1047);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ENGRAVE";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.ruler3);
            this.groupBox4.Controls.Add(this.ruler4);
            this.groupBox4.Controls.Add(this.panel1);
            this.groupBox4.Location = new System.Drawing.Point(305, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1151, 931);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "groupBox4";
            // 
            // ruler3
            // 
            this.ruler3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ruler3.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ruler3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ruler3.isVertical = false;
            this.ruler3.Location = new System.Drawing.Point(26, 904);
            this.ruler3.Name = "ruler3";
            this.ruler3.pixelsPerUnit = 50F;
            this.ruler3.Size = new System.Drawing.Size(1119, 21);
            this.ruler3.TabIndex = 19;
            // 
            // ruler4
            // 
            this.ruler4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ruler4.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ruler4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ruler4.isVertical = true;
            this.ruler4.Location = new System.Drawing.Point(6, 24);
            this.ruler4.Name = "ruler4";
            this.ruler4.pixelsPerUnit = 50F;
            this.ruler4.Size = new System.Drawing.Size(21, 880);
            this.ruler4.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(27, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1118, 880);
            this.panel1.TabIndex = 5;
            // 
            // grbGcode
            // 
            this.grbGcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbGcode.Controls.Add(this.lblEngravePercent);
            this.grbGcode.Controls.Add(this.lblEngraveTime);
            this.grbGcode.Controls.Add(this.pbEngrave);
            this.grbGcode.Controls.Add(this.cbEngraveCheckMode);
            this.grbGcode.Controls.Add(this.dataGridView1);
            this.grbGcode.Controls.Add(this.btnSave);
            this.grbGcode.Controls.Add(this.btnGenerate);
            this.grbGcode.Controls.Add(this.btnPause);
            this.grbGcode.Controls.Add(this.btnStop);
            this.grbGcode.Controls.Add(this.btnSend);
            this.grbGcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbGcode.Location = new System.Drawing.Point(1462, 3);
            this.grbGcode.Name = "grbGcode";
            this.grbGcode.Size = new System.Drawing.Size(426, 931);
            this.grbGcode.TabIndex = 9;
            this.grbGcode.TabStop = false;
            this.grbGcode.Text = "Output G-code";
            // 
            // lblEngravePercent
            // 
            this.lblEngravePercent.AutoSize = true;
            this.lblEngravePercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEngravePercent.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblEngravePercent.Location = new System.Drawing.Point(206, 62);
            this.lblEngravePercent.Name = "lblEngravePercent";
            this.lblEngravePercent.Size = new System.Drawing.Size(25, 15);
            this.lblEngravePercent.TabIndex = 31;
            this.lblEngravePercent.Text = "0%";
            // 
            // lblEngraveTime
            // 
            this.lblEngraveTime.AutoSize = true;
            this.lblEngraveTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEngraveTime.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblEngraveTime.Location = new System.Drawing.Point(243, 62);
            this.lblEngraveTime.Name = "lblEngraveTime";
            this.lblEngraveTime.Size = new System.Drawing.Size(55, 15);
            this.lblEngraveTime.TabIndex = 30;
            this.lblEngraveTime.Text = "00:00:00";
            // 
            // pbEngrave
            // 
            this.pbEngrave.Location = new System.Drawing.Point(126, 62);
            this.pbEngrave.Name = "pbEngrave";
            this.pbEngrave.Size = new System.Drawing.Size(77, 17);
            this.pbEngrave.TabIndex = 29;
            // 
            // cbEngraveCheckMode
            // 
            this.cbEngraveCheckMode.AutoSize = true;
            this.cbEngraveCheckMode.Location = new System.Drawing.Point(4, 59);
            this.cbEngraveCheckMode.Name = "cbEngraveCheckMode";
            this.cbEngraveCheckMode.Size = new System.Drawing.Size(103, 20);
            this.cbEngraveCheckMode.TabIndex = 28;
            this.cbEngraveCheckMode.Text = "Check Mode";
            this.cbEngraveCheckMode.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(4, 85);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.Size = new System.Drawing.Size(416, 840);
            this.dataGridView1.TabIndex = 5;
            // 
            // grbOuput
            // 
            this.grbOuput.Controls.Add(this.label35);
            this.grbOuput.Controls.Add(this.label33);
            this.grbOuput.Controls.Add(this.label10);
            this.grbOuput.Controls.Add(this.lblMaxHeight);
            this.grbOuput.Controls.Add(this.lblMaxWidth);
            this.grbOuput.Controls.Add(this.label16);
            this.grbOuput.Controls.Add(this.label15);
            this.grbOuput.Controls.Add(this.nudHeight);
            this.grbOuput.Controls.Add(this.nudWidth);
            this.grbOuput.Controls.Add(this.label8);
            this.grbOuput.Controls.Add(this.label14);
            this.grbOuput.Controls.Add(this.cboResolution);
            this.grbOuput.Controls.Add(this.label7);
            this.grbOuput.Enabled = false;
            this.grbOuput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbOuput.Location = new System.Drawing.Point(1, 213);
            this.grbOuput.Name = "grbOuput";
            this.grbOuput.Size = new System.Drawing.Size(299, 116);
            this.grbOuput.TabIndex = 8;
            this.grbOuput.TabStop = false;
            this.grbOuput.Text = "Actual Output Size";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(155, 26);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(30, 16);
            this.label35.TabIndex = 29;
            this.label35.Text = "mm";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(155, 83);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(30, 16);
            this.label33.TabIndex = 28;
            this.label33.Text = "mm";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(155, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 16);
            this.label10.TabIndex = 27;
            this.label10.Text = "mm";
            // 
            // lblMaxHeight
            // 
            this.lblMaxHeight.AutoSize = true;
            this.lblMaxHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxHeight.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblMaxHeight.Location = new System.Drawing.Point(239, 83);
            this.lblMaxHeight.Name = "lblMaxHeight";
            this.lblMaxHeight.Size = new System.Drawing.Size(15, 16);
            this.lblMaxHeight.TabIndex = 26;
            this.lblMaxHeight.Text = "0";
            // 
            // lblMaxWidth
            // 
            this.lblMaxWidth.AutoSize = true;
            this.lblMaxWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxWidth.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblMaxWidth.Location = new System.Drawing.Point(239, 56);
            this.lblMaxWidth.Name = "lblMaxWidth";
            this.lblMaxWidth.Size = new System.Drawing.Size(15, 16);
            this.lblMaxWidth.TabIndex = 25;
            this.lblMaxWidth.Text = "0";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.OrangeRed;
            this.label16.Location = new System.Drawing.Point(206, 83);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(36, 16);
            this.label16.TabIndex = 24;
            this.label16.Text = "Max:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.OrangeRed;
            this.label15.Location = new System.Drawing.Point(206, 56);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(36, 16);
            this.label15.TabIndex = 23;
            this.label15.Text = "Max:";
            // 
            // nudHeight
            // 
            this.nudHeight.DecimalPlaces = 2;
            this.nudHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudHeight.Location = new System.Drawing.Point(87, 81);
            this.nudHeight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(68, 22);
            this.nudHeight.TabIndex = 22;
            this.nudHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHeight.ValueChanged += new System.EventHandler(this.nudHeight_ValueChanged);
            // 
            // nudWidth
            // 
            this.nudWidth.DecimalPlaces = 2;
            this.nudWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudWidth.Location = new System.Drawing.Point(87, 54);
            this.nudWidth.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(68, 22);
            this.nudWidth.TabIndex = 21;
            this.nudWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWidth.ValueChanged += new System.EventHandler(this.nudWidth_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(10, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 16);
            this.label8.TabIndex = 19;
            this.label8.Text = "Height:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(10, 26);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 16);
            this.label14.TabIndex = 17;
            this.label14.Text = "Resolution:";
            // 
            // cboResolution
            // 
            this.cboResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResolution.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboResolution.FormattingEnabled = true;
            this.cboResolution.Items.AddRange(new object[] {
            "0.04",
            "0.08",
            "0.12",
            "0.16",
            "0.20",
            "0.24",
            "0.28",
            "0.32"});
            this.cboResolution.Location = new System.Drawing.Point(86, 22);
            this.cboResolution.Name = "cboResolution";
            this.cboResolution.Size = new System.Drawing.Size(68, 24);
            this.cboResolution.TabIndex = 16;
            this.cboResolution.TextChanged += new System.EventHandler(this.cboResolution_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(10, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 16);
            this.label7.TabIndex = 18;
            this.label7.Text = "Width:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label34);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.nudPixelDelay);
            this.groupBox3.Controls.Add(this.nudLaserIntensity);
            this.groupBox3.Controls.Add(this.nudFeedrate);
            this.groupBox3.Controls.Add(this.cbSmartScan);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(1, 70);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(299, 142);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Engrave Settings";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(180, 82);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(73, 16);
            this.label34.TabIndex = 10;
            this.label34.Text = "milisecond";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(179, 54);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 16);
            this.label18.TabIndex = 9;
            this.label18.Text = "(0-255)";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(182, 25);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(55, 16);
            this.label17.TabIndex = 8;
            this.label17.Text = "mm/min";
            // 
            // nudPixelDelay
            // 
            this.nudPixelDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudPixelDelay.Location = new System.Drawing.Point(110, 80);
            this.nudPixelDelay.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudPixelDelay.Name = "nudPixelDelay";
            this.nudPixelDelay.Size = new System.Drawing.Size(68, 22);
            this.nudPixelDelay.TabIndex = 7;
            this.nudPixelDelay.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudPixelDelay.ValueChanged += new System.EventHandler(this.nudPixelDelay_ValueChanged);
            // 
            // nudLaserIntensity
            // 
            this.nudLaserIntensity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudLaserIntensity.Location = new System.Drawing.Point(110, 51);
            this.nudLaserIntensity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudLaserIntensity.Name = "nudLaserIntensity";
            this.nudLaserIntensity.Size = new System.Drawing.Size(68, 22);
            this.nudLaserIntensity.TabIndex = 6;
            this.nudLaserIntensity.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudLaserIntensity.ValueChanged += new System.EventHandler(this.nudLaserIntensity_ValueChanged);
            // 
            // nudFeedrate
            // 
            this.nudFeedrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudFeedrate.Location = new System.Drawing.Point(110, 22);
            this.nudFeedrate.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudFeedrate.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudFeedrate.Name = "nudFeedrate";
            this.nudFeedrate.Size = new System.Drawing.Size(68, 22);
            this.nudFeedrate.TabIndex = 4;
            this.nudFeedrate.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudFeedrate.ValueChanged += new System.EventHandler(this.nudFeedrate_ValueChanged);
            // 
            // cbSmartScan
            // 
            this.cbSmartScan.AutoSize = true;
            this.cbSmartScan.Checked = true;
            this.cbSmartScan.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSmartScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSmartScan.Location = new System.Drawing.Point(13, 108);
            this.cbSmartScan.Name = "cbSmartScan";
            this.cbSmartScan.Size = new System.Drawing.Size(93, 20);
            this.cbSmartScan.TabIndex = 3;
            this.cbSmartScan.Text = "SmartScan";
            this.cbSmartScan.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(9, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "Pixel Delay:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(10, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "Laser Intensity:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Feed Rate:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button11);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.btnEngraveOpen);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(1, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 66);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image Edit";
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button11.Location = new System.Drawing.Point(232, 24);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(56, 30);
            this.button11.TabIndex = 5;
            this.button11.Text = "Invert";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(176, 24);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(56, 30);
            this.button6.TabIndex = 0;
            this.button6.Text = "Rotate";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(120, 24);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(56, 30);
            this.button8.TabIndex = 2;
            this.button8.Text = "Flip Y";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(64, 24);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(56, 30);
            this.button7.TabIndex = 1;
            this.button7.Text = "Flip X";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.grCutViewer);
            this.tabPage2.Controls.Add(this.groupBox9);
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1896, 1047);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "CUT";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // grCutViewer
            // 
            this.grCutViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grCutViewer.Controls.Add(this.ruler2);
            this.grCutViewer.Controls.Add(this.ruler1);
            this.grCutViewer.Controls.Add(this.panelCanvas);
            this.grCutViewer.Location = new System.Drawing.Point(227, 6);
            this.grCutViewer.Name = "grCutViewer";
            this.grCutViewer.Size = new System.Drawing.Size(1232, 937);
            this.grCutViewer.TabIndex = 21;
            this.grCutViewer.TabStop = false;
            this.grCutViewer.Text = "Preview";
            // 
            // ruler2
            // 
            this.ruler2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ruler2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ruler2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ruler2.isVertical = true;
            this.ruler2.Location = new System.Drawing.Point(6, 21);
            this.ruler2.Name = "ruler2";
            this.ruler2.pixelsPerUnit = 50F;
            this.ruler2.Size = new System.Drawing.Size(21, 889);
            this.ruler2.TabIndex = 19;
            // 
            // ruler1
            // 
            this.ruler1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ruler1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ruler1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ruler1.isVertical = false;
            this.ruler1.Location = new System.Drawing.Point(27, 910);
            this.ruler1.Name = "ruler1";
            this.ruler1.pixelsPerUnit = 50F;
            this.ruler1.Size = new System.Drawing.Size(1199, 21);
            this.ruler1.TabIndex = 18;
            // 
            // panelCanvas
            // 
            this.panelCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCanvas.AutoSize = true;
            this.panelCanvas.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelCanvas.Location = new System.Drawing.Point(27, 21);
            this.panelCanvas.Name = "panelCanvas";
            this.panelCanvas.Size = new System.Drawing.Size(1199, 889);
            this.panelCanvas.TabIndex = 0;
            this.panelCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCanvas_Paint);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.pictureBox7);
            this.groupBox9.Controls.Add(this.pictureBox6);
            this.groupBox9.Controls.Add(this.pictureBox5);
            this.groupBox9.Controls.Add(this.pictureBox4);
            this.groupBox9.Controls.Add(this.pictureBox3);
            this.groupBox9.Controls.Add(this.pictureBox2);
            this.groupBox9.Controls.Add(this.lblLayers);
            this.groupBox9.Controls.Add(this.label37);
            this.groupBox9.Controls.Add(this.lblLwPolylines);
            this.groupBox9.Controls.Add(this.lblEllipses);
            this.groupBox9.Controls.Add(this.lblCircles);
            this.groupBox9.Controls.Add(this.lblArcs);
            this.groupBox9.Controls.Add(this.lblLines);
            this.groupBox9.Controls.Add(this.btnCutOpen);
            this.groupBox9.Controls.Add(this.label27);
            this.groupBox9.Controls.Add(this.label28);
            this.groupBox9.Controls.Add(this.label30);
            this.groupBox9.Controls.Add(this.label31);
            this.groupBox9.Controls.Add(this.label32);
            this.groupBox9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox9.Location = new System.Drawing.Point(1, 6);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(220, 218);
            this.groupBox9.TabIndex = 20;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Job";
            // 
            // lblLayers
            // 
            this.lblLayers.AutoSize = true;
            this.lblLayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLayers.Location = new System.Drawing.Point(90, 187);
            this.lblLayers.Name = "lblLayers";
            this.lblLayers.Size = new System.Drawing.Size(15, 16);
            this.lblLayers.TabIndex = 35;
            this.lblLayers.Text = "0";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(40, 186);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(52, 16);
            this.label37.TabIndex = 34;
            this.label37.Text = "Layers:";
            // 
            // lblLwPolylines
            // 
            this.lblLwPolylines.AutoSize = true;
            this.lblLwPolylines.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLwPolylines.Location = new System.Drawing.Point(90, 162);
            this.lblLwPolylines.Name = "lblLwPolylines";
            this.lblLwPolylines.Size = new System.Drawing.Size(15, 16);
            this.lblLwPolylines.TabIndex = 33;
            this.lblLwPolylines.Text = "0";
            // 
            // lblEllipses
            // 
            this.lblEllipses.AutoSize = true;
            this.lblEllipses.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEllipses.Location = new System.Drawing.Point(90, 137);
            this.lblEllipses.Name = "lblEllipses";
            this.lblEllipses.Size = new System.Drawing.Size(15, 16);
            this.lblEllipses.TabIndex = 32;
            this.lblEllipses.Text = "0";
            // 
            // lblCircles
            // 
            this.lblCircles.AutoSize = true;
            this.lblCircles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCircles.Location = new System.Drawing.Point(90, 113);
            this.lblCircles.Name = "lblCircles";
            this.lblCircles.Size = new System.Drawing.Size(15, 16);
            this.lblCircles.TabIndex = 31;
            this.lblCircles.Text = "0";
            // 
            // lblArcs
            // 
            this.lblArcs.AutoSize = true;
            this.lblArcs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArcs.Location = new System.Drawing.Point(90, 89);
            this.lblArcs.Name = "lblArcs";
            this.lblArcs.Size = new System.Drawing.Size(15, 16);
            this.lblArcs.TabIndex = 30;
            this.lblArcs.Text = "0";
            // 
            // lblLines
            // 
            this.lblLines.AutoSize = true;
            this.lblLines.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLines.Location = new System.Drawing.Point(90, 64);
            this.lblLines.Name = "lblLines";
            this.lblLines.Size = new System.Drawing.Size(15, 16);
            this.lblLines.TabIndex = 29;
            this.lblLines.Text = "0";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(11, 161);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(82, 16);
            this.label27.TabIndex = 25;
            this.label27.Text = "LwPolylines:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(40, 112);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(52, 16);
            this.label28.TabIndex = 24;
            this.label28.Text = "Circles:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(33, 136);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(59, 16);
            this.label30.TabIndex = 19;
            this.label30.Text = "Ellipses:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(49, 63);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(43, 16);
            this.label31.TabIndex = 17;
            this.label31.Text = "Lines:";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(54, 88);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(38, 16);
            this.label32.TabIndex = 18;
            this.label32.Text = "Arcs:";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.button5);
            this.groupBox8.Controls.Add(this.label26);
            this.groupBox8.Controls.Add(this.label25);
            this.groupBox8.Controls.Add(this.label21);
            this.groupBox8.Controls.Add(this.nudCutRepeat);
            this.groupBox8.Controls.Add(this.nudCutLaserIntensity);
            this.groupBox8.Controls.Add(this.nudCutFeedRate);
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Controls.Add(this.label19);
            this.groupBox8.Controls.Add(this.label20);
            this.groupBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox8.Location = new System.Drawing.Point(2, 392);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(219, 163);
            this.groupBox8.TabIndex = 17;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Cut Settings";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(160, 126);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(40, 16);
            this.label26.TabIndex = 10;
            this.label26.Text = "times";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(159, 71);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(55, 16);
            this.label25.TabIndex = 9;
            this.label25.Text = "mm/min";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(160, 98);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(48, 16);
            this.label21.TabIndex = 8;
            this.label21.Text = "(0-255)";
            // 
            // nudCutRepeat
            // 
            this.nudCutRepeat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCutRepeat.Location = new System.Drawing.Point(107, 123);
            this.nudCutRepeat.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudCutRepeat.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCutRepeat.Name = "nudCutRepeat";
            this.nudCutRepeat.Size = new System.Drawing.Size(53, 22);
            this.nudCutRepeat.TabIndex = 7;
            this.nudCutRepeat.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudCutRepeat.ValueChanged += new System.EventHandler(this.nudCutRepeat_ValueChanged);
            // 
            // nudCutLaserIntensity
            // 
            this.nudCutLaserIntensity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCutLaserIntensity.Location = new System.Drawing.Point(107, 95);
            this.nudCutLaserIntensity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCutLaserIntensity.Name = "nudCutLaserIntensity";
            this.nudCutLaserIntensity.Size = new System.Drawing.Size(53, 22);
            this.nudCutLaserIntensity.TabIndex = 6;
            this.nudCutLaserIntensity.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCutLaserIntensity.ValueChanged += new System.EventHandler(this.nudCutLaserIntensity_ValueChanged);
            // 
            // nudCutFeedRate
            // 
            this.nudCutFeedRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCutFeedRate.Location = new System.Drawing.Point(107, 68);
            this.nudCutFeedRate.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudCutFeedRate.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudCutFeedRate.Name = "nudCutFeedRate";
            this.nudCutFeedRate.Size = new System.Drawing.Size(52, 22);
            this.nudCutFeedRate.TabIndex = 4;
            this.nudCutFeedRate.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudCutFeedRate.ValueChanged += new System.EventHandler(this.nudCutFeedRate_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(7, 125);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 16);
            this.label13.TabIndex = 2;
            this.label13.Text = "Repeat:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(7, 98);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(96, 16);
            this.label19.TabIndex = 1;
            this.label19.Text = "Laser Intensity:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(7, 71);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(75, 16);
            this.label20.TabIndex = 0;
            this.label20.Text = "Feed Rate:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.numScale);
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.label29);
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.nudCutHeight);
            this.groupBox7.Controls.Add(this.nudCutWidth);
            this.groupBox7.Controls.Add(this.label22);
            this.groupBox7.Controls.Add(this.label23);
            this.groupBox7.Controls.Add(this.label24);
            this.groupBox7.Enabled = false;
            this.groupBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.Location = new System.Drawing.Point(1, 230);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(220, 157);
            this.groupBox7.TabIndex = 16;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Actual Output Size";
            // 
            // numScale
            // 
            this.numScale.DecimalPlaces = 15;
            this.numScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numScale.Location = new System.Drawing.Point(81, 49);
            this.numScale.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numScale.Name = "numScale";
            this.numScale.Size = new System.Drawing.Size(128, 22);
            this.numScale.TabIndex = 28;
            this.numScale.ValueChanged += new System.EventHandler(this.numScale_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 16);
            this.label2.TabIndex = 27;
            this.label2.Text = "Set Scale:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(54, 27);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(15, 16);
            this.label29.TabIndex = 26;
            this.label29.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(142, 108);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(30, 16);
            this.label12.TabIndex = 25;
            this.label12.Text = "mm";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(143, 80);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(30, 16);
            this.label11.TabIndex = 24;
            this.label11.Text = "mm";
            // 
            // nudCutHeight
            // 
            this.nudCutHeight.DecimalPlaces = 2;
            this.nudCutHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCutHeight.Location = new System.Drawing.Point(81, 105);
            this.nudCutHeight.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nudCutHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCutHeight.Name = "nudCutHeight";
            this.nudCutHeight.Size = new System.Drawing.Size(61, 22);
            this.nudCutHeight.TabIndex = 22;
            this.nudCutHeight.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudCutHeight.ValueChanged += new System.EventHandler(this.nudCutHeight_ValueChanged);
            // 
            // nudCutWidth
            // 
            this.nudCutWidth.DecimalPlaces = 2;
            this.nudCutWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCutWidth.Location = new System.Drawing.Point(81, 77);
            this.nudCutWidth.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudCutWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCutWidth.Name = "nudCutWidth";
            this.nudCutWidth.Size = new System.Drawing.Size(61, 22);
            this.nudCutWidth.TabIndex = 21;
            this.nudCutWidth.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudCutWidth.ValueChanged += new System.EventHandler(this.nudCutWidth_ValueChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(10, 108);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(50, 16);
            this.label22.TabIndex = 19;
            this.label22.Text = "Height:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(10, 26);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(46, 16);
            this.label23.TabIndex = 17;
            this.label23.Text = "Scale:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(11, 80);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(45, 16);
            this.label24.TabIndex = 18;
            this.label24.Text = "Width:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.grG_CodeProcess);
            this.groupBox2.Controls.Add(this.grG_CodeControl);
            this.groupBox2.Controls.Add(this.dataGridView2);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(1465, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(426, 937);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output G-code";
            // 
            // grG_CodeProcess
            // 
            this.grG_CodeProcess.Controls.Add(this.lblRepertTimes);
            this.grG_CodeProcess.Controls.Add(this.label9);
            this.grG_CodeProcess.Controls.Add(this.label39);
            this.grG_CodeProcess.Controls.Add(this.pbCut);
            this.grG_CodeProcess.Controls.Add(this.lblCutPercent);
            this.grG_CodeProcess.Controls.Add(this.lblCutTime);
            this.grG_CodeProcess.Location = new System.Drawing.Point(6, 123);
            this.grG_CodeProcess.Name = "grG_CodeProcess";
            this.grG_CodeProcess.Size = new System.Drawing.Size(414, 78);
            this.grG_CodeProcess.TabIndex = 35;
            this.grG_CodeProcess.TabStop = false;
            this.grG_CodeProcess.Text = "Process";
            // 
            // lblRepertTimes
            // 
            this.lblRepertTimes.AutoSize = true;
            this.lblRepertTimes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblRepertTimes.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblRepertTimes.Location = new System.Drawing.Point(68, 48);
            this.lblRepertTimes.Name = "lblRepertTimes";
            this.lblRepertTimes.Size = new System.Drawing.Size(29, 16);
            this.lblRepertTimes.TabIndex = 36;
            this.lblRepertTimes.Text = "0%";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(99, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 16);
            this.label9.TabIndex = 35;
            this.label9.Text = "times";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(6, 47);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(56, 16);
            this.label39.TabIndex = 34;
            this.label39.Text = "Repeat:";
            // 
            // pbCut
            // 
            this.pbCut.Location = new System.Drawing.Point(6, 24);
            this.pbCut.Name = "pbCut";
            this.pbCut.Size = new System.Drawing.Size(283, 15);
            this.pbCut.TabIndex = 31;
            // 
            // lblCutPercent
            // 
            this.lblCutPercent.AutoSize = true;
            this.lblCutPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblCutPercent.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblCutPercent.Location = new System.Drawing.Point(293, 23);
            this.lblCutPercent.Name = "lblCutPercent";
            this.lblCutPercent.Size = new System.Drawing.Size(29, 16);
            this.lblCutPercent.TabIndex = 33;
            this.lblCutPercent.Text = "0%";
            // 
            // lblCutTime
            // 
            this.lblCutTime.AutoSize = true;
            this.lblCutTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblCutTime.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblCutTime.Location = new System.Drawing.Point(344, 23);
            this.lblCutTime.Name = "lblCutTime";
            this.lblCutTime.Size = new System.Drawing.Size(64, 16);
            this.lblCutTime.TabIndex = 32;
            this.lblCutTime.Text = "00:00:00";
            // 
            // grG_CodeControl
            // 
            this.grG_CodeControl.Controls.Add(this.cbCutCheckMode);
            this.grG_CodeControl.Controls.Add(this.btnCutGenerate);
            this.grG_CodeControl.Controls.Add(this.btnCutSend);
            this.grG_CodeControl.Controls.Add(this.btnCutStop);
            this.grG_CodeControl.Controls.Add(this.btnCutPause);
            this.grG_CodeControl.Controls.Add(this.btnCutSave);
            this.grG_CodeControl.Location = new System.Drawing.Point(6, 21);
            this.grG_CodeControl.Name = "grG_CodeControl";
            this.grG_CodeControl.Size = new System.Drawing.Size(414, 95);
            this.grG_CodeControl.TabIndex = 34;
            this.grG_CodeControl.TabStop = false;
            this.grG_CodeControl.Text = "Control";
            // 
            // cbCutCheckMode
            // 
            this.cbCutCheckMode.AutoSize = true;
            this.cbCutCheckMode.Location = new System.Drawing.Point(9, 65);
            this.cbCutCheckMode.Name = "cbCutCheckMode";
            this.cbCutCheckMode.Size = new System.Drawing.Size(103, 20);
            this.cbCutCheckMode.TabIndex = 27;
            this.cbCutCheckMode.Text = "Check Mode";
            this.cbCutCheckMode.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView2.Location = new System.Drawing.Point(4, 207);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView2.Size = new System.Drawing.Size(416, 724);
            this.dataGridView2.TabIndex = 28;
            // 
            // tabDesign
            // 
            this.tabDesign.Controls.Add(this.groupBox11);
            this.tabDesign.Controls.Add(this.groupBox10);
            this.tabDesign.Controls.Add(this.groupBox6);
            this.tabDesign.Controls.Add(this.groupBox5);
            this.tabDesign.Location = new System.Drawing.Point(4, 28);
            this.tabDesign.Name = "tabDesign";
            this.tabDesign.Padding = new System.Windows.Forms.Padding(3);
            this.tabDesign.Size = new System.Drawing.Size(1896, 1047);
            this.tabDesign.TabIndex = 3;
            this.tabDesign.Text = "CUSTOM DESIGN";
            this.tabDesign.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox11.Location = new System.Drawing.Point(1160, 6);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(278, 926);
            this.groupBox11.TabIndex = 6;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Properties";
            // 
            // groupBox10
            // 
            this.groupBox10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox10.Controls.Add(this.treeItems);
            this.groupBox10.Location = new System.Drawing.Point(6, 9);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(254, 926);
            this.groupBox10.TabIndex = 5;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Items";
            // 
            // treeItems
            // 
            this.treeItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeItems.Location = new System.Drawing.Point(3, 18);
            this.treeItems.Name = "treeItems";
            this.treeItems.Size = new System.Drawing.Size(248, 905);
            this.treeItems.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.label44);
            this.groupBox6.Controls.Add(this.label45);
            this.groupBox6.Controls.Add(this.progressBar1);
            this.groupBox6.Controls.Add(this.dataGridView4);
            this.groupBox6.Controls.Add(this.checkBox1);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Controls.Add(this.button2);
            this.groupBox6.Controls.Add(this.button3);
            this.groupBox6.Controls.Add(this.button4);
            this.groupBox6.Location = new System.Drawing.Point(1444, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(444, 929);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "G-code Programming";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label44.ForeColor = System.Drawing.Color.LimeGreen;
            this.label44.Location = new System.Drawing.Point(336, 86);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(29, 16);
            this.label44.TabIndex = 35;
            this.label44.Text = "0%";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label45.ForeColor = System.Drawing.Color.LimeGreen;
            this.label45.Location = new System.Drawing.Point(374, 86);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(64, 16);
            this.label45.TabIndex = 34;
            this.label45.Text = "00:00:00";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 88);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(321, 15);
            this.progressBar1.TabIndex = 33;
            // 
            // dataGridView4
            // 
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.AllowUserToDeleteRows = false;
            this.dataGridView4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView4.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridView4.Location = new System.Drawing.Point(6, 114);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.ReadOnly = true;
            this.dataGridView4.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView4.RowHeadersWidth = 15;
            this.dataGridView4.Size = new System.Drawing.Size(432, 809);
            this.dataGridView4.TabIndex = 30;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.FillWeight = 70F;
            this.dataGridViewTextBoxColumn1.HeaderText = "G-code";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.FillWeight = 30F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Status";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(10, 67);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(103, 20);
            this.checkBox1.TabIndex = 29;
            this.checkBox1.Text = "Check Mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.ctrlDesign);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(266, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(888, 928);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Design";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupFanControl);
            this.tabPage3.Controls.Add(this.groupGcodeProgramming);
            this.tabPage3.Controls.Add(this.groupGRBLTerminal);
            this.tabPage3.Controls.Add(this.groupLaserIntensity);
            this.tabPage3.Controls.Add(this.groupJogging);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1896, 1047);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "MANUAL CONTROL";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupFanControl
            // 
            this.groupFanControl.Controls.Add(this.btnFanOff);
            this.groupFanControl.Controls.Add(this.btnFanOn);
            this.groupFanControl.Enabled = false;
            this.groupFanControl.Location = new System.Drawing.Point(0, 322);
            this.groupFanControl.Name = "groupFanControl";
            this.groupFanControl.Size = new System.Drawing.Size(274, 59);
            this.groupFanControl.TabIndex = 11;
            this.groupFanControl.TabStop = false;
            this.groupFanControl.Text = "Fan Control";
            // 
            // btnFanOff
            // 
            this.btnFanOff.Location = new System.Drawing.Point(137, 21);
            this.btnFanOff.Name = "btnFanOff";
            this.btnFanOff.Size = new System.Drawing.Size(91, 25);
            this.btnFanOff.TabIndex = 1;
            this.btnFanOff.Text = "Fan OFF";
            this.btnFanOff.UseVisualStyleBackColor = true;
            this.btnFanOff.Click += new System.EventHandler(this.btnFanOff_Click);
            // 
            // btnFanOn
            // 
            this.btnFanOn.Location = new System.Drawing.Point(39, 21);
            this.btnFanOn.Name = "btnFanOn";
            this.btnFanOn.Size = new System.Drawing.Size(91, 25);
            this.btnFanOn.TabIndex = 0;
            this.btnFanOn.Text = "Fan ON";
            this.btnFanOn.UseVisualStyleBackColor = true;
            this.btnFanOn.Click += new System.EventHandler(this.btnFanOn_Click);
            // 
            // groupGcodeProgramming
            // 
            this.groupGcodeProgramming.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupGcodeProgramming.Controls.Add(this.lblManualPercent);
            this.groupGcodeProgramming.Controls.Add(this.lblManualTime);
            this.groupGcodeProgramming.Controls.Add(this.pbManual);
            this.groupGcodeProgramming.Controls.Add(this.dataGridView3);
            this.groupGcodeProgramming.Controls.Add(this.cbManualCheckMode);
            this.groupGcodeProgramming.Controls.Add(this.btnLoadGcode);
            this.groupGcodeProgramming.Controls.Add(this.btnManualPause);
            this.groupGcodeProgramming.Controls.Add(this.btnManualStop);
            this.groupGcodeProgramming.Controls.Add(this.btnManualSend);
            this.groupGcodeProgramming.Location = new System.Drawing.Point(1444, 5);
            this.groupGcodeProgramming.Name = "groupGcodeProgramming";
            this.groupGcodeProgramming.Size = new System.Drawing.Size(444, 929);
            this.groupGcodeProgramming.TabIndex = 3;
            this.groupGcodeProgramming.TabStop = false;
            this.groupGcodeProgramming.Text = "G-code Programming";
            // 
            // lblManualPercent
            // 
            this.lblManualPercent.AutoSize = true;
            this.lblManualPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblManualPercent.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblManualPercent.Location = new System.Drawing.Point(336, 86);
            this.lblManualPercent.Name = "lblManualPercent";
            this.lblManualPercent.Size = new System.Drawing.Size(29, 16);
            this.lblManualPercent.TabIndex = 35;
            this.lblManualPercent.Text = "0%";
            // 
            // lblManualTime
            // 
            this.lblManualTime.AutoSize = true;
            this.lblManualTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblManualTime.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblManualTime.Location = new System.Drawing.Point(374, 86);
            this.lblManualTime.Name = "lblManualTime";
            this.lblManualTime.Size = new System.Drawing.Size(64, 16);
            this.lblManualTime.TabIndex = 34;
            this.lblManualTime.Text = "00:00:00";
            // 
            // pbManual
            // 
            this.pbManual.Location = new System.Drawing.Point(9, 88);
            this.pbManual.Name = "pbManual";
            this.pbManual.Size = new System.Drawing.Size(321, 15);
            this.pbManual.TabIndex = 33;
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView3.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Block,
            this.Status});
            this.dataGridView3.Location = new System.Drawing.Point(6, 114);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView3.RowHeadersWidth = 15;
            this.dataGridView3.Size = new System.Drawing.Size(432, 809);
            this.dataGridView3.TabIndex = 30;
            // 
            // Block
            // 
            this.Block.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Block.FillWeight = 70F;
            this.Block.HeaderText = "G-code";
            this.Block.Name = "Block";
            this.Block.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Status.FillWeight = 30F;
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // cbManualCheckMode
            // 
            this.cbManualCheckMode.AutoSize = true;
            this.cbManualCheckMode.Location = new System.Drawing.Point(10, 67);
            this.cbManualCheckMode.Name = "cbManualCheckMode";
            this.cbManualCheckMode.Size = new System.Drawing.Size(103, 20);
            this.cbManualCheckMode.TabIndex = 29;
            this.cbManualCheckMode.Text = "Check Mode";
            this.cbManualCheckMode.UseVisualStyleBackColor = true;
            // 
            // groupGRBLTerminal
            // 
            this.groupGRBLTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupGRBLTerminal.Controls.Add(this.rtbOutgoing);
            this.groupGRBLTerminal.Controls.Add(this.rtbIncoming);
            this.groupGRBLTerminal.Controls.Add(this.btnClear);
            this.groupGRBLTerminal.Controls.Add(this.lblInput);
            this.groupGRBLTerminal.Location = new System.Drawing.Point(280, 5);
            this.groupGRBLTerminal.Name = "groupGRBLTerminal";
            this.groupGRBLTerminal.Size = new System.Drawing.Size(1158, 929);
            this.groupGRBLTerminal.TabIndex = 2;
            this.groupGRBLTerminal.TabStop = false;
            this.groupGRBLTerminal.Text = "GRBL Terminal";
            // 
            // rtbOutgoing
            // 
            this.rtbOutgoing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbOutgoing.Location = new System.Drawing.Point(47, 898);
            this.rtbOutgoing.Name = "rtbOutgoing";
            this.rtbOutgoing.Size = new System.Drawing.Size(1012, 25);
            this.rtbOutgoing.TabIndex = 8;
            this.rtbOutgoing.Text = "";
            this.rtbOutgoing.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtbOutgoing_KeyPress);
            // 
            // rtbIncoming
            // 
            this.rtbIncoming.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbIncoming.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.rtbIncoming.ForeColor = System.Drawing.Color.LimeGreen;
            this.rtbIncoming.Location = new System.Drawing.Point(7, 21);
            this.rtbIncoming.Name = "rtbIncoming";
            this.rtbIncoming.ReadOnly = true;
            this.rtbIncoming.Size = new System.Drawing.Size(1146, 871);
            this.rtbIncoming.TabIndex = 7;
            this.rtbIncoming.Text = "";
            // 
            // lblInput
            // 
            this.lblInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInput.AutoSize = true;
            this.lblInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInput.Location = new System.Drawing.Point(6, 902);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(36, 16);
            this.lblInput.TabIndex = 1;
            this.lblInput.Text = "Input";
            // 
            // groupLaserIntensity
            // 
            this.groupLaserIntensity.Controls.Add(this.btnStopBeam);
            this.groupLaserIntensity.Controls.Add(this.btnPlayBeam);
            this.groupLaserIntensity.Controls.Add(this.label40);
            this.groupLaserIntensity.Controls.Add(this.btnLaserIntensityMax);
            this.groupLaserIntensity.Controls.Add(this.nmuLaserIntensity);
            this.groupLaserIntensity.Controls.Add(this.btnLaserIntensity4);
            this.groupLaserIntensity.Controls.Add(this.label43);
            this.groupLaserIntensity.Controls.Add(this.btnLaserIntensity3);
            this.groupLaserIntensity.Controls.Add(this.btnLaserIntensity2);
            this.groupLaserIntensity.Controls.Add(this.btnLaserIntensity1);
            this.groupLaserIntensity.Controls.Add(this.btnLaserOff);
            this.groupLaserIntensity.Location = new System.Drawing.Point(0, 387);
            this.groupLaserIntensity.Name = "groupLaserIntensity";
            this.groupLaserIntensity.Size = new System.Drawing.Size(274, 115);
            this.groupLaserIntensity.TabIndex = 1;
            this.groupLaserIntensity.TabStop = false;
            this.groupLaserIntensity.Text = "Laser Intensity";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(125, 74);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(48, 16);
            this.label40.TabIndex = 12;
            this.label40.Text = "(0-255)";
            // 
            // nmuLaserIntensity
            // 
            this.nmuLaserIntensity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nmuLaserIntensity.Location = new System.Drawing.Point(70, 71);
            this.nmuLaserIntensity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmuLaserIntensity.Name = "nmuLaserIntensity";
            this.nmuLaserIntensity.Size = new System.Drawing.Size(49, 22);
            this.nmuLaserIntensity.TabIndex = 11;
            this.nmuLaserIntensity.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(21, 74);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(46, 16);
            this.label43.TabIndex = 10;
            this.label43.Text = "Value:";
            // 
            // groupJogging
            // 
            this.groupJogging.Controls.Add(this.groupJoggingDistance);
            this.groupJogging.Controls.Add(this.label38);
            this.groupJogging.Controls.Add(this.label36);
            this.groupJogging.Controls.Add(this.btnArrow8);
            this.groupJogging.Controls.Add(this.btnArrow7);
            this.groupJogging.Controls.Add(this.btnArrow6);
            this.groupJogging.Controls.Add(this.btnArrow5);
            this.groupJogging.Controls.Add(this.btnArrow4);
            this.groupJogging.Controls.Add(this.btnArrow3);
            this.groupJogging.Controls.Add(this.btnArrow2);
            this.groupJogging.Controls.Add(this.btnArrow1);
            this.groupJogging.Location = new System.Drawing.Point(0, 5);
            this.groupJogging.Name = "groupJogging";
            this.groupJogging.Size = new System.Drawing.Size(274, 311);
            this.groupJogging.TabIndex = 0;
            this.groupJogging.TabStop = false;
            this.groupJogging.Text = "Jogging";
            // 
            // groupJoggingDistance
            // 
            this.groupJoggingDistance.Controls.Add(this.trackBar1);
            this.groupJoggingDistance.Location = new System.Drawing.Point(10, 195);
            this.groupJoggingDistance.Name = "groupJoggingDistance";
            this.groupJoggingDistance.Size = new System.Drawing.Size(258, 74);
            this.groupJoggingDistance.TabIndex = 10;
            this.groupJoggingDistance.TabStop = false;
            this.groupJoggingDistance.Text = "Speed:";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(10, 23);
            this.trackBar1.Maximum = 5000;
            this.trackBar1.Minimum = 200;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(242, 45);
            this.trackBar1.SmallChange = 200;
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TickFrequency = 100;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.Value = 3000;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(49, 98);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(16, 16);
            this.label38.TabIndex = 9;
            this.label38.Text = "X";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(125, 21);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(17, 16);
            this.label36.TabIndex = 8;
            this.label36.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Status:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(71, 36);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(103, 16);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Disconnected";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(185, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Port:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "BITMAP Files | *.bmp";
            this.openFileDialog1.Title = "Open BITMAP file";
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // cboPorts
            // 
            this.cboPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPorts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.cboPorts.FormattingEnabled = true;
            this.cboPorts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cboPorts.Location = new System.Drawing.Point(225, 30);
            this.cboPorts.Name = "cboPorts";
            this.cboPorts.Size = new System.Drawing.Size(88, 28);
            this.cboPorts.TabIndex = 6;
            this.cboPorts.TextChanged += new System.EventHandler(this.cboPorts_TextChanged);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "File.nc";
            this.saveFileDialog1.Filter = "G-Code Files(*.nc;*.cnc;*.tap;*.txt)|*.nc;*.cnc;*.tap;*.txt|All files (*.*)|*.*";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.Filter = "DXF Files | *.dxf";
            // 
            // openFileDialog3
            // 
            this.openFileDialog3.Filter = "NC Files | *.nc";
            // 
            // timerCoordinates
            // 
            this.timerCoordinates.Interval = 250;
            this.timerCoordinates.Tick += new System.EventHandler(this.timerCoordinates_Tick);
            // 
            // timerUpdateSystemStatus
            // 
            this.timerUpdateSystemStatus.Tick += new System.EventHandler(this.timerUpdateSystemStatus_Tick);
            // 
            // backgroundWorkerStreaming
            // 
            this.backgroundWorkerStreaming.WorkerSupportsCancellation = true;
            this.backgroundWorkerStreaming.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerStreaming_DoWork);
            this.backgroundWorkerStreaming.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerStreaming_RunWorkerCompleted);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(646, 37);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(54, 16);
            this.label41.TabIndex = 8;
            this.label41.Text = "Work X:";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(768, 37);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(55, 16);
            this.label42.TabIndex = 9;
            this.label42.Text = "Work Y:";
            // 
            // lblWorkX
            // 
            this.lblWorkX.AutoSize = true;
            this.lblWorkX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblWorkX.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblWorkX.Location = new System.Drawing.Point(695, 37);
            this.lblWorkX.Name = "lblWorkX";
            this.lblWorkX.Size = new System.Drawing.Size(44, 16);
            this.lblWorkX.TabIndex = 10;
            this.lblWorkX.Text = "0.000";
            // 
            // lblWorkY
            // 
            this.lblWorkY.AutoSize = true;
            this.lblWorkY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblWorkY.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblWorkY.Location = new System.Drawing.Point(817, 37);
            this.lblWorkY.Name = "lblWorkY";
            this.lblWorkY.Size = new System.Drawing.Size(44, 16);
            this.lblWorkY.TabIndex = 10;
            this.lblWorkY.Text = "0.000";
            // 
            // btnKillAlarm
            // 
            this.btnKillAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKillAlarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKillAlarm.Image = global::laserCraft_Control.Properties.Resources.icons8_bell_30;
            this.btnKillAlarm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnKillAlarm.Location = new System.Drawing.Point(1657, 30);
            this.btnKillAlarm.Name = "btnKillAlarm";
            this.btnKillAlarm.Size = new System.Drawing.Size(113, 31);
            this.btnKillAlarm.TabIndex = 11;
            this.btnKillAlarm.Text = "Kill Alarm";
            this.btnKillAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnKillAlarm.UseVisualStyleBackColor = true;
            this.btnKillAlarm.Click += new System.EventHandler(this.btnKillAlarm_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Image = global::laserCraft_Control.Properties.Resources.icons8_synchronize_26;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(1775, 29);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(117, 32);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset GRBL";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnFindPorts
            // 
            this.btnFindPorts.Image = global::laserCraft_Control.Properties.Resources.icons8_hard_to_find_24;
            this.btnFindPorts.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFindPorts.Location = new System.Drawing.Point(318, 29);
            this.btnFindPorts.Name = "btnFindPorts";
            this.btnFindPorts.Size = new System.Drawing.Size(113, 32);
            this.btnFindPorts.TabIndex = 7;
            this.btnFindPorts.Text = "Find Ports";
            this.btnFindPorts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFindPorts.UseVisualStyleBackColor = true;
            this.btnFindPorts.Click += new System.EventHandler(this.button29_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Image = global::laserCraft_Control.Properties.Resources.icons8_connected_24;
            this.btnConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.Location = new System.Drawing.Point(437, 29);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(106, 32);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.button28_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(0, 17);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(469, 354);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::laserCraft_Control.Properties.Resources.icons8_save_as_26;
            this.btnSave.Location = new System.Drawing.Point(297, 19);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(38, 38);
            this.btnSave.TabIndex = 4;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.Image = global::laserCraft_Control.Properties.Resources.icons8_code_24;
            this.btnGenerate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenerate.Location = new System.Drawing.Point(3, 19);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(171, 38);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate G-code";
            this.btnGenerate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnPause
            // 
            this.btnPause.Image = global::laserCraft_Control.Properties.Resources.icons8_pause_26;
            this.btnPause.Location = new System.Drawing.Point(258, 19);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(38, 38);
            this.btnPause.TabIndex = 3;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::laserCraft_Control.Properties.Resources.icons8_stop_26;
            this.btnStop.Location = new System.Drawing.Point(219, 19);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(38, 38);
            this.btnStop.TabIndex = 2;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSend
            // 
            this.btnSend.Image = global::laserCraft_Control.Properties.Resources.icons8_play_26;
            this.btnSend.Location = new System.Drawing.Point(180, 19);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(38, 38);
            this.btnSend.TabIndex = 1;
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnEngraveOpen
            // 
            this.btnEngraveOpen.Image = global::laserCraft_Control.Properties.Resources.icons8_open_24;
            this.btnEngraveOpen.Location = new System.Drawing.Point(4, 24);
            this.btnEngraveOpen.Name = "btnEngraveOpen";
            this.btnEngraveOpen.Size = new System.Drawing.Size(60, 30);
            this.btnEngraveOpen.TabIndex = 0;
            this.btnEngraveOpen.UseVisualStyleBackColor = true;
            this.btnEngraveOpen.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::laserCraft_Control.Properties.Resources.icons8_layers_24;
            this.pictureBox7.Location = new System.Drawing.Point(183, 181);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(24, 24);
            this.pictureBox7.TabIndex = 41;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::laserCraft_Control.Properties.Resources.icons8_polyline_24;
            this.pictureBox6.Location = new System.Drawing.Point(183, 157);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(24, 24);
            this.pictureBox6.TabIndex = 40;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::laserCraft_Control.Properties.Resources.icons8_oval_24;
            this.pictureBox5.Location = new System.Drawing.Point(183, 133);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(24, 24);
            this.pictureBox5.TabIndex = 39;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::laserCraft_Control.Properties.Resources.icons8_circle_24;
            this.pictureBox4.Location = new System.Drawing.Point(183, 109);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(24, 24);
            this.pictureBox4.TabIndex = 38;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::laserCraft_Control.Properties.Resources.Arc;
            this.pictureBox3.Location = new System.Drawing.Point(183, 85);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(24, 24);
            this.pictureBox3.TabIndex = 37;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::laserCraft_Control.Properties.Resources.icons8_line_32;
            this.pictureBox2.Location = new System.Drawing.Point(183, 61);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(24, 24);
            this.pictureBox2.TabIndex = 36;
            this.pictureBox2.TabStop = false;
            // 
            // btnCutOpen
            // 
            this.btnCutOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCutOpen.Image = global::laserCraft_Control.Properties.Resources.icons8_open_26;
            this.btnCutOpen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCutOpen.Location = new System.Drawing.Point(7, 18);
            this.btnCutOpen.Name = "btnCutOpen";
            this.btnCutOpen.Size = new System.Drawing.Size(202, 33);
            this.btnCutOpen.TabIndex = 28;
            this.btnCutOpen.Text = "Open DXF File";
            this.btnCutOpen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCutOpen.UseVisualStyleBackColor = true;
            this.btnCutOpen.Click += new System.EventHandler(this.btnCutOpen_Click);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Image = global::laserCraft_Control.Properties.Resources.icons8_template_24;
            this.button5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button5.Location = new System.Drawing.Point(6, 24);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(202, 33);
            this.button5.TabIndex = 42;
            this.button5.Text = "Open Material Templates";
            this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // btnCutGenerate
            // 
            this.btnCutGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCutGenerate.Image = global::laserCraft_Control.Properties.Resources.icons8_code_24;
            this.btnCutGenerate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCutGenerate.Location = new System.Drawing.Point(6, 21);
            this.btnCutGenerate.Name = "btnCutGenerate";
            this.btnCutGenerate.Size = new System.Drawing.Size(239, 38);
            this.btnCutGenerate.TabIndex = 2;
            this.btnCutGenerate.Text = "Generate G-code";
            this.btnCutGenerate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCutGenerate.UseVisualStyleBackColor = true;
            this.btnCutGenerate.Click += new System.EventHandler(this.btnCutGenerate_Click);
            // 
            // btnCutSend
            // 
            this.btnCutSend.Image = global::laserCraft_Control.Properties.Resources.icons8_play_26;
            this.btnCutSend.Location = new System.Drawing.Point(251, 21);
            this.btnCutSend.Name = "btnCutSend";
            this.btnCutSend.Size = new System.Drawing.Size(38, 38);
            this.btnCutSend.TabIndex = 1;
            this.btnCutSend.UseVisualStyleBackColor = true;
            this.btnCutSend.Click += new System.EventHandler(this.btnCutSend_Click);
            // 
            // btnCutStop
            // 
            this.btnCutStop.Image = global::laserCraft_Control.Properties.Resources.icons8_stop_26;
            this.btnCutStop.Location = new System.Drawing.Point(290, 21);
            this.btnCutStop.Name = "btnCutStop";
            this.btnCutStop.Size = new System.Drawing.Size(38, 38);
            this.btnCutStop.TabIndex = 2;
            this.btnCutStop.UseVisualStyleBackColor = true;
            this.btnCutStop.Click += new System.EventHandler(this.btnCutStop_Click);
            // 
            // btnCutPause
            // 
            this.btnCutPause.Image = global::laserCraft_Control.Properties.Resources.icons8_pause_26;
            this.btnCutPause.Location = new System.Drawing.Point(329, 21);
            this.btnCutPause.Name = "btnCutPause";
            this.btnCutPause.Size = new System.Drawing.Size(38, 38);
            this.btnCutPause.TabIndex = 3;
            this.btnCutPause.UseVisualStyleBackColor = true;
            this.btnCutPause.Click += new System.EventHandler(this.btnCutPause_Click);
            // 
            // btnCutSave
            // 
            this.btnCutSave.Image = global::laserCraft_Control.Properties.Resources.icons8_save_as_26;
            this.btnCutSave.Location = new System.Drawing.Point(368, 21);
            this.btnCutSave.Name = "btnCutSave";
            this.btnCutSave.Size = new System.Drawing.Size(38, 38);
            this.btnCutSave.TabIndex = 4;
            this.btnCutSave.UseVisualStyleBackColor = true;
            this.btnCutSave.Click += new System.EventHandler(this.btnCutSave_Click);
            // 
            // button1
            // 
            this.button1.Image = global::laserCraft_Control.Properties.Resources.icons8_open_26;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(10, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(193, 38);
            this.button1.TabIndex = 9;
            this.button1.Text = "Load G-code File";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Image = global::laserCraft_Control.Properties.Resources.icons8_pause_26;
            this.button2.Location = new System.Drawing.Point(287, 23);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(38, 38);
            this.button2.TabIndex = 7;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Image = global::laserCraft_Control.Properties.Resources.icons8_stop_26;
            this.button3.Location = new System.Drawing.Point(248, 23);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(38, 38);
            this.button3.TabIndex = 6;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Image = global::laserCraft_Control.Properties.Resources.icons8_play_26;
            this.button4.Location = new System.Drawing.Point(209, 23);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(38, 38);
            this.button4.TabIndex = 5;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // btnLoadGcode
            // 
            this.btnLoadGcode.Image = global::laserCraft_Control.Properties.Resources.icons8_open_26;
            this.btnLoadGcode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadGcode.Location = new System.Drawing.Point(10, 23);
            this.btnLoadGcode.Name = "btnLoadGcode";
            this.btnLoadGcode.Size = new System.Drawing.Size(193, 38);
            this.btnLoadGcode.TabIndex = 9;
            this.btnLoadGcode.Text = "Load G-code File";
            this.btnLoadGcode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoadGcode.UseVisualStyleBackColor = true;
            this.btnLoadGcode.Click += new System.EventHandler(this.btnLoadGcode_Click);
            // 
            // btnManualPause
            // 
            this.btnManualPause.Image = global::laserCraft_Control.Properties.Resources.icons8_pause_26;
            this.btnManualPause.Location = new System.Drawing.Point(287, 23);
            this.btnManualPause.Name = "btnManualPause";
            this.btnManualPause.Size = new System.Drawing.Size(38, 38);
            this.btnManualPause.TabIndex = 7;
            this.btnManualPause.UseVisualStyleBackColor = true;
            this.btnManualPause.Click += new System.EventHandler(this.btnManualPause_Click);
            // 
            // btnManualStop
            // 
            this.btnManualStop.Image = global::laserCraft_Control.Properties.Resources.icons8_stop_26;
            this.btnManualStop.Location = new System.Drawing.Point(248, 23);
            this.btnManualStop.Name = "btnManualStop";
            this.btnManualStop.Size = new System.Drawing.Size(38, 38);
            this.btnManualStop.TabIndex = 6;
            this.btnManualStop.UseVisualStyleBackColor = true;
            this.btnManualStop.Click += new System.EventHandler(this.btnManualStop_Click);
            // 
            // btnManualSend
            // 
            this.btnManualSend.Image = global::laserCraft_Control.Properties.Resources.icons8_play_26;
            this.btnManualSend.Location = new System.Drawing.Point(209, 23);
            this.btnManualSend.Name = "btnManualSend";
            this.btnManualSend.Size = new System.Drawing.Size(38, 38);
            this.btnManualSend.TabIndex = 5;
            this.btnManualSend.UseVisualStyleBackColor = true;
            this.btnManualSend.Click += new System.EventHandler(this.btnManualSend_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Image = global::laserCraft_Control.Properties.Resources.icons8_ccleaner_24;
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.Location = new System.Drawing.Point(1065, 897);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(87, 25);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnStopBeam
            // 
            this.btnStopBeam.Image = global::laserCraft_Control.Properties.Resources.icons8_stop_26;
            this.btnStopBeam.Location = new System.Drawing.Point(209, 67);
            this.btnStopBeam.Name = "btnStopBeam";
            this.btnStopBeam.Size = new System.Drawing.Size(29, 29);
            this.btnStopBeam.TabIndex = 13;
            this.btnStopBeam.UseVisualStyleBackColor = true;
            this.btnStopBeam.Click += new System.EventHandler(this.btnStopBeam_Click);
            // 
            // btnPlayBeam
            // 
            this.btnPlayBeam.Image = global::laserCraft_Control.Properties.Resources.icons8_play_26;
            this.btnPlayBeam.Location = new System.Drawing.Point(172, 67);
            this.btnPlayBeam.Name = "btnPlayBeam";
            this.btnPlayBeam.Size = new System.Drawing.Size(31, 29);
            this.btnPlayBeam.TabIndex = 4;
            this.btnPlayBeam.UseVisualStyleBackColor = true;
            this.btnPlayBeam.Click += new System.EventHandler(this.btnPlayBeam_Click);
            // 
            // btnLaserIntensityMax
            // 
            this.btnLaserIntensityMax.Image = global::laserCraft_Control.Properties.Resources.icons8_5_24;
            this.btnLaserIntensityMax.Location = new System.Drawing.Point(222, 22);
            this.btnLaserIntensityMax.Name = "btnLaserIntensityMax";
            this.btnLaserIntensityMax.Size = new System.Drawing.Size(32, 29);
            this.btnLaserIntensityMax.TabIndex = 1;
            this.btnLaserIntensityMax.UseVisualStyleBackColor = true;
            this.btnLaserIntensityMax.Click += new System.EventHandler(this.btnLaserIntensityMax_Click);
            // 
            // btnLaserIntensity4
            // 
            this.btnLaserIntensity4.Image = global::laserCraft_Control.Properties.Resources.icons8_4_24;
            this.btnLaserIntensity4.Location = new System.Drawing.Point(185, 22);
            this.btnLaserIntensity4.Name = "btnLaserIntensity4";
            this.btnLaserIntensity4.Size = new System.Drawing.Size(31, 29);
            this.btnLaserIntensity4.TabIndex = 1;
            this.btnLaserIntensity4.UseVisualStyleBackColor = true;
            this.btnLaserIntensity4.Click += new System.EventHandler(this.btnLaserIntensity4_Click);
            // 
            // btnLaserIntensity3
            // 
            this.btnLaserIntensity3.Image = global::laserCraft_Control.Properties.Resources.icons8_3_24;
            this.btnLaserIntensity3.Location = new System.Drawing.Point(147, 22);
            this.btnLaserIntensity3.Name = "btnLaserIntensity3";
            this.btnLaserIntensity3.Size = new System.Drawing.Size(32, 29);
            this.btnLaserIntensity3.TabIndex = 1;
            this.btnLaserIntensity3.UseVisualStyleBackColor = true;
            this.btnLaserIntensity3.Click += new System.EventHandler(this.btnLaserIntensity3_Click);
            // 
            // btnLaserIntensity2
            // 
            this.btnLaserIntensity2.Image = global::laserCraft_Control.Properties.Resources.icons8_2_24;
            this.btnLaserIntensity2.Location = new System.Drawing.Point(106, 22);
            this.btnLaserIntensity2.Name = "btnLaserIntensity2";
            this.btnLaserIntensity2.Size = new System.Drawing.Size(35, 29);
            this.btnLaserIntensity2.TabIndex = 1;
            this.btnLaserIntensity2.UseVisualStyleBackColor = true;
            this.btnLaserIntensity2.Click += new System.EventHandler(this.btnLaserIntensity2_Click);
            // 
            // btnLaserIntensity1
            // 
            this.btnLaserIntensity1.Image = global::laserCraft_Control.Properties.Resources.icons8_1_24;
            this.btnLaserIntensity1.Location = new System.Drawing.Point(65, 22);
            this.btnLaserIntensity1.Name = "btnLaserIntensity1";
            this.btnLaserIntensity1.Size = new System.Drawing.Size(35, 29);
            this.btnLaserIntensity1.TabIndex = 1;
            this.btnLaserIntensity1.UseVisualStyleBackColor = true;
            this.btnLaserIntensity1.Click += new System.EventHandler(this.btnLaserIntensity1_Click);
            // 
            // btnLaserOff
            // 
            this.btnLaserOff.Image = global::laserCraft_Control.Properties.Resources.icons8_switch_off_26;
            this.btnLaserOff.Location = new System.Drawing.Point(20, 22);
            this.btnLaserOff.Name = "btnLaserOff";
            this.btnLaserOff.Size = new System.Drawing.Size(39, 29);
            this.btnLaserOff.TabIndex = 0;
            this.btnLaserOff.UseVisualStyleBackColor = true;
            this.btnLaserOff.Click += new System.EventHandler(this.btnLaserOff_Click);
            // 
            // btnArrow8
            // 
            this.btnArrow8.Image = global::laserCraft_Control.Properties.Resources.icons8_circled_up_right_26;
            this.btnArrow8.Location = new System.Drawing.Point(155, 40);
            this.btnArrow8.Name = "btnArrow8";
            this.btnArrow8.Size = new System.Drawing.Size(45, 45);
            this.btnArrow8.TabIndex = 7;
            this.btnArrow8.UseVisualStyleBackColor = true;
            this.btnArrow8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow8_MouseDown);
            this.btnArrow8.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow8_MouseUp);
            // 
            // btnArrow7
            // 
            this.btnArrow7.Image = global::laserCraft_Control.Properties.Resources.icons8_circled_right_26;
            this.btnArrow7.Location = new System.Drawing.Point(155, 84);
            this.btnArrow7.Name = "btnArrow7";
            this.btnArrow7.Size = new System.Drawing.Size(45, 45);
            this.btnArrow7.TabIndex = 6;
            this.btnArrow7.UseVisualStyleBackColor = true;
            this.btnArrow7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow7_MouseDown);
            this.btnArrow7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow7_MouseUp);
            // 
            // btnArrow6
            // 
            this.btnArrow6.Image = global::laserCraft_Control.Properties.Resources.icons8_circled_down_right_26;
            this.btnArrow6.Location = new System.Drawing.Point(155, 128);
            this.btnArrow6.Name = "btnArrow6";
            this.btnArrow6.Size = new System.Drawing.Size(45, 45);
            this.btnArrow6.TabIndex = 5;
            this.btnArrow6.UseVisualStyleBackColor = true;
            this.btnArrow6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow6_MouseDown);
            this.btnArrow6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow6_MouseUp);
            // 
            // btnArrow5
            // 
            this.btnArrow5.Image = global::laserCraft_Control.Properties.Resources.icons8_below_26;
            this.btnArrow5.Location = new System.Drawing.Point(111, 128);
            this.btnArrow5.Name = "btnArrow5";
            this.btnArrow5.Size = new System.Drawing.Size(45, 45);
            this.btnArrow5.TabIndex = 4;
            this.btnArrow5.UseVisualStyleBackColor = true;
            this.btnArrow5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow5_MouseDown);
            this.btnArrow5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow5_MouseUp);
            // 
            // btnArrow4
            // 
            this.btnArrow4.Image = global::laserCraft_Control.Properties.Resources.icons8_circled_down_left_26;
            this.btnArrow4.Location = new System.Drawing.Point(67, 128);
            this.btnArrow4.Name = "btnArrow4";
            this.btnArrow4.Size = new System.Drawing.Size(45, 45);
            this.btnArrow4.TabIndex = 3;
            this.btnArrow4.UseVisualStyleBackColor = true;
            this.btnArrow4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow4_MouseDown);
            this.btnArrow4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow4_MouseUp);
            // 
            // btnArrow3
            // 
            this.btnArrow3.Image = global::laserCraft_Control.Properties.Resources.icons8_go_back_26;
            this.btnArrow3.Location = new System.Drawing.Point(67, 84);
            this.btnArrow3.Name = "btnArrow3";
            this.btnArrow3.Size = new System.Drawing.Size(45, 45);
            this.btnArrow3.TabIndex = 2;
            this.btnArrow3.UseVisualStyleBackColor = true;
            this.btnArrow3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow3_MouseDown);
            this.btnArrow3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow3_MouseUp);
            // 
            // btnArrow2
            // 
            this.btnArrow2.Image = global::laserCraft_Control.Properties.Resources.icons8_circled_up_left_26;
            this.btnArrow2.Location = new System.Drawing.Point(67, 40);
            this.btnArrow2.Name = "btnArrow2";
            this.btnArrow2.Size = new System.Drawing.Size(45, 45);
            this.btnArrow2.TabIndex = 1;
            this.btnArrow2.UseVisualStyleBackColor = true;
            this.btnArrow2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow2_MouseDown);
            this.btnArrow2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow2_MouseUp);
            // 
            // btnArrow1
            // 
            this.btnArrow1.Image = global::laserCraft_Control.Properties.Resources.icons8_upward_arrow_26;
            this.btnArrow1.Location = new System.Drawing.Point(111, 40);
            this.btnArrow1.Name = "btnArrow1";
            this.btnArrow1.Size = new System.Drawing.Size(45, 45);
            this.btnArrow1.TabIndex = 0;
            this.btnArrow1.UseVisualStyleBackColor = true;
            this.btnArrow1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnArrow1_MouseDown);
            this.btnArrow1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnArrow1_MouseUp);
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(187, 22);
            this.settingsToolStripMenuItem1.Text = "Settings";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(184, 6);
            // 
            // materialTemplatesToolStripMenuItem
            // 
            this.materialTemplatesToolStripMenuItem.Name = "materialTemplatesToolStripMenuItem";
            this.materialTemplatesToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.materialTemplatesToolStripMenuItem.Text = "Material Templates";
            this.materialTemplatesToolStripMenuItem.Click += new System.EventHandler(this.materialTemplatesToolStripMenuItem_Click);
            // 
            // ctrlDesign
            // 
            this.ctrlDesign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlDesign.Location = new System.Drawing.Point(3, 16);
            this.ctrlDesign.Margin = new System.Windows.Forms.Padding(4);
            this.ctrlDesign.Name = "ctrlDesign";
            this.ctrlDesign.Size = new System.Drawing.Size(882, 909);
            this.ctrlDesign.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.btnKillAlarm);
            this.Controls.Add(this.lblWorkY);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblWorkX);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.btnFindPorts);
            this.Controls.Add(this.cboPorts);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "laserCraft Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grbGcode.ResumeLayout(false);
            this.grbGcode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.grbOuput.ResumeLayout(false);
            this.grbOuput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPixelDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFeedrate)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.grCutViewer.ResumeLayout(false);
            this.grCutViewer.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutRepeat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutLaserIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutFeedRate)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.grG_CodeProcess.ResumeLayout(false);
            this.grG_CodeProcess.PerformLayout();
            this.grG_CodeControl.ResumeLayout(false);
            this.grG_CodeControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabDesign.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupFanControl.ResumeLayout(false);
            this.groupGcodeProgramming.ResumeLayout(false);
            this.groupGcodeProgramming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.groupGRBLTerminal.ResumeLayout(false);
            this.groupGRBLTerminal.PerformLayout();
            this.groupLaserIntensity.ResumeLayout(false);
            this.groupLaserIntensity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmuLaserIntensity)).EndInit();
            this.groupJogging.ResumeLayout(false);
            this.groupJogging.PerformLayout();
            this.groupJoggingDistance.ResumeLayout(false);
            this.groupJoggingDistance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void initiateOutputSize()
        {
            float num = float.Parse(this.adjustedImage.Width.ToString(), CultureInfo.InvariantCulture.NumberFormat) * float.Parse(this.cboResolution.Text, CultureInfo.InvariantCulture.NumberFormat);
            float num2 = float.Parse(this.adjustedImage.Height.ToString(), CultureInfo.InvariantCulture.NumberFormat) * float.Parse(this.cboResolution.Text, CultureInfo.InvariantCulture.NumberFormat);
            this.nudWidth.Value = (decimal) num;
            this.nudHeight.Value = (decimal) num2;
            this.lblMaxWidth.Text = num.ToString();
            this.lblMaxHeight.Text = num2.ToString();
            this.calculateOutputSize(true);
        }

        public void localize()
        {
            if (Settings.Default.language == "English")
            {
                this.fileToolStripMenuItem.Text = "&System";
                this.exitToolStripMenuItem.Text = "Exit";
                this.toolsToolStripMenuItem.Text = "&Tools";
                this.helpToolStripMenuItem.Text = "&Help";
                this.manualToolStripMenuItem.Text = "User Guide";
                this.aboutToolStripMenuItem.Text = "About";
                this.tabControl.TabPages[0].Text = "ENGRAVE";
                this.tabControl.TabPages[1].Text = "CUT";
                this.tabControl.TabPages[2].Text = "CUSTOM DESIGN";
                this.tabControl.TabPages[3].Text = "MANUAL CONTROL";
                this.label1.Text = "Status:";
                this.lblStatus.Text = "Disconnected";
                this.label3.Text = "Port:";
                this.btnFindPorts.Text = "Find Ports";
                this.btnConnect.Text = "Connect";
                this.groupBox1.Text = "Image Edit";
                this.button7.Text = "Flip X";
                this.button8.Text = "Flip Y";
                this.button6.Text = "Rotate";
                this.button11.Text = "Invert";
                this.groupBox3.Text = "Engrave Settings";
                this.label4.Text = "Feed Rate:";
                this.label5.Text = "Laser Intensity:";
                this.label6.Text = "Pixel Delay:";
                this.label34.Text = "milisecond";
                this.grbOuput.Text = "Actual Output Size";
                this.label14.Text = "Resolution:";
                this.label7.Text = "Width:";
                this.label8.Text = "Height:";
                this.grbGcode.Text = "Output G-code";
                this.btnGenerate.Text = "Generate G-code";
                this.groupBox9.Text = "Job";
                this.btnCutOpen.Text = "Open DXF File";
                this.groupBox7.Text = "Actual Output Size";
                this.label23.Text = "Scale:";
                this.label24.Text = "Width:";
                this.label22.Text = "Height:";
                this.groupBox8.Text = "Cut Settings";
                this.label20.Text = "Feed Rate:";
                this.label13.Text = "Repeat:";
                this.label26.Text = "times";
                this.label19.Text = "Laser Intensity:";
                this.groupBox2.Text = "Output G-code";
                this.btnCutGenerate.Text = "Generate G-code";
                this.groupJogging.Text = "Jogging";
                this.groupJoggingDistance.Text = "Jogging Speed: ";
                this.groupFanControl.Text = "Fan Control";
                this.btnFanOn.Text = "Fan ON";
                this.btnFanOff.Text = "Fan OFF";
                this.groupLaserIntensity.Text = "Laser Intensity";
                this.groupGcodeProgramming.Text = "G-code Programming";
                this.groupGRBLTerminal.Text = "GRBL Terminal";
                this.btnClear.Text = "Clear";
                this.lblInput.Text = "Input";
                this.btnReset.Text = "Reset GRBL";
                this.btnKillAlarm.Text = "Kill Alarm";
                this.btnLoadGcode.Text = "Load G-code File";
                this.label2.Text = "Set Scale:";
            }
            else
            {
                this.fileToolStripMenuItem.Text = "&Hệ thống";
                this.exitToolStripMenuItem.Text = "Tho\x00e1t";
                this.toolsToolStripMenuItem.Text = "&C\x00f4ng cụ";
                this.helpToolStripMenuItem.Text = "&Trợ gi\x00fap";
                this.manualToolStripMenuItem.Text = "Hướng dẫn";
                this.aboutToolStripMenuItem.Text = "Giới thiệu";
                this.tabControl.TabPages[0].Text = "KHẮC ẢNH";
                this.tabControl.TabPages[1].Text = "CẮT VẬT LIỆU";
                this.tabControl.TabPages[2].Text = "THIẾT KẾ";
                this.tabControl.TabPages[3].Text = "ĐIỀU KHIỂN TAY";
                this.label1.Text = "Trạng th\x00e1i:";
                this.lblStatus.Text = "Chưa kết nối";
                this.label3.Text = "Cổng:";
                this.btnFindPorts.Text = "T\x00ecm cổng";
                this.btnConnect.Text = "Kết nối";
                this.groupBox1.Text = "Hiệu chỉnh ảnh";
                this.button7.Text = "Lật X";
                this.button8.Text = "Lật Y";
                this.button6.Text = "Quay";
                this.button11.Text = "Đảo";
                this.groupBox3.Text = "Thiết lập khắc ảnh";
                this.label4.Text = "Tốc độ:";
                this.label5.Text = "Cường độ laser:";
                this.label6.Text = "Độ trễ mỗi pixel:";
                this.label34.Text = "mili gi\x00e2y";
                this.grbOuput.Text = "K\x00edch thước thực tế";
                this.label14.Text = "Ph\x00e2n giải:";
                this.label7.Text = "Rộng:";
                this.label8.Text = "Cao:";
                this.grbGcode.Text = "Sinh m\x00e3 lệnh";
                this.btnGenerate.Text = "Sinh m\x00e3 lệnh";
                this.groupBox9.Text = "T\x00e1c vụ";
                this.btnCutOpen.Text = "Mở tập tin DXF";
                this.groupBox7.Text = "K\x00edch thước thực tế";
                this.label23.Text = "Tỉ lệ:";
                this.label24.Text = "Rộng:";
                this.label22.Text = "Cao:";
                this.groupBox8.Text = "Thiết lập cắt";
                this.label20.Text = "Tốc độ:";
                this.label13.Text = "Lặp lại:";
                this.label26.Text = "lần";
                this.label19.Text = "Cường độ laser:";
                this.groupBox2.Text = "Sinh m\x00e3 lệnh";
                this.btnCutGenerate.Text = "Sinh m\x00e3 lệnh";
                this.groupJogging.Text = "Di chuyển đầu laser";
                this.groupJoggingDistance.Text = "Tốc độ: ";
                this.groupFanControl.Text = "Điều khiển quạt gi\x00f3";
                this.btnFanOn.Text = "BẬT";
                this.btnFanOff.Text = "TẮT";
                this.groupLaserIntensity.Text = "Cường độ laser";
                this.groupGcodeProgramming.Text = "Lập tr\x00ecnh G-code";
                this.groupGRBLTerminal.Text = "Cửa sổ lệnh";
                this.btnClear.Text = "X\x00f3a";
                this.lblInput.Text = "Lệnh";
                this.btnLoadGcode.Text = "Nạp File G-code";
                this.btnReset.Text = "Reset m\x00e1y";
                this.btnKillAlarm.Text = "X\x00f3a Alarm";
                this.label2.Text = "Đặt tỉ lệ:";
            }
            if (Settings.Default.isMetric)
            {
                this.label17.Text = "mm/min";
                this.label25.Text = "mm/min";
                this.label10.Text = "mm";
                this.label11.Text = "mm";
                this.label12.Text = "mm";
                this.label33.Text = "mm";
                this.label35.Text = "mm";
            }
            else
            {
                this.label17.Text = "inches/min";
                this.label25.Text = "inches/min";
                this.label10.Text = "inches";
                this.label11.Text = "in.";
                this.label12.Text = "in.";
                this.label33.Text = "inches";
                this.label35.Text = "inches";
            }
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "Help_VIE.chm");
        }

        private int mapCutSValue(int val, int min, int max)
        {
            int num = max - min;
            return (min + ((val * num) / 0xff));
        }

        private int mapSValue(int grayValue, int min, int max)
        {
            int num = max - min;
            int num2 = (grayValue * this.laserIntensity) / 0xff;
            return (min + ((num2 * num) / 0xff));
        }

        private void nudCutFeedRate_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.cutFeedrate = Convert.ToInt32(this.nudCutFeedRate.Value);
            Settings.Default.Save();
        }

        private void nudCutHeight_ValueChanged(object sender, EventArgs e)
        {
            double num = this.canvasWidth / this.canvasHeight;
            decimal num2 = this.nudCutHeight.Value * ((decimal) num);
            if (num2 > this.nudCutWidth.Maximum)
            {
                this.nudCutWidth.Value = this.nudCutWidth.Maximum;
            }
            else if (num2 < this.nudCutWidth.Minimum)
            {
                this.nudCutWidth.Value = this.nudCutWidth.Minimum;
                this.nudCutHeight.Value = this.nudCutWidth.Value / ((decimal) num);
            }
            else
            {
                this.nudCutWidth.Value = num2;
            }
            this.adjustCutRulers();
        }

        private void nudCutLaserIntensity_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.cutLaserIntensity = Convert.ToInt32(this.nudCutLaserIntensity.Value);
            Settings.Default.Save();
        }

        private void nudCutRepeat_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.cutRepeat = Convert.ToInt32(this.nudCutRepeat.Value);
            Settings.Default.Save();
        }

        private void nudCutWidth_ValueChanged(object sender, EventArgs e)
        {
            double num = this.canvasWidth / this.canvasHeight;
            decimal num2 = this.nudCutWidth.Value / ((decimal) num);
            if (num2 > this.nudCutHeight.Maximum)
            {
                this.nudCutHeight.Value = this.nudCutHeight.Maximum;
            }
            else if (num2 < this.nudCutHeight.Minimum)
            {
                this.nudCutHeight.Value = this.nudCutHeight.Minimum;
                this.nudCutWidth.Value = this.nudCutHeight.Value * ((decimal) num);
            }
            else
            {
                this.nudCutHeight.Value = num2;
            }

            

            this.adjustCutRulers();
        }

        private void nudFeedrate_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.feedRate = Convert.ToInt32(this.nudFeedrate.Value);
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            this.calculateOutputSize(false);
        }

        private void nudLaserIntensity_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.laserIntensity = Convert.ToInt32(this.nudLaserIntensity.Value);
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void nudPixelDelay_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.pixelDelay = Convert.ToInt32(this.nudPixelDelay.Value);
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            this.calculateOutputSize(true);
        }

        private void panelCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (this.isFileLoaded)
            {
                this.visualize();
            }
        }

        private void rtbOutgoing_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    if (this.systemStatus != 1)
                    {
                        string str;
                        if (Settings.Default.language == "English")
                        {
                            str = "GRBL is busy. Please try again later.";
                        }
                        else
                        {
                            str = "Hệ thống đang bận. Vui l\x00f2ng thử lại sau";
                        }
                        MessageBox.Show(str, "Try again", MessageBoxButtons.OK);
                        this.rtbOutgoing.Text = "";
                    }
                    else if (this.rtbOutgoing.Text != string.Empty)
                    {
                        this.serialPort1.Write(this.rtbOutgoing.Text);
                        if (this.rtbOutgoing.Text.Contains("?"))
                        {
                            this.isStatusReportByUser = true;
                        }
                        this.rtbOutgoing.Text = "";
                    }
                }
            }
            catch (Exception exception1)
            {
                MessageBox.Show(exception1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.systemStatus == 1)
            {
                string str = this.serialPort1.ReadLine();
                if (str != string.Empty)
                {
                    object[] args = new object[] { str };
                    base.BeginInvoke(new SetTextCallback(this.SetText), args);
                }
            }
        }

        private void setSystemState(int _systemStatus)
        {
            switch (_systemStatus)
            {
                case 0:
                    this.lblStatus.ForeColor = Color.Red;
                    this.btnFindPorts.Enabled = true;
                    this.cboPorts.Enabled = true;
                    if (Settings.Default.language != "English")
                    {
                        this.btnConnect.Text = "Kết nối";
                        this.lblStatus.Text = "Chưa kết nối";
                    }
                    else
                    {
                        this.btnConnect.Text = "Connect";
                        this.lblStatus.Text = "Disconnected";
                    }
                    this.timerUpdateSystemStatus.Enabled = false;
                    this.timerCoordinates.Enabled = false;
                    return;

                case 1:
                    this.lblStatus.ForeColor = Color.Lime;
                    this.serialPort1.NewLine = "\r\n";
                    this.btnFindPorts.Enabled = false;
                    this.cboPorts.Enabled = false;
                    if (Settings.Default.language != "English")
                    {
                        this.btnConnect.Text = "Ngắt kết nối";
                        break;
                    }
                    this.btnConnect.Text = "Disconnect";
                    break;

                case 2:
                case 3:
                case 4:
                    this.pbCut.Value = 0;
                    this.pbEngrave.Value = 0;
                    this.pbManual.Value = 0;
                    this.lblCutTime.Text = "00:00:00";
                    this.lblEngraveTime.Text = "00:00:00";
                    this.lblManualTime.Text = "00:00:00";
                    this.streamResponseCount = 0;
                    this.streamResponse.Clear();
                    this.streamStatusReport = "";
                    this.streamResponseDisplayed = 0;
                    this.streamTimeInMillis = 0;
                    this.isStreamingPaused = false;
                    this.timerCoordinates.Enabled = false;
                    this.timerUpdateSystemStatus.Enabled = true;
                    this.timerUpdateSystemStatus.Start();
                    return;

                default:
                    return;
            }
            this.timerUpdateSystemStatus.Enabled = false;
            this.timerCoordinates.Enabled = true;
            this.timerCoordinates.Start();
        }

        private void SetText(string text)
        {
            if (text.Contains("<"))
            {
                this.updateCoordinates(text);
                this.updateStatusLabel(text);
                if (this.isStatusReportByUser)
                {
                    this.isStatusReportByUser = false;
                    this.rtbIncoming.Focus();
                    this.rtbIncoming.AppendText(text + "\n");
                    this.rtbOutgoing.Focus();
                }
            }
            else if (this.rtbOutgoing.Focused && this.rtbOutgoing.Enabled)
            {
                this.rtbIncoming.Focus();
                this.rtbIncoming.AppendText(text + "\n");
                this.rtbOutgoing.Focus();
            }
        }

        private void timerCoordinates_Tick(object sender, EventArgs e)
        {
            try
            {
                this.serialPort1.Write("?");
            }
            catch
            {
                Application.Exit();
            }
        }

        private void timerUpdateSystemStatus_Tick(object sender, EventArgs e)
        {
            int streamResponseCount;
            string streamStatusReport = "";
            try
            {
                streamStatusReport = this.streamStatusReport;
            }
            catch (Exception exception1)
            {
                MessageBox.Show(exception1.Message, "report = streamStatusReport");
                return;
            }
            if (streamStatusReport.Contains("<"))
            {
                try
                {
                    int startIndex = streamStatusReport.IndexOf("WPos:") + 5;
                    int length = streamStatusReport.IndexOf(",", startIndex) - startIndex;
                    this.lblWorkX.Text = streamStatusReport.Substring(startIndex, length);
                    int num8 = streamStatusReport.IndexOf(",", startIndex) + 1;
                    int num9 = streamStatusReport.IndexOf(",", num8) - num8;
                    this.lblWorkY.Text = streamStatusReport.Substring(num8, num9);
                    if (Settings.Default.language == "English")
                    {
                        if (this.streamStatusReport.Contains("Idle"))
                        {
                            this.lblStatus.Text = "Idle";
                        }
                        else if (streamStatusReport.Contains("Run"))
                        {
                            this.lblStatus.Text = "Run";
                        }
                        else if (streamStatusReport.Contains("Hold"))
                        {
                            this.lblStatus.Text = "Hold";
                        }
                        else if (streamStatusReport.Contains("Alarm"))
                        {
                            this.lblStatus.Text = "Alarm";
                        }
                        else if (streamStatusReport.Contains("Check"))
                        {
                            this.lblStatus.Text = "Check";
                        }
                        else if (streamStatusReport.Contains("Home"))
                        {
                            this.lblStatus.Text = "Home";
                        }
                    }
                    else if (streamStatusReport.Contains("Idle"))
                    {
                        this.lblStatus.Text = "Chờ lệnh";
                    }
                    else if (streamStatusReport.Contains("Run"))
                    {
                        this.lblStatus.Text = "Đang chạy";
                    }
                    else if (streamStatusReport.Contains("Hold"))
                    {
                        this.lblStatus.Text = "Tạm ngưng";
                    }
                    else if (streamStatusReport.Contains("Alarm"))
                    {
                        this.lblStatus.Text = "Cảnh b\x00e1o";
                    }
                    else if (streamStatusReport.Contains("Check"))
                    {
                        this.lblStatus.Text = "Kiểm tra";
                    }
                    else if (streamStatusReport.Contains("Home"))
                    {
                        this.lblStatus.Text = "Home";
                    }
                }
                catch (Exception exception2)
                {
                    this.lblWorkX.Text = "N/A";
                    this.lblWorkY.Text = "N/A";
                    this.lblStatus.Text = "N/A";
                    MessageBox.Show(exception2.Message, "report.Contains(<)");
                }
            }
            streamStatusReport = "";
            try
            {
                streamStatusReport = this.streamStatusReport;
            }
            catch (Exception exception3)
            {
                MessageBox.Show(exception3.Message, "report = streamStatusReport");
                return;
            }
            if (!streamStatusReport.Contains("["))
            {
                streamStatusReport.Contains("ALARM");
            }
            this.streamTimeInMillis += this.timerUpdateSystemStatus.Interval;
            int num1 = this.streamTimeInMillis / 0x3e8;
            int num = num1 / 0xe10;
            int num2 = (num1 % 0xe10) / 60;
            int num3 = num1 % 60;
            string str2 = "";
            if (num < 10)
            {
                str2 = str2 + "0" + num.ToString() + ":";
            }
            else
            {
                str2 = str2 + num.ToString() + ":";
            }
            if (num2 < 10)
            {
                str2 = str2 + "0" + num2.ToString() + ":";
            }
            else
            {
                str2 = str2 + num2.ToString() + ":";
            }
            if (num3 < 10)
            {
                str2 = str2 + "0" + num3.ToString();
            }
            else
            {
                str2 = str2 + num3.ToString();
            }
            try
            {
                streamResponseCount = this.streamResponseCount;
            }
            catch
            {
                return;
            }
            for (int i = this.streamResponseDisplayed; i <= (streamResponseCount - 1); i++)
            {
                try
                {
                    switch (this.systemStatus)
                    {
                        case 2:
                        {
                            this.dataGridView1.Rows[i].Cells[1].Value = this.streamResponse[i];
                            continue;
                        }
                        case 3:
                        {
                            this.dataGridView2.Rows[i].Cells[1].Value = this.streamResponse[i];
                            continue;
                        }
                        case 4:
                        {
                            this.dataGridView3.Rows[i].Cells[1].Value = this.streamResponse[i];
                            continue;
                        }
                    }
                }
                catch (Exception exception4)
                {
                    MessageBox.Show(exception4.Message, "streamResponseDisplayed");
                    return;
                }
            }
            if (streamResponseCount >= 1)
            {
                int num5;
                switch (this.systemStatus)
                {
                    case 2:
                        this.dataGridView1.CurrentCell = this.dataGridView1.Rows[streamResponseCount - 1].Cells[1];
                        num5 = (int) ((((double) streamResponseCount) / ((double) this.streamResponseTotal)) * 100.0);
                        this.pbEngrave.Value = num5;
                        this.lblEngravePercent.Text = num5.ToString() + "%";
                        this.lblEngraveTime.Text = str2;
                        break;

                    case 3:
                        this.dataGridView2.CurrentCell = this.dataGridView2.Rows[streamResponseCount - 1].Cells[1];
                        num5 = (int) ((((double) streamResponseCount) / ((double) this.streamResponseTotal)) * 100.0);
                        this.pbCut.Value = num5;
                        this.lblCutPercent.Text = num5.ToString() + "%";
                        this.lblCutTime.Text = str2;
                        break;

                    case 4:
                        this.dataGridView3.CurrentCell = this.dataGridView3.Rows[streamResponseCount - 1].Cells[1];
                        num5 = (int) ((((double) streamResponseCount) / ((double) this.streamResponseTotal)) * 100.0);
                        this.pbManual.Value = num5;
                        this.lblManualPercent.Text = num5.ToString() + "%";
                        this.lblManualTime.Text = str2;
                        break;
                }
            }
            this.streamResponseDisplayed = streamResponseCount;
        }

        private void updateCoordinates(string text)
        {
            try
            {
                int startIndex = text.IndexOf("WPos:") + 5;
                int length = text.IndexOf(",", startIndex) - startIndex;
                this.lblWorkX.Text = text.Substring(startIndex, length);
                int num3 = text.IndexOf(",", startIndex) + 1;
                int num4 = text.IndexOf(",", num3) - num3;
                this.lblWorkY.Text = text.Substring(num3, num4);
            }
            catch
            {
                this.lblWorkX.Text = "N/A";
                this.lblWorkY.Text = "N/A";
            }
        }

        private void updatePictureBox()
        {
            if (this.adjustedImage != null)
            {
                if ((this.adjustedImage.Width <= this.panel1.Width) && (this.adjustedImage.Height <= this.panel1.Height))
                {
                    this.displayedImage = new Bitmap(this.adjustedImage);
                    this.pictureBox1.Width = this.displayedImage.Width;
                    this.pictureBox1.Height = this.displayedImage.Height;
                    System.Drawing.Point point = new System.Drawing.Point {
                        X = 0,
                        Y = this.panel1.Height - this.displayedImage.Height
                    };
                    this.pictureBox1.Location = point;
                    this.pictureBox1.Image = this.displayedImage;
                }
                else if ((((float) this.adjustedImage.Width) / ((float) this.adjustedImage.Height)) >= (((float) this.panel1.Width) / ((float) this.panel1.Height)))
                {
                    float num = ((float) this.adjustedImage.Width) / ((float) this.adjustedImage.Height);
                    int height = Convert.ToInt32((float) (((float) this.panel1.Width) / num));
                    this.displayedImage = new Bitmap(this.adjustedImage, new Size(this.panel1.Width, height));
                    this.pictureBox1.Width = this.displayedImage.Width;
                    this.pictureBox1.Height = this.displayedImage.Height;
                    System.Drawing.Point point2 = new System.Drawing.Point {
                        X = 0,
                        Y = this.panel1.Height - this.displayedImage.Height
                    };
                    this.pictureBox1.Location = point2;
                    this.pictureBox1.Image = this.displayedImage;
                }
                else
                {
                    float num3 = ((float) this.adjustedImage.Width) / ((float) this.adjustedImage.Height);
                    int width = Convert.ToInt32((float) (this.panel1.Height * num3));
                    this.displayedImage = new Bitmap(this.adjustedImage, new Size(width, this.panel1.Height));
                    this.pictureBox1.Width = this.displayedImage.Width;
                    this.pictureBox1.Height = this.displayedImage.Height;
                    System.Drawing.Point point3 = new System.Drawing.Point {
                        X = 0,
                        Y = this.panel1.Height - this.displayedImage.Height
                    };
                    this.pictureBox1.Location = point3;
                    this.pictureBox1.Image = this.displayedImage;
                }
            }
        }

        private void updateStatusLabel(string text)
        {
            try
            {
                if (Settings.Default.language == "English")
                {
                    if (text.Contains("Idle"))
                    {
                        this.lblStatus.Text = "Idle";
                    }
                    else if (text.Contains("Run"))
                    {
                        this.lblStatus.Text = "Run";
                    }
                    else if (text.Contains("Hold"))
                    {
                        this.lblStatus.Text = "Hold";
                    }
                    else if (text.Contains("Alarm"))
                    {
                        this.lblStatus.Text = "Alarm";
                    }
                    else if (text.Contains("Check"))
                    {
                        this.lblStatus.Text = "Check";
                    }
                    else if (text.Contains("Home"))
                    {
                        this.lblStatus.Text = "Home";
                    }
                }
                else if (text.Contains("Idle"))
                {
                    this.lblStatus.Text = "Chờ lệnh";
                }
                else if (text.Contains("Run"))
                {
                    this.lblStatus.Text = "Đang chạy";
                }
                else if (text.Contains("Hold"))
                {
                    this.lblStatus.Text = "Tạm ngưng";
                }
                else if (text.Contains("Alarm"))
                {
                    this.lblStatus.Text = "Cảnh b\x00e1o";
                }
                else if (text.Contains("Check"))
                {
                    this.lblStatus.Text = "Kiểm tra";
                }
                else if (text.Contains("Home"))
                {
                    this.lblStatus.Text = "Home";
                }
            }
            catch
            {
                this.lblStatus.Text = "N/A";
            }
        }

        private void visualize()
        {
            double canvasLeft = this.canvasLeft;
            double canvasBottom = this.canvasBottom;
            bool flag = true;
            List<System.Drawing.Point> list = new List<System.Drawing.Point>();
            Graphics graphics = this.panelCanvas.CreateGraphics();
            Pen pen = new Pen(Color.Black);
            if (this.isFileLoaded)
            {
                if ((this.canvasWidth / this.canvasHeight) > (((double) this.panelCanvas.Width) / ((double) this.panelCanvas.Height)))
                {
                    this.pixelPerDXFUnit = ((double) (this.panelCanvas.Width - 1)) / this.canvasWidth;
                }
                else
                {
                    this.pixelPerDXFUnit = ((double) (this.panelCanvas.Height - 1)) / this.canvasHeight;
                }
                if (this.dxf.Lines.Count > 0)
                {
                    foreach (netDxf.Entities.Line line in this.dxf.Lines)
                    {
                        System.Drawing.Point point = new System.Drawing.Point();
                        System.Drawing.Point point2 = new System.Drawing.Point();
                        point.X = Convert.ToInt32((double) ((line.StartPoint.X - canvasLeft) * this.pixelPerDXFUnit));
                        point.Y = Convert.ToInt32((double) ((line.StartPoint.Y - canvasBottom) * this.pixelPerDXFUnit));
                        if (flag)
                        {
                            point.Y = this.flipY(point.Y);
                        }
                        point2.X = Convert.ToInt32((double) ((line.EndPoint.X - canvasLeft) * this.pixelPerDXFUnit));
                        point2.Y = Convert.ToInt32((double) ((line.EndPoint.Y - canvasBottom) * this.pixelPerDXFUnit));
                        if (flag)
                        {
                            point2.Y = this.flipY(point2.Y);
                        }
                        graphics.DrawLine(pen, point, point2);
                    }
                }
                if (this.dxf.LwPolylines.Count > 0)
                {
                    foreach (LwPolyline polyline in this.dxf.LwPolylines)
                    {
                        list.Clear();
                        foreach (LwPolylineVertex vertex in polyline.Vertexes)
                        {
                            System.Drawing.Point item = new System.Drawing.Point {
                                X = Convert.ToInt32((double) ((vertex.Position.X - canvasLeft) * this.pixelPerDXFUnit)),
                                Y = Convert.ToInt32((double) ((vertex.Position.Y - canvasBottom) * this.pixelPerDXFUnit))
                            };
                            if (flag)
                            {
                                item.Y = this.flipY(item.Y);
                            }
                            list.Add(item);
                        }
                        if (polyline.IsClosed)
                        {
                            graphics.DrawPolygon(pen, list.ToArray());
                        }
                        else
                        {
                            System.Drawing.Point point4 = list[0];
                            foreach (System.Drawing.Point point5 in list)
                            {
                                graphics.DrawLine(pen, point4, point5);
                                point4 = point5;
                            }
                        }
                    }
                }
                if (this.dxf.Circles.Count > 0)
                {
                    using (IEnumerator<Circle> enumerator5 = this.dxf.Circles.GetEnumerator())
                    {
                        while (enumerator5.MoveNext())
                        {
                            list.Clear();
                            foreach (LwPolylineVertex vertex2 in enumerator5.Current.ToPolyline(200).Vertexes)
                            {
                                System.Drawing.Point item = new System.Drawing.Point {
                                    X = Convert.ToInt32((double) ((vertex2.Position.X - canvasLeft) * this.pixelPerDXFUnit)),
                                    Y = Convert.ToInt32((double) ((vertex2.Position.Y - canvasBottom) * this.pixelPerDXFUnit))
                                };
                                if (flag)
                                {
                                    item.Y = this.flipY(item.Y);
                                }
                                list.Add(item);
                            }
                            graphics.DrawPolygon(pen, list.ToArray());
                        }
                    }
                }
                if (this.dxf.Ellipses.Count > 0)
                {
                    foreach (netDxf.Entities.Ellipse ellipse in this.dxf.Ellipses)
                    {
                        list.Clear();
                        foreach (LwPolylineVertex vertex3 in ellipse.ToPolyline(200).Vertexes)
                        {
                            System.Drawing.Point item = new System.Drawing.Point {
                                X = Convert.ToInt32((double) ((vertex3.Position.X - canvasLeft) * this.pixelPerDXFUnit)),
                                Y = Convert.ToInt32((double) ((vertex3.Position.Y - canvasBottom) * this.pixelPerDXFUnit))
                            };
                            if (flag)
                            {
                                item.Y = this.flipY(item.Y);
                            }
                            list.Add(item);
                        }
                        if (ellipse.IsFullEllipse)
                        {
                            graphics.DrawPolygon(pen, list.ToArray());
                        }
                        else
                        {
                            System.Drawing.Point point8 = list[0];
                            foreach (System.Drawing.Point point9 in list)
                            {
                                graphics.DrawLine(pen, point8, point9);
                                point8 = point9;
                            }
                        }
                    }
                }
                if (this.dxf.Arcs.Count > 0)
                {
                    using (IEnumerator<netDxf.Entities.Arc> enumerator7 = this.dxf.Arcs.GetEnumerator())
                    {
                        while (enumerator7.MoveNext())
                        {
                            list.Clear();
                            foreach (LwPolylineVertex vertex4 in enumerator7.Current.ToPolyline(200).Vertexes)
                            {
                                System.Drawing.Point item = new System.Drawing.Point {
                                    X = Convert.ToInt32((double) ((vertex4.Position.X - canvasLeft) * this.pixelPerDXFUnit)),
                                    Y = Convert.ToInt32((double) ((vertex4.Position.Y - canvasBottom) * this.pixelPerDXFUnit))
                                };
                                if (flag)
                                {
                                    item.Y = this.flipY(item.Y);
                                }
                                list.Add(item);
                            }
                            graphics.DrawCurve(pen, list.ToArray());
                        }
                    }
                }
            }
        }

        public void writeGRBLSettingsV09j(int xStepPerMM, int yStepPerMM, int xMaxRate, int yMaxRate)
        {
            if (this.systemStatus != 1)
            {
                MessageBox.Show("System is not currently idle, please try again later.", "Unable to write settings", MessageBoxButtons.OK);
            }
            else
            {
                this.systemStatus = 5;
                this.timerCoordinates.Enabled = false;
                List<string> list = new List<string> { 
                    "$0=10",
                    "$1=50",
                    "$2=0",
                    "$3=0",
                    "$4=0",
                    "$5=0",
                    "$6=0",
                    "$10=3",
                    "$11=0.010",
                    "$12=0.002"
                };
                if (Settings.Default.isMetric)
                {
                    list.Add("$13=0");
                }
                else
                {
                    list.Add("$13=1");
                }
                list.Add("$20=0");
                list.Add("$21=0");
                list.Add("$22=0");
                list.Add("$23=0");
                list.Add("$24=25.000");
                list.Add("$25=500.000");
                list.Add("$26=250");
                list.Add("$27=1.000");
                list.Add("$100=" + xStepPerMM.ToString());
                list.Add("$101=" + yStepPerMM.ToString());
                list.Add("$102=" + yStepPerMM.ToString());
                list.Add("$110=" + xMaxRate.ToString());
                list.Add("$111=" + yMaxRate.ToString());
                list.Add("$112=" + yMaxRate.ToString());
                list.Add("$120=80");
                list.Add("$121=80");
                list.Add("$122=80");
                list.Add("$130=600");
                list.Add("$131=600");
                list.Add("$132=600");
                string str = "";
                foreach (string str2 in list)
                {
                    try
                    {
                        this.serialPort1.Write(str2 + "\n");
                        while (!str.Contains("ok") && !str.Contains("error"))
                        {
                            str = this.serialPort1.ReadLine();
                        }
                        if (str.Contains("error"))
                        {
                            MessageBox.Show("Configuration failed. Command: " + str2 + ". GLBR's response: " + str, "Configuration failed", MessageBoxButtons.OK);
                            this.systemStatus = 1;
                            this.timerCoordinates.Enabled = true;
                            return;
                        }
                        Thread.Sleep(10);
                    }
                    catch (Exception exception1)
                    {
                        MessageBox.Show(exception1.Message);
                        this.systemStatus = 1;
                        this.timerCoordinates.Enabled = true;
                        return;
                    }
                }
                MessageBox.Show("Configuration successful! Type $$ in GRBL terminal to verify", "Successful", MessageBoxButtons.OK);
                this.systemStatus = 1;
                this.timerCoordinates.Enabled = true;
            }
        }

        public void writeGRBLSettingsV11()
        {
            if (this.systemStatus != 1)
            {
                MessageBox.Show("System is not currently idle, please try again later.", "Unable to write settings", MessageBoxButtons.OK);
            }
            else
            {
                this.systemStatus = 5;
                this.timerCoordinates.Enabled = false;
                List<string> list = new List<string> { 
                    "$0=10",
                    "$1=255",
                    "$2=0",
                    "$3=0",
                    "$4=0",
                    "$5=0",
                    "$6=0",
                    "$10=2",
                    "$11=0.010",
                    "$12=0.002",
                    "$13=0"
                };
                if (Settings.Default.isMetric)
                {
                    list.Add("$13=0");
                }
                else
                {
                    list.Add("$13=1");
                }
                list.Add("$20=0");
                list.Add("$21=0");
                list.Add("$22=0");
                list.Add("$23=0");
                list.Add("$24=25.000");
                list.Add("$25=500.000");
                list.Add("$26=250");
                list.Add("$27=1.000");
                list.Add("$30=5000");
                list.Add("$31=0");
                list.Add("$32=1");
                list.Add("$100=50.000");
                list.Add("$101=50.000");
                list.Add("$102=50.000");
                list.Add("$110=5000");
                list.Add("$111=5000");
                list.Add("$112=5000");
                list.Add("$120=400");
                list.Add("$121=400");
                list.Add("$122=400");
                list.Add("$130=500");
                list.Add("$131=500");
                list.Add("$132=500");
                string str = "";
                foreach (string str2 in list)
                {
                    try
                    {
                        this.serialPort1.Write(str2 + "\n");
                        while (!str.Contains("ok") && !str.Contains("error"))
                        {
                            str = this.serialPort1.ReadLine();
                        }
                        if (str.Contains("error"))
                        {
                            MessageBox.Show("Configuration failed. Command: " + str2 + ". GLBR's response: " + str, "Configuration failed", MessageBoxButtons.OK);
                            this.systemStatus = 1;
                            this.timerCoordinates.Enabled = true;
                            return;
                        }
                        Thread.Sleep(10);
                    }
                    catch (Exception exception1)
                    {
                        MessageBox.Show(exception1.Message);
                        this.systemStatus = 1;
                        this.timerCoordinates.Enabled = true;
                        return;
                    }
                }
                MessageBox.Show("Configuration successful! Type $$ in GRBL terminal to verify", "Successful", MessageBoxButtons.OK);
                this.systemStatus = 1;
                this.timerCoordinates.Enabled = true;
            }
        }

        private delegate void SetTextCallback(string text);

        public class StringValue
        {
            private string _value;

            public StringValue(string s)
            {
                this._value = s;
            }

            public string Value
            {
                get
                {
                    return _value;

                }
                set
                {
                    _value = value;
                }
            }
        }

        private void numScale_ValueChanged(object sender, EventArgs e)
        {
            double num = (double)numScale.Value;
            this.nudCutWidth.Value = (decimal)( this.canvasWidth * num);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SizeChanged -= frmMain_SizeChanged;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {

        }

        private void btnPlayBeam_Click(object sender, EventArgs e)
        {
            if (this.systemStatus == 1)
            {
                int num = (int)nmuLaserIntensity.Value;
                try
                {
                    this.serialPort1.Write("G1M3F1000S" + num.ToString() + "\n");
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message);
                }
            }
        }

        private void btnStopBeam_Click(object sender, EventArgs e)
        {
            btnLaserOff_Click(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (frmMaterialTemplete frm = new frmMaterialTemplete())
            {
                frm.FormType = FormStateType.Dialog;
                frm.ShowDialog(this);
            }
        }
        
        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool isMetric = Settings.Default.isMetric;
            new frmSettings { MyParent = this }.ShowDialog();
            if ((Settings.Default.language != Settings.Default.language) || (isMetric != Settings.Default.isMetric))
            {
                this.localize();
            }
        }

        private void materialTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmMaterialTemplete frm = new frmMaterialTemplete())
            {
                frm.FormType = FormStateType.InputData;
                frm.ShowDialog(this);
            }
        }
    }
}

