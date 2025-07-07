using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.SheheryarRaza
{
    public static class DBContext
    {
        private static string _connectionString;

        public static void InitializeDatabase(string connectionString)
        {
            _connectionString = connectionString;

            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("Error: Database connection string is null or empty.");
                return;
            }

            using (var connection = new SqliteConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Execute(@"
                        CREATE TABLE IF NOT EXISTS CodingSessions (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            StartTime TEXT NOT NULL,
                            EndTime TEXT NOT NULL,
                            Duration TEXT NOT NULL
                        );
                    ");
                    Console.WriteLine("Database initialized and 'CodingSessions' table ensured.");
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred during database initialization: {ex.Message}");
                }
            }
        }

        public static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database context has not been initialized. Call InitializeDatabase() first.");
            }
            return _connectionString;
        }
    }
}
