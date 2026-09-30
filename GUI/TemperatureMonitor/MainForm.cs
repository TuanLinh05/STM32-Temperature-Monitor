using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace TemperatureMonitor
{
    public partial class MainForm : Form
    {
        // ── Serial Port ──
        private SerialPort? _serialPort;
        private bool _isConnected = false;

        // ── Data Storage ──
        private readonly List<double> _temperatures = new();
        private readonly List<DateTime> _timestamps = new();
        private readonly object _dataLock = new();
        private string _rxBuffer = "";

        // ── Statistics ──
        private double _minTemp = double.MaxValue;
        private double _maxTemp = double.MinValue;
        private double _sumTemp = 0;
        private int _sampleCount = 0;

        public MainForm()
        {
            InitializeComponent();
            SetupChart();
            RefreshComPorts();
        }

        // ════════════════════════════════════════
        //  CHART SETUP
        // ════════════════════════════════════════
        private void SetupChart()
        {
            var plt = formsPlot1.Plot;

            // Dark theme
            plt.FigureBackground.Color = ScottPlot.Color.FromHex("#0F111A");
            plt.DataBackground.Color = ScottPlot.Color.FromHex("#141824");

            // Axes styling
            plt.Axes.Bottom.Label.Text = "Time (s)";
            plt.Axes.Left.Label.Text = "Temperature (°C)";

            plt.Axes.Bottom.Label.ForeColor = ScottPlot.Color.FromHex("#8890B0");
            plt.Axes.Left.Label.ForeColor = ScottPlot.Color.FromHex("#8890B0");

            plt.Axes.Bottom.TickLabelStyle.ForeColor = ScottPlot.Color.FromHex("#6670A0");
            plt.Axes.Left.TickLabelStyle.ForeColor = ScottPlot.Color.FromHex("#6670A0");

            plt.Axes.Bottom.MajorTickStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Left.MajorTickStyle.Color = ScottPlot.Color.FromHex("#2A2F45");

            plt.Axes.Bottom.MinorTickStyle.Color = ScottPlot.Color.FromHex("#1E2235");
            plt.Axes.Left.MinorTickStyle.Color = ScottPlot.Color.FromHex("#1E2235");

            plt.Axes.Bottom.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Left.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Top.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Right.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");

            plt.Grid.MajorLineColor = ScottPlot.Color.FromHex("#1E2235");

            plt.Title("Real-Time Temperature Plot");
            plt.Axes.Title.Label.ForeColor = ScottPlot.Color.FromHex("#64B4FF");
            plt.Axes.Title.Label.FontSize = 16;

            formsPlot1.Refresh();
        }

        // ════════════════════════════════════════
        //  COM PORT
        // ════════════════════════════════════════
        private void RefreshComPorts()
        {
            cmbPorts.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            cmbPorts.Items.AddRange(ports);
            if (ports.Length > 0)
                cmbPorts.SelectedIndex = 0;
        }

        // ════════════════════════════════════════
        //  CONNECT / DISCONNECT
        // ════════════════════════════════════════
        private void btnConnect_Click(object? sender, EventArgs e)
        {
            if (_isConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private void Connect()
        {
            if (cmbPorts.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM port.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshComPorts();
                return;
            }

            try
            {
                _serialPort = new SerialPort
                {
                    PortName = cmbPorts.SelectedItem.ToString()!,
                    BaudRate = int.Parse(cmbBaudRate.SelectedItem?.ToString() ?? "115200"),
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    ReadTimeout = 500,
                    WriteTimeout = 500,
                    DtrEnable = true,
                    RtsEnable = true
                };

                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.Open();

                _isConnected = true;
                _rxBuffer = "";

                // UI Update
                lblStatus.Text = "⬤ Connected";
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(0, 230, 130);
                btnConnect.Text = "🔌  Disconnect";
                btnConnect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 90, 90);
                btnConnect.BackColor = System.Drawing.Color.FromArgb(80, 25, 25);
                btnConnect.ForeColor = System.Drawing.Color.FromArgb(255, 120, 100);
                cmbPorts.Enabled = false;
                cmbBaudRate.Enabled = false;

                timerRefresh.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open serial port:\n{ex.Message}",
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Disconnect()
        {
            timerRefresh.Stop();

            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }
            catch { }

            _isConnected = false;

            // UI Update
            lblStatus.Text = "⬤ Disconnected";
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(255, 90, 90);
            btnConnect.Text = "🔗  Connect";
            btnConnect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 140, 255);
            btnConnect.BackColor = System.Drawing.Color.FromArgb(25, 50, 100);
            btnConnect.ForeColor = System.Drawing.Color.FromArgb(100, 180, 255);
            cmbPorts.Enabled = true;
            cmbBaudRate.Enabled = true;

            RefreshComPorts();
        }

        // ════════════════════════════════════════
        //  SERIAL DATA RECEIVED (Background Thread)
        // ════════════════════════════════════════
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort == null || !_serialPort.IsOpen) return;

                string incoming = _serialPort.ReadExisting();
                _rxBuffer += incoming;

                // Process complete lines
                while (_rxBuffer.Contains("\n"))
                {
                    int idx = _rxBuffer.IndexOf("\n");
                    string line = _rxBuffer.Substring(0, idx).Trim();
                    _rxBuffer = _rxBuffer.Substring(idx + 1);

                    ProcessLine(line);
                }
            }
            catch { }
        }

        private void ProcessLine(string line)
        {
            // Expected format: "TEMP:xx.xx"
            if (line.StartsWith("TEMP:", StringComparison.OrdinalIgnoreCase))
            {
                string valueStr = line.Substring(5).Trim();
                if (double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double temp))
                {
                    lock (_dataLock)
                    {
                        _temperatures.Add(temp);
                        _timestamps.Add(DateTime.Now);

                        _sampleCount++;
                        _sumTemp += temp;
                        if (temp < _minTemp) _minTemp = temp;
                        if (temp > _maxTemp) _maxTemp = temp;
                    }
                }
            }
        }

        // ════════════════════════════════════════
        //  TIMER TICK — UI REFRESH (10 Hz)
        // ════════════════════════════════════════
        private void timerRefresh_Tick(object? sender, EventArgs e)
        {
            lock (_dataLock)
            {
                if (_temperatures.Count == 0) return;

                double lastTemp = _temperatures[^1];
                double avg = _sumTemp / _sampleCount;

                // ── Update labels ──
                lblTempValue.Text = lastTemp.ToString("F1");

                // Color based on temperature range
                if (lastTemp > 50)
                    lblTempValue.ForeColor = System.Drawing.Color.FromArgb(255, 80, 60);
                else if (lastTemp > 35)
                    lblTempValue.ForeColor = System.Drawing.Color.FromArgb(255, 180, 50);
                else
                    lblTempValue.ForeColor = System.Drawing.Color.FromArgb(0, 230, 180);

                lblMinValue.Text = $"{_minTemp:F1} °C";
                lblMaxValue.Text = $"{_maxTemp:F1} °C";
                lblAvgValue.Text = $"{avg:F1} °C";
                lblSampleValue.Text = _sampleCount.ToString("N0");

                // ── Update chart ──
                UpdateChart();
            }
        }

        private void UpdateChart()
        {
            if (_temperatures.Count < 2) return;

            var plt = formsPlot1.Plot;
            plt.Clear();

            // Create time axis in seconds from start
            double[] timeSeconds = new double[_timestamps.Count];
            DateTime startTime = _timestamps[0];
            for (int i = 0; i < _timestamps.Count; i++)
            {
                timeSeconds[i] = (_timestamps[i] - startTime).TotalSeconds;
            }

            double[] temps = _temperatures.ToArray();

            // Main line
            var scatter = plt.Add.Scatter(timeSeconds, temps);
            scatter.Color = ScottPlot.Color.FromHex("#00E6B4");
            scatter.LineWidth = 2.5f;
            scatter.MarkerSize = 0;

            // Fill area under curve
            var fill = plt.Add.FillY(timeSeconds, temps, Enumerable.Repeat(0.0, temps.Length).ToArray());
            fill.FillStyle.Color = ScottPlot.Color.FromHex("#00E6B4").WithAlpha(30);

            // Auto-scale with padding
            plt.Axes.AutoScale();
            plt.Axes.Margins(left: 0.02, right: 0.02, bottom: 0.1, top: 0.15);

            // Re-apply styling (cleared by plt.Clear())
            plt.FigureBackground.Color = ScottPlot.Color.FromHex("#0F111A");
            plt.DataBackground.Color = ScottPlot.Color.FromHex("#141824");
            plt.Grid.MajorLineColor = ScottPlot.Color.FromHex("#1E2235");

            plt.Axes.Bottom.Label.Text = "Time (s)";
            plt.Axes.Left.Label.Text = "Temperature (°C)";
            plt.Axes.Bottom.Label.ForeColor = ScottPlot.Color.FromHex("#8890B0");
            plt.Axes.Left.Label.ForeColor = ScottPlot.Color.FromHex("#8890B0");
            plt.Axes.Bottom.TickLabelStyle.ForeColor = ScottPlot.Color.FromHex("#6670A0");
            plt.Axes.Left.TickLabelStyle.ForeColor = ScottPlot.Color.FromHex("#6670A0");
            plt.Axes.Bottom.MajorTickStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Left.MajorTickStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Bottom.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Left.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Top.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");
            plt.Axes.Right.FrameLineStyle.Color = ScottPlot.Color.FromHex("#2A2F45");

            plt.Title("Real-Time Temperature Plot");
            plt.Axes.Title.Label.ForeColor = ScottPlot.Color.FromHex("#64B4FF");
            plt.Axes.Title.Label.FontSize = 16;

            formsPlot1.Refresh();
        }

        // ════════════════════════════════════════
        //  CLEAR DATA
        // ════════════════════════════════════════
        private void btnClear_Click(object? sender, EventArgs e)
        {
            lock (_dataLock)
            {
                _temperatures.Clear();
                _timestamps.Clear();
                _minTemp = double.MaxValue;
                _maxTemp = double.MinValue;
                _sumTemp = 0;
                _sampleCount = 0;
            }

            lblTempValue.Text = "--.-";
            lblTempValue.ForeColor = System.Drawing.Color.FromArgb(0, 230, 180);
            lblMinValue.Text = "--.- °C";
            lblMaxValue.Text = "--.- °C";
            lblAvgValue.Text = "--.- °C";
            lblSampleValue.Text = "0";

            formsPlot1.Plot.Clear();
            SetupChart();
        }

        // ════════════════════════════════════════
        //  EXPORT TO CSV
        // ════════════════════════════════════════
        private void btnExport_Click(object? sender, EventArgs e)
        {
            if (_temperatures.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = $"Temperature_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                Title = "Export Temperature Data"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lock (_dataLock)
                    {
                        using var writer = new StreamWriter(sfd.FileName);
                        writer.WriteLine("Timestamp,Temperature_C");
                        for (int i = 0; i < _temperatures.Count; i++)
                        {
                            writer.WriteLine($"{_timestamps[i]:yyyy-MM-dd HH:mm:ss.fff},{_temperatures[i]:F2}");
                        }
                    }

                    MessageBox.Show($"Exported {_sampleCount} samples to:\n{sfd.FileName}",
                        "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Export failed:\n{ex.Message}",
                        "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ════════════════════════════════════════
        //  FORM CLOSING
        // ════════════════════════════════════════
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isConnected)
            {
                Disconnect();
            }
            base.OnFormClosing(e);
        }
    }
}
