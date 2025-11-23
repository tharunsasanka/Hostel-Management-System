using System;
using System.Data.SQLite;
using System.Windows.Forms;
using HostelManagementSystem.Classes;

namespace HostelManagementSystem
{
    public partial class EditStudentForm : Form
    {
        private int studentId;

        // 👇 Constructor that takes studentId
        public EditStudentForm(int studentId)
        {
            InitializeComponent();
            this.studentId = studentId;
            LoadStudentData();
        }

        // 👇 Load data from database into textboxes
        private void LoadStudentData()
        {
            try
            {
                using (var connection = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Students WHERE StudentID = @id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", studentId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader["Name"].ToString();
                                txtNIC.Text = reader["NIC"].ToString();
                                txtBatch.Text = reader["Batch"].ToString();
                                txtRoomNo.Text = reader["RoomNo"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // 👇 Save updated data back to database
        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string nic = txtNIC.Text.Trim();
            string batch = txtBatch.Text.Trim();
            string roomNo = txtRoomNo.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(nic))
            {
                MessageBox.Show("Name and NIC are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Students 
                                     SET Name = @name, NIC = @nic, Batch = @batch, RoomNo = @roomNo 
                                     WHERE StudentID = @id";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@nic", nic);
                        command.Parameters.AddWithValue("@batch", batch);
                        command.Parameters.AddWithValue("@roomNo", roomNo);
                        command.Parameters.AddWithValue("@id", studentId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Student updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK; // Helps parent form know it was successful
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("No changes were made.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 👇 Close without saving
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}