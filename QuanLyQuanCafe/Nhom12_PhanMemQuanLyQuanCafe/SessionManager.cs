using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public static class SessionManager
    {
        public static string CurrentUsername { get; set; }
        public static int CurrentMaNV { get; set; }
        public static string CurrentRole { get; set; }
        public static string CurrentHoTen { get; set; }
        public static DateTime LoginTime { get; private set; }
        public static string SessionId { get; private set; }
        public static DateTime LastActivity { get; private set; }

        private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);
        private static readonly Dictionary<string, bool> _permissions = new Dictionary<string, bool>();
        private static readonly Dictionary<string, DateTime> _moduleAccessTime = new Dictionary<string, DateTime>();

        public static void InitializeSession(string username, int maNV, string role, string hoTen)
        {
            CurrentUsername = username ?? throw new ArgumentNullException(nameof(username));
            CurrentMaNV = maNV;
            CurrentRole = role ?? throw new ArgumentNullException(nameof(role));
            CurrentHoTen = hoTen ?? throw new ArgumentNullException(nameof(hoTen));
            LoginTime = DateTime.Now;
            LastActivity = DateTime.Now;
            SessionId = GenerateSecureSessionId();

            LoadPermissions();
            LogSessionActivity("SESSION_START", $"User {username} logged in with role {role}");
        }

        public static void UpdateActivity()
        {
            LastActivity = DateTime.Now;
        }

        public static void UpdateModuleActivity(string moduleName)
        {
            UpdateActivity();
            _moduleAccessTime[moduleName] = DateTime.Now;
        }

        public static bool IsSessionExpired()
        {
            return (DateTime.Now - LastActivity) > SessionTimeout;
        }

        public static bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(CurrentUsername) &&
                   !string.IsNullOrEmpty(CurrentRole) &&
                   CurrentMaNV > 0 &&
                   !IsSessionExpired();
        }

        public static bool IsAdmin()
        {
            return CurrentRole == "ADMIN";
        }

        public static bool IsNhanVien()
        {
            return CurrentRole == "NHANVIEN";
        }

        public static bool HasPermission(string permission)
        {
            if (!IsLoggedIn()) return false;

            UpdateActivity();

            if (_permissions.ContainsKey(permission))
                return _permissions[permission];

            if (IsAdmin())
                return true;

            return false;
        }

        public static bool CanAccessModule(string moduleName)
        {
            if (!IsLoggedIn()) return false;

            UpdateActivity();

            switch (moduleName.ToUpper())
            {
                case "QUANLYNHANVIEN":
                case "QUANLYTAIKHOAN":
                case "BAOCAOTHONGKE":
                case "QUANLYPHANQUYEN":
                case "QUANLYCAIDAT":
                    return IsAdmin();

                case "BANHANG":
                case "QUANLYBAN":
                case "QUANLYMON":
                case "QUANLYKHO":
                case "QUANLYKHACHHANG":
                case "THONGKEBANHANG":
                    return IsAdmin() || IsNhanVien();

                case "DOIMATKHAU":
                case "THONGTINCA NHAN":
                    return true;

                default:
                    return false;
            }
        }

        public static List<string> GetAccessibleModules()
        {
            var modules = new List<string>();

            if (IsAdmin())
            {
                modules.AddRange(new[] {
                    "QUANLYNHANVIEN", "QUANLYTAIKHOAN", "BAOCAOTHONGKE",
                    "QUANLYPHANQUYEN", "QUANLYCAIDAT", "BANHANG",
                    "QUANLYBAN", "QUANLYMON", "QUANLYKHO",
                    "QUANLYKHACHHANG", "THONGKEBANHANG", "DOIMATKHAU"
                });
            }
            else if (IsNhanVien())
            {
                modules.AddRange(new[] {
                    "BANHANG", "QUANLYBAN", "QUANLYMON",
                    "QUANLYKHO", "QUANLYKHACHHANG", "THONGKEBANHANG",
                    "DOIMATKHAU"
                });
            }

            return modules;
        }

        public static void ClearSession()
        {
            if (!string.IsNullOrEmpty(CurrentUsername))
            {
                LogSessionActivity("SESSION_END", $"User {CurrentUsername} logged out");
            }

            if (IsLoggedIn())
            {
                try
                {
                    string query = "BEGIN dang_xuat(:username, :ip); END;";
                    var parameters = new OracleParameter[]
                    {
                        new OracleParameter("username", CurrentUsername),
                        new OracleParameter("ip", GetLocalIPAddress())
                    };
                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");
                }
            }

            CurrentUsername = null;
            CurrentMaNV = 0;
            CurrentRole = null;
            CurrentHoTen = null;
            LoginTime = DateTime.MinValue;
            LastActivity = DateTime.MinValue;
            SessionId = null;
            _permissions.Clear();
            _moduleAccessTime.Clear();
        }

        public static TimeSpan GetSessionDuration()
        {
            return DateTime.Now - LoginTime;
        }

        public static TimeSpan GetInactivityDuration()
        {
            return DateTime.Now - LastActivity;
        }

        public static string GetSessionInfo()
        {
            if (!IsLoggedIn()) return "Chưa đăng nhập";

            return $"User: {CurrentHoTen} ({CurrentUsername}) | Role: {CurrentRole} | " +
                   $"Duration: {GetSessionDuration():hh\\:mm\\:ss} | " +
                   $"Inactive: {GetInactivityDuration():mm\\:ss}";
        }

        public static string GetSessionSummary()
        {
            if (!IsLoggedIn()) return "Chưa đăng nhập";

            return $"{CurrentHoTen} ({CurrentRole}) - Đăng nhập lúc {LoginTime:HH:mm}";
        }

        public static void ForceLogout()
        {
            string username = CurrentUsername;
            ClearSession();
            System.Diagnostics.Debug.WriteLine($"Force logout for user: {username}");
        }

        public static bool CheckAndRenewSession()
        {
            if (IsSessionExpired())
            {
                ForceLogout();
                return false;
            }

            UpdateActivity();
            return true;
        }

        private static void LoadPermissions()
        {
            _permissions.Clear();

            try
            {
                string query = @"SELECT QUYEN FROM PHANQUYEN_CHITIET WHERE ROLE_CODE = :role";
                var dt = DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("role", CurrentRole));

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string quyen = row["QUYEN"]?.ToString();
                    if (!string.IsNullOrEmpty(quyen))
                    {
                        _permissions[quyen] = true;
                    }
                }

                if (_permissions.Count == 0)
                {
                    LoadDefaultPermissions();
                }
            }
            catch
            {
                LoadDefaultPermissions();
            }
        }

        private static void LoadDefaultPermissions()
        {
            if (IsAdmin())
            {
                _permissions["MANAGE_USERS"] = true;
                _permissions["VIEW_REPORTS"] = true;
                _permissions["MANAGE_MENU"] = true;
                _permissions["PROCESS_ORDERS"] = true;
                _permissions["MANAGE_INVENTORY"] = true;
                _permissions["VIEW_FINANCIALS"] = true;
                _permissions["MANAGE_SETTINGS"] = true;
                _permissions["BACKUP_DATA"] = true;
            }
            else if (IsNhanVien())
            {
                _permissions["MANAGE_USERS"] = false;
                _permissions["VIEW_REPORTS"] = false;
                _permissions["MANAGE_MENU"] = true;
                _permissions["PROCESS_ORDERS"] = true;
                _permissions["MANAGE_INVENTORY"] = true;
                _permissions["VIEW_FINANCIALS"] = false;
                _permissions["MANAGE_SETTINGS"] = false;
                _permissions["BACKUP_DATA"] = false;
            }
        }

        private static string GenerateSecureSessionId()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

        private static void LogSessionActivity(string activity, string description = "")
        {
            try
            {
                string query = @"INSERT INTO LICHSU_SESSION (SESSION_ID, USERNAME, HOATDONG, MOTA, THOIGIAN, IP_ADDRESS) 
                               VALUES (:session_id, :username, :hoatdong, :mota, SYSDATE, :ip)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("session_id", SessionId),
                    new OracleParameter("username", CurrentUsername),
                    new OracleParameter("hoatdong", activity),
                    new OracleParameter("mota", description),
                    new OracleParameter("ip", GetLocalIPAddress())
                };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Session log error: {ex.Message}");
            }
        }

        private static string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        public static void LogUserAction(string action, string details = "")
        {
            if (!IsLoggedIn()) return;

            try
            {
                string query = @"INSERT INTO AUDIT_LOG (USERNAME, ACTION_TYPE, TABLE_NAME, RECORD_ID, OLD_VALUE, NEW_VALUE, ACTION_TIME, IP_ADDRESS)
                               VALUES (:username, :action_type, :table_name, :record_id, :old_value, :new_value, SYSDATE, :ip)";

                var parameters = new OracleParameter[]
                {
                    new OracleParameter("username", CurrentUsername),
                    new OracleParameter("action_type", action),
                    new OracleParameter("table_name", DBNull.Value),
                    new OracleParameter("record_id", DBNull.Value),
                    new OracleParameter("old_value", DBNull.Value),
                    new OracleParameter("new_value", details),
                    new OracleParameter("ip", GetLocalIPAddress())
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audit log error: {ex.Message}");
            }
        }
    }
}