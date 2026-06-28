using System;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Helper class cho mã hóa/giải mã dữ liệu nhạy cảm
    /// Tích hợp với file 16_FIX_ENCRYPTION_IV.sql
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// Mã hóa thông tin nhân viên (CCCD và địa chỉ)
        /// </summary>
        public static void EncryptEmployeeData(int manv, string cccd, string diachi)
        {
            try
            {
                string query = "BEGIN ma_hoa_cccd_nhanvien_v2(:manv, :cccd, :diachi); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("manv", manv),
                    new OracleParameter("cccd", cccd ?? string.Empty),
                    new OracleParameter("diachi", diachi ?? string.Empty));

                System.Diagnostics.Debug.WriteLine($"Encrypted data for employee: {manv}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi mã hóa dữ liệu nhân viên: {ex.Message}");
            }
        }

        /// <summary>
        /// Giải mã CCCD của nhân viên
        /// </summary>
        public static string DecryptCCCD(int manv)
        {
            try
            {
                string query = "SELECT giai_ma_cccd_nhanvien_v2(:manv) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("manv", manv));

                if (result == null || result == DBNull.Value)
                    return null;

                string decrypted = result.ToString();
                
                // Kiểm tra lỗi giải mã
                if (decrypted == "[LOI_GIAI_MA]")
                    return null;

                return decrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting CCCD: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Giải mã địa chỉ của nhân viên
        /// </summary>
        public static string DecryptAddress(int manv)
        {
            try
            {
                string query = "SELECT giai_ma_diachi_nhanvien(:manv) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("manv", manv));

                if (result == null || result == DBNull.Value)
                    return null;

                string decrypted = result.ToString();
                
                if (decrypted == "[LOI_GIAI_MA]")
                    return null;

                return decrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting address: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Mã hóa thông tin khách hàng
        /// </summary>
        public static void EncryptCustomerData(int makh, string cccd, string diachi)
        {
            try
            {
                string query = "BEGIN ma_hoa_thongtin_khachhang_v2(:makh, :cccd, :diachi); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("makh", makh),
                    new OracleParameter("cccd", cccd ?? string.Empty),
                    new OracleParameter("diachi", diachi ?? string.Empty));

                System.Diagnostics.Debug.WriteLine($"Encrypted data for customer: {makh}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi mã hóa dữ liệu khách hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Mã hóa thông tin khách hàng bao gồm email
        /// </summary>
        public static void EncryptCustomerFullData(int makh, string cccd, string diachi, string email)
        {
            try
            {
                string query = @"BEGIN 
                                   ma_hoa_khachhang_v2(:makh, :cccd, :diachi, :email); 
                                 END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("makh", makh),
                    new OracleParameter("cccd", cccd ?? string.Empty),
                    new OracleParameter("diachi", diachi ?? string.Empty),
                    new OracleParameter("email", email ?? string.Empty));

                System.Diagnostics.Debug.WriteLine($"Encrypted full data for customer: {makh}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi mã hóa đầy đủ khách hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Giải mã CCCD của khách hàng
        /// </summary>
        public static string DecryptCustomerCCCD(int makh)
        {
            try
            {
                string query = "SELECT giai_ma_khachhang_v2(:makh, 'CCCD') FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("makh", makh));

                if (result == null || result == DBNull.Value)
                    return null;

                string decrypted = result.ToString();
                
                if (decrypted == "[LOI_GIAI_MA]")
                    return null;

                return decrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting customer CCCD: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Giải mã địa chỉ khách hàng
        /// </summary>
        public static string DecryptCustomerAddress(int makh)
        {
            try
            {
                string query = "SELECT giai_ma_khachhang_v2(:makh, 'DIACHI') FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("makh", makh));

                if (result == null || result == DBNull.Value)
                    return null;

                string decrypted = result.ToString();
                
                if (decrypted == "[LOI_GIAI_MA]")
                    return null;

                return decrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting customer address: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Giải mã email khách hàng
        /// </summary>
        public static string DecryptCustomerEmail(int makh)
        {
            try
            {
                string query = "SELECT giai_ma_khachhang_v2(:makh, 'EMAIL') FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("makh", makh));

                if (result == null || result == DBNull.Value)
                    return null;

                string decrypted = result.ToString();
                
                if (decrypted == "[LOI_GIAI_MA]")
                    return null;

                return decrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting customer email: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Mã hóa thông tin thanh toán hóa đơn
        /// </summary>
        public static void EncryptPaymentInfo(int mahd, string paymentInfo)
        {
            try
            {
                string query = "BEGIN ma_hoa_thongtin_thanhtoan(:mahd, :payment_info); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("mahd", mahd),
                    new OracleParameter("payment_info", paymentInfo ?? string.Empty));

                System.Diagnostics.Debug.WriteLine($"Encrypted payment info for invoice: {mahd}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi mã hóa thông tin thanh toán: {ex.Message}");
            }
        }

        /// <summary>
        /// Giải mã thông tin thanh toán hóa đơn
        /// </summary>
        public static string DecryptPaymentInfo(int mahd)
        {
            try
            {
                string query = @"SELECT giai_ma_thongtin_thanhtoan(:mahd) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("mahd", mahd));

                if (result == null || result == DBNull.Value)
                    return null;

                string decrypted = result.ToString();
                
                if (decrypted == "[LOI_GIAI_MA]")
                    return null;

                return decrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting payment info: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra dữ liệu đã được mã hóa chưa
        /// </summary>
        public static bool IsDataEncrypted(int manv)
        {
            try
            {
                string query = @"SELECT CASE 
                                       WHEN CCCD_ENC IS NOT NULL AND CCCD_IV IS NOT NULL 
                                       THEN 1 ELSE 0 
                                     END AS IS_ENCRYPTED
                                FROM NHANVIEN 
                                WHERE MANV = :manv";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("manv", manv));

                return result != null && Convert.ToInt32(result) == 1;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra tính toàn vẹn của mã hóa
        /// </summary>
        public static void CheckEncryptionIntegrity()
        {
            try
            {
                string query = "BEGIN kiem_tra_ma_hoa; END;";
                DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking encryption integrity: {ex.Message}");
            }
        }
    }
}
