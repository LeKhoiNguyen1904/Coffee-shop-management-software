using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormQuanLyTaiKhoan : Form
    {
        public FormQuanLyTaiKhoan()
        {
            InitializeComponent();
        }

        private void FormQuanLyTaiKhoan_Load(object sender, EventArgs e)
        {
            LoadNhanVien();
            LoadTaiKhoan();
            cboRole.Items.AddRange(new string[] { "ADMIN", "NHANVIEN" });
            cboRole.SelectedIndex = 1;
        }

        private void LoadNhanVien()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM NHANVIEN WHERE TRANGTHAI = 'ACTIVE' ORDER BY MANV");
                cboNhanVien.DataSource = dt;
                cboNhanVien.DisplayMember = "HOTEN";
                cboNhanVien.ValueMember = "MANV";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load nhân viên: " + ex.Message);
            }
        }

        private void LoadTaiKhoan()
        {
            try
            {
                string query = @"SELECT t.USERNAME, n.HOTEN, t.ROLE_CODE, t.LOGIN_FAILS, t.LAST_LOGIN_AT 
                                FROM TAIKHOAN t 
                                JOIN NHANVIEN n ON t.MANV = n.MANV 
                                ORDER BY t.USERNAME";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvTaiKhoan.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load tài khoản: " + ex.Message);
            }
        }

        private void btnTaoTaiKhoan_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || cboNhanVien.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int manv = Convert.ToInt32(cboNhanVien.SelectedValue);
                string query = "BEGIN tao_tai_khoan(:username, :manv, :password, :role); END;";
                
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("username", txtUsername.Text),
                    new OracleParameter("manv", manv),
                    new OracleParameter("password", txtPassword.Text),
                    new OracleParameter("role", cboRole.SelectedItem.ToString()));

                MessageBox.Show("Tạo tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTaiKhoan();
                txtUsername.Clear();
                txtPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo tài khoản: " + ex.Message);
            }
        }
    }
}
