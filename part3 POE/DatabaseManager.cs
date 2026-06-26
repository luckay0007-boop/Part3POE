using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace MyPOE
{
    public class UserTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class DatabaseManager
    {

        private string _server = @"(localdb)\MSSQLLocalDB";
        private string _database = "cyberbotDatabase";

        private string GetConnectionString(bool targetMaster = false)
        {
            string db = targetMaster ? "master" : _database;
            return $"Server={_server};Database={db};Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public DatabaseManager()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {

                using (var conn = new SqlConnection(GetConnectionString(true)))
                {
                    conn.Open();
                    string checkDbQuery = $"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{_database}') CREATE DATABASE [{_database}]";
                    using (var cmd = new SqlCommand(checkDbQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }


                using (var conn = new SqlConnection(GetConnectionString(false)))
                {
                    conn.Open();
                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' and xtype='U')
                        BEGIN
                            CREATE TABLE Tasks (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Title NVARCHAR(255) NOT NULL,
                                Description NVARCHAR(MAX),
                                ReminderDate DATETIME NULL,
                                IsCompleted BIT DEFAULT 0
                            )
                        END";
                    using (var cmd = new SqlCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database initialization failed: " + ex.Message);
            }
        }

        public bool AddTask(string title, string description, DateTime? reminderDate = null)
        {
            try
            {
                using (var conn = new SqlConnection(GetConnectionString(false)))
                {
                    conn.Open();
                    string query = "INSERT INTO Tasks (Title, Description, ReminderDate, IsCompleted) VALUES (@title, @desc, @reminder, 0)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@reminder", reminderDate.HasValue ? (object)reminderDate.Value : DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<UserTask> GetTasks()
        {
            var tasks = new List<UserTask>();
            try
            {
                using (var conn = new SqlConnection(GetConnectionString(false)))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, Description, ReminderDate, IsCompleted FROM Tasks ORDER BY Id ASC";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(new UserTask
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                                    ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ReminderDate")),
                                    IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted"))
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return tasks;
        }

        public bool CompleteTask(int id)
        {
            try
            {
                using (var conn = new SqlConnection(GetConnectionString(false)))
                {
                    conn.Open();
                    string query = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTask(int id)
        {
            try
            {
                using (var conn = new SqlConnection(GetConnectionString(false)))
                {
                    conn.Open();
                    string query = "DELETE FROM Tasks WHERE Id = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
