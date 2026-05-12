using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        private System.IO.Ports.SerialPort serialPortArduino;
        private System.Windows.Forms.Timer timerOefening3;
        private System.Windows.Forms.Timer timerVerbinding;

        public Form1()
        {
            InitializeComponent();
            radioButtonVerbonden.AutoCheck = false;

            serialPortArduino = new System.IO.Ports.SerialPort();
            serialPortArduino.ReadTimeout = 1000;
            serialPortArduino.WriteTimeout = 1000;

            // Timer for Oefening 3
            timerOefening3 = new System.Windows.Forms.Timer();
            timerOefening3.Interval = 1000;
            timerOefening3.Tick += timerOefening3_Tick;

            // Timer voor controleren USB verbinding
            timerVerbinding = new System.Windows.Forms.Timer();
            timerVerbinding.Interval = 500;
            timerVerbinding.Tick += timerVerbinding_Tick;
            timerVerbinding.Start();

            // Handle tab selection changes to enable/disable the timer
            this.tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
        }

        private void timerVerbinding_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null &&
                    serialPortArduino.IsOpen)
                {
                    // Test of poort nog reageert
                    serialPortArduino.WriteLine("");

                    // Controleer of COM-poort nog bestaat
                    string[] poorten = SerialPort.GetPortNames();

                    if (!poorten.Contains(serialPortArduino.PortName))
                    {
                        VerbreekVerbinding();
                    }
                }
            }
            catch
            {
                VerbreekVerbinding();
            }
        }
        private void VerbreekVerbinding()
        {
            try
            {
                timerOefening3.Stop();
                timerOefening4.Stop();
                timerOefening5.Stop();

                if (serialPortArduino != null)
                {
                    if (serialPortArduino.IsOpen)
                    {
                        serialPortArduino.Close();
                    }

                    serialPortArduino.Dispose();
                }
            }
            catch
            {

            }

            radioButtonVerbonden.Checked = false;

            buttonConnect.Text = "Connect";

            labelStatus.Text = "Arduino losgekoppeld";

            this.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPortArduino.IsOpen)
                {
                    // Configure from UI controls
                    string portName = comboBoxPoort.SelectedItem?.ToString() ?? comboBoxPoort.Text;
                    if (string.IsNullOrWhiteSpace(portName))
                    {
                        labelStatus.Text = "Geen poort geselecteerd";
                        return;
                    }
                    serialPortArduino.PortName = portName;

                    int baud;
                    if (!int.TryParse(comboBoxBaudrate.SelectedItem?.ToString() ?? comboBoxBaudrate.Text, out baud))
                        baud = 115200;
                    serialPortArduino.BaudRate = baud;

                    serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;

                    if (radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
                    else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
                    else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
                    else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;
                    else serialPortArduino.Parity = Parity.None;

                    if (radioButtonStopbitsNone.Checked) serialPortArduino.StopBits = StopBits.None;
                    else if (radioButtonStopbitsOne.Checked) serialPortArduino.StopBits = StopBits.One;
                    else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
                    else if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;
                    else serialPortArduino.StopBits = StopBits.One;

                    if (radioButtonHandshakeNone.Checked) serialPortArduino.Handshake = Handshake.None;
                    else if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
                    else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
                    else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;
                    else serialPortArduino.Handshake = Handshake.None;

                    serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;
                    serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;

                    serialPortArduino.Open();

                    radioButtonVerbonden.Checked = true;
                    radioButtonVerbonden.Refresh();
                    buttonConnect.Text = "Disconnect";
                    labelStatus.Text = "Verbonden";
                }
                else
                {
                    serialPortArduino.Close();
                    radioButtonVerbonden.Checked = false;
                    buttonConnect.Text = "Connect";
                    labelStatus.Text = "Niet verbonden";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout: " + ex.Message;
            }
        }

        private void checkBoxDigital2_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = checkBoxDigital2.Checked ? "set d2 high" : "set d2 low";
                    serialPortArduino.WriteLine(command);
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout bij verzenden: " + ex.Message;
            }
        }

        private void checkBoxDigital3_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = checkBoxDigital3.Checked ? "set d3 high" : "set d3 low";
                    serialPortArduino.WriteLine(command);
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout bij verzenden: " + ex.Message;
            }
        }

        private void checkBoxDigital4_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = checkBoxDigital4.Checked ? "set d4 high" : "set d4 low";
                    serialPortArduino.WriteLine(command);
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout bij verzenden: " + ex.Message;
            }
        }

        private void trackBarPWM9_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = "set pwm9 " + trackBarPWM9.Value;
                    serialPortArduino.WriteLine(command);
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout bij verzenden: " + ex.Message;
            }
        }

        private void trackBarPWM10_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = "set pwm10 " + trackBarPWM10.Value;
                    serialPortArduino.WriteLine(command);
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout bij verzenden: " + ex.Message;
            }
        }

        private void trackBarPWM11_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = "set pwm11 " + trackBarPWM11.Value;
                    serialPortArduino.WriteLine(command);
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout bij verzenden: " + ex.Message;
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                timerOefening3.Enabled = (this.tabControl.SelectedTab == this.tabPageOefening3);
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout: " + ex.Message;
            }

            timerOefening4.Enabled = tabControl.SelectedIndex == 4;
            timerOefening5.Enabled = tabControl.SelectedIndex == 5;
        }

        private void timerOefening3_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    // Clear previous incoming data
                    serialPortArduino.ReadExisting();

                    // digital5
                    serialPortArduino.WriteLine("get d5");
                    string resp5 = serialPortArduino.ReadLine().Trim();
                    string val5 = resp5.Contains(":") ? resp5.Split(':')[1].Trim() : resp5;
                    radioButtonDigital5.Checked = (val5 == "1");

                    // digital6
                    serialPortArduino.WriteLine("get d6");
                    string resp6 = serialPortArduino.ReadLine().Trim();
                    string val6 = resp6.Contains(":") ? resp6.Split(':')[1].Trim() : resp6;
                    radioButtonDigital6.Checked = (val6 == "1");

                    // digital7
                    serialPortArduino.WriteLine("get d7");
                    string resp7 = serialPortArduino.ReadLine().Trim();
                    string val7 = resp7.Contains(":") ? resp7.Split(':')[1].Trim() : resp7;
                    radioButtonDigital7.Checked = (val7 == "1");

                    // Show raw responses for debugging
                    labelStatus.Text = string.Format("d5={0} d6={1} d3={2}", resp5, resp6, resp7);
                }
                else
                {
                    labelStatus.Text = "Niet verbonden";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Fout timer: " + ex.Message;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void labelAnalog0_Click(object sender, EventArgs e)
        {
       
        }

        private void timerOefening4_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    serialPortArduino.ReadExisting();
                    string commando = "get a0";
                    serialPortArduino.WriteLine(commando);
                    string antwoord = serialPortArduino.ReadLine();
                    antwoord = antwoord.TrimEnd();
                    antwoord = antwoord.Substring(4);

                    int value = Int32.Parse(antwoord);
                    labelAnalog0.Text = value.ToString();
                }
            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "connect";
            }
        }

        private void timerOefening5_Tick(object sender, EventArgs e)
        {

            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    serialPortArduino.ReadExisting();

                    // =========================
                    // GEWENSTE TEMPERATUUR (A0)
                    // =========================

                    serialPortArduino.WriteLine("get a0");

                    string antwoordA0 = serialPortArduino.ReadLine();
                    antwoordA0 = antwoordA0.TrimEnd();
                    antwoordA0 = antwoordA0.Substring(4);

                    int waardeA0 = Int32.Parse(antwoordA0);

                    // Herschalen 0..1023 -> 5..45 °C
                    double ricoPot = (45.0 - 5.0) / 1023.0;
                    double gewensteTemp = (waardeA0 * ricoPot) + 5.0;

                    labelGewensteTemp.Text =
                        gewensteTemp.ToString("0.0") + " °C";


                    // =========================
                    // HUIDIGE TEMPERATUUR (A1)
                    // =========================

                    serialPortArduino.WriteLine("get a1");

                    string antwoordA1 = serialPortArduino.ReadLine();
                    antwoordA1 = antwoordA1.TrimEnd();
                    antwoordA1 = antwoordA1.Substring(4);

                    int waardeA1 = Int32.Parse(antwoordA1);

                    // Herschalen 0..1023 -> 0..500 °C
                    double ricoLM35 = 500.0 / 1023.0;
                    double huidigeTemp = waardeA1 * ricoLM35;

                    labelHuidigeTemp.Text =
                        huidigeTemp.ToString("0.0") + " °C";


                    // =========================
                    // LED OP D2 AANSTUREN
                    // =========================

                    if (huidigeTemp < gewensteTemp)
                    {
                        serialPortArduino.WriteLine("set d2 high");
                    }
                    else
                    {
                        serialPortArduino.WriteLine("set d2 low");
                    }
                }
                else
                {
                    labelStatus.Text = "Error: kabel niet aangelosten";

                    serialPortArduino.Close();

                    radioButtonVerbonden.Checked = false;

                    buttonConnect.Text = "connect";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;

                serialPortArduino.Close();

                radioButtonVerbonden.Checked = false;

                buttonConnect.Text = "connect";
            }
        }
        }
    }
    
