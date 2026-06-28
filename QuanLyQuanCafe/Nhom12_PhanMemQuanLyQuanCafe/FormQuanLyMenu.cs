using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormQuanLyMenu : Form
    {
        public FormQuanLyMenu()
        {
            InitializeComponent();
        }

        private void FormQuanLyMenu_Load(object sender, EventArgs e)
        {
            LoadLoaiMon();
            LoadMon();
        }

        private void LoadLoaiMon()
        {
            try
            {
                cboNhomMon.Items.Clear();
                cboNhomMon.Items.Add("Cà phê");
                cboNhomMon.Items.Add("Trà");
                cboNhomMon.Items.Add("Sinh tố");
                cboNhomMon.Items.Add("Nước ep");
                cboNhomMon.Items.Add("Đồ ăn");
                cboNhomMon.Items.Add("Bánh");
                cboNhomMon.Items.Add("Khác");
                cboNhomMon.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load loại món: " + ex.Message);
            }
        }

        private void LoadMon()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM MON ORDER BY MAMON");
                dgvMon.DataSource = dt;

                // Đặt tên cột
                if (dgvMon.Columns.Count > 0)
                {
                    dgvMon.Columns["MAMON"].HeaderText = "Mã món";
                    dgvMon.Columns["TENMON"].HeaderText = "Tên món";
                    dgvMon.Columns["LOAIMON"].HeaderText = "Loại món";
                    dgvMon.Columns["DONGIA"].HeaderText = "Đơn giá";
                    dgvMon.Columns["DONGIA"].DefaultCellStyle.Format = "N0";
                    dgvMon.Columns["TRANGTHAI"].HeaderText = "Trạng thái";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load món: " + ex.Message);
            }
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTenMon.Text) || numGia.Value <= 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // TRIM INPUT
                string tenMonTrimmed = txtTenMon.Text.Trim();
                string nhomMonTrimmed = cboNhomMon.SelectedItem?.ToString() ?? "";

                // CHECK IF MON NAME ALREADY EXISTS (DUPLICATE CHECK)
                string checkDuplicate = "SELECT COUNT(*) FROM MON WHERE LOWER(TENMON) = LOWER(:tenmon)";
                object dupResult = DatabaseHelper.ExecuteScalar(checkDuplicate,
                    new OracleParameter("tenmon", tenMonTrimmed));

                if (Convert.ToInt32(dupResult) > 0)
                {
                    MessageBox.Show($"Món '{tenMonTrimmed}' đã tồn tại! Vui lòng nhập tên khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenMon.Clear();
                    txtTenMon.Focus();
                    return;
                }

                // GET NEXT MAMON - Use function if available, fallback to sequence
                int nextMaMon = 1;
                try
                {
                    string getNextId = "SELECT get_next_mamon() FROM dual";
                    nextMaMon = Convert.ToInt32(DatabaseHelper.ExecuteScalar(getNextId));
                }
                catch
                {
                    // Fallback: Get from max value
                    string getMaxId = "SELECT NVL(MAX(MAMON), 0) + 1 FROM MON";
                    nextMaMon = Convert.ToInt32(DatabaseHelper.ExecuteScalar(getMaxId));
                }

                string query = @"INSERT INTO MON(MAMON, TENMON, LOAIMON, DONGIA, TRANGTHAI) 
                        VALUES (:mamon, :tenmon, :loaimon, :dongia, 'BAN')";

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("mamon", nextMaMon),
                    new OracleParameter("tenmon", tenMonTrimmed),
                    new OracleParameter("loaimon", nhomMonTrimmed),
                    new OracleParameter("dongia", numGia.Value));

                MessageBox.Show($"Thêm món thành công! Mã món: {nextMaMon}\nGiá: {numGia.Value:N0} VND", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMon();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm món: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMon.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn món cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int maMon = Convert.ToInt32(dgvMon.CurrentRow.Cells["MAMON"].Value);

                string query = @"UPDATE MON SET TENMON = :tenmon, LOAIMON = :loaimon, DONGIA = :dongia 
                                WHERE MAMON = :mamon";

                DatabaseHelper.ExecuteNonQuery(query,
                    new OracleParameter("tenmon", txtTenMon.Text),
                    new OracleParameter("loaimon", cboNhomMon.SelectedItem?.ToString() ?? ""),
                    new OracleParameter("dongia", numGia.Value),
                    new OracleParameter("mamon", maMon));

                MessageBox.Show($"Cập nhật món thành công!\nGiá: {numGia.Value:N0} VND", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa món: " + ex.Message);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMon.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn món cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Bạn có chắc muốn ngừng bán món này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int maMon = Convert.ToInt32(dgvMon.CurrentRow.Cells["MAMON"].Value);

                    // Cập nhật trạng thái thay vì xóa
                    string query = "UPDATE MON SET TRANGTHAI = 'NGUNG_BAN' WHERE MAMON = :mamon";
                    DatabaseHelper.ExecuteNonQuery(query, new OracleParameter("mamon", maMon));

                    MessageBox.Show("Đã ngừng bán món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMon();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật món: " + ex.Message);
            }
        }

        private void DgvMon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMon.Rows[e.RowIndex];
                txtTenMon.Text = row.Cells["TENMON"].Value.ToString();
                
                string loaiMon = row.Cells["LOAIMON"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(loaiMon) && cboNhomMon.Items.Contains(loaiMon))
                {
                    cboNhomMon.SelectedItem = loaiMon;
                }
                else
                {
                    cboNhomMon.SelectedIndex = 0;
                }
                
                // Get price from database value (not formatted display)
                // The value in DataGridView is the original number from database
                decimal price = 0;
                object cellValue = row.Cells["DONGIA"].Value;
                
                if (cellValue != null && decimal.TryParse(cellValue.ToString(), out decimal parsedPrice))
                {
                    price = parsedPrice;
                }
                
                numGia.Value = price;
            }
        }

        private void ClearInputs()
        {
            txtTenMon.Clear();
            cboNhomMon.SelectedIndex = 0;
            numGia.Value = 0;
        }

        private void BtnTaiLai_Click(object sender, EventArgs e)
        {
            LoadMon();
        }

        private void BtnKichHoat_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMon.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn món cần kích hoạt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int maMon = Convert.ToInt32(dgvMon.CurrentRow.Cells["MAMON"].Value);

                string query = "UPDATE MON SET TRANGTHAI = 'BAN' WHERE MAMON = :mamon";
                DatabaseHelper.ExecuteNonQuery(query, new OracleParameter("mamon", maMon));

                MessageBox.Show("Kích hoạt món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kích hoạt món: " + ex.Message);
            }
        }
    }
}