using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public class DatabaseHelper : IDisposable
    {
        private static string connectionString =
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SID=orcl)));" + //orcl2
            "User Id=C##CAFE_APP;Password=Cafe@123;" +
            "Pooling=true;Min Pool Size=1;Max Pool Size=20;Incr Pool Size=5;Decr Pool Size=2;";

        private static OracleConnection _connection;
        private static readonly object _lockObject = new object();
        private bool _disposed = false;

        public static OracleConnection Connection
        {
            get
            {
                lock (_lockObject)
                {
                    if (_connection == null || _connection.State != ConnectionState.Open)
                    {
                        _connection = new OracleConnection(connectionString);
                        _connection.Open();
                    }
                    return _connection;
                }
            }
        }

        public static string GetConnectionString()
        {
            lock (_lockObject)
            {
                return connectionString;
            }
        }

        public static void SetConnectionString(string connStr)
        {
            lock (_lockObject)
            {
                connectionString = connStr;
                if (_connection != null)
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        public static void CloseConnection()
        {
            lock (_lockObject)
            {
                if (_connection != null)
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        public static OracleConnection GetNewConnection()
        {
            var conn = new OracleConnection(connectionString);
            conn.Open();
            return conn;
        }

        // Basic query execution without timeout parameter
        public static DataTable ExecuteQuery(string query, params OracleParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleCommand cmd = new OracleCommand(query, Connection))
                {
                    cmd.CommandTimeout = 30;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (OracleException oraEx)
            {
                throw new Exception($"Lỗi Oracle [{oraEx.Number}]: {oraEx.Message}", oraEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi truy vấn: " + ex.Message, ex);
            }
            return dt;
        }

        // Query execution with explicit timeout
        public static DataTable ExecuteQueryWithTimeout(string query, int timeout, params OracleParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleCommand cmd = new OracleCommand(query, Connection))
                {
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (OracleException oraEx)
            {
                throw new Exception($"Lỗi Oracle [{oraEx.Number}]: {oraEx.Message}", oraEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi truy vấn: " + ex.Message, ex);
            }
            return dt;
        }

        public static async Task<DataTable> ExecuteQueryAsync(string query, params OracleParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (var conn = GetNewConnection())
            {
                try
                {
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.CommandTimeout = 30;
                        cmd.CommandType = CommandType.Text;

                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            await Task.Run(() => adapter.Fill(dt));
                        }
                    }
                }
                catch (OracleException oraEx)
                {
                    throw new Exception($"Lỗi Oracle [{oraEx.Number}]: {oraEx.Message}", oraEx);
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi truy vấn: " + ex.Message, ex);
                }
            }
            return dt;
        }

        // Non-query execution without transaction
        public static int ExecuteNonQuery(string query, params OracleParameter[] parameters)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand(query, Connection))
                {
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException oraEx)
            {
                throw new Exception($"Lỗi Oracle [{oraEx.Number}]: {oraEx.Message}", oraEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thực thi: " + ex.Message, ex);
            }
        }

        // Non-query execution with transaction
        public static int ExecuteNonQueryWithTransaction(string query, OracleTransaction transaction, params OracleParameter[] parameters)
        {
            OracleConnection conn = transaction?.Connection ?? Connection;
            try
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;

                    cmd.CommandType = CommandType.Text;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException oraEx)
            {
                throw new Exception($"Lỗi Oracle [{oraEx.Number}]: {oraEx.Message}", oraEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thực thi: " + ex.Message, ex);
            }
        }

        public static object ExecuteScalar(string query, params OracleParameter[] parameters)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand(query, Connection))
                {
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
            catch (OracleException oraEx)
            {
                throw new Exception($"Lỗi Oracle [{oraEx.Number}]: {oraEx.Message}", oraEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thực thi: " + ex.Message, ex);
            }
        }

        public static DataTable ExecuteStoredProcedure(string procedureName, params OracleParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleCommand cmd = new OracleCommand(procedureName, Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (OracleException oraEx)
            {
                throw new Exception($"Lỗi stored procedure [{oraEx.Number}]: {oraEx.Message}", oraEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thực thi stored procedure: " + ex.Message, ex);
            }
            return dt;
        }

        public static OracleTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        public static ConnectionState GetConnectionState()
        {
            lock (_lockObject)
            {
                return _connection?.State ?? ConnectionState.Closed;
            }
        }

        public static OracleParameter[] CreateParameters(Dictionary<string, object> paramDict)
        {
            if (paramDict == null) return null;

            var parameters = new List<OracleParameter>();
            foreach (var param in paramDict)
            {
                parameters.Add(new OracleParameter(param.Key, param.Value ?? DBNull.Value));
            }
            return parameters.ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseConnection();
                }
                _disposed = true;
            }
        }

        ~DatabaseHelper()
        {
            Dispose(false);
        }
    }
}