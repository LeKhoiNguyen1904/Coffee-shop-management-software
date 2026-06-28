using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    /// <summary>
    /// Form demo Oracle Label Security (OLS) Simulation
    /// Hiển thị nguyên liệu theo mức độ bảo mật và role
    /// </summary>
    public partial class FormOLS : Form
    {
        public FormOLS()
        {
            InitializeComponent();
        }

        private void FormOLS_Load(object sender, EventArgs e)
        {
            LoadOLSInfo();
            LoadNguyenLieu();
            LoadStatistics();
            SetupSecurityLevelComboBox();
        }

        /// <summary>
        /// Hiển thị thông tin OLS và quyền truy cập
        /// </summary>
        private void LoadOLSInfo()
        {
            try
            {
                string role = VPDContextManager.GetCurrentRole();
                int? manv = VPDContextManager.GetCurrentManv();

                lblRole.Text = $"Role: {role}";
                lblManv.Text = $"MANV: {manv}";
                lblOLSInfo.Text = OLSHelper.GetOLSInfo();

                // Hiển thị quyền truy cập
                bool canPublic = OLSHelper.CanAccessSecurityLevel(OLSHelper.SecurityLevel.PUBLIC);
                bool canInternal = OLSHelper.CanAccessSecurityLevel(OLSHelper.SecurityLevel.INTERNAL);
                bool canConfidential = OLSHelper.CanAccessSecurityLevel(OLSHelper.SecurityLevel.CONFIDENTIAL);

                lblAccessRights.Text = $"Quyền truy cập:\n" +
                    $"• PUBLIC: {(canPublic ? "✓" : "✗")}\n" +
                    $"• INTERNAL: {(canInternal ? "✓" : "✗")}\n" +
                    $"• CONFIDENTIAL: {(canConfidential ? "✓" : "✗")}";

                // Chỉ ADMIN mới được thêm/sửa
                btnThem.Enabled = (role == "ADMIN");
                btnCapNhatLevel.Enabled = (role == "ADMIN");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load OLS info: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load danh sách nguyên liệu theo role
        /// </summary>
        private void LoadNguyenLieu()
        {
            try
            {
                DataTable dt = OLSHelper.GetNguyenLieuByRole();
                dgvNguyenLieu.DataSource = dt;

                // Format DataGridView
                if (dgvNguyenLieu.Columns.Count > 0)
                {
                    dgvNguyenLieu.Columns["MANL"].HeaderText = "Mã NL";
                    dgvNguyenLieu.Columns["TENNL"].HeaderText = "Tên nguyên liệu";
                    dgvNguyenLieu.Columns["DONVI"].HeaderText = "Đơn vị";
                    dgvNguyenLieu.Columns["SOLUONGTON"].HeaderText = "Số lượng tồn";
                    dgvNguyenLieu.Columns["SECURITY_LEVEL"].HeaderText = "Mức bảo mật";
                    dgvNguyenLieu.Columns["MUC_DO_BAO_MAT"].HeaderText = "Mô tả";

                    // Color code theo security level
                    foreach (DataGridViewRow row in dgvNguyenLieu.Rows)
                    {
                        if (row.Cells["SECURITY_LEVEL"].Value != null)
                        {
                            string level = row.Cells["SECURITY_LEVEL"].Value.ToString();
                            switch (level)
                            {
                                case "PUBLIC":
                                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                                    break;
                                case "INTERNAL":
                                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                                    break;
                                case "CONFIDENTIAL":
                                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                                    break;
                            }
                        }
                    }
                }

                lblTongSo.Text = $"Tổng số: {dt.Rows.Count} nguyên liệu";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load nguyên liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load thống kê theo mức bảo mật
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                DataTable dt = OLSHelper.GetSecurityLevelStatistics();
                dgvStatistics.DataSource = dt;

                if (dgvStatistics.Columns.Count > 0)
                {
                    dgvStatistics.Columns["SECURITY_LEVEL"].HeaderText = "Mức bảo mật";
                    dgvStatistics.Columns["SO_LUONG"].HeaderText = "Số lượng";
                    dgvStatistics.Columns["TONG_TON_KHO"].HeaderText = "Tổng tồn kho";
                    dgvStatistics.Columns["MUC_DO_BAO_MAT"].HeaderText = "Mô tả";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Setup ComboBox cho security level
        /// </summary>
        private void SetupSecurityLevelComboBox()
        {
            cboSecurityLevel.Items.Clear();
            cboSecurityLevel.Items.Add("PUBLIC");
            cboSecurityLevel.Items.Add("INTERNAL");
            cboSecurityLevel.Items.Add("CONFIDENTIAL");
            cboSecurityLevel.SelectedIndex = 0;
        }

        /// <summary>
        /// Thêm nguyên liệu mới
        /// </summary>
        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTenNL.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên nguyên liệu", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDonVi.Text))
                {
                    MessageBox.Show("Vui lòng nhập đơn vị", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal soluong;
                if (!decimal.TryParse(txtSoLuong.Text, out soluong))
                {
                    MessageBox.Show("Số lượng không hợp lệ", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OLSHelper.SecurityLevel level = (OLSHelper.SecurityLevel)
                    Enum.Parse(typeof(OLSHelper.SecurityLevel), cboSecurityLevel.SelectedItem.ToString());

                OLSHelper.ThemNguyenLieuSecure(
                    txtTenNL.Text,
                    txtDonVi.Text,
                    soluong,
                    level,
                    null
                );

                MessageBox.Show("Thêm nguyên liệu thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear form
                txtTenNL.Clear();
                txtDonVi.Clear();
                txtSoLuong.Clear();
                cboSecurityLevel.SelectedIndex = 0;

                // Reload
                LoadNguyenLieu();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm nguyên liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cập nhật mức bảo mật
        /// </summary>
        private void BtnCapNhatLevel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvNguyenLieu.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn nguyên liệu cần cập nhật", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int manl = Convert.ToInt32(dgvNguyenLieu.SelectedRows[0].Cells["MANL"].Value);
                string tennl = dgvNguyenLieu.SelectedRows[0].Cells["TENNL"].Value.ToString();

                OLSHelper.SecurityLevel newLevel = (OLSHelper.SecurityLevel)
                    Enum.Parse(typeof(OLSHelper.SecurityLevel), cboSecurityLevel.SelectedItem.ToString());

                DialogResult result = MessageBox.Show(
                    $"Cập nhật mức bảo mật của '{tennl}' thành {newLevel}?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    OLSHelper.UpdateSecurityLevel(manl, newLevel);
                    MessageBox.Show("Cập nhật thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadNguyenLieu();
                    LoadStatistics();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải lại dữ liệu
        /// </summary>
        private void BtnTaiLai_Click(object sender, EventArgs e)
        {
            LoadOLSInfo();
            LoadNguyenLieu();
            LoadStatistics();
        }

        /// <summary>
        /// Đóng form
        /// </summary>
        private void BtnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Xem chi tiết nguyên liệu
        /// </summary>
        private void DgvNguyenLieu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int manl = Convert.ToInt32(dgvNguyenLieu.Rows[e.RowIndex].Cells["MANL"].Value);
                OLSHelper.SecurityLevel? level = OLSHelper.GetSecurityLevel(manl);

                if (level.HasValue)
                {
                    string info = $"Mã NL: {manl}\n" +
                        $"Mức bảo mật: {level.Value}\n" +
                        $"Mô tả: {OLSHelper.GetSecurityLevelDescription(level.Value)}\n" +
                        $"Bạn có quyền xem: {(OLSHelper.CanViewNguyenLieu(manl) ? "Có" : "Không")}";

                    MessageBox.Show(info, "Chi tiết mức bảo mật",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // InitializeComponent is provided by the designer in FormOLS.Designer.cs
    }
}
