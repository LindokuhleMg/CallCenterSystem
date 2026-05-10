using CallCenterSystem.Manager;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static CallCenterSystem.Patterns.StatePattern;
using AppContext = CallCenterSystem.Manager.AppContext;

namespace CallCenterSystem.Forms
{
    public class StudentForm : Form
    {
        private TextBox txtStudentNumberBox, txtStudentNameBox, txtSearchCallBox;
        private Button btnMakeCallStudent, btnDropCallStudent, btnHoldCallStudent, btnResumeCallStudent;
        private DataGridView dgvCallLogStudent;
        private Timer uiTimerStudent;

        private GroupBox grpActiveCallStudent;
        private Label lblStatusStudent, lblStartTimeStudent, lblDurationStudent;

        private Call activeStudentCall;

        public StudentForm()
        {
            Text = "Student Portal";
            ClientSize = new Size(800, 550);
            StartPosition = FormStartPosition.CenterScreen;

            InitializeControls();
            InitializeTimer();
        }

        private void InitializeControls()
        {
            Controls.Add(new Label() { Text = "Student Number:", Location = new Point(20, 18), AutoSize = true });
            txtStudentNumberBox = new TextBox()
            {
                Location = new Point(140, 14),
                Width = 150,
                Text = AppContext.CurrentStudentNumber,
                Enabled = false
            };
            Controls.Add(txtStudentNumberBox);

            Controls.Add(new Label() { Text = "Name:", Location = new Point(320, 18), AutoSize = true });
            txtStudentNameBox = new TextBox()
            {
                Location = new Point(360, 14),
                Width = 200,
                Text = AppContext.CurrentStudentName,
                Enabled = false
            };
            Controls.Add(txtStudentNameBox);

            btnMakeCallStudent = new Button() { Text = "Make Call", Location = new Point(580, 12), Width = 90 };
            btnMakeCallStudent.Click += BtnMakeCallStudent_Click;
            Controls.Add(btnMakeCallStudent);

            btnDropCallStudent = new Button() { Text = "Hang Up", Location = new Point(680, 12), Width = 90 };
            btnDropCallStudent.Click += BtnDropCallStudent_Click;
            Controls.Add(btnDropCallStudent);

            btnHoldCallStudent = new Button() { Text = "Hold", Location = new Point(580, 42), Width = 90 };
            btnHoldCallStudent.Click += BtnHoldCallStudent_Click;
            Controls.Add(btnHoldCallStudent);

            btnResumeCallStudent = new Button() { Text = "Resume", Location = new Point(680, 42), Width = 90 };
            btnResumeCallStudent.Click += BtnResumeCallStudent_Click;
            Controls.Add(btnResumeCallStudent);

            // Active Call GroupBox
            grpActiveCallStudent = new GroupBox() { Text = "Active Call", Location = new Point(20, 70), Size = new Size(750, 100) };
            lblStatusStudent = new Label() { Text = "Status: None", Location = new Point(20, 25), AutoSize = true };
            lblStartTimeStudent = new Label() { Text = "Start Time: -", Location = new Point(20, 50), AutoSize = true };
            lblDurationStudent = new Label() { Text = "Duration: 00:00:00", Location = new Point(20, 75), AutoSize = true };
            grpActiveCallStudent.Controls.AddRange(new Control[] { lblStatusStudent, lblStartTimeStudent, lblDurationStudent });
            Controls.Add(grpActiveCallStudent);

            // Search Box with placeholder
            Controls.Add(new Label() { Text = "Search Calls:", Location = new Point(20, 180), AutoSize = true });
            txtSearchCallBox = new TextBox() { Location = new Point(100, 176), Width = 200 };
            txtSearchCallBox.Text = "Search by date (yyyy-MM-dd)";
            txtSearchCallBox.ForeColor = Color.Gray;

            txtSearchCallBox.Enter += (s, e) =>
            {
                if (txtSearchCallBox.Text == "Search by date (yyyy-MM-dd)")
                {
                    txtSearchCallBox.Text = "";
                    txtSearchCallBox.ForeColor = Color.Black;
                }
            };

            txtSearchCallBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearchCallBox.Text))
                {
                    txtSearchCallBox.Text = "Search by date (yyyy-MM-dd)";
                    txtSearchCallBox.ForeColor = Color.Gray;
                }
            };

            txtSearchCallBox.TextChanged += TxtSearchCallBox_TextChanged;
            Controls.Add(txtSearchCallBox);

            // Call Log Grid
            dgvCallLogStudent = new DataGridView()
            {
                Location = new Point(20, 210),
                Size = new Size(750, 320),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false
            };

            dgvCallLogStudent.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Student Number", DataPropertyName = "StudentNumber", Width = 120 });
            dgvCallLogStudent.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Caller Name", DataPropertyName = "CallerName", Width = 150 });
            dgvCallLogStudent.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "State", DataPropertyName = "StateName", Width = 100 });
            dgvCallLogStudent.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Start Time", DataPropertyName = "StartTime", Width = 150 });
            dgvCallLogStudent.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "End Time", DataPropertyName = "EndTime", Width = 150 });
            dgvCallLogStudent.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Duration", DataPropertyName = "Duration", Width = 100 });

            Controls.Add(dgvCallLogStudent);

            RefreshCallLogGridStudent();
        }

        private void InitializeTimer()
        {
            uiTimerStudent = new Timer() { Interval = 1000 };
            uiTimerStudent.Tick += (s, e) =>
            {
                RefreshActiveCallStudent();
                RefreshCallLogGridStudent();
            };
            uiTimerStudent.Start();
        }

        private void BtnMakeCallStudent_Click(object sender, EventArgs e)
        {
            if (activeStudentCall != null && activeStudentCall.StateName != "Call Ended")
            {
                MessageBox.Show("You already have an active call!");
                return;
            }

            string number = AppContext.CurrentStudentNumber;
            string name = AppContext.CurrentStudentName;

            activeStudentCall = new Call(number, name, fromTechnician: false);
            AppManager.PhoneProxy.MakeCall(activeStudentCall);

            MessageBox.Show("Call connected with call center!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshActiveCallStudent();
        }

        private void BtnDropCallStudent_Click(object sender, EventArgs e)
        {
            if (activeStudentCall == null) return;
            AppManager.PhoneProxy.DropCall(activeStudentCall);
            activeStudentCall = null;
            RefreshActiveCallStudent();
            RefreshCallLogGridStudent();
        }

        private void BtnHoldCallStudent_Click(object sender, EventArgs e)
        {
            if (activeStudentCall == null) return;
            AppManager.PhoneProxy.HoldCall(activeStudentCall);
            RefreshActiveCallStudent();
        }

        private void BtnResumeCallStudent_Click(object sender, EventArgs e)
        {
            if (activeStudentCall == null) return;
            AppManager.PhoneProxy.ResumeCall(activeStudentCall);
            RefreshActiveCallStudent();
        }

        // Search handler
        private void TxtSearchCallBox_TextChanged(object sender, EventArgs e)
        {
            string filter = txtSearchCallBox.Text;
            if (filter == "Search by date (yyyy-MM-dd)")
                filter = "";
            RefreshCallLogGridStudent(filter.Trim());
        }

        // We use this to refresh active call
        private void RefreshActiveCallStudent()
        {
            if (activeStudentCall == null)
            {
                lblStatusStudent.Text = "Status: None";
                lblStartTimeStudent.Text = "Start Time: -";
                lblDurationStudent.Text = "Duration: 00:00:00";
                return;
            }

            lblStatusStudent.Text = $"Status: {activeStudentCall.StateName}";
            lblStartTimeStudent.Text = $"Start Time: {activeStudentCall.CreatedAt:yyyy-MM-dd HH:mm:ss}";
            lblDurationStudent.Text = $"Duration: {activeStudentCall.DurationString()}";
        }

        //  We use this to refresh Call Log
        private void RefreshCallLogGridStudent(string filter = "")
        {
            var calls = AppManager.CallLog.GetAll()
                .Where(c => !c.FromTechnician && c.StateName == "Call Ended");

            if (!string.IsNullOrEmpty(filter))
            {
                calls = calls.Where(c =>
                    (c.StateName != null && c.StateName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (c.CreatedAt.ToString("yyyy-MM-dd").Contains(filter))
                );
            }

            dgvCallLogStudent.DataSource = calls
                .Select(c => new
                {
                    c.StudentNumber,
                    c.CallerName,
                    c.StateName,
                    StartTime = c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = c.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-",
                    Duration = c.DurationString()
                })
                .ToList();

            if (dgvCallLogStudent.Rows.Count > 0)
                dgvCallLogStudent.ClearSelection();
        }
    }
}
