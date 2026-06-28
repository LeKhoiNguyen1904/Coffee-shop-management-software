using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Helper class để quản lý FGA (Fine-Grained Auditing) và Audit Logs
    /// Tích hợp với file 03_SETUP_CRYPTO_PROFILE.sql
    /// </summary>
    public static class AuditHelper
    {
        /// <summary>
        /// Lấy danh sách toàn bộ audit logs
        /// </summary>
        public static DataTable GetAllAuditLogs()
        {
            try
            {
                string query = @"SELECT 
                                   AUDIT_ID,
                                   USERNAME,
                                   TABLE_NAME,
                                   OPERATION,
                                   AUDIT_TIME,
                                   ROW_DESC
                                FROM AUDIT_LOG
                                ORDER BY AUDIT_TIME DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting audit logs: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy audit logs của một bảng cụ thể
        /// </summary>
        public static DataTable GetAuditLogsByTable(string tableName)
        {
            try
            {
                string query = @"SELECT 
                                   AUDIT_ID,
                                   USERNAME,
                                   TABLE_NAME,
                                   OPERATION,
                                   AUDIT_TIME,
                                   ROW_DESC
                                FROM AUDIT_LOG
                                WHERE TABLE_NAME = :table_name
                                ORDER BY AUDIT_TIME DESC";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("table_name", tableName));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting audit logs by table: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy audit logs của một người dùng cụ thể
        /// </summary>
        public static DataTable GetAuditLogsByUser(string username)
        {
            try
            {
                string query = @"SELECT 
                                   AUDIT_ID,
                                   USERNAME,
                                   TABLE_NAME,
                                   OPERATION,
                                   AUDIT_TIME,
                                   ROW_DESC
                                FROM AUDIT_LOG
                                WHERE USERNAME = :username
                                ORDER BY AUDIT_TIME DESC";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("username", username));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting audit logs by user: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy audit logs trong một khoảng thời gian
        /// </summary>
        public static DataTable GetAuditLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                string query = @"SELECT 
                                   AUDIT_ID,
                                   USERNAME,
                                   TABLE_NAME,
                                   OPERATION,
                                   AUDIT_TIME,
                                   ROW_DESC
                                FROM AUDIT_LOG
                                WHERE AUDIT_TIME BETWEEN :start_date AND :end_date
                                ORDER BY AUDIT_TIME DESC";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("start_date", startDate),
                    new OracleParameter("end_date", endDate));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting audit logs by date range: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy lịch sử đăng nhập
        /// </summary>
        public static DataTable GetLoginHistory()
        {
            try
            {
                string query = @"SELECT 
                                   MANV,
                                   USERNAME,
                                   LANHDANGNHAP,
                                   LANHDANGNHAPSAI,
                                   NGAYSUDUNGCUOI,
                                   LOCKED
                                FROM LICHSU_DANGNHAP
                                ORDER BY NGAYSUDUNGCUOI DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting login history: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy lịch sử đăng nhập của một người dùng
        /// </summary>
        public static DataTable GetLoginHistoryByUser(string username)
        {
            try
            {
                string query = @"SELECT 
                                   MANV,
                                   USERNAME,
                                   LANHDANGNHAP,
                                   LANHDANGNHAPSAI,
                                   NGAYSUDUNGCUOI,
                                   LOCKED
                                FROM LICHSU_DANGNHAP
                                WHERE USERNAME = :username
                                ORDER BY NGAYSUDUNGCUOI DESC";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("username", username));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting login history by user: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy thống kê FGA - số lần truy cập nhạy cảm
        /// </summary>
        public static DataTable GetFGAStatistics()
        {
            try
            {
                // Thử gọi view FGA nếu tồn tại
                string query = "SELECT * FROM v_fga_statistics";
                try
                {
                    return DatabaseHelper.ExecuteQuery(query);
                }
                catch
                {
                    // Fallback: Truy vấn trực tiếp AUDIT_LOG để lấy thống kê
                    string fallbackQuery = @"SELECT 
                                       TABLE_NAME,
                                       OPERATION,
                                       COUNT(*) AS ACCESS_COUNT,
                                       MAX(AUDIT_TIME) AS LAST_ACCESS
                                    FROM AUDIT_LOG
                                    GROUP BY TABLE_NAME, OPERATION
                                    ORDER BY ACCESS_COUNT DESC";
                    return DatabaseHelper.ExecuteQuery(fallbackQuery);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting FGA statistics: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy chi tiết FGA audit logs
        /// </summary>
        public static DataTable GetFGADetailedAudit()
        {
            try
            {
                string query = "SELECT * FROM v_fga_audit_detailed ORDER BY AUDIT_TIME DESC";
                try
                {
                    return DatabaseHelper.ExecuteQuery(query);
                }
                catch
                {
                    // Fallback
                    return GetAllAuditLogs();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting FGA detailed audit: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy tổng số audit entries
        /// </summary>
        public static int GetAuditLogCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM AUDIT_LOG";
                object result = DatabaseHelper.ExecuteScalar(query);
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting audit log count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Lấy số lần truy cập của một người dùng
        /// </summary>
        public static int GetUserAccessCount(string username)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM AUDIT_LOG WHERE USERNAME = :username";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("username", username));
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user access count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Lấy thống kê hoạt động theo bảng
        /// </summary>
        public static DataTable GetTableActivityStatistics()
        {
            try
            {
                string query = @"SELECT 
                                   TABLE_NAME,
                                   COUNT(*) AS TOTAL_OPERATIONS,
                                   SUM(CASE WHEN OPERATION = 'INSERT' THEN 1 ELSE 0 END) AS INSERTS,
                                   SUM(CASE WHEN OPERATION = 'UPDATE' THEN 1 ELSE 0 END) AS UPDATES,
                                   SUM(CASE WHEN OPERATION = 'DELETE' THEN 1 ELSE 0 END) AS DELETES
                                FROM AUDIT_LOG
                                GROUP BY TABLE_NAME
                                ORDER BY TOTAL_OPERATIONS DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting table activity statistics: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Kiểm tra xem người dùng bị lock sau khi đăng nhập sai
        /// </summary>
        public static bool IsUserLocked(string username)
        {
            try
            {
                string query = "SELECT LOCKED FROM LICHSU_DANGNHAP WHERE USERNAME = :username";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("username", username));
                if (result != null && result != DBNull.Value)
                {
                    return result.ToString() == "Y";
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking user lock status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy số lần đăng nhập sai của người dùng
        /// </summary>
        public static int GetUserFailedLoginAttempts(string username)
        {
            try
            {
                string query = "SELECT LANHDANGNHAPSAI FROM LICHSU_DANGNHAP WHERE USERNAME = :username";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("username", username));
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting failed login attempts: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Xóa audit logs cũ hơn ngày được chỉ định
        /// </summary>
        public static void DeleteOldAuditLogs(int daysOld)
        {
            try
            {
                string query = @"DELETE FROM AUDIT_LOG 
                                WHERE AUDIT_TIME < SYSDATE - :days_old";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("days_old", daysOld));

                System.Diagnostics.Debug.WriteLine($"Deleted audit logs older than {daysOld} days");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting old audit logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo audit theo người dùng và thời gian
        /// </summary>
        public static DataTable GenerateAuditReport(string username, DateTime startDate, DateTime endDate)
        {
            try
            {
                string query = @"SELECT 
                                   USERNAME,
                                   TABLE_NAME,
                                   OPERATION,
                                   COUNT(*) AS OPERATION_COUNT,
                                   MIN(AUDIT_TIME) AS FIRST_TIME,
                                   MAX(AUDIT_TIME) AS LAST_TIME
                                FROM AUDIT_LOG
                                WHERE USERNAME = :username
                                  AND AUDIT_TIME BETWEEN :start_date AND :end_date
                                GROUP BY USERNAME, TABLE_NAME, OPERATION
                                ORDER BY MAX(AUDIT_TIME) DESC";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("username", username),
                    new OracleParameter("start_date", startDate),
                    new OracleParameter("end_date", endDate));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating audit report: {ex.Message}");
                return new DataTable();
            }
        }
    }
}
