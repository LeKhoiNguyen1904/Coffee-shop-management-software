namespace Nhom12_PhanMemQuanLyQuanCafe
{
    partial class FormMain
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
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnQuanLyBanHang = new System.Windows.Forms.Button();
            this.btnQuanLyMenu = new System.Windows.Forms.Button();
            this.btnQuanLyKho = new System.Windows.Forms.Button();
            this.btnQuanLyTaiKhoan = new System.Windows.Forms.Button();
            this.btnLichSuDangNhap = new System.Windows.Forms.Button();
            this.btnThongKe = new System.Windows.Forms.Button();
            this.btnAuditLog = new System.Windows.Forms.Button();
            this.btnOLS = new System.Windows.Forms.Button();
            this.btnDangXuat = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblWelcome.Location = new System.Drawing.Point(30, 20);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(100, 24);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Xin chào:";
            // 
            // btnQuanLyBanHang
            // 
            this.btnQuanLyBanHang.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnQuanLyBanHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnQuanLyBanHang.ForeColor = System.Drawing.Color.White;
            this.btnQuanLyBanHang.Location = new System.Drawing.Point(50, 80);
            this.btnQuanLyBanHang.Name = "btnQuanLyBanHang";
            this.btnQuanLyBanHang.Size = new System.Drawing.Size(200, 60);
            this.btnQuanLyBanHang.TabIndex = 1;
            this.btnQuanLyBanHang.Text = "Quản lý bán hàng";
            this.btnQuanLyBanHang.UseVisualStyleBackColor = false;
            this.btnQuanLyBanHang.Click += new System.EventHandler(this.btnQuanLyBanHang_Click);
            // 
            // btnQuanLyMenu
            // 
            this.btnQuanLyMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnQuanLyMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnQuanLyMenu.ForeColor = System.Drawing.Color.White;
            this.btnQuanLyMenu.Location = new System.Drawing.Point(280, 80);
            this.btnQuanLyMenu.Name = "btnQuanLyMenu";
            this.btnQuanLyMenu.Size = new System.Drawing.Size(200, 60);
            this.btnQuanLyMenu.TabIndex = 2;
            this.btnQuanLyMenu.Text = "Quản lý Menu";
            this.btnQuanLyMenu.UseVisualStyleBackColor = false;
            this.btnQuanLyMenu.Click += new System.EventHandler(this.btnQuanLyMenu_Click);
            // 
            // btnQuanLyKho
            // 
            this.btnQuanLyKho.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnQuanLyKho.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnQuanLyKho.ForeColor = System.Drawing.Color.White;
            this.btnQuanLyKho.Location = new System.Drawing.Point(510, 80);
            this.btnQuanLyKho.Name = "btnQuanLyKho";
            this.btnQuanLyKho.Size = new System.Drawing.Size(200, 60);
            this.btnQuanLyKho.TabIndex = 3;
            this.btnQuanLyKho.Text = "Quản lý Kho";
            this.btnQuanLyKho.UseVisualStyleBackColor = false;
            this.btnQuanLyKho.Click += new System.EventHandler(this.btnQuanLyKho_Click);
            // 
            // btnQuanLyTaiKhoan
            // 
            this.btnQuanLyTaiKhoan.BackColor = System.Drawing.Color.Purple;
            this.btnQuanLyTaiKhoan.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnQuanLyTaiKhoan.ForeColor = System.Drawing.Color.White;
            this.btnQuanLyTaiKhoan.Location = new System.Drawing.Point(50, 170);
            this.btnQuanLyTaiKhoan.Name = "btnQuanLyTaiKhoan";
            this.btnQuanLyTaiKhoan.Size = new System.Drawing.Size(200, 60);
            this.btnQuanLyTaiKhoan.TabIndex = 4;
            this.btnQuanLyTaiKhoan.Text = "Quản lý Tài khoản";
            this.btnQuanLyTaiKhoan.UseVisualStyleBackColor = false;
            this.btnQuanLyTaiKhoan.Click += new System.EventHandler(this.btnQuanLyTaiKhoan_Click);
            // 
            // btnLichSuDangNhap
            // 
            this.btnLichSuDangNhap.BackColor = System.Drawing.Color.Teal;
            this.btnLichSuDangNhap.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnLichSuDangNhap.ForeColor = System.Drawing.Color.White;
            this.btnLichSuDangNhap.Location = new System.Drawing.Point(280, 170);
            this.btnLichSuDangNhap.Name = "btnLichSuDangNhap";
            this.btnLichSuDangNhap.Size = new System.Drawing.Size(200, 60);
            this.btnLichSuDangNhap.TabIndex = 5;
            this.btnLichSuDangNhap.Text = "Lịch sử đăng nhập";
            this.btnLichSuDangNhap.UseVisualStyleBackColor = false;
            this.btnLichSuDangNhap.Click += new System.EventHandler(this.btnLichSuDangNhap_Click);
            // 
            // btnThongKe
            // 
            this.btnThongKe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnThongKe.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnThongKe.ForeColor = System.Drawing.Color.White;
            this.btnThongKe.Location = new System.Drawing.Point(510, 170);
            this.btnThongKe.Name = "btnThongKe";
            this.btnThongKe.Size = new System.Drawing.Size(200, 60);
            this.btnThongKe.TabIndex = 6;
            this.btnThongKe.Text = "Thống kê";
            this.btnThongKe.UseVisualStyleBackColor = false;
            this.btnThongKe.Click += new System.EventHandler(this.btnThongKe_Click);
            // 
            // btnAuditLog
            // 
            this.btnAuditLog.BackColor = System.Drawing.Color.DarkSlateGray;
            this.btnAuditLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnAuditLog.ForeColor = System.Drawing.Color.White;
            this.btnAuditLog.Location = new System.Drawing.Point(50, 260);
            this.btnAuditLog.Name = "btnAuditLog";
            this.btnAuditLog.Size = new System.Drawing.Size(200, 60);
            this.btnAuditLog.TabIndex = 7;
            this.btnAuditLog.Text = "Audit Log";
            this.btnAuditLog.UseVisualStyleBackColor = false;
            this.btnAuditLog.Click += new System.EventHandler(this.btnAuditLog_Click);
            // 
            // btnOLS
            // 
            this.btnOLS.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.btnOLS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnOLS.ForeColor = System.Drawing.Color.White;
            this.btnOLS.Location = new System.Drawing.Point(280, 260);
            this.btnOLS.Name = "btnOLS";
            this.btnOLS.Size = new System.Drawing.Size(200, 60);
            this.btnOLS.TabIndex = 8;
            this.btnOLS.Text = "Oracle Label Security";
            this.btnOLS.UseVisualStyleBackColor = false;
            this.btnOLS.Click += new System.EventHandler(this.btnOLS_Click);
            // 
            // btnDangXuat
            // 
            this.btnDangXuat.BackColor = System.Drawing.Color.Red;
            this.btnDangXuat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnDangXuat.ForeColor = System.Drawing.Color.White;
            this.btnDangXuat.Location = new System.Drawing.Point(510, 260);
            this.btnDangXuat.Name = "btnDangXuat";
            this.btnDangXuat.Size = new System.Drawing.Size(200, 60);
            this.btnDangXuat.TabIndex = 9;
            this.btnDangXuat.Text = "Đăng xuất";
            this.btnDangXuat.UseVisualStyleBackColor = false;
            this.btnDangXuat.Click += new System.EventHandler(this.btnDangXuat_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(760, 360);
            this.Controls.Add(this.btnDangXuat);
            this.Controls.Add(this.btnOLS);
            this.Controls.Add(this.btnAuditLog);
            this.Controls.Add(this.btnThongKe);
            this.Controls.Add(this.btnLichSuDangNhap);
            this.Controls.Add(this.btnQuanLyTaiKhoan);
            this.Controls.Add(this.btnQuanLyKho);
            this.Controls.Add(this.btnQuanLyMenu);
            this.Controls.Add(this.btnQuanLyBanHang);
            this.Controls.Add(this.lblWelcome);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý Quán Cafe - Trang chủ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnQuanLyBanHang;
        private System.Windows.Forms.Button btnQuanLyMenu;
        private System.Windows.Forms.Button btnQuanLyKho;
        private System.Windows.Forms.Button btnQuanLyTaiKhoan;
        private System.Windows.Forms.Button btnLichSuDangNhap;
        private System.Windows.Forms.Button btnThongKe;
        private System.Windows.Forms.Button btnAuditLog;
        private System.Windows.Forms.Button btnOLS;
        private System.Windows.Forms.Button btnDangXuat;
    }
}
