using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Helper class cho Oracle Label Security (OLS) Simulation
    /// Tích hợp với file 11_CREATE_OLS.sql
    /// Sử dụng SECURITY_LEVEL column và VIEW để mô phỏng OLS
    /// </summary>
    public static class OLSHelper
    {
        /// <summary>
        /// Các mức độ bảo mật
        /// </summary>
        public enum SecurityLevel
        {
            PUBLIC,        // Công khai - Tất cả đều xem được
            INTERNAL,      // Nội bộ - Chỉ nhân viên và admin
            CONFIDENTIAL   // Mật - Chỉ admin
        }

        /// <summary>
        /// Lấy danh sách nguyên liệu theo role hiện tại
        /// Tự động chọn VIEW phù hợp dựa trên role
        /// </summary>
        public static DataTable GetNguyenLieuByRole()
        {
            try
            {
                // Lấy role hiện tại từ VPD Context
                string currentRole = VPDContextManager.GetCurrentRole();

                string viewName;
                if (currentRole == "ADMIN")
                {
                    viewName = "v_nguyenlieu_admin";
                }
                else if (currentRole == "NHANVIEN")
                {
                    viewName = "v_nguyenlieu_nhanvien";
                }
                else
                {
                    viewName = "v_nguyenlieu_public";
                }

                string query = $"SELECT * FROM {viewName} ORDER BY MANL";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting nguyen lieu by role: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy danh sách nguyên liệu sử dụng function Oracle
        /// </summary>
        public static DataTable GetNguyenLieuByRoleFunction()
        {
            try
            {
                string query = @"SELECT * FROM TABLE(
                                   CAST(get_nguyenlieu_by_role() AS SYS_REFCURSOR)
                                 )";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calling get_nguyenlieu_by_role: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Thêm nguyên liệu với mức độ bảo mật
        /// </summary>
        public static void ThemNguyenLieuSecure(
            string tennl, 
            string donvi, 
            decimal soluongton,
            SecurityLevel securityLevel = SecurityLevel.PUBLIC,
            int? manccChinh = null)
        {
            try
            {
                string query = @"BEGIN 
                                   them_nguyenlieu_secure(
                                     :tennl, 
                                     :donvi, 
                                     :soluongton, 
                                     :security_level, 
                                     :mancc_chinh
                                   ); 
                                 END;";

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("tennl", tennl),
                    new OracleParameter("donvi", donvi),
                    new OracleParameter("soluongton", soluongton),
                    new OracleParameter("security_level", securityLevel.ToString()),
                    new OracleParameter("mancc_chinh", 
                        manccChinh.HasValue ? (object)manccChinh.Value : DBNull.Value));

                System.Diagnostics.Debug.WriteLine(
                    $"Added nguyen lieu: {tennl} with security level: {securityLevel}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi thêm nguyên liệu: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra quyền truy cập theo mức độ bảo mật
        /// </summary>
        public static bool CanAccessSecurityLevel(SecurityLevel level)
        {
            string currentRole = VPDContextManager.GetCurrentRole();

            switch (level)
            {
                case SecurityLevel.PUBLIC:
                    return true; // Tất cả đều truy cập được

                case SecurityLevel.INTERNAL:
                    return currentRole == "ADMIN" || currentRole == "NHANVIEN";

                case SecurityLevel.CONFIDENTIAL:
                    return currentRole == "ADMIN";

                default:
                    return false;
            }
        }

        /// <summary>
        /// Lấy thống kê theo mức độ bảo mật (sử dụng function Oracle)
        /// </summary>
        public static DataTable GetSecurityLevelStatistics()
        {
            try
            {
                // Try using the Oracle function first
                string query = @"SELECT * FROM TABLE(
                                   CAST(get_security_level_statistics() AS SYS_REFCURSOR)
                                 )";
                try
                {
                    return DatabaseHelper.ExecuteQuery(query);
                }
                catch
                {
                    // Fallback to direct SQL query if function doesn't exist
                    string fallbackQuery = @"SELECT 
                                       SECURITY_LEVEL,
                                       COUNT(*) AS SO_LUONG,
                                       SUM(SOLUONGTON) AS TONG_TON_KHO,
                                       CASE SECURITY_LEVEL
                                         WHEN 'PUBLIC' THEN 'Công khai'
                                         WHEN 'INTERNAL' THEN 'Nội bộ'
                                         WHEN 'CONFIDENTIAL' THEN 'Mật'
                                         ELSE 'Không xác định'
                                       END AS MUC_DO_BAO_MAT
                                     FROM NGUYENLIEU
                                     GROUP BY SECURITY_LEVEL
                                     ORDER BY 
                                       CASE SECURITY_LEVEL
                                         WHEN 'PUBLIC' THEN 1
                                         WHEN 'INTERNAL' THEN 2
                                         WHEN 'CONFIDENTIAL' THEN 3
                                       END";
                    return DatabaseHelper.ExecuteQuery(fallbackQuery);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting security statistics: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy danh sách nguyên liệu ADMIN (xem tất cả)
        /// </summary>
        public static DataTable GetNguyenLieuAdmin()
        {
            try
            {
                string query = "SELECT * FROM v_nguyenlieu_admin ORDER BY MANL";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting admin view: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy danh sách nguyên liệu NHANVIEN (PUBLIC + INTERNAL)
        /// </summary>
        public static DataTable GetNguyenLieuNhanVien()
        {
            try
            {
                string query = "SELECT * FROM v_nguyenlieu_nhanvien ORDER BY MANL";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting nhanvien view: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy danh sách nguyên liệu PUBLIC (chỉ công khai)
        /// </summary>
        public static DataTable GetNguyenLieuPublic()
        {
            try
            {
                string query = "SELECT * FROM v_nguyenlieu_public ORDER BY MANL";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting public view: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Cập nhật mức độ bảo mật của nguyên liệu
        /// Chỉ ADMIN mới được phép
        /// </summary>
        public static void UpdateSecurityLevel(int manl, SecurityLevel newLevel)
        {
            try
            {
                // Kiểm tra quyền
                string currentRole = VPDContextManager.GetCurrentRole();
                if (currentRole != "ADMIN")
                {
                    throw new Exception("Chỉ ADMIN mới được cập nhật mức độ bảo mật");
                }

                string query = @"UPDATE NGUYENLIEU 
                                SET SECURITY_LEVEL = :security_level 
                                WHERE MANL = :manl";

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("security_level", newLevel.ToString()),
                    new OracleParameter("manl", manl));

                System.Diagnostics.Debug.WriteLine(
                    $"Updated security level for MANL={manl} to {newLevel}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật mức bảo mật: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy mức độ bảo mật của nguyên liệu
        /// </summary>
        public static SecurityLevel? GetSecurityLevel(int manl)
        {
            try
            {
                string query = "SELECT SECURITY_LEVEL FROM NGUYENLIEU WHERE MANL = :manl";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("manl", manl));

                if (result == null || result == DBNull.Value)
                    return null;

                string levelStr = result.ToString();
                return (SecurityLevel)Enum.Parse(typeof(SecurityLevel), levelStr);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra xem user có thể xem nguyên liệu này không
        /// </summary>
        public static bool CanViewNguyenLieu(int manl)
        {
            SecurityLevel? level = GetSecurityLevel(manl);
            if (!level.HasValue)
                return false;

            return CanAccessSecurityLevel(level.Value);
        }

        /// <summary>
        /// Lấy mô tả mức độ bảo mật
        /// </summary>
        public static string GetSecurityLevelDescription(SecurityLevel level)
        {
            switch (level)
            {
                case SecurityLevel.PUBLIC:
                    return "Công khai - Tất cả đều xem được";
                case SecurityLevel.INTERNAL:
                    return "Nội bộ - Chỉ nhân viên và admin";
                case SecurityLevel.CONFIDENTIAL:
                    return "Mật - Chỉ admin";
                default:
                    return "Không xác định";
            }
        }

        /// <summary>
        /// Đếm số lượng nguyên liệu theo mức bảo mật mà user có thể xem
        /// </summary>
        public static int CountAccessibleNguyenLieu()
        {
            try
            {
                DataTable dt = GetNguyenLieuByRole();
                return dt.Rows.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// <summary>
        /// Lấy danh sách nhân viên theo role hiện tại sử dụng function Oracle
        /// ADMIN: Xem tất cả
        /// QUANLY: Xem tất cả
        /// NHANVIEN/THUKHO: Chỉ xem thông tin của chính mình
        /// </summary>
        public static DataTable GetNhanVienByRole()
        {
            try
            {
                // 🔒 Gọi function get_nhanvien_by_role() từ Oracle
                // Function này tự động áp dụng VPD logic dựa trên role
                string query = @"SELECT * FROM v_nhanvien_admin 
                               WHERE MANV IN (
                                   SELECT MANV FROM NHANVIEN
                               )";
                
                string currentRole = VPDContextManager.GetCurrentRole();
                
                if (currentRole == "QUANLY")
                {
                    query = "SELECT * FROM v_nhanvien_quanly";
                }
                else if (currentRole == "NHANVIEN" || currentRole == "THUKHO")
                {
                    query = "SELECT * FROM v_nhanvien_staff";
                }
                else
                {
                    query = "SELECT * FROM v_nhanvien_admin";
                }
                
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calling get_nhanvien_by_role: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy thông tin OLS hiện tại
        /// </summary>
        public static string GetOLSInfo()
        {
            string role = VPDContextManager.GetCurrentRole();
            int count = CountAccessibleNguyenLieu();

            string accessLevel;
            if (role == "ADMIN")
                accessLevel = "Toàn bộ (PUBLIC + INTERNAL + CONFIDENTIAL)";
            else if (role == "NHANVIEN")
                accessLevel = "Giới hạn (PUBLIC + INTERNAL)";
            else
                accessLevel = "Công khai (PUBLIC only)";

            return $"OLS Access: {accessLevel} - Có thể xem {count} nguyên liệu";
        }
    }
}
