namespace Nhom12_PhanMemQuanLyQuanCafe
{
    partial class FormBanHang
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblBan = new System.Windows.Forms.Label();
            this.cboBan = new System.Windows.Forms.ComboBox();
            this.btnTaoHoaDon = new System.Windows.Forms.Button();
            this.lblMaHD = new System.Windows.Forms.Label();
            this.lblMon = new System.Windows.Forms.Label();
            this.cboMon = new System.Windows.Forms.ComboBox();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.numSoLuong = new System.Windows.Forms.NumericUpDown();
            this.btnThemMon = new System.Windows.Forms.Button();
            this.dgvChiTiet = new System.Windows.Forms.DataGridView();
            this.btnThanhToan = new System.Windows.Forms.Button();
            this.dgvHoaDon = new System.Windows.Forms.DataGridView();
            this.lblDSHD = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numSoLuong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDon)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(350, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(250, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "QUẢN LÝ BÁN HÀNG";
            // lblBan
            this.lblBan.AutoSize = true;
            this.lblBan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblBan.Location = new System.Drawing.Point(20, 60);
            this.lblBan.Name = "lblBan";
            this.lblBan.Size = new System.Drawing.Size(37, 17);
            this.lblBan.TabIndex = 1;
            this.lblBan.Text = "Bàn:";
            // cboBan
            this.cboBan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cboBan.FormattingEnabled = true;
            this.cboBan.Location = new System.Drawing.Point(80, 57);
            this.cboBan.Name = "cboBan";
            this.cboBan.Size = new System.Drawing.Size(150, 24);
            this.cboBan.TabIndex = 2;
            // btnTaoHoaDon
            this.btnTaoHoaDon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnTaoHoaDon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnTaoHoaDon.ForeColor = System.Drawing.Color.White;
            this.btnTaoHoaDon.Location = new System.Drawing.Point(250, 50);
            this.btnTaoHoaDon.Name = "btnTaoHoaDon";
            this.btnTaoHoaDon.Size = new System.Drawing.Size(120, 35);
            this.btnTaoHoaDon.TabIndex = 3;
            this.btnTaoHoaDon.Text = "Tạo hóa đơn";
            this.btnTaoHoaDon.UseVisualStyleBackColor = false;
            this.btnTaoHoaDon.Click += new System.EventHandler(this.btnTaoHoaDon_Click);
            // lblMaHD
            this.lblMaHD.AutoSize = true;
            this.lblMaHD.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblMaHD.ForeColor = System.Drawing.Color.Red;
            this.lblMaHD.Location = new System.Drawing.Point(390, 60);
            this.lblMaHD.Name = "lblMaHD";
            this.lblMaHD.Size = new System.Drawing.Size(70, 20);
            this.lblMaHD.TabIndex = 4;
            this.lblMaHD.Text = "Mã HĐ:";
            // lblMon
            this.lblMon.AutoSize = true;
            this.lblMon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblMon.Location = new System.Drawing.Point(20, 110);
            this.lblMon.Name = "lblMon";
            this.lblMon.Size = new System.Drawing.Size(39, 17);
            this.lblMon.TabIndex = 5;
            this.lblMon.Text = "Món:";
            // cboMon
            this.cboMon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cboMon.FormattingEnabled = true;
            this.cboMon.Location = new System.Drawing.Point(80, 107);
            this.cboMon.Name = "cboMon";
            this.cboMon.Size = new System.Drawing.Size(250, 24);
            this.cboMon.TabIndex = 6;
            // lblSoLuong
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblSoLuong.Location = new System.Drawing.Point(350, 110);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(68, 17);
            this.lblSoLuong.TabIndex = 7;
            this.lblSoLuong.Text = "Số lượng:";
            // numSoLuong
            this.numSoLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.numSoLuong.Location = new System.Drawing.Point(430, 108);
            this.numSoLuong.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numSoLuong.Name = "numSoLuong";
            this.numSoLuong.Size = new System.Drawing.Size(80, 23);
            this.numSoLuong.TabIndex = 8;
            this.numSoLuong.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // btnThemMon
            this.btnThemMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnThemMon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnThemMon.ForeColor = System.Drawing.Color.White;
            this.btnThemMon.Location = new System.Drawing.Point(530, 100);
            this.btnThemMon.Name = "btnThemMon";
            this.btnThemMon.Size = new System.Drawing.Size(100, 35);
            this.btnThemMon.TabIndex = 9;
            this.btnThemMon.Text = "Thêm món";
            this.btnThemMon.UseVisualStyleBackColor = false;
            this.btnThemMon.Click += new System.EventHandler(this.btnThemMon_Click);
            // dgvChiTiet
            this.dgvChiTiet.AllowUserToAddRows = false;
            this.dgvChiTiet.AllowUserToDeleteRows = false;
            this.dgvChiTiet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChiTiet.Location = new System.Drawing.Point(20, 150);
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.ReadOnly = true;
            this.dgvChiTiet.Size = new System.Drawing.Size(610, 200);
            this.dgvChiTiet.TabIndex = 10;
            // btnThanhToan
            this.btnThanhToan.BackColor = System.Drawing.Color.Red;
            this.btnThanhToan.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnThanhToan.ForeColor = System.Drawing.Color.White;
            this.btnThanhToan.Location = new System.Drawing.Point(250, 370);
            this.btnThanhToan.Name = "btnThanhToan";
            this.btnThanhToan.Size = new System.Drawing.Size(150, 45);
            this.btnThanhToan.TabIndex = 11;
            this.btnThanhToan.Text = "Thanh toán";
            this.btnThanhToan.UseVisualStyleBackColor = false;
            this.btnThanhToan.Click += new System.EventHandler(this.btnThanhToan_Click);
            // lblDSHD
            this.lblDSHD.AutoSize = true;
            this.lblDSHD.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblDSHD.Location = new System.Drawing.Point(650, 60);
            this.lblDSHD.Name = "lblDSHD";
            this.lblDSHD.Size = new System.Drawing.Size(165, 17);
            this.lblDSHD.TabIndex = 12;
            this.lblDSHD.Text = "Danh sách hóa đơn:";
            // dgvHoaDon
            this.dgvHoaDon.AllowUserToAddRows = false;
            this.dgvHoaDon.AllowUserToDeleteRows = false;
            this.dgvHoaDon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHoaDon.Location = new System.Drawing.Point(650, 90);
            this.dgvHoaDon.Name = "dgvHoaDon";
            this.dgvHoaDon.ReadOnly = true;
            this.dgvHoaDon.Size = new System.Drawing.Size(350, 325);
            this.dgvHoaDon.TabIndex = 13;
            this.dgvHoaDon.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHoaDon_CellClick);
            // FormBanHang
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1020, 450);
            this.Controls.Add(this.dgvHoaDon);
            this.Controls.Add(this.lblDSHD);
            this.Controls.Add(this.btnThanhToan);
            this.Controls.Add(this.dgvChiTiet);
            this.Controls.Add(this.btnThemMon);
            this.Controls.Add(this.numSoLuong);
            this.Controls.Add(this.lblSoLuong);
            this.Controls.Add(this.cboMon);
            this.Controls.Add(this.lblMon);
            this.Controls.Add(this.lblMaHD);
            this.Controls.Add(this.btnTaoHoaDon);
            this.Controls.Add(this.cboBan);
            this.Controls.Add(this.lblBan);
            this.Controls.Add(this.lblTitle);
            this.Name = "FormBanHang";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý bán hàng";
            this.Load += new System.EventHandler(this.FormBanHang_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numSoLuong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBan;
        private System.Windows.Forms.ComboBox cboBan;
        private System.Windows.Forms.Button btnTaoHoaDon;
        private System.Windows.Forms.Label lblMaHD;
        private System.Windows.Forms.Label lblMon;
        private System.Windows.Forms.ComboBox cboMon;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.NumericUpDown numSoLuong;
        private System.Windows.Forms.Button btnThemMon;
        private System.Windows.Forms.DataGridView dgvChiTiet;
        private System.Windows.Forms.Button btnThanhToan;
        private System.Windows.Forms.DataGridView dgvHoaDon;
        private System.Windows.Forms.Label lblDSHD;
    }
}
