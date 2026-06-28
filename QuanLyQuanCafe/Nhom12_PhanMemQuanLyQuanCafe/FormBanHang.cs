using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormBanHang : Form
    {
        private int currentMaHD = 0;

        public FormBanHang()
        {
            InitializeComponent();
        }

        private void FormBanHang_Load(object sender, EventArgs e)
        {
            LoadBan();
            LoadMon();
            LoadHoaDonCuaToi();
        }

        private void LoadBan()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM BAN ORDER BY MABAN");
                cboBan.DataSource = dt;
                cboBan.DisplayMember = "TENBAN";
                cboBan.ValueMember = "MABAN";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load bàn: " + ex.Message);
            }
        }

        private void LoadMon()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM MON WHERE TRANGTHAI = 'BAN' ORDER BY TENMON");
                cboMon.DataSource = dt;
                cboMon.DisplayMember = "TENMON";
                cboMon.ValueMember = "MAMON";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load món: " + ex.Message);
            }
        }

        private void LoadHoaDonCuaToi()
        {
            try
            {
                string query = @"SELECT h.MAHD, h.NGAYLAP, b.TENBAN, h.TRANGTHAI, h.TONGTIEN
                                FROM HOADON h
                                LEFT JOIN BAN b ON h.MABAN = b.MABAN
                                WHERE h.MANV = :manv
                                ORDER BY h.NGAYLAP DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(query, 
                    new OracleParameter("manv", SessionManager.CurrentMaNV));
                dgvHoaDon.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load hóa đơn: " + ex.Message);
            }
        }

        private void btnTaoHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboBan.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn bàn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int maBan = Convert.ToInt32(cboBan.SelectedValue);

                // KIỂM TRA BÀN
                string checkQuery = "SELECT COUNT(*) FROM HOADON WHERE MABAN = :maban AND TRANGTHAI = 'CHO'";
                object checkResult = DatabaseHelper.ExecuteScalar(checkQuery,
                    new OracleParameter("maban", maBan));

                if (Convert.ToInt32(checkResult) > 0)
                {
                    MessageBox.Show("Bàn này đang có hóa đơn chưa thanh toán!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // TẠO HÓA ĐƠN TRỰC TIẾP - SỬA LẠI PHẦN OUTPUT PARAMETER
                string query = @"
            DECLARE
                v_mahd NUMBER;
            BEGIN
                v_mahd := seq_hoadon.NEXTVAL;
                INSERT INTO HOADON(MAHD, MANV, MABAN, MAKH, NGAYLAP, TONGTIEN, TRANGTHAI)
                VALUES (v_mahd, :manv, :maban, NULL, SYSDATE, 0, 'CHO');
                UPDATE BAN SET TRANGTHAI = 'DANG_SU_DUNG' WHERE MABAN = :maban;
                :mahd := v_mahd;
            END;";

                OracleParameter mahdParam = new OracleParameter("mahd", OracleDbType.Decimal);
                mahdParam.Direction = ParameterDirection.Output;

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("manv", SessionManager.CurrentMaNV),
                    new OracleParameter("maban", maBan),
                    mahdParam);

                // SỬA CÁCH CONVERT ORACLEDECIMAL
                currentMaHD = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)mahdParam.Value).Value);
                lblMaHD.Text = "Mã HĐ: " + currentMaHD;

                MessageBox.Show("Tạo hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHoaDonCuaToi();
                LoadChiTietHoaDon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo hóa đơn: " + ex.Message);
            }
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentMaHD == 0)
                {
                    MessageBox.Show("Vui lòng tạo hóa đơn trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboMon.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn món!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int maMon = Convert.ToInt32(cboMon.SelectedValue);
                int soLuong = (int)numSoLuong.Value;
                
                // Lấy giá món
                string queryGia = "SELECT DONGIA FROM MON WHERE MAMON = :mamon";
                decimal gia = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(queryGia, 
                    new OracleParameter("mamon", maMon)));

                string query = "BEGIN pkg_hoadon.them_cthd(:mahd, :mamon, :soluong, :dongia, 0); END;";
                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("mahd", currentMaHD),
                    new OracleParameter("mamon", maMon),
                    new OracleParameter("soluong", soLuong),
                    new OracleParameter("dongia", gia));

                MessageBox.Show("Thêm món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadChiTietHoaDon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm món: " + ex.Message);
            }
        }

        private void LoadChiTietHoaDon()
        {
            if (currentMaHD == 0) return;

            try
            {
                string query = @"SELECT c.MAMON, m.TENMON, c.SOLUONG, c.DONGIA, c.THANHTIEN
                                FROM CHITIET_HD c
                                JOIN MON m ON c.MAMON = m.MAMON
                                WHERE c.MAHD = :mahd";
                DataTable dt = DatabaseHelper.ExecuteQuery(query, new OracleParameter("mahd", currentMaHD));
                dgvChiTiet.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load chi tiết: " + ex.Message);
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentMaHD == 0)
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật tổng tiền
                DatabaseHelper.ExecuteNonQuery("BEGIN pkg_hoadon.cap_nhat_tongtien(:mahd); END;",
                    new OracleParameter("mahd", currentMaHD));

                // Thanh toán
                DatabaseHelper.ExecuteNonQuery("BEGIN pkg_hoadon.thanh_toan(:mahd); END;",
                    new OracleParameter("mahd", currentMaHD));

                // Lấy tổng tiền
                string query = "SELECT TONGTIEN FROM HOADON WHERE MAHD = :mahd";
                object result = DatabaseHelper.ExecuteScalar(query, new OracleParameter("mahd", currentMaHD));
                decimal tongTien = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                MessageBox.Show($"Thanh toán thành công!\nTổng tiền: {tongTien:N0} VNĐ", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                currentMaHD = 0;
                lblMaHD.Text = "Mã HĐ: ";
                LoadHoaDonCuaToi();
                dgvChiTiet.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thanh toán: " + ex.Message);
            }
        }

        private void dgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvHoaDon.Rows[e.RowIndex];
                currentMaHD = Convert.ToInt32(row.Cells["MAHD"].Value);
                lblMaHD.Text = "Mã HĐ: " + currentMaHD;
                LoadChiTietHoaDon();
            }
        }
    }
}
