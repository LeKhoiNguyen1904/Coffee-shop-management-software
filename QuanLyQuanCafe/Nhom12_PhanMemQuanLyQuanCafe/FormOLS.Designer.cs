namespace Nhom12_PhanMemQuanLyQuanCafe
{
    partial class FormOLS
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblManv;
        private System.Windows.Forms.Label lblOLSInfo;
        private System.Windows.Forms.Label lblAccessRights;
        private System.Windows.Forms.DataGridView dgvNguyenLieu;
        private System.Windows.Forms.DataGridView dgvStatistics;
        private System.Windows.Forms.GroupBox grpThemMoi;
        private System.Windows.Forms.Label lblTenNL;
        private System.Windows.Forms.Label lblDonVi;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.Label lblSecurityLevel;
        private System.Windows.Forms.TextBox txtTenNL;
        private System.Windows.Forms.TextBox txtDonVi;
        private System.Windows.Forms.TextBox txtSoLuong;
        private System.Windows.Forms.ComboBox cboSecurityLevel;
        private System.Windows.Forms.Button btnThem;
        private System.Windows.Forms.Button btnCapNhatLevel;
        private System.Windows.Forms.Button btnTaiLai;
        private System.Windows.Forms.Button btnDong;
        private System.Windows.Forms.Label lblTongSo;
        private System.Windows.Forms.GroupBox grpInfo;
        private System.Windows.Forms.GroupBox grpDanhSach;
        private System.Windows.Forms.GroupBox grpThongKe;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblManv = new System.Windows.Forms.Label();
            this.lblOLSInfo = new System.Windows.Forms.Label();
            this.lblAccessRights = new System.Windows.Forms.Label();
            this.dgvNguyenLieu = new System.Windows.Forms.DataGridView();
            this.dgvStatistics = new System.Windows.Forms.DataGridView();
            this.grpThemMoi = new System.Windows.Forms.GroupBox();
            this.lblTenNL = new System.Windows.Forms.Label();
            this.lblDonVi = new System.Windows.Forms.Label();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.lblSecurityLevel = new System.Windows.Forms.Label();
            this.txtTenNL = new System.Windows.Forms.TextBox();
            this.txtDonVi = new System.Windows.Forms.TextBox();
            this.txtSoLuong = new System.Windows.Forms.TextBox();
            this.cboSecurityLevel = new System.Windows.Forms.ComboBox();
            this.btnThem = new System.Windows.Forms.Button();
            this.btnCapNhatLevel = new System.Windows.Forms.Button();
            this.btnTaiLai = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.lblTongSo = new System.Windows.Forms.Label();
            this.grpInfo = new System.Windows.Forms.GroupBox();
            this.grpDanhSach = new System.Windows.Forms.GroupBox();
            this.grpThongKe = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNguyenLieu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).BeginInit();
            this.grpThemMoi.SuspendLayout();
            this.grpInfo.SuspendLayout();
            this.grpDanhSach.SuspendLayout();
            this.grpThongKe.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Location = new System.Drawing.Point(300, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(450, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ORACLE LABEL SECURITY (OLS) DEMO";
            // 
            // grpInfo
            // 
            this.grpInfo.Controls.Add(this.lblRole);
            this.grpInfo.Controls.Add(this.lblManv);
            this.grpInfo.Controls.Add(this.lblOLSInfo);
            this.grpInfo.Controls.Add(this.lblAccessRights);
            this.grpInfo.Location = new System.Drawing.Point(12, 50);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(300, 150);
            this.grpInfo.TabIndex = 1;
            this.grpInfo.TabStop = false;
            this.grpInfo.Text = "Thông tin OLS";
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblRole.Location = new System.Drawing.Point(10, 25);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(100, 17);
            this.lblRole.TabIndex = 0;
            this.lblRole.Text = "Role: ADMIN";
            // 
            // lblManv
            // 
            this.lblManv.AutoSize = true;
            this.lblManv.Location = new System.Drawing.Point(10, 50);
            this.lblManv.Name = "lblManv";
            this.lblManv.Size = new System.Drawing.Size(60, 13);
            this.lblManv.TabIndex = 1;
            this.lblManv.Text = "MANV: 1";
            // 
            // lblOLSInfo
            // 
            this.lblOLSInfo.AutoSize = true;
            this.lblOLSInfo.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblOLSInfo.Location = new System.Drawing.Point(10, 70);
            this.lblOLSInfo.Name = "lblOLSInfo";
            this.lblOLSInfo.Size = new System.Drawing.Size(100, 13);
            this.lblOLSInfo.TabIndex = 2;
            this.lblOLSInfo.Text = "OLS Access: ...";
            // 
            // lblAccessRights
            // 
            this.lblAccessRights.AutoSize = true;
            this.lblAccessRights.Location = new System.Drawing.Point(10, 90);
            this.lblAccessRights.Name = "lblAccessRights";
            this.lblAccessRights.Size = new System.Drawing.Size(100, 13);
            this.lblAccessRights.TabIndex = 3;
            this.lblAccessRights.Text = "Quyền truy cập:";
            // 
            // grpThemMoi
            // 
            this.grpThemMoi.Controls.Add(this.lblTenNL);
            this.grpThemMoi.Controls.Add(this.txtTenNL);
            this.grpThemMoi.Controls.Add(this.lblDonVi);
            this.grpThemMoi.Controls.Add(this.txtDonVi);
            this.grpThemMoi.Controls.Add(this.lblSoLuong);
            this.grpThemMoi.Controls.Add(this.txtSoLuong);
            this.grpThemMoi.Controls.Add(this.lblSecurityLevel);
            this.grpThemMoi.Controls.Add(this.cboSecurityLevel);
            this.grpThemMoi.Controls.Add(this.btnThem);
            this.grpThemMoi.Controls.Add(this.btnCapNhatLevel);
            this.grpThemMoi.Location = new System.Drawing.Point(320, 50);
            this.grpThemMoi.Name = "grpThemMoi";
            this.grpThemMoi.Size = new System.Drawing.Size(650, 150);
            this.grpThemMoi.TabIndex = 2;
            this.grpThemMoi.TabStop = false;
            this.grpThemMoi.Text = "Thêm/Cập nhật nguyên liệu (Chỉ ADMIN)";
            // 
            // lblTenNL
            // 
            this.lblTenNL.AutoSize = true;
            this.lblTenNL.Location = new System.Drawing.Point(10, 30);
            this.lblTenNL.Name = "lblTenNL";
            this.lblTenNL.Size = new System.Drawing.Size(100, 13);
            this.lblTenNL.TabIndex = 0;
            this.lblTenNL.Text = "Tên nguyên liệu:";
            // 
            // txtTenNL
            // 
            this.txtTenNL.Location = new System.Drawing.Point(120, 27);
            this.txtTenNL.Name = "txtTenNL";
            this.txtTenNL.Size = new System.Drawing.Size(200, 20);
            this.txtTenNL.TabIndex = 1;
            // 
            // lblDonVi
            // 
            this.lblDonVi.AutoSize = true;
            this.lblDonVi.Location = new System.Drawing.Point(10, 60);
            this.lblDonVi.Name = "lblDonVi";
            this.lblDonVi.Size = new System.Drawing.Size(50, 13);
            this.lblDonVi.TabIndex = 2;
            this.lblDonVi.Text = "Đơn vị:";
            // 
            // txtDonVi
            // 
            this.txtDonVi.Location = new System.Drawing.Point(120, 57);
            this.txtDonVi.Name = "txtDonVi";
            this.txtDonVi.Size = new System.Drawing.Size(100, 20);
            this.txtDonVi.TabIndex = 3;
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Location = new System.Drawing.Point(10, 90);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(60, 13);
            this.lblSoLuong.TabIndex = 4;
            this.lblSoLuong.Text = "Số lượng:";
            // 
            // txtSoLuong
            // 
            this.txtSoLuong.Location = new System.Drawing.Point(120, 87);
            this.txtSoLuong.Name = "txtSoLuong";
            this.txtSoLuong.Size = new System.Drawing.Size(100, 20);
            this.txtSoLuong.TabIndex = 5;
            // 
            // lblSecurityLevel
            // 
            this.lblSecurityLevel.AutoSize = true;
            this.lblSecurityLevel.Location = new System.Drawing.Point(10, 120);
            this.lblSecurityLevel.Name = "lblSecurityLevel";
            this.lblSecurityLevel.Size = new System.Drawing.Size(90, 13);
            this.lblSecurityLevel.TabIndex = 6;
            this.lblSecurityLevel.Text = "Mức bảo mật:";
            // 
            // cboSecurityLevel
            // 
            this.cboSecurityLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSecurityLevel.FormattingEnabled = true;
            this.cboSecurityLevel.Location = new System.Drawing.Point(120, 117);
            this.cboSecurityLevel.Name = "cboSecurityLevel";
            this.cboSecurityLevel.Size = new System.Drawing.Size(150, 21);
            this.cboSecurityLevel.TabIndex = 7;
            // 
            // btnThem
            // 
            this.btnThem.BackColor = System.Drawing.Color.LightGreen;
            this.btnThem.Location = new System.Drawing.Point(350, 50);
            this.btnThem.Name = "btnThem";
            this.btnThem.Size = new System.Drawing.Size(120, 35);
            this.btnThem.TabIndex = 8;
            this.btnThem.Text = "Thêm mới";
            this.btnThem.UseVisualStyleBackColor = false;
            this.btnThem.Click += new System.EventHandler(this.BtnThem_Click);
            // 
            // btnCapNhatLevel
            // 
            this.btnCapNhatLevel.BackColor = System.Drawing.Color.LightBlue;
            this.btnCapNhatLevel.Location = new System.Drawing.Point(480, 50);
            this.btnCapNhatLevel.Name = "btnCapNhatLevel";
            this.btnCapNhatLevel.Size = new System.Drawing.Size(150, 35);
            this.btnCapNhatLevel.TabIndex = 9;
            this.btnCapNhatLevel.Text = "Cập nhật mức bảo mật";
            this.btnCapNhatLevel.UseVisualStyleBackColor = false;
            this.btnCapNhatLevel.Click += new System.EventHandler(this.BtnCapNhatLevel_Click);
            // 
            // grpDanhSach
            // 
            this.grpDanhSach.Controls.Add(this.dgvNguyenLieu);
            this.grpDanhSach.Controls.Add(this.lblTongSo);
            this.grpDanhSach.Location = new System.Drawing.Point(12, 210);
            this.grpDanhSach.Name = "grpDanhSach";
            this.grpDanhSach.Size = new System.Drawing.Size(650, 350);
            this.grpDanhSach.TabIndex = 3;
            this.grpDanhSach.TabStop = false;
            this.grpDanhSach.Text = "Danh sách nguyên liệu (theo role)";
            // 
            // dgvNguyenLieu
            // 
            this.dgvNguyenLieu.AllowUserToAddRows = false;
            this.dgvNguyenLieu.AllowUserToDeleteRows = false;
            this.dgvNguyenLieu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNguyenLieu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNguyenLieu.Location = new System.Drawing.Point(10, 20);
            this.dgvNguyenLieu.Name = "dgvNguyenLieu";
            this.dgvNguyenLieu.ReadOnly = true;
            this.dgvNguyenLieu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNguyenLieu.Size = new System.Drawing.Size(630, 290);
            this.dgvNguyenLieu.TabIndex = 0;
            this.dgvNguyenLieu.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvNguyenLieu_CellDoubleClick);
            // 
            // lblTongSo
            // 
            this.lblTongSo.AutoSize = true;
            this.lblTongSo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTongSo.Location = new System.Drawing.Point(10, 320);
            this.lblTongSo.Name = "lblTongSo";
            this.lblTongSo.Size = new System.Drawing.Size(100, 15);
            this.lblTongSo.TabIndex = 1;
            this.lblTongSo.Text = "Tổng số: 0";
            // 
            // grpThongKe
            // 
            this.grpThongKe.Controls.Add(this.dgvStatistics);
            this.grpThongKe.Location = new System.Drawing.Point(670, 210);
            this.grpThongKe.Name = "grpThongKe";
            this.grpThongKe.Size = new System.Drawing.Size(300, 350);
            this.grpThongKe.TabIndex = 4;
            this.grpThongKe.TabStop = false;
            this.grpThongKe.Text = "Thống kê theo mức bảo mật";
            // 
            // dgvStatistics
            // 
            this.dgvStatistics.AllowUserToAddRows = false;
            this.dgvStatistics.AllowUserToDeleteRows = false;
            this.dgvStatistics.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStatistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStatistics.Location = new System.Drawing.Point(10, 20);
            this.dgvStatistics.Name = "dgvStatistics";
            this.dgvStatistics.ReadOnly = true;
            this.dgvStatistics.Size = new System.Drawing.Size(280, 320);
            this.dgvStatistics.TabIndex = 0;
            // 
            // btnTaiLai
            // 
            this.btnTaiLai.BackColor = System.Drawing.Color.LightYellow;
            this.btnTaiLai.Location = new System.Drawing.Point(700, 570);
            this.btnTaiLai.Name = "btnTaiLai";
            this.btnTaiLai.Size = new System.Drawing.Size(120, 35);
            this.btnTaiLai.TabIndex = 5;
            this.btnTaiLai.Text = "Tải lại";
            this.btnTaiLai.UseVisualStyleBackColor = false;
            this.btnTaiLai.Click += new System.EventHandler(this.BtnTaiLai_Click);
            // 
            // btnDong
            // 
            this.btnDong.BackColor = System.Drawing.Color.LightCoral;
            this.btnDong.Location = new System.Drawing.Point(830, 570);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(120, 35);
            this.btnDong.TabIndex = 6;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.BtnDong_Click);
            // 
            // FormOLS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.btnTaiLai);
            this.Controls.Add(this.grpThongKe);
            this.Controls.Add(this.grpDanhSach);
            this.Controls.Add(this.grpThemMoi);
            this.Controls.Add(this.grpInfo);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormOLS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Oracle Label Security (OLS) Demo";
            this.Load += new System.EventHandler(this.FormOLS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNguyenLieu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).EndInit();
            this.grpThemMoi.ResumeLayout(false);
            this.grpThemMoi.PerformLayout();
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            this.grpDanhSach.ResumeLayout(false);
            this.grpDanhSach.PerformLayout();
            this.grpThongKe.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
