using System;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Quản lý VPD Context để hỗ trợ Row-Level Security
    /// Tích hợp với file 10_CREATE_VPD.sql (sử dụng Global Temporary Table)
    /// </summary>
    public static class VPDContextManager
    {
        /// <summary>
        /// Thiết lập context cho VPD (Virtual Private Database)
        /// </summary>
        public static void SetContext(int manv, string role)
        {
            try
            {
                // Sử dụng pkg_context (với Global Temporary Table)
                string query = @"BEGIN 
                                   pkg_context.set_manv(:manv);
                                   pkg_context.set_role(:role);
                                 END;";

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("manv", manv),
                    new OracleParameter("role", role));

                System.Diagnostics.Debug.WriteLine($"VPD Context set: MANV={manv}, ROLE={role}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting VPD context: {ex.Message}");
                // Không throw exception để không ảnh hưởng đến đăng nhập
            }
        }

        /// <summary>
        /// Thiết lập tên người dùng trong VPD Context
        /// </summary>
        public static void SetUsername(string username)
        {
            try
            {
                string query = "BEGIN pkg_context.set_username(:username); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("username", username));

                System.Diagnostics.Debug.WriteLine($"VPD Username set: {username}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting VPD username: {ex.Message}");
            }
        }

        /// <summary>
        /// Thiết lập địa chỉ IP người dùng trong VPD Context
        /// </summary>
        public static void SetIPAddress(string ipAddress)
        {
            try
            {
                string query = "BEGIN pkg_context.set_ip(:ip_address); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("ip_address", ipAddress));

                System.Diagnostics.Debug.WriteLine($"VPD IP Address set: {ipAddress}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting VPD IP: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy username từ VPD Context
        /// </summary>
        public static string GetUsername()
        {
            try
            {
                string query = "SELECT pkg_context.get_username FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query);
                return result?.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy IP Address từ VPD Context
        /// </summary>
        public static string GetIPAddress()
        {
            try
            {
                string query = "SELECT pkg_context.get_ip FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query);
                return result?.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Thiết lập toàn bộ VPD Context (wrapper function)
        /// </summary>
        public static void SetCompleteContext(int manv, string role, string username, string ipAddress)
        {
            SetContext(manv, role);
            SetUsername(username);
            SetIPAddress(ipAddress);
        }

        /// <summary>
        /// Lấy MANV hiện tại từ context
        /// </summary>
        public static int? GetCurrentManv()
        {
            try
            {
                string query = "SELECT pkg_context.get_manv FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query);
                
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy Role hiện tại từ context
        /// </summary>
        public static string GetCurrentRole()
        {
            try
            {
                string query = "SELECT pkg_context.get_role FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query);
                return result?.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Xóa context (khi đăng xuất)
        /// </summary>
        public static void ClearContext()
        {
            try
            {
                string query = "BEGIN pkg_context.clear_context; END;";
                DatabaseHelper.ExecuteNonQuery(query);
                System.Diagnostics.Debug.WriteLine("VPD Context cleared");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing VPD context: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra xem context đã được thiết lập chưa
        /// </summary>
        public static bool IsContextSet()
        {
            return GetCurrentManv().HasValue && !string.IsNullOrEmpty(GetCurrentRole());
        }

        /// <summary>
        /// Lấy thông tin context hiện tại
        /// </summary>
        public static string GetContextInfo()
        {
            int? manv = GetCurrentManv();
            string role = GetCurrentRole();

            if (manv.HasValue && !string.IsNullOrEmpty(role))
            {
                return $"VPD Context: MANV={manv}, ROLE={role}";
            }
            return "VPD Context: Not set";
        }
    }
}
