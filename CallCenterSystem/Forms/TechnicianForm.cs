using CallCenterSystem.Manager;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static CallCenterSystem.Patterns.StatePattern;
using MyAppContext = CallCenterSystem.Manager.AppContext;

namespace CallCenterSystem.Forms
{
    public class TechnicianForm : Form
    {
        private TextBox txtTechNumber_Tech, txtTechName_Tech;
        private TextBox txtStudentNumber_Tech, txtStudentName_Tech;
        private Button btnMakeCall_Tech, btnDropCall_Tech, btnHoldCall_Tech, btnResumeCall_Tech;
        private GroupBox grpActiveCall_Tech;
        private Label lblStatus_Tech, lblStartTime_Tech, lblDuration_Tech;
        private DataGridView dgvCallLog_Tech;
        private TextBox txtSearchCall_Tech;
        private Timer uiTimer_Tech;

        // Active call
        private Call activeCall_Tech;

        public TechnicianForm()
        {
            Text = "Technician Portal";
            ClientSize = new Size(750, 550);
            StartPosition = FormStartPosition.CenterScreen;

            InitializeControls();
            InitializeTimer();
            RefreshCallLog();
        }

        private void InitializeControls()
        {
            Controls.Add(new Label() { Text = "Technician Number:", Location = new Point(20, 18), AutoSize = true });
            txtTechNumber_Tech = new TextBox()
            {
                Location = new Point(140, 14),
                Width = 150,
                Text = MyAppContext.CurrentTechnicianNumber,
                Enabled = false
            };
            Controls.Add(txtTechNumber_Tech);

            Controls.Add(new Label() { Text = "Technician Name:", Location = new Point(320, 18), AutoSize = true });
            txtTechName_Tech = new TextBox()
            {
                Location = new Point(420, 14),
                Width = 200,
                Text = MyAppContext.CurrentTechnicianName,
                Enabled = false
            };
            Controls.Add(txtTechName_Tech);

    
            Controls.Add(new Label() { Text = "Student Number:", Location = new Point(20, 50), AutoSize = true });
            txtStudentNumber_Tech = new TextBox() { Location = new Point(140, 46), Width = 150 };
            Controls.Add(txtStudentNumber_Tech);

            Controls.Add(new Label() { Text = "Student Name:", Location = new Point(320, 50), AutoSize = true });
            txtStudentName_Tech = new TextBox() { Location = new Point(420, 46), Width = 200 };
            Controls.Add(txtStudentName_Tech);

     
            btnMakeCall_Tech = new Button() { Text = "Make Call", Location = new Point(140, 80), Width = 80 };
            btnMakeCall_Tech.Click += BtnMakeCall_Tech_Click;
            Controls.Add(btnMakeCall_Tech);

            btnDropCall_Tech = new Button() { Text = "Hang Up", Location = new Point(230, 80), Width = 80 };
            btnDropCall_Tech.Click += BtnDropCall_Tech_Click;
            Controls.Add(btnDropCall_Tech);

            btnHoldCall_Tech = new Button() { Text = "Hold", Location = new Point(320, 80), Width = 80 };
            btnHoldCall_Tech.Click += BtnHoldCall_Tech_Click;
            Controls.Add(btnHoldCall_Tech);

            btnResumeCall_Tech = new Button() { Text = "Resume", Location = new Point(410, 80), Width = 80 };
            btnResumeCall_Tech.Click += BtnResumeCall_Tech_Click;
            Controls.Add(btnResumeCall_Tech);

  
            grpActiveCall_Tech = new GroupBox() { Text = "Active Call", Location = new Point(20, 120), Size = new Size(710, 100) };
            lblStatus_Tech = new Label() { Text = "Status: None", Location = new Point(20, 25), AutoSize = true };
            lblStartTime_Tech = new Label() { Text = "Start Time: -", Location = new Point(20, 50), AutoSize = true };
            lblDuration_Tech = new Label() { Text = "Duration: 00:00:00", Location = new Point(300, 50), AutoSize = true };
            grpActiveCall_Tech.Controls.AddRange(new Control[] { lblStatus_Tech, lblStartTime_Tech, lblDuration_Tech });
            Controls.Add(grpActiveCall_Tech);


            Controls.Add(new Label() { Text = "Search Student:", Location = new Point(20, 230), AutoSize = true });
            txtSearchCall_Tech = new TextBox()
            {
                Location = new Point(140, 226),
                Width = 200,
                ForeColor = Color.Gray,
                Text = "Enter student number"
            };
            txtSearchCall_Tech.Enter += (s, e) =>
            {
                if (txtSearchCall_Tech.Text == "Enter student number")
                {
                    txtSearchCall_Tech.Text = "";
                    txtSearchCall_Tech.ForeColor = Color.Black;
                }
            };
            txtSearchCall_Tech.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearchCall_Tech.Text))
                {
                    txtSearchCall_Tech.Text = "Enter student number";
                    txtSearchCall_Tech.ForeColor = Color.Gray;
                }
            };
            txtSearchCall_Tech.TextChanged += TxtSearchCall_Tech_TextChanged;
            Controls.Add(txtSearchCall_Tech);

 
            dgvCallLog_Tech = new DataGridView()
            {
                Location = new Point(20, 260),
                Size = new Size(710, 230),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false
            };

            dgvCallLog_Tech.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Student Number", DataPropertyName = "StudentNumber", Width = 120 });
            dgvCallLog_Tech.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Caller Name", DataPropertyName = "CallerName", Width = 150 });
            dgvCallLog_Tech.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "State", DataPropertyName = "StateName", Width = 100 });
            dgvCallLog_Tech.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Start Time", DataPropertyName = "StartTime", Width = 120 });
            dgvCallLog_Tech.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "End Time", DataPropertyName = "EndTime", Width = 120 });
            dgvCallLog_Tech.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Duration", DataPropertyName = "Duration", Width = 100 });

            Controls.Add(dgvCallLog_Tech);
        }

        private void InitializeTimer()
        {
            uiTimer_Tech = new Timer() { Interval = 1000 };
            uiTimer_Tech.Tick += (s, e) =>
            {
                RefreshActiveCall();
                RefreshCallLog();
            };
            uiTimer_Tech.Start();
        }

        //Button Handlers
        private void BtnMakeCall_Tech_Click(object sender, EventArgs e)
        {
            if (activeCall_Tech != null && activeCall_Tech.StateName != "Call Ended")
            {
                MessageBox.Show("You already have an active call!");
                return;
            }

            string studentNumber = txtStudentNumber_Tech.Text.Trim();
            string studentName = txtStudentName_Tech.Text.Trim();

            if (string.IsNullOrEmpty(studentNumber) || string.IsNullOrEmpty(studentName))
            {
                MessageBox.Show("Please enter both student number and name.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            activeCall_Tech = new Call(studentNumber, studentName, fromTechnician: true);
            AppManager.PhoneProxy.MakeCall(activeCall_Tech);

            MessageBox.Show("Call connected with student!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshActiveCall();
        }

        private void BtnDropCall_Tech_Click(object sender, EventArgs e)
        {
            if (activeCall_Tech == null) return;

            AppManager.PhoneProxy.DropCall(activeCall_Tech);
            activeCall_Tech = null;

            RefreshActiveCall();
            RefreshCallLog();
        }

        private void BtnHoldCall_Tech_Click(object sender, EventArgs e)
        {
            if (activeCall_Tech == null || activeCall_Tech.StateName == "Call Ended") return;

            activeCall_Tech.Hold();
            RefreshActiveCall();
        }

        private void BtnResumeCall_Tech_Click(object sender, EventArgs e)
        {
            if (activeCall_Tech == null || activeCall_Tech.StateName == "Call Ended") return;

            activeCall_Tech.Resume();
            RefreshActiveCall();
        }

        // We use this to refresh active call
        private void RefreshActiveCall()
        {
            if (activeCall_Tech == null)
            {
                lblStatus_Tech.Text = "Status: None";
                lblStartTime_Tech.Text = "Start Time: -";
                lblDuration_Tech.Text = "Duration: 00:00:00";
                return;
            }

            lblStatus_Tech.Text = $"Status: {activeCall_Tech.StateName}";
            lblStartTime_Tech.Text = $"Start Time: {activeCall_Tech.CreatedAt:yyyy-MM-dd HH:mm:ss}";
            lblDuration_Tech.Text = $"Duration: {activeCall_Tech.DurationString()}";
        }

        //  We use this to refresh Call Log
        private void RefreshCallLog()
        {
            string filter = txtSearchCall_Tech.Text.Trim();
            if (filter == "Enter student number") filter = "";

            var calls = AppManager.CallLog.GetAll()
                .Where(c => c.FromTechnician && c.StateName == "Call Ended");

            if (!string.IsNullOrEmpty(filter))
            {
                calls = calls.Where(c => c.StudentNumber != null && c.StudentNumber.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            dgvCallLog_Tech.DataSource = calls
                .Select(c => new
                {
                    c.StudentNumber,
                    c.CallerName,
                    c.StateName,
                    StartTime = c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = c.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-",
                    Duration = c.DurationString()
                }).ToList();

            if (dgvCallLog_Tech.Rows.Count > 0)
                dgvCallLog_Tech.ClearSelection();
        }

        private void TxtSearchCall_Tech_TextChanged(object sender, EventArgs e)
        {
            RefreshCallLog();
        }
    }
}
