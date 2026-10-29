namespace RailScope
{
    partial class Form1
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer? components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new Panel();
            btnSearchStations = new Button();
            btnRefreshDepartures = new Button();
            lblStation = new Label();
            txtStation = new TextBox();
            txtResults = new RichTextBox();
            dvgDepartures = new DataGridView();
            lblLastUpdated = new Label();
            lblStatus = new Label();
            cbStations = new ComboBox();
            lblCurrentStationInfo = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dvgDepartures).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDarkDark;
            panel1.Controls.Add(btnSearchStations);
            panel1.Controls.Add(btnRefreshDepartures);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(155, 720);
            panel1.TabIndex = 0;
            // 
            // btnSearchStations
            // 
            btnSearchStations.BackColor = Color.FromArgb(0, 120, 215);
            btnSearchStations.ForeColor = Color.White;
            btnSearchStations.Location = new Point(10, 11);
            btnSearchStations.Name = "btnSearchStations";
            btnSearchStations.Size = new Size(135, 29);
            btnSearchStations.TabIndex = 1;
            btnSearchStations.Text = "Search Stations";
            btnSearchStations.UseVisualStyleBackColor = false;
            btnSearchStations.Click += btnSearchStations_Click;
            // 
            // btnRefreshDepartures
            // 
            btnRefreshDepartures.BackColor = Color.FromArgb(0, 200, 50);
            btnRefreshDepartures.ForeColor = Color.Black;
            btnRefreshDepartures.Location = new Point(10, 46);
            btnRefreshDepartures.Name = "btnRefreshDepartures";
            btnRefreshDepartures.Size = new Size(135, 29);
            btnRefreshDepartures.TabIndex = 2;
            btnRefreshDepartures.Text = "Live Departures";
            btnRefreshDepartures.UseVisualStyleBackColor = false;
            btnRefreshDepartures.Click += btnRefreshDepartures_Click;
            // 
            // lblStation
            // 
            lblStation.AutoSize = true;
            lblStation.BackColor = Color.White;
            lblStation.ForeColor = SystemColors.ControlText;
            lblStation.Location = new Point(161, 31);
            lblStation.Name = "lblStation";
            lblStation.Size = new Size(185, 20);
            lblStation.TabIndex = 4;
            lblStation.Text = "Station name or CRS code:";
            // 
            // txtStation
            // 
            txtStation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtStation.Location = new Point(352, 28);
            txtStation.Name = "txtStation";
            txtStation.Size = new Size(213, 27);
            txtStation.TabIndex = 5;
            // 
            // txtResults
            // 
            txtResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtResults.Font = new Font("Consolas", 9F);
            txtResults.Location = new Point(163, 57);
            txtResults.Name = "txtResults";
            txtResults.ReadOnly = true;
            txtResults.Size = new Size(1002, 630);
            txtResults.TabIndex = 6;
            txtResults.Text = "";
            // 
            // dvgDepartures
            // 
            dvgDepartures.BackgroundColor = Color.FromArgb(45, 45, 45);
            dvgDepartures.ColumnHeadersHeight = 29;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(60, 60, 60);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            dvgDepartures.DefaultCellStyle = dataGridViewCellStyle1;
            dvgDepartures.Location = new Point(163, 105);
            dvgDepartures.Name = "dvgDepartures";
            dvgDepartures.RowHeadersWidth = 51;
            dvgDepartures.Size = new Size(1002, 582);
            dvgDepartures.TabIndex = 0;
            // 
            // lblLastUpdated
            // 
            lblLastUpdated.AutoSize = true;
            lblLastUpdated.BackColor = Color.White;
            lblLastUpdated.Location = new Point(571, 4);
            lblLastUpdated.Name = "lblLastUpdated";
            lblLastUpdated.Size = new Size(99, 20);
            lblLastUpdated.TabIndex = 7;
            lblLastUpdated.Text = "Data Retreval";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.BackColor = Color.White;
            lblStatus.ForeColor = SystemColors.ActiveCaptionText;
            lblStatus.Location = new Point(161, 4);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(49, 20);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "Status";
            // 
            // cbStations
            // 
            cbStations.FormattingEnabled = true;
            cbStations.Location = new Point(571, 27);
            cbStations.Name = "cbStations";
            cbStations.Size = new Size(282, 28);
            cbStations.TabIndex = 9;
            cbStations.SelectedIndexChanged += cbStations_SelectedIndexChanged;
            // 
            // lblCurrentStationInfo
            // 
            lblCurrentStationInfo.AutoSize = true;
            lblCurrentStationInfo.BackColor = Color.White;
            lblCurrentStationInfo.Font = new Font("Segoe UI", 9F);
            lblCurrentStationInfo.Location = new Point(163, 82);
            lblCurrentStationInfo.Name = "lblCurrentStationInfo";
            lblCurrentStationInfo.Size = new Size(149, 20);
            lblCurrentStationInfo.TabIndex = 5;
            lblCurrentStationInfo.Text = "Station: Not Selected";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1280, 720);
            Controls.Add(lblCurrentStationInfo);
            Controls.Add(cbStations);
            Controls.Add(lblStatus);
            Controls.Add(lblLastUpdated);
            Controls.Add(dvgDepartures);
            Controls.Add(txtResults);
            Controls.Add(txtStation);
            Controls.Add(lblStation);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(700, 450);
            Name = "Form1";
            Text = "RailScope";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dvgDepartures).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Button btnSearchStations;
        private Button btnRefreshDepartures;
        private Label lblStation;
        private TextBox txtStation;
        private RichTextBox txtResults;
        private DataGridView dvgDepartures;
        private Label lblLastUpdated;
        private Label lblStatus;
        private ComboBox cbStations;
        private Label lblCurrentStationInfo;
    }
}
