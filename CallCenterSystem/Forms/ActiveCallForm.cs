using CallCenterSystem.Manager;
using System.Drawing;
using System.Windows.Forms;
using static CallCenterSystem.Patterns.StatePattern;

namespace CallCenterSystem.Forms
{
    public class ActiveCallForm : Form
    {
        private Call activeCall;

        private Label activeCallLblCaller, activeCallLblStudent, activeCallLblStatus, activeCallLblStartTime, activeCallLblDuration;
        private Button activeCallBtnHold, activeCallBtnResume, activeCallBtnDrop;
        private Timer activeCallUiTimer;

        public ActiveCallForm(Call call)
        {
            activeCall = call;

            Text = "Active Call - " + (string.IsNullOrEmpty(call.StudentNumber) ? call.CallerName : call.StudentNumber);
            ClientSize = new Size(420, 260);
            StartPosition = FormStartPosition.CenterParent;

            InitializeControls();
            InitializeTimer();
        }

        private void InitializeControls()
        {
            Controls.Add(new Label() { Text = "Caller:", Location = new Point(20, 20), AutoSize = true });
            activeCallLblCaller = new Label() { Text = activeCall.CallerName, Location = new Point(140, 20), AutoSize = true };
            Controls.Add(activeCallLblCaller);

            Controls.Add(new Label() { Text = "Student Number:", Location = new Point(20, 50), AutoSize = true });
            activeCallLblStudent = new Label() { Text = activeCall.StudentNumber, Location = new Point(140, 50), AutoSize = true };
            Controls.Add(activeCallLblStudent);

            activeCallLblStatus = new Label() { Text = "Status: " + activeCall.StateName, Location = new Point(20, 80), AutoSize = true };
            Controls.Add(activeCallLblStatus);

            activeCallLblStartTime = new Label() { Text = "Start Time: " + activeCall.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), Location = new Point(20, 110), AutoSize = true };
            Controls.Add(activeCallLblStartTime);

            activeCallLblDuration = new Label() { Text = "Duration: " + activeCall.DurationString(), Location = new Point(20, 140), AutoSize = true };
            Controls.Add(activeCallLblDuration);

            activeCallBtnHold = new Button() { Text = "Hold", Location = new Point(20, 180), Width = 80 };
            activeCallBtnHold.Click += (s, e) => { AppManager.PhoneProxy.HoldCall(activeCall); RefreshUI(); };
            Controls.Add(activeCallBtnHold);

            activeCallBtnResume = new Button() { Text = "Resume", Location = new Point(120, 180), Width = 80 };
            activeCallBtnResume.Click += (s, e) => { AppManager.PhoneProxy.ResumeCall(activeCall); RefreshUI(); };
            Controls.Add(activeCallBtnResume);

            activeCallBtnDrop = new Button() { Text = "Hang Up", Location = new Point(220, 180), Width = 100 };
            activeCallBtnDrop.Click += (s, e) => { AppManager.PhoneProxy.DropCall(activeCall); RefreshUI(); Close(); };
            Controls.Add(activeCallBtnDrop);
        }

        private void InitializeTimer()
        {
            activeCallUiTimer = new Timer() { Interval = 1000 };
            activeCallUiTimer.Tick += (s, e) => RefreshUI();
            activeCallUiTimer.Start();
        }

        private void RefreshUI()
        {
            activeCallLblStatus.Text = "Status: " + activeCall.StateName;
            activeCallLblDuration.Text = "Duration: " + activeCall.DurationString();

            if (activeCall.StateName == "Ended/HungUp" || activeCall.StateName == "Ended")
            {
                activeCallUiTimer.Stop();
                Close();
            }
        }
    }
}
