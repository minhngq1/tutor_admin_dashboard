using System;
using System.Data;
using Npgsql;

namespace Giasu123
{
    /// <summary>
    /// Database = the ONLY file that talks to PostgreSQL.
    ///
    /// Every screen in the app calls these few methods instead of writing its own
    /// connection code. Keeping all database access in one place means:
    ///   - the connection details (password etc.) are written only once
    ///   - every query is safe from "SQL injection" because we use parameters
    ///   - if you ever change database, you only edit this one file.
    /// </summary>
    public static class Database
    {
        // The "connection string" tells Npgsql how to find and log into the database.
        //   Host     = the computer running PostgreSQL (localhost = this PC)
        //   Port     = 5432 is PostgreSQL's default port
        //   Database = the database name you gave me
        //   Username = the PostgreSQL login (default admin user is "postgres")
        //   Password = the password you gave me
        private const string ConnString =
            "Host=localhost;Port=5432;Database=legal_text2sql;Username=postgres;Password=123";

        /// <summary>Open a fresh connection to the database.</summary>
        private static NpgsqlConnection Open()
        {
            var conn = new NpgsqlConnection(ConnString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Run a SELECT and return the rows as a DataTable.
        /// A DataTable is like an in-memory spreadsheet — perfect for showing in a grid.
        /// </summary>
        public static DataTable Query(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = Open();                        // "using" = auto-close when done
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            using var adapter = new NpgsqlDataAdapter(cmd);
            var table = new DataTable();
            adapter.Fill(table);                            // copy all result rows into the table
            return table;
        }

        /// <summary>
        /// Run an INSERT, UPDATE or DELETE. Returns how many rows were changed.
        /// </summary>
        public static int Execute(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Run a query that returns a single value, e.g. SELECT COUNT(*) FROM tutors.
        /// </summary>
        public static object? Scalar(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Shortcut to build a query parameter. Using parameters (instead of gluing
        /// text into the SQL) is what keeps the app safe and handles quotes/nulls for you.
        /// Example:  Database.Execute("DELETE FROM tutors WHERE tutor_id=@id", Database.P("id", 5));
        /// </summary>
        public static NpgsqlParameter P(string name, object? value)
        {
            return new NpgsqlParameter(name, value ?? DBNull.Value);
        }

        /// <summary>
        /// Try to connect once at startup. Returns null if all good,
        /// or an error message we can show the user if something is wrong.
        /// </summary>
        public static string? TestConnection()
        {
            try
            {
                using var conn = Open();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
