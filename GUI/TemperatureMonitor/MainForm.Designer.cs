namespace TemperatureMonitor
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Controls ──
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelChart;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTempValue;
        private System.Windows.Forms.Label lblTempUnit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblMinLabel;
        private System.Windows.Forms.Label lblMinValue;
        private System.Windows.Forms.Label lblMaxLabel;
        private System.Windows.Forms.Label lblMaxValue;
        private System.Windows.Forms.Label lblAvgLabel;
        private System.Windows.Forms.Label lblAvgValue;
        private System.Windows.Forms.Label lblSampleLabel;
        private System.Windows.Forms.Label lblSampleValue;

        private System.Windows.Forms.Label lblComPort;
        private System.Windows.Forms.ComboBox cmbPorts;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExport;

        private ScottPlot.WinForms.FormsPlot formsPlot1;

        private System.Windows.Forms.Timer timerRefresh;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            // ════════════════════════════════════════
            //  FORM
            // ════════════════════════════════════════
            this.Text = "🌡️ Temperature Monitor — USART1";
            this.Size = new System.Drawing.Size(1280, 780);
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 17, 26);
            this.ForeColor = Color.FromArgb(220, 225, 240);
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.DoubleBuffered = true;

            // ════════════════════════════════════════
            //  TOP BAR
            // ════════════════════════════════════════
            panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 60;
            panelTop.BackColor = Color.FromArgb(20, 24, 38);
            panelTop.Padding = new Padding(20, 0, 20, 0);

            lblTitle = new Label();
            lblTitle.Text = "🌡️  STM32F103 Temperature Monitor";
            lblTitle.Font = new Font("Segoe UI Semibold", 16F);
            lblTitle.ForeColor = Color.FromArgb(100, 180, 255);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            lblStatus = new Label();
            lblStatus.Text = "⬤ Disconnected";
            lblStatus.Font = new Font("Segoe UI", 11F);
            lblStatus.ForeColor = Color.FromArgb(255, 90, 90);
            lblStatus.AutoSize = true;
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblStatus.Location = new Point(1080, 18);

            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(lblStatus);

            // ════════════════════════════════════════
            //  LEFT SIDEBAR
            // ════════════════════════════════════════
            panelLeft = new Panel();
            panelLeft.Dock = DockStyle.Left;
            panelLeft.Width = 300;
            panelLeft.BackColor = Color.FromArgb(18, 21, 33);
            panelLeft.Padding = new Padding(20, 20, 20, 20);

            int y = 15;

            // ── Big Temperature Display ──
            lblTempValue = new Label();
            lblTempValue.Text = "--.-";
            lblTempValue.Font = new Font("Consolas", 48F, FontStyle.Bold);
            lblTempValue.ForeColor = Color.FromArgb(0, 230, 180);
            lblTempValue.Location = new Point(15, y);
            lblTempValue.Size = new Size(220, 70);
            lblTempValue.TextAlign = ContentAlignment.MiddleRight;
            panelLeft.Controls.Add(lblTempValue);

            lblTempUnit = new Label();
            lblTempUnit.Text = "°C";
            lblTempUnit.Font = new Font("Segoe UI", 22F);
            lblTempUnit.ForeColor = Color.FromArgb(0, 230, 180);
            lblTempUnit.Location = new Point(235, y + 20);
            lblTempUnit.AutoSize = true;
            panelLeft.Controls.Add(lblTempUnit);

            y += 85;

            // ── Divider ──
            var div1 = CreateDivider(y); panelLeft.Controls.Add(div1); y += 15;

            // ── Statistics ──
            lblMinLabel = new Label { Text = "MIN", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(120, 130, 160), Location = new Point(20, y), AutoSize = true };
            lblMinValue = new Label { Text = "--.- °C", Font = new Font("Consolas", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(80, 200, 255), Location = new Point(140, y - 3), AutoSize = true };
            panelLeft.Controls.Add(lblMinLabel);
            panelLeft.Controls.Add(lblMinValue);
            y += 35;

            lblMaxLabel = new Label { Text = "MAX", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(120, 130, 160), Location = new Point(20, y), AutoSize = true };
            lblMaxValue = new Label { Text = "--.- °C", Font = new Font("Consolas", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(255, 120, 80), Location = new Point(140, y - 3), AutoSize = true };
            panelLeft.Controls.Add(lblMaxLabel);
            panelLeft.Controls.Add(lblMaxValue);
            y += 35;

            lblAvgLabel = new Label { Text = "AVG", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(120, 130, 160), Location = new Point(20, y), AutoSize = true };
            lblAvgValue = new Label { Text = "--.- °C", Font = new Font("Consolas", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(255, 220, 80), Location = new Point(140, y - 3), AutoSize = true };
            panelLeft.Controls.Add(lblAvgLabel);
            panelLeft.Controls.Add(lblAvgValue);
            y += 35;

            lblSampleLabel = new Label { Text = "SAMPLES", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(120, 130, 160), Location = new Point(20, y), AutoSize = true };
            lblSampleValue = new Label { Text = "0", Font = new Font("Consolas", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(180, 160, 255), Location = new Point(140, y - 3), AutoSize = true };
            panelLeft.Controls.Add(lblSampleLabel);
            panelLeft.Controls.Add(lblSampleValue);
            y += 45;

            // ── Divider ──
            var div2 = CreateDivider(y); panelLeft.Controls.Add(div2); y += 20;

            // ── COM Port Selection ──
            lblComPort = new Label { Text = "COM Port", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(150, 160, 180), Location = new Point(20, y), AutoSize = true };
            panelLeft.Controls.Add(lblComPort);
            y += 22;

            cmbPorts = new ComboBox();
            cmbPorts.Location = new Point(20, y);
            cmbPorts.Size = new Size(245, 30);
            cmbPorts.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPorts.BackColor = Color.FromArgb(30, 35, 55);
            cmbPorts.ForeColor = Color.FromArgb(220, 225, 240);
            cmbPorts.FlatStyle = FlatStyle.Flat;
            cmbPorts.Font = new Font("Consolas", 11F);
            panelLeft.Controls.Add(cmbPorts);
            y += 40;

            // ── Baud Rate ──
            lblBaudRate = new Label { Text = "Baud Rate", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(150, 160, 180), Location = new Point(20, y), AutoSize = true };
            panelLeft.Controls.Add(lblBaudRate);
            y += 22;

            cmbBaudRate = new ComboBox();
            cmbBaudRate.Location = new Point(20, y);
            cmbBaudRate.Size = new Size(245, 30);
            cmbBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBaudRate.BackColor = Color.FromArgb(30, 35, 55);
            cmbBaudRate.ForeColor = Color.FromArgb(220, 225, 240);
            cmbBaudRate.FlatStyle = FlatStyle.Flat;
            cmbBaudRate.Font = new Font("Consolas", 11F);
            cmbBaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200", "230400", "460800" });
            cmbBaudRate.SelectedItem = "115200";
            panelLeft.Controls.Add(cmbBaudRate);
            y += 45;

            // ── Buttons ──
            btnConnect = new Button();
            btnConnect.Text = "🔗  Connect";
            btnConnect.Location = new Point(20, y);
            btnConnect.Size = new Size(245, 42);
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.FlatAppearance.BorderColor = Color.FromArgb(60, 140, 255);
            btnConnect.FlatAppearance.BorderSize = 2;
            btnConnect.BackColor = Color.FromArgb(25, 50, 100);
            btnConnect.ForeColor = Color.FromArgb(100, 180, 255);
            btnConnect.Font = new Font("Segoe UI Semibold", 11F);
            btnConnect.Cursor = Cursors.Hand;
            btnConnect.Click += btnConnect_Click;
            panelLeft.Controls.Add(btnConnect);
            y += 52;

            btnClear = new Button();
            btnClear.Text = "🗑️  Clear Data";
            btnClear.Location = new Point(20, y);
            btnClear.Size = new Size(118, 38);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(80, 85, 110);
            btnClear.BackColor = Color.FromArgb(30, 35, 50);
            btnClear.ForeColor = Color.FromArgb(180, 185, 200);
            btnClear.Font = new Font("Segoe UI", 9.5F);
            btnClear.Cursor = Cursors.Hand;
            btnClear.Click += btnClear_Click;
            panelLeft.Controls.Add(btnClear);

            btnExport = new Button();
            btnExport.Text = "💾  Export";
            btnExport.Location = new Point(147, y);
            btnExport.Size = new Size(118, 38);
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.FlatAppearance.BorderColor = Color.FromArgb(80, 85, 110);
            btnExport.BackColor = Color.FromArgb(30, 35, 50);
            btnExport.ForeColor = Color.FromArgb(180, 185, 200);
            btnExport.Font = new Font("Segoe UI", 9.5F);
            btnExport.Cursor = Cursors.Hand;
            btnExport.Click += btnExport_Click;
            panelLeft.Controls.Add(btnExport);

            // ════════════════════════════════════════
            //  CHART PANEL
            // ════════════════════════════════════════
            panelChart = new Panel();
            panelChart.Dock = DockStyle.Fill;
            panelChart.BackColor = Color.FromArgb(15, 17, 26);
            panelChart.Padding = new Padding(10, 10, 10, 10);

            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            formsPlot1.Dock = DockStyle.Fill;
            panelChart.Controls.Add(formsPlot1);

            // ════════════════════════════════════════
            //  TIMER
            // ════════════════════════════════════════
            timerRefresh = new System.Windows.Forms.Timer(this.components);
            timerRefresh.Interval = 100; // 10 Hz refresh
            timerRefresh.Tick += timerRefresh_Tick;

            // ════════════════════════════════════════
            //  ADD CONTROLS (order matters for Dock)
            // ════════════════════════════════════════
            this.Controls.Add(panelChart);   // Fill – added first
            this.Controls.Add(panelLeft);    // Left
            this.Controls.Add(panelTop);     // Top

            this.ResumeLayout(false);
        }

        private Panel CreateDivider(int yPos)
        {
            return new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(245, 1),
                BackColor = Color.FromArgb(50, 55, 75)
            };
        }
    }
}
