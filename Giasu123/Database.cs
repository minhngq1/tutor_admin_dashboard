using System;
using System.Data;
using Npgsql;

namespace Giasu123
{
    public static class Database
    {
        private const string ConnString =
            "Host=localhost;Port=5432;Database=legal_text2sql;Username=postgres;Password=123";

        /// <summary>Open a fresh connection to the database</summary>
        private static NpgsqlConnection Open()
        {
            var conn = new NpgsqlConnection(ConnString);
            conn.Open();
            return conn;
        }

        /// <summary>Run a SELECT and return the rows as a DataTable</summary>
        public static DataTable Query(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            using var adapter = new NpgsqlDataAdapter(cmd);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        /// <summary>Run an INSERT, UPDATE or DELETE</summary>
        public static int Execute(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Run a query that returns a single value</summary>
        public static object? Scalar(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }

        /// <summary>Shortcut to build a query parameter</summary>
        public static NpgsqlParameter P(string name, object? value)
        {
            return new NpgsqlParameter(name, value ?? DBNull.Value);
        }

        /// <summary>Try to connect once at startup</summary>
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
