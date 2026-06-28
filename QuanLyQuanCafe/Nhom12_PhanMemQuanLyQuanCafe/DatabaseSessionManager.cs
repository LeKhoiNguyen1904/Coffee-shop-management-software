using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Quản lý session với database
    /// Tích hợp với file 15_CREATE_PROFILE_SESSION.sql
    /// </summary>
    public static class DatabaseSessionManager
    {
        private static int _currentSessionId = 0;

        /// <summary>
        /// Đăng ký session mới trong database
        /// </summary>
        public static void RegisterSession(string username, int manv)
        {
            try
            {
                string query = @"DECLARE
                                   v_session_id NUMBER;
                                 BEGIN 
                                   pkg_session.register_session(:username, :manv, v_session_id);
                                   :session_id := v_session_id;
                                 END;";

                var sessionIdParam = new OracleParameter("session_id", OracleDbType.Int32);
                sessionIdParam.Direction = System.Data.ParameterDirection.Output;

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("username", username),
                    new OracleParameter("manv", manv),
                    sessionIdParam);

                if (sessionIdParam.Value != null && sessionIdParam.Value != DBNull.Value)
                {
                    _currentSessionId = Convert.ToInt32(sessionIdParam.Value.ToString());
                    System.Diagnostics.Debug.WriteLine($"Session registered: ID={_currentSessionId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registering session: {ex.Message}");
                // Không throw exception để không ảnh hưởng đến đăng nhập
            }
        }

        /// <summary>
        /// Cập nhật hoạt động của session
        /// </summary>
        public static void UpdateActivity()
        {
            if (_currentSessionId <= 0) return;

            try
            {
                string query = "BEGIN pkg_session.update_activity(:session_id); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("session_id", _currentSessionId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating session activity: {ex.Message}");
            }
        }

        /// <summary>
        /// Đóng session
        /// </summary>
        public static void CloseSession()
        {
            if (_currentSessionId <= 0) return;

            try
            {
                string query = "BEGIN pkg_session.close_session(:session_id); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("session_id", _currentSessionId));

                System.Diagnostics.Debug.WriteLine($"Session closed: ID={_currentSessionId}");
                _currentSessionId = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing session: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách session đang hoạt động
        /// </summary>
        public static DataTable GetActiveSessions()
        {
            try
            {
                string query = @"SELECT SESSION_ID, USERNAME, MANV, 
                                       TO_CHAR(LOGIN_TIME, 'YYYY-MM-DD HH24:MI:SS') AS LOGIN_TIME,
                                       TO_CHAR(LAST_ACTIVITY, 'YYYY-MM-DD HH24:MI:SS') AS LAST_ACTIVITY,
                                       IP_ADDRESS, STATUS
                                FROM SESSION_TRACKING
                                WHERE STATUS = 'ACTIVE'
                                ORDER BY LOGIN_TIME DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting active sessions: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Đếm số session của user
        /// </summary>
        public static int CountUserSessions(string username)
        {
            try
            {
                string query = "SELECT pkg_session.count_user_sessions(:username) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("username", username));

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Lấy thông tin session hiện tại
        /// </summary>
        public static string GetCurrentSessionInfo()
        {
            try
            {
                string query = "BEGIN pkg_session.get_current_session_info; END;";
                DatabaseHelper.ExecuteNonQuery(query);
                return $"Session ID: {_currentSessionId}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Lấy ID session hiện tại
        /// </summary>
        public static int GetCurrentSessionId()
        {
            return _currentSessionId;
        }

        /// <summary>
        /// Kiểm tra session có hợp lệ không
        /// </summary>
        public static bool IsSessionValid()
        {
            return _currentSessionId > 0;
        }
    }
}
