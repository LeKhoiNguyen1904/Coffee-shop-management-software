using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormDangNhap : Form
    {
        public FormDangNhap()
        {
            InitializeComponent();
        }

        private void FormDangNhap_Load(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '●';
        }

        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string ipAddress = "127.0.0.1"; // Local machine
                // 🔒 Sử dụng dang_nhap_v2() - tự động set VPD context
                string query = "SELECT dang_nhap_v2(:username, :password, :ip) FROM dual";
                
                OracleParameter[] parameters = {
                    new OracleParameter("username", username),
                    new OracleParameter("password", password),
                    new OracleParameter("ip", ipAddress)
                };

                object result = DatabaseHelper.ExecuteScalar(query, parameters);
                string resultStr = result?.ToString();

                if (resultStr != null && resultStr.StartsWith("OK:"))
                {
                    string role = resultStr.Substring(3).Split(':')[0]; // Lấy role từ "OK:ROLE:SESSION_X"

                    // Lấy thông tin nhân viên
                    string queryNV = @"SELECT t.MANV, n.HOTEN 
                                      FROM TAIKHOAN t 
                                      JOIN NHANVIEN n ON t.MANV = n.MANV 
                                      WHERE t.USERNAME = :username";
                    DataTable dt = DatabaseHelper.ExecuteQuery(queryNV, new OracleParameter("username", username));

                    if (dt.Rows.Count > 0)
                    {
                        int manv = Convert.ToInt32(dt.Rows[0]["MANV"]);
                        string hoten = dt.Rows[0]["HOTEN"].ToString();

                        // Khởi tạo session
                        SessionManager.InitializeSession(username, manv, role, hoten);

                        // Thiết lập VPD Context đầy đủ (include username và IP)
                        try
                        {
                            VPDContextManager.SetCompleteContext(manv, role, username, ipAddress);
                        }
                        catch (Exception vpdEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"VPD Context error: {vpdEx.Message}");
                        }

                        // Đăng ký session trong database
                        try
                        {
                            DatabaseSessionManager.RegisterSession(username, manv);
                        }
                        catch (Exception sessEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Session registration error: {sessEx.Message}");
                        }

                        MessageBox.Show($"Đăng nhập thành công!\nXin chào {hoten}", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();
                        FormMain mainForm = new FormMain();
                        mainForm.ShowDialog();
                        this.Close();
                    }
                }
                else if (resultStr == "LOCKED")
                {
                    MessageBox.Show("Tài khoản đang bị khóa do đăng nhập sai quá nhiều lần!", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (resultStr == "NOUSER")
                {
                    MessageBox.Show("Tài khoản không tồn tại!", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Sai mật khẩu!", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng nhập: " + ex.Message, 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
