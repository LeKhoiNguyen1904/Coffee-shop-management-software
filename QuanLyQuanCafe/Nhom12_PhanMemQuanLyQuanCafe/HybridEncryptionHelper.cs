using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Helper class cho mã hóa lai (Hybrid Encryption)
    /// Tích hợp với file 17_HYBRID_ENCRYPTION_DEMO.sql
    /// </summary>
    public static class HybridEncryptionHelper
    {
        /// <summary>
        /// Mã hóa file đính kèm hóa đơn
        /// </summary>
        public static int EncryptFile(int mahd, string fileName, string fileData, int rsaKeyId = 2)
        {
            try
            {
                string query = @"DECLARE
                                   v_attach_id NUMBER;
                                 BEGIN
                                   pkg_hybrid.encrypt_file(:mahd, :file_name, :file_data, :rsa_key_id, v_attach_id);
                                   :attachment_id := v_attach_id;
                                 END;";

                var attachIdParam = new OracleParameter("attachment_id", OracleDbType.Int32);
                attachIdParam.Direction = System.Data.ParameterDirection.Output;

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("mahd", mahd),
                    new OracleParameter("file_name", fileName),
                    new OracleParameter("file_data", fileData),
                    new OracleParameter("rsa_key_id", rsaKeyId),
                    attachIdParam);

                if (attachIdParam.Value != null && attachIdParam.Value != DBNull.Value)
                {
                    int attachmentId = Convert.ToInt32(attachIdParam.Value.ToString());
                    System.Diagnostics.Debug.WriteLine($"File encrypted: {fileName}, Attachment ID: {attachmentId}");
                    return attachmentId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi mã hóa file: {ex.Message}");
            }
        }

        /// <summary>
        /// Giải mã file đính kèm
        /// </summary>
        public static string DecryptFile(int attachmentId)
        {
            try
            {
                string query = "SELECT pkg_hybrid.decrypt_file(:attach_id) FROM DUAL";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("attach_id", attachmentId));

                if (result == null || result == DBNull.Value)
                    return null;

                string decrypted = result.ToString();
                
                // Kiểm tra lỗi giải mã
                if (decrypted.StartsWith("[LOI_GIAI_MA"))
                    return null;

                return decrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách file đính kèm
        /// </summary>
        public static DataTable GetAttachments()
        {
            try
            {
                string query = "SELECT * FROM v_hoadon_attachments ORDER BY CREATED_AT DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting attachments: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy file đính kèm của hóa đơn
        /// </summary>
        public static DataTable GetInvoiceAttachments(int mahd)
        {
            try
            {
                string query = @"SELECT * FROM v_hoadon_attachments 
                                WHERE MAHD = :mahd 
                                ORDER BY CREATED_AT DESC";
                return DatabaseHelper.ExecuteQuery(query,
                    new OracleParameter("mahd", mahd));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting invoice attachments: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Xóa file đính kèm
        /// </summary>
        public static void DeleteAttachment(int attachmentId)
        {
            try
            {
                string query = "DELETE FROM HOADON_ATTACHMENTS WHERE ATTACHMENT_ID = :attach_id";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("attach_id", attachmentId));

                System.Diagnostics.Debug.WriteLine($"Attachment deleted: {attachmentId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xóa file đính kèm: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra hóa đơn có file đính kèm không
        /// </summary>
        public static bool HasAttachments(int mahd)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM HOADON_ATTACHMENTS WHERE MAHD = :mahd";
                object result = DatabaseHelper.ExecuteScalar(query,
                    new OracleParameter("mahd", mahd));

                return result != null && Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Thống kê file đính kèm
        /// </summary>
        public static DataTable GetAttachmentStatistics()
        {
            try
            {
                string query = @"SELECT 
                                   COUNT(*) AS TONG_FILE,
                                   SUM(DBMS_LOB.GETLENGTH(FILE_DATA_ENC)) AS TONG_DUNG_LUONG_BYTE,
                                   ROUND(AVG(DBMS_LOB.GETLENGTH(FILE_DATA_ENC)), 2) AS TB_DUNG_LUONG
                                FROM HOADON_ATTACHMENTS";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting attachment statistics: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Demo mã hóa lai
        /// </summary>
        public static void RunHybridEncryptionDemo()
        {
            try
            {
                string query = "BEGIN demo_hybrid_encryption; END;";
                DatabaseHelper.ExecuteNonQuery(query);
                System.Diagnostics.Debug.WriteLine("Hybrid encryption demo completed");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi chạy demo: {ex.Message}");
            }
        }
    }
}
