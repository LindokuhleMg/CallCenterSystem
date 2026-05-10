using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CallCenterSystem.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtEmail, txtPassword;
        private Button btnLogin;
        private Label lblEmail, lblPassword;

        // Simulated database
        private List<User> users = new List<User>()
        {
            new User { Email = "student1@example.com", Password = "stud123", Role = UserRole.Student },
            new User { Email = "tech1@example.com", Password = "tech123", Role = UserRole.Technician },
            new User { Email = "manager1@example.com", Password = "admin123", Role = UserRole.Manager }
        };

        public LoginForm()
        {
            Text = "Call Center - Login";
            ClientSize = new Size(400, 250);
            StartPosition = FormStartPosition.CenterScreen;

            lblEmail = new Label() { Text = "Email:", Location = new Point(30, 40), AutoSize = true };
            txtEmail = new TextBox() { Location = new Point(120, 36), Width = 200 };
            lblPassword = new Label() { Text = "Password:", Location = new Point(30, 90), AutoSize = true };
            txtPassword = new TextBox() { Location = new Point(120, 86), Width = 200, UseSystemPasswordChar = true };

            btnLogin = new Button() { Text = "Login", Location = new Point(150, 150), Size = new Size(100, 35) };
            btnLogin.Click += BtnLogin_Click;

            Controls.AddRange(new Control[] { lblEmail, txtEmail, lblPassword, txtPassword, btnLogin });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            User foundUser = users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (foundUser != null)
            {
                Form userForm = null;

                // Open form based on role
                switch (foundUser.Role)
                {
                    case UserRole.Student:
                        userForm = new StudentForm();
                        break;
                    case UserRole.Technician:
                        //userForm = new TechnicianForm();
                        break;
                    case UserRole.Manager:
                        //userForm = new ManagerForm();
                        break;
                }

                if (userForm != null)
                {
                    // When the user closes their form, show login again
                    userForm.FormClosed += (s, args) =>
                    {
                        this.Show(); // Show login again
                        this.BringToFront();
                    };

                    userForm.Show();
                    this.Hide(); // hide login while user form is open
                }
            }
            else
            {
                MessageBox.Show("Invalid email or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public enum UserRole { Student, Technician, Manager }

    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
