using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Xin chào: {SessionManager.CurrentHoTen} ({SessionManager.CurrentRole})";
            
            // Ẩn menu admin nếu không phải admin
            if (!SessionManager.IsAdmin())
            {
                btnQuanLyTaiKhoan.Visible = false;
                btnLichSuDangNhap.Visible = false;
            }
        }

        private void btnQuanLyBanHang_Click(object sender, EventArgs e)
        {
            FormBanHang frm = new FormBanHang();
            frm.ShowDialog();
        }

        private void btnQuanLyMenu_Click(object sender, EventArgs e)
        {
            FormQuanLyMenu frm = new FormQuanLyMenu();
            frm.ShowDialog();
        }

        private void btnQuanLyKho_Click(object sender, EventArgs e)
        {
            FormQuanLyKho frm = new FormQuanLyKho();
            frm.ShowDialog();
        }

        private void btnQuanLyTaiKhoan_Click(object sender, EventArgs e)
        {
            FormQuanLyTaiKhoan frm = new FormQuanLyTaiKhoan();
            frm.ShowDialog();
        }

        private void btnLichSuDangNhap_Click(object sender, EventArgs e)
        {
            FormLichSuDangNhap frm = new FormLichSuDangNhap();
            frm.ShowDialog();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            FormThongKe frm = new FormThongKe();
            frm.ShowDialog();
        }

        private void btnAuditLog_Click(object sender, EventArgs e)
        {
            FormAuditLog frm = new FormAuditLog();
            frm.ShowDialog();
        }

        private void btnOLS_Click(object sender, EventArgs e)
        {
            FormOLS frm = new FormOLS();
            frm.ShowDialog();
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            try
            {
                // Đóng session trong database
                try
                {
                    DatabaseSessionManager.CloseSession();
                }
                catch { }

                // Xóa VPD context
                try
                {
                    VPDContextManager.ClearContext();
                }
                catch { }

                // Gọi procedure đăng xuất (optional - if doesn't exist, still logout)
                try
                {
                    DatabaseHelper.ExecuteNonQuery("BEGIN dang_xuat(:username, :ip); END;",
                        new OracleParameter("username", SessionManager.CurrentUsername),
                        new OracleParameter("ip", "127.0.0.1"));
                }
                catch (Exception logoutEx)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: dang_xuat procedure call failed: " + logoutEx.Message);
                    // Continue even if procedure doesn't exist
                }

                SessionManager.ClearSession();
                MessageBox.Show("Đăng xuất thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng xuất: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SessionManager.IsLoggedIn())
            {
                DialogResult result = MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Try to logout, but don't block if procedure doesn't exist
                        try
                        {
                            DatabaseHelper.ExecuteNonQuery("BEGIN dang_xuat(:username, :ip); END;",
                                new OracleParameter("username", SessionManager.CurrentUsername),
                                new OracleParameter("ip", "127.0.0.1"));
                        }
                        catch { }
                        
                        SessionManager.ClearSession();
                    }
                    catch { }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
