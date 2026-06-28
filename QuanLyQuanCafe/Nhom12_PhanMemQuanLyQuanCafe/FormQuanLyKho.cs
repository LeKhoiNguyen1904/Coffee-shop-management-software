using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormQuanLyKho : Form
    {
        public FormQuanLyKho()
        {
            InitializeComponent();
        }

        private void FormQuanLyKho_Load(object sender, EventArgs e)
        {
            LoadNguyenLieu();
            LoadNhaCungCap();
            
            // Thêm event handler cho combobox
            cboNCC.SelectedIndexChanged += CboNCC_SelectedIndexChanged;
            
            // Thêm event handler cho datagridview
            dgvNguyenLieu.SelectionChanged += DgvNguyenLieu_SelectionChanged;
            
            // Thêm event handler cho textbox giá nhập (chỉ cho phép nhập số)
            txtGiaNhap.KeyPress += TxtGiaNhap_KeyPress;
        }
        
        private void TxtGiaNhap_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số, backspace, và dấu chấm/phẩy
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && 
                e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            
            // Chỉ cho phép 1 dấu chấm/phẩy
            if ((e.KeyChar == '.' || e.KeyChar == ',') && 
                (txtGiaNhap.Text.Contains(".") || txtGiaNhap.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }
        
        private void CboNCC_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Lọc và chỉ hiển thị nguyên liệu của nhà cung cấp được chọn
            if (cboNCC.SelectedValue != null)
            {
                try
                {
                    int mancc = Convert.ToInt32(cboNCC.SelectedValue);
                    
                    // Lọc nguyên liệu theo nhà cung cấp chính
                    string query = @"SELECT MANL, TENNL, DONVI, SOLUONGTON, SECURITY_LEVEL 
                                    FROM NGUYENLIEU 
                                    WHERE MANCC_CHINH = :mancc
                                    ORDER BY MANL";
                    DataTable dt = DatabaseHelper.ExecuteQuery(query, new OracleParameter("mancc", mancc));
                    
                    dgvNguyenLieu.DataSource = dt;
                    
                    // Đặt tên cột
                    if (dgvNguyenLieu.Columns.Count > 0)
                    {
                        dgvNguyenLieu.Columns["MANL"].HeaderText = "Mã NL";
                        dgvNguyenLieu.Columns["TENNL"].HeaderText = "Tên nguyên liệu";
                        dgvNguyenLieu.Columns["DONVI"].HeaderText = "Đơn vị";
                        dgvNguyenLieu.Columns["SOLUONGTON"].HeaderText = "Tồn kho";
                        dgvNguyenLieu.Columns["SECURITY_LEVEL"].HeaderText = "Mức độ";
                    }
                    
                    // Cập nhật title
                    string queryInfo = "SELECT TENNCC FROM NHACUNGCAP WHERE MANCC = :mancc";
                    DataTable dtInfo = DatabaseHelper.ExecuteQuery(queryInfo, new OracleParameter("mancc", mancc));
                    if (dtInfo.Rows.Count > 0)
                    {
                        this.Text = $"Quản lý Kho - {dtInfo.Rows[0]["TENNCC"]} ({dt.Rows.Count} nguyên liệu)";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lọc nguyên liệu: " + ex.Message);
                }
            }
        }
        
        private void DgvNguyenLieu_SelectionChanged(object sender, EventArgs e)
        {
            // Tự động điền thông tin khi chọn nguyên liệu
            if (dgvNguyenLieu.CurrentRow != null)
            {
                try
                {
                    string tennl = dgvNguyenLieu.CurrentRow.Cells["TENNL"].Value.ToString();
                    string donvi = dgvNguyenLieu.CurrentRow.Cells["DONVI"].Value.ToString();
                    int tonkho = Convert.ToInt32(dgvNguyenLieu.CurrentRow.Cells["SOLUONGTON"].Value);
                    
                    // Cập nhật ghi chú tự động
                    txtGhiChu.Text = $"Nhập {tennl} ({donvi}) - Tồn kho hiện tại: {tonkho}";
                }
                catch { }
            }
        }

        private void LoadNguyenLieu()
        {
            try
            {
                // Load tất cả nguyên liệu ban đầu
                string query = "SELECT MANL, TENNL, DONVI, SOLUONGTON, SECURITY_LEVEL FROM NGUYENLIEU ORDER BY MANL";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvNguyenLieu.DataSource = dt;
                
                // Đặt tên cột
                if (dgvNguyenLieu.Columns.Count > 0)
                {
                    dgvNguyenLieu.Columns["MANL"].HeaderText = "Mã NL";
                    dgvNguyenLieu.Columns["TENNL"].HeaderText = "Tên nguyên liệu";
                    dgvNguyenLieu.Columns["DONVI"].HeaderText = "Đơn vị";
                    dgvNguyenLieu.Columns["SOLUONGTON"].HeaderText = "Tồn kho";
                    dgvNguyenLieu.Columns["SECURITY_LEVEL"].HeaderText = "Mức độ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load nguyên liệu: " + ex.Message);
            }
        }

        private void LoadNhaCungCap()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT MANCC, TENNCC FROM NHACUNGCAP ORDER BY MANCC");
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Chưa có nhà cung cấp!\nVui lòng thêm nhà cung cấp trước.", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                cboNCC.DataSource = dt;
                cboNCC.DisplayMember = "TENNCC";
                cboNCC.ValueMember = "MANCC";
                cboNCC.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load nhà cung cấp: " + ex.Message);
            }
        }

        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboNCC.SelectedValue == null || dgvNguyenLieu.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn nhà cung cấp và nguyên liệu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int mancc = Convert.ToInt32(cboNCC.SelectedValue);
                int manl = Convert.ToInt32(dgvNguyenLieu.CurrentRow.Cells["MANL"].Value);
                decimal soluong = numSoLuong.Value;

                // Parse giá nhập từ TextBox
                if (!decimal.TryParse(txtGiaNhap.Text, out decimal gianhap))
                {
                    MessageBox.Show("Giá nhập không hợp lệ! Vui lòng nhập số.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGiaNhap.Focus();
                    return;
                }

                if (soluong <= 0 || gianhap <= 0)
                {
                    MessageBox.Show("Số lượng và giá nhập phải lớn hơn 0!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo phiếu nhập
                string queryPN = "SELECT pkg_nhaphang.tao_phieunhap(:mancc, :manv) FROM dual";
                object result = DatabaseHelper.ExecuteScalar(queryPN,
                    new OracleParameter("mancc", mancc),
                    new OracleParameter("manv", SessionManager.CurrentMaNV));

                int mapn = Convert.ToInt32(result);

                // Thêm chi tiết phiếu nhập - convert decimal to OracleDecimal
                string queryCT = "BEGIN pkg_nhaphang.them_ctpn(:mapn, :manl, :soluong, :dongia); END;";
                DatabaseHelper.ExecuteNonQuery(queryCT,
                    new OracleParameter("mapn", mapn),
                    new OracleParameter("manl", manl),
                    new OracleParameter("soluong", soluong),
                    new OracleParameter("dongia", Convert.ToDecimal(gianhap))); // Fix: Convert explicitly

                // Cập nhật tổng tiền
                DatabaseHelper.ExecuteNonQuery("BEGIN pkg_nhaphang.cap_nhat_tongtien(:mapn); END;",
                    new OracleParameter("mapn", mapn));

                // Hoàn tất nhập (cập nhật tồn kho)
                DatabaseHelper.ExecuteNonQuery("BEGIN pkg_nhaphang.hoan_tat_nhap(:mapn); END;",
                    new OracleParameter("mapn", mapn));

                string tennl = dgvNguyenLieu.CurrentRow.Cells["TENNL"].Value.ToString();
                string tenncc = cboNCC.Text;

                MessageBox.Show($"✓ Nhập hàng thành công!\n\n" +
                               $"Mã phiếu nhập: PN{mapn}\n" +
                               $"Nhà cung cấp: {tenncc}\n" +
                               $"Nguyên liệu: {tennl}\n" +
                               $"Số lượng: {soluong}\n" +
                               $"Đơn giá: {gianhap:N0} VNĐ\n" +
                               $"Tổng tiền: {(soluong * gianhap):N0} VNĐ",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadNguyenLieu();
                numSoLuong.Value = 1;
                txtGiaNhap.Text = "0";
                txtGhiChu.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhập hàng: " + ex.Message);
            }
        }
    }
}
