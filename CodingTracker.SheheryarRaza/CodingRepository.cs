using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.SheheryarRaza
{
    public class CodingRepository
    {
        private readonly string _connectionString;

        public CodingRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public int InsertSession(CodingSession session)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                    INSERT INTO CodingSessions (StartTime, EndTime, Duration)
                    VALUES (@StartTime, @EndTime, @Duration);
                    SELECT last_insert_rowid();";

                int id = connection.ExecuteScalar<int>(sql, new
                {
                    session.StartTime,
                    session.EndTime,
                    Duration = session.Duration.ToString("hh\\:mm\\:ss")
                });
                return id;
            }
        }

        public List<CodingSession> GetAllSessions()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, StartTime, EndTime, Duration FROM CodingSessions ORDER BY StartTime DESC;";

                var sessions = connection.Query<CodingSession>(sql).ToList();

                foreach (var session in sessions)
                {
                    session.Duration = TimeSpan.Parse(session.Duration.ToString());
                }
                return sessions;
            }
        }

        public bool UpdateSession(CodingSession session)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
                    UPDATE CodingSessions
                    SET StartTime = @StartTime,
                        EndTime = @EndTime,
                        Duration = @Duration
                    WHERE Id = @Id;";

                int rowsAffected = connection.Execute(sql, new
                {
                    session.StartTime,
                    session.EndTime,
                    Duration = session.Duration.ToString("hh\\:mm\\:ss"),
                    session.Id
                });
                return rowsAffected > 0;
            }
        }

        public bool DeleteSession(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM CodingSessions WHERE Id = @Id;";
                int rowsAffected = connection.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public CodingSession GetSessionById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, StartTime, EndTime, Duration FROM CodingSessions WHERE Id = @Id;";
                var session = connection.QuerySingleOrDefault<CodingSession>(sql, new { Id = id });

                if (session != null)
                {
                    session.Duration = TimeSpan.Parse(session.Duration.ToString());
                }
                return session;
            }
        }
    }
}
