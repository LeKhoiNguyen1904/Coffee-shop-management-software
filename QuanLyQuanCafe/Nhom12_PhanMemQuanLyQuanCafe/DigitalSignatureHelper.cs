using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Helper class cho chữ ký số
    /// Tích hợp với file 14_CREATE_ASYMMETRIC_CRYPTO.sql
    /// </summary>
    public static class DigitalSignatureHelper
    {
        /// <summary>
        /// Ký hóa đơn
        /// </summary>
        public static void SignInvoice(int mahd, int keyId = 1)
        {
            try
            {
                string query = "BEGIN ky_hoadon(:mahd, :key_id); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("mahd", mahd),
                    new OracleParameter("key_id", keyId));

                System.Diagnostics.Debug.WriteLine($"Invoice {mahd} signed with key {keyId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi ký hóa đơn: {ex.Message}");
            }
        }

        /// <summary>
        /// Ký phiếu nhập
        /// </summary>
        public static void SignPurchaseOrder(int mapn, int keyId = 1)
        {
            try
            {
                string query = "BEGIN ky_phieunhap(:mapn, :key_id); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("mapn", mapn),
                    new OracleParameter("key_id", keyId));

                System.Diagnostics.Debug.WriteLine($"Purchase order {mapn} signed with key {keyId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi ký phiếu nhập: {ex.Message}");
            }
        }

        /// <summary>
        /// Xác thực chữ ký
        /// </summary>
        public static bool VerifySignature(int signatureId)
        {
            try
            {
                string query = "SELECT pkg_asymmetric.verify_signature(:sig_id) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("sig_id", signatureId));

                if (result == null || result == DBNull.Value)
                    return false;

                return Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying signature: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách chữ ký số
        /// </summary>
        public static DataTable GetSignatures()
        {
            try
            {
                string query = "SELECT * FROM v_digital_signatures ORDER BY SIGNED_AT DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting signatures: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy chữ ký của hóa đơn
        /// </summary>
        public static DataTable GetInvoiceSignatures(int mahd)
        {
            try
            {
                string query = @"SELECT * FROM v_digital_signatures 
                                WHERE TABLE_NAME = 'HOADON' 
                                AND RECORD_ID = :mahd
                                ORDER BY SIGNED_AT DESC";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("mahd", mahd.ToString()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting invoice signatures: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Kiểm tra hóa đơn đã được ký chưa
        /// </summary>
        public static bool IsInvoiceSigned(int mahd)
        {
            try
            {
                string query = @"SELECT COUNT(*) FROM DIGITAL_SIGNATURES 
                                WHERE TABLE_NAME = 'HOADON' 
                                AND RECORD_ID = :mahd";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("mahd", mahd.ToString()));

                return result != null && Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tạo cặp khóa RSA mới
        /// </summary>
        public static int GenerateRSAKeyPair(string keyName, string purpose = "GENERAL")
        {
            try
            {
                string query = @"DECLARE
                                   v_key_id NUMBER;
                                 BEGIN
                                   pkg_asymmetric.generate_rsa_keypair(:key_name, :purpose, v_key_id);
                                   :key_id := v_key_id;
                                 END;";

                var keyIdParam = new OracleParameter("key_id", OracleDbType.Int32);
                keyIdParam.Direction = System.Data.ParameterDirection.Output;

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("key_name", keyName),
                    new OracleParameter("purpose", purpose),
                    keyIdParam);

                if (keyIdParam.Value != null && keyIdParam.Value != DBNull.Value)
                {
                    return Convert.ToInt32(keyIdParam.Value.ToString());
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tạo khóa RSA: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách khóa RSA
        /// </summary>
        public static DataTable GetRSAKeys()
        {
            try
            {
                string query = @"SELECT KEY_ID, KEY_NAME, KEY_SIZE, KEY_PURPOSE, 
                                       TO_CHAR(CREATED_AT, 'YYYY-MM-DD HH24:MI:SS') AS CREATED_AT
                                FROM RSA_KEYS 
                                ORDER BY CREATED_AT DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting RSA keys: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Thống kê chữ ký theo bảng
        /// </summary>
        public static DataTable GetSignatureStatistics()
        {
            try
            {
                string query = @"SELECT 
                                   TABLE_NAME,
                                   COUNT(*) AS SO_CHU_KY,
                                   SUM(CASE WHEN VERIFIED = 'Y' THEN 1 ELSE 0 END) AS DA_XAC_THUC,
                                   SUM(CASE WHEN VERIFIED = 'N' THEN 1 ELSE 0 END) AS CHUA_XAC_THUC
                                FROM DIGITAL_SIGNATURES
                                GROUP BY TABLE_NAME
                                ORDER BY SO_CHU_KY DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting signature statistics: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Vô hiệu hóa khóa RSA
        /// </summary>
        public static void DeactivateKey(int keyId)
        {
            try
            {
                string query = "BEGIN pkg_asymmetric.deactivate_key(:key_id); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("key_id", keyId));

                System.Diagnostics.Debug.WriteLine($"Key {keyId} deactivated");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi vô hiệu hóa khóa: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra khóa có còn hợp lệ không
        /// </summary>
        public static bool IsKeyValid(int keyId)
        {
            try
            {
                string query = "SELECT pkg_asymmetric.is_key_valid(:key_id) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("key_id", keyId));

                if (result == null || result == DBNull.Value)
                    return false;

                return Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking key validity: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra tính toàn vẹn của dữ liệu
        /// </summary>
        public static bool VerifyDataIntegrity(int signatureId)
        {
            try
            {
                string query = "SELECT pkg_asymmetric.verify_data_integrity(:sig_id) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("sig_id", signatureId));

                if (result == null || result == DBNull.Value)
                    return false;

                return Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying data integrity: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lưu khóa vào Key Vault
        /// </summary>
        public static void StoreKeyInVault(string keyName, string keyType, string keyValue)
        {
            try
            {
                string query = @"BEGIN 
                                   pkg_asymmetric.store_key_in_vault(
                                     :key_name, 
                                     :key_type, 
                                     :key_value
                                   ); 
                                 END;";

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("key_name", keyName),
                    new OracleParameter("key_type", keyType),
                    new OracleParameter("key_value", keyValue));

                System.Diagnostics.Debug.WriteLine($"Key {keyName} stored in vault");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lưu khóa: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy khóa từ Key Vault
        /// </summary>
        public static string RetrieveKeyFromVault(string keyName)
        {
            try
            {
                string query = "SELECT pkg_asymmetric.retrieve_key_from_vault(:key_name) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("key_name", keyName));

                return result?.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving key from vault: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tạo hash cho dữ liệu
        /// </summary>
        public static string GenerateHash(string data, string algorithm = "SHA256")
        {
            try
            {
                // Chuyển CLOB data sang Oracle function
                string query = "SELECT pkg_asymmetric.generate_hash(:data, :algorithm) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("data", data),
                    new OracleParameter("algorithm", algorithm));

                return result?.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating hash: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Xác thực async chữ ký (ghi log xác thực)
        /// </summary>
        public static void VerifySignatureAsync(int signatureId, string verifier)
        {
            try
            {
                string query = "BEGIN pkg_asymmetric.verify_signature_async(:sig_id, :verifier); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("sig_id", signatureId),
                    new OracleParameter("verifier", verifier));

                System.Diagnostics.Debug.WriteLine($"Async verification logged for signature {signatureId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying signature async: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của chữ ký
        /// </summary>
        public static DataTable GetSignatureDetails(int signatureId)
        {
            try
            {
                string query = @"SELECT 
                                   SIGNATURE_ID,
                                   TABLE_NAME,
                                   RECORD_ID,
                                   SIGNED_BY,
                                   TO_CHAR(SIGNED_AT, 'YYYY-MM-DD HH24:MI:SS') AS SIGNED_AT,
                                   SIGNATURE_DATA,
                                   VERIFIED
                                FROM DIGITAL_SIGNATURES
                                WHERE SIGNATURE_ID = :sig_id";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("sig_id", signatureId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting signature details: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết khóa RSA
        /// </summary>
        public static DataTable GetKeyDetails(int keyId)
        {
            try
            {
                string query = @"SELECT 
                                   KEY_ID,
                                   KEY_NAME,
                                   KEY_SIZE,
                                   KEY_PURPOSE,
                                   TO_CHAR(CREATED_AT, 'YYYY-MM-DD HH24:MI:SS') AS CREATED_AT,
                                   IS_ACTIVE
                                FROM RSA_KEYS
                                WHERE KEY_ID = :key_id";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("key_id", keyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting key details: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy khóa hoạt động
        /// </summary>
        public static DataTable GetActiveKeys()
        {
            try
            {
                string query = @"SELECT KEY_ID, KEY_NAME, KEY_SIZE, KEY_PURPOSE,
                                       TO_CHAR(CREATED_AT, 'YYYY-MM-DD HH24:MI:SS') AS CREATED_AT
                                FROM RSA_KEYS 
                                WHERE IS_ACTIVE = 'Y'
                                ORDER BY CREATED_AT DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting active keys: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Đếm số chữ ký được xác thực
        /// </summary>
        public static int GetVerifiedSignatureCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM DIGITAL_SIGNATURES WHERE VERIFIED = 'Y'";
                object result = DatabaseHelper.ExecuteScalar(query);
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting verified signature count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Đếm số chữ ký chưa được xác thực
        /// </summary>
        public static int GetUnverifiedSignatureCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM DIGITAL_SIGNATURES WHERE VERIFIED = 'N'";
                object result = DatabaseHelper.ExecuteScalar(query);
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting unverified signature count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Lấy khóa mặc định cho ký tài liệu
        /// </summary>
        public static int GetDefaultSigningKey()
        {
            try
            {
                // Lấy khóa hoạt động đầu tiên (thường là khóa mặc định)
                string query = "SELECT KEY_ID FROM RSA_KEYS WHERE IS_ACTIVE = 'Y' ORDER BY CREATED_AT LIMIT 1";
                object result = DatabaseHelper.ExecuteScalar(query);
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return 1; // Fallback to key 1
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting default signing key: {ex.Message}");
                return 1;
            }
        }
    }
}
