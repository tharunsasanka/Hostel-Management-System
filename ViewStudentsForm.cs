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
    public partial class ViewStudentsForm : Form
    {
        public ViewStudentsForm()
        {
            InitializeComponent();

            // Define columns for DataGridView
            dgvStudents.Columns.Add("ID", "Student ID");
            dgvStudents.Columns.Add("Name", "Name");
            dgvStudents.Columns.Add("NIC", "NIC");
            dgvStudents.Columns.Add("Batch", "Batch");
            dgvStudents.Columns.Add("RoomNo", "Room No");

            // Optional: Set column widths
            dgvStudents.Columns["ID"].Width = 50;
            dgvStudents.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvStudents.Columns["NIC"].Width = 150;
            dgvStudents.Columns["Batch"].Width = 100;
            dgvStudents.Columns["RoomNo"].Width = 100;

            // Load data when form loads
            LoadStudents();
        }
        private void LoadStudents()
        {
            try
            {
                using (var connection = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Students";
                    using (var command = new SQLiteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        dgvStudents.Rows.Clear(); // Clear existing rows
                        while (reader.Read())
                        {
                            dgvStudents.Rows.Add(
                                reader["StudentID"],
                                reader["Name"],
                                reader["NIC"],
                                reader["Batch"],
                                reader["RoomNo"]
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                try
                {
                    using (var connection = new SQLiteConnection(DatabaseHelper.ConnectionString))
                    {
                        connection.Open();
                        string query = "SELECT * FROM Students WHERE Name LIKE @search OR NIC LIKE @search OR Batch LIKE @search OR RoomNo LIKE @search";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@search", $"%{searchQuery}%");
                            using (var reader = command.ExecuteReader())
                            {
                                dgvStudents.Rows.Clear();
                                while (reader.Read())
                                {
                                    dgvStudents.Rows.Add(
                                        reader["StudentID"],
                                        reader["Name"],
                                        reader["NIC"],
                                        reader["Batch"],
                                        reader["RoomNo"]
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error searching students: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a search term.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddStudentForm addForm = new AddStudentForm();
            addForm.ShowDialog();
            LoadStudents(); // Refresh grid after adding
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                int studentId = Convert.ToInt32(dgvStudents.SelectedRows[0].Cells["ID"].Value);
                EditStudentForm editForm = new EditStudentForm(studentId);
                editForm.ShowDialog();
                LoadStudents(); // Refresh grid after editing
            }
            else
            {
                MessageBox.Show("Please select a student to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                int studentId = Convert.ToInt32(dgvStudents.SelectedRows[0].Cells["ID"].Value);
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this student?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (var connection = new SQLiteConnection(DatabaseHelper.ConnectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM Students WHERE StudentID = @id";
                            using (var command = new SQLiteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@id", studentId);
                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Student deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadStudents(); // Refresh grid after deletion
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting student: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a student to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStudents();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Closes the entire app
        }
    }
}
