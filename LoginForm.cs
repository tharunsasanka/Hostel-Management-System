using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HostelManagementSystem.Classes;

namespace HostelManagementSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            txtPass.PasswordChar = '●';
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
           
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();
            

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @user AND Password = @pass";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user", username);
                        command.Parameters.AddWithValue("@pass", password);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Login successful!", "Welcome",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.Hide(); // Hide login form
                            ViewStudentsForm viewForm = new ViewStudentsForm();
                            viewForm.ShowDialog(); // Show dashboard

                            // After logout, return to main form
                            this.Close(); // Close login form permanently
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Login Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
