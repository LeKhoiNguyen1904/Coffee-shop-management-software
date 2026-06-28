using System;
using System.Data;
using System.Windows.Forms;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormLichSuDangNhap : Form
    {
        public FormLichSuDangNhap()
        {
            InitializeComponent();
        }

        private void FormLichSuDangNhap_Load(object sender, EventArgs e)
        {
            LoadLichSu();
        }

        private void LoadLichSu()
        {
            try
            {
                string query = @"SELECT l.ID, l.USERNAME, n.HOTEN, l.HOATDONG, l.THOIGIAN, l.IP_ADDRESS, l.GHI_CHU 
                                FROM LICHSU_DANGNHAP l
                                LEFT JOIN TAIKHOAN t ON l.USERNAME = t.USERNAME
                                LEFT JOIN NHANVIEN n ON t.MANV = n.MANV
                                ORDER BY l.THOIGIAN DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvLichSu.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load lịch sử: " + ex.Message);
            }
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"SELECT l.ID, l.USERNAME, n.HOTEN, l.HOATDONG, l.THOIGIAN, l.IP_ADDRESS, l.GHI_CHU 
                                FROM LICHSU_DANGNHAP l
                                LEFT JOIN TAIKHOAN t ON l.USERNAME = t.USERNAME
                                LEFT JOIN NHANVIEN n ON t.MANV = n.MANV
                                WHERE l.THOIGIAN BETWEEN :tungay AND :denngay
                                ORDER BY l.THOIGIAN DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new Oracle.ManagedDataAccess.Client.OracleParameter("tungay", dtpTuNgay.Value.Date),
                    new Oracle.ManagedDataAccess.Client.OracleParameter("denngay", dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1)));
                dgvLichSu.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lọc dữ liệu: " + ex.Message);
            }
        }
    }
}
