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
    public partial class AddStudentForm : Form
    {
        public AddStudentForm()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Step 1: Get data from textboxes
            string name = txtName.Text.Trim();
            string nic = txtNIC.Text.Trim();
            string batch = txtBatch.Text.Trim();
            string roomNo = txtRoomNo.Text.Trim();

            // Step 2: Validate input
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(nic))
            {
                MessageBox.Show("Name and NIC are required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Step 3: Create connection and insert into database
            try
            {
                using (var connection = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Students (Name, NIC, Batch, RoomNo) 
                         VALUES (@name, @nic, @batch, @roomNo)";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        // Use parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@nic", nic);
                        command.Parameters.AddWithValue("@batch", batch);
                        command.Parameters.AddWithValue("@roomNo", roomNo);

                        command.ExecuteNonQuery(); // Executes the INSERT
                    }
                }

                // Success!
                MessageBox.Show("Student added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ✅ Close the form after successful save
                this.DialogResult = DialogResult.OK;
                this.Close(); // This closes the form
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving student: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes the Add Student form
        }
    }
}
