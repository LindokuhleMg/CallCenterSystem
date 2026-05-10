using CallCenterSystem.Manager;
using CallCenterSystem.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CallCenterSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            AppManager.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var launcher = new Form
            {
                Text = "Call Center Launcher",
                ClientSize = new Size(360, 160),
                StartPosition = FormStartPosition.CenterScreen
            };

            var btnStudent = new Button { Text = "Open Student Console", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(300, 30) };
            btnStudent.Click += (s, e) => { var f = new StudentForm(); f.Show(); };

            var btnTechnician = new Button { Text = "Open Technician Console", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(300, 30) };
            btnTechnician.Click += (s, e) => { var f = new TechnicianForm(); f.Show(); };

            var btnManager = new Button { Text = "Open Manager Dashboard", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(300, 30) };
            btnManager.Click += (s, e) => { var f = new ManagerForm(); f.Show(); };

            launcher.Controls.AddRange(new Control[] { btnStudent, btnTechnician, btnManager });

            Application.Run(launcher);
        }
    }
}
