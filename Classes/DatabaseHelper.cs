using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace HostelManagementSystem.Classes
{
    public class DatabaseHelper
    {
        // Path to the database file
        private static string dbPath = Path.Combine(Application.StartupPath, "Database", "hostel.db");
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static string ConnectionString => connectionString;

        // This method creates the database and table if they don't exist
        public static void InitializeDatabase()
        {
            // Create Database folder if not exists
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            // If database file doesn't exist, create it and the table
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);

                string createTableQuery = @"
                    CREATE TABLE Students (
                        StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        NIC TEXT UNIQUE NOT NULL,
                        Batch TEXT,
                        RoomNo TEXT
                    );";

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}