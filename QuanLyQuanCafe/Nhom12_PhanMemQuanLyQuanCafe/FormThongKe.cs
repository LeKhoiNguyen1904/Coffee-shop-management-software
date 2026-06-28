using System;
using System.Data;
using System.Windows.Forms;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormThongKe : Form
    {
        public FormThongKe()
        {
            InitializeComponent();
        }

        private void FormThongKe_Load(object sender, EventArgs e)
        {
            try
            {
                // Set ngày mặc định
                dtpTuNgay.Value = DateTime.Now.AddDays(-30);
                dtpDenNgay.Value = DateTime.Now;
                
                LoadDoanhThuNgay();
                LoadDoanhThuMon();
                LoadTop5Mon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo form: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDoanhThuNgay()
        {
            try
            {
                string query = @"SELECT NGAY, SO_HOA_DON, DOANH_THU 
                                FROM v_doanhthu_ngay
                                ORDER BY NGAY DESC
                                FETCH FIRST 30 ROWS ONLY";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                
                if (dt.Rows.Count == 0 && dt.Columns.Count == 0)
                {
                    // Tạo bảng trống với cấu trúc chỉ khi chưa có cột
                    dt.Columns.Add("NGAY", typeof(DateTime));
                    dt.Columns.Add("SO_HOA_DON", typeof(int));
                    dt.Columns.Add("DOANH_THU", typeof(decimal));
                    dt.Rows.Add(DateTime.Now, 0, 0);
                }
                
                dgvDoanhThuNgay.DataSource = dt;
                
                // Đặt tên cột
                if (dgvDoanhThuNgay.Columns.Count > 0)
                {
                    dgvDoanhThuNgay.Columns["NGAY"].HeaderText = "Ngày";
                    dgvDoanhThuNgay.Columns["NGAY"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvDoanhThuNgay.Columns["SO_HOA_DON"].HeaderText = "Số hóa đơn";
                    dgvDoanhThuNgay.Columns["DOANH_THU"].HeaderText = "Doanh thu (VNĐ)";
                    dgvDoanhThuNgay.Columns["DOANH_THU"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load doanh thu ngày: " + ex.Message + "\n\nChi tiết: " + ex.StackTrace);
            }
        }

        private void LoadDoanhThuMon()
        {
            try
            {
                string query = @"SELECT MAMON, TENMON, LOAIMON, SO_LAN_BAN, TONG_SO_LUONG, TONG_DOANH_THU 
                                FROM v_mon_ban_chay
                                FETCH FIRST 20 ROWS ONLY";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                
                if (dt.Rows.Count == 0)
                {
                    // Hiện danh sách món nhưng chưa bán
                    query = "SELECT MAMON, TENMON, LOAIMON, 0 AS SO_LAN_BAN, 0 AS TONG_SO_LUONG, 0 AS TONG_DOANH_THU FROM MON WHERE TRANGTHAI = 'BAN'";
                    dt = DatabaseHelper.ExecuteQuery(query);
                }
                
                dgvDoanhThuMon.DataSource = dt;
                
                // Đặt tên cột
                if (dgvDoanhThuMon.Columns.Count > 0)
                {
                    dgvDoanhThuMon.Columns["MAMON"].HeaderText = "Mã món";
                    dgvDoanhThuMon.Columns["TENMON"].HeaderText = "Tên món";
                    dgvDoanhThuMon.Columns["LOAIMON"].HeaderText = "Loại";
                    dgvDoanhThuMon.Columns["SO_LAN_BAN"].HeaderText = "Số lần bán";
                    dgvDoanhThuMon.Columns["TONG_SO_LUONG"].HeaderText = "Tổng SL";
                    dgvDoanhThuMon.Columns["TONG_DOANH_THU"].HeaderText = "Doanh thu (VNĐ)";
                    dgvDoanhThuMon.Columns["TONG_DOANH_THU"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load doanh thu món: " + ex.Message);
            }
        }

        private void LoadTop5Mon()
        {
            try
            {
                string query = @"SELECT MAMON, TENMON, LOAIMON, SO_LAN_BAN, TONG_SO_LUONG, TONG_DOANH_THU 
                                FROM v_mon_ban_chay
                                FETCH FIRST 5 ROWS ONLY";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                
                if (dt.Rows.Count == 0)
                {
                    // Hiện 5 món đầu tiên
                    query = "SELECT MAMON, TENMON, LOAIMON, 0 AS SO_LAN_BAN, 0 AS TONG_SO_LUONG, 0 AS TONG_DOANH_THU FROM MON WHERE TRANGTHAI = 'BAN' FETCH FIRST 5 ROWS ONLY";
                    dt = DatabaseHelper.ExecuteQuery(query);
                }
                
                dgvTop5.DataSource = dt;
                
                // Đặt tên cột
                if (dgvTop5.Columns.Count > 0)
                {
                    dgvTop5.Columns["MAMON"].HeaderText = "Mã";
                    dgvTop5.Columns["TENMON"].HeaderText = "Tên món";
                    dgvTop5.Columns["LOAIMON"].HeaderText = "Loại";
                    dgvTop5.Columns["SO_LAN_BAN"].HeaderText = "Lần bán";
                    dgvTop5.Columns["TONG_SO_LUONG"].HeaderText = "Tổng SL";
                    dgvTop5.Columns["TONG_DOANH_THU"].HeaderText = "Doanh thu (VNĐ)";
                    dgvTop5.Columns["TONG_DOANH_THU"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load top 5 món: " + ex.Message);
            }
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"SELECT NGAY, SO_HOA_DON, DOANH_THU 
                                FROM v_doanhthu_ngay
                                WHERE NGAY BETWEEN :tungay AND :denngay
                                ORDER BY NGAY DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new Oracle.ManagedDataAccess.Client.OracleParameter("tungay", dtpTuNgay.Value.Date),
                    new Oracle.ManagedDataAccess.Client.OracleParameter("denngay", dtpDenNgay.Value.Date));
                dgvDoanhThuNgay.DataSource = dt;
                
                // Tính tổng
                decimal tongDoanhThu = 0;
                int tongHoaDon = 0;
                foreach (DataRow row in dt.Rows)
                {
                    tongDoanhThu += Convert.ToDecimal(row["DOANH_THU"]);
                    tongHoaDon += Convert.ToInt32(row["SO_HOA_DON"]);
                }
                
                MessageBox.Show($"Thống kê từ {dtpTuNgay.Value:dd/MM/yyyy} đến {dtpDenNgay.Value:dd/MM/yyyy}\n\n" +
                               $"Tổng hóa đơn: {tongHoaDon}\n" +
                               $"Tổng doanh thu: {tongDoanhThu:N0} VNĐ",
                               "Kết quả thống kê", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lọc dữ liệu: " + ex.Message);
            }
        }
    }
}
