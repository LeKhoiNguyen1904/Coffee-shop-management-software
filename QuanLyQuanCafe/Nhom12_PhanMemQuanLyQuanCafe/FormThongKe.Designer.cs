namespace Nhom12_PhanMemQuanLyQuanCafe
{
    partial class FormThongKe
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDoanhThuNgay = new System.Windows.Forms.TabPage();
            this.dgvDoanhThuNgay = new System.Windows.Forms.DataGridView();
            this.btnLoc = new System.Windows.Forms.Button();
            this.dtpDenNgay = new System.Windows.Forms.DateTimePicker();
            this.lblDenNgay = new System.Windows.Forms.Label();
            this.dtpTuNgay = new System.Windows.Forms.DateTimePicker();
            this.lblTuNgay = new System.Windows.Forms.Label();
            this.tabDoanhThuMon = new System.Windows.Forms.TabPage();
            this.dgvDoanhThuMon = new System.Windows.Forms.DataGridView();
            this.tabTop5 = new System.Windows.Forms.TabPage();
            this.dgvTop5 = new System.Windows.Forms.DataGridView();
            this.tabDoanhThuNgay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoanhThuNgay)).BeginInit();
            this.tabDoanhThuMon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoanhThuMon)).BeginInit();
            this.tabTop5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(400, 12);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(164, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "THỐNG KÊ";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabDoanhThuNgay);
            this.tabControl.Controls.Add(this.tabDoanhThuMon);
            this.tabControl.Controls.Add(this.tabTop5);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.tabControl.Location = new System.Drawing.Point(27, 62);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1013, 468);
            this.tabControl.TabIndex = 1;
            // 
            // tabDoanhThuNgay
            // 
            this.tabDoanhThuNgay.Controls.Add(this.dgvDoanhThuNgay);
            this.tabDoanhThuNgay.Controls.Add(this.btnLoc);
            this.tabDoanhThuNgay.Controls.Add(this.dtpDenNgay);
            this.tabDoanhThuNgay.Controls.Add(this.lblDenNgay);
            this.tabDoanhThuNgay.Controls.Add(this.dtpTuNgay);
            this.tabDoanhThuNgay.Controls.Add(this.lblTuNgay);
            this.tabDoanhThuNgay.Location = new System.Drawing.Point(4, 26);
            this.tabDoanhThuNgay.Name = "tabDoanhThuNgay";
            this.tabDoanhThuNgay.Padding = new System.Windows.Forms.Padding(3);
            this.tabDoanhThuNgay.Size = new System.Drawing.Size(752, 350);
            this.tabDoanhThuNgay.TabIndex = 0;
            this.tabDoanhThuNgay.Text = "Doanh thu theo ngày";
            this.tabDoanhThuNgay.UseVisualStyleBackColor = true;
            // 
            // dgvDoanhThuNgay
            // 
            this.dgvDoanhThuNgay.AllowUserToAddRows = false;
            this.dgvDoanhThuNgay.AllowUserToDeleteRows = false;
            this.dgvDoanhThuNgay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDoanhThuNgay.Location = new System.Drawing.Point(10, 60);
            this.dgvDoanhThuNgay.Name = "dgvDoanhThuNgay";
            this.dgvDoanhThuNgay.ReadOnly = true;
            this.dgvDoanhThuNgay.RowHeadersWidth = 51;
            this.dgvDoanhThuNgay.Size = new System.Drawing.Size(730, 280);
            this.dgvDoanhThuNgay.TabIndex = 6;
            // 
            // btnLoc
            // 
            this.btnLoc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnLoc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnLoc.ForeColor = System.Drawing.Color.White;
            this.btnLoc.Location = new System.Drawing.Point(540, 15);
            this.btnLoc.Name = "btnLoc";
            this.btnLoc.Size = new System.Drawing.Size(100, 30);
            this.btnLoc.TabIndex = 5;
            this.btnLoc.Text = "Lọc";
            this.btnLoc.UseVisualStyleBackColor = false;
            this.btnLoc.Click += new System.EventHandler(this.btnLoc_Click);
            // 
            // dtpDenNgay
            // 
            this.dtpDenNgay.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDenNgay.Location = new System.Drawing.Point(360, 18);
            this.dtpDenNgay.Name = "dtpDenNgay";
            this.dtpDenNgay.Size = new System.Drawing.Size(150, 22);
            this.dtpDenNgay.TabIndex = 4;
            // 
            // lblDenNgay
            // 
            this.lblDenNgay.AutoSize = true;
            this.lblDenNgay.Location = new System.Drawing.Point(280, 21);
            this.lblDenNgay.Name = "lblDenNgay";
            this.lblDenNgay.Size = new System.Drawing.Size(67, 16);
            this.lblDenNgay.TabIndex = 3;
            this.lblDenNgay.Text = "Đến ngày:";
            // 
            // dtpTuNgay
            // 
            this.dtpTuNgay.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTuNgay.Location = new System.Drawing.Point(100, 18);
            this.dtpTuNgay.Name = "dtpTuNgay";
            this.dtpTuNgay.Size = new System.Drawing.Size(150, 22);
            this.dtpTuNgay.TabIndex = 2;
            // 
            // lblTuNgay
            // 
            this.lblTuNgay.AutoSize = true;
            this.lblTuNgay.Location = new System.Drawing.Point(20, 21);
            this.lblTuNgay.Name = "lblTuNgay";
            this.lblTuNgay.Size = new System.Drawing.Size(59, 16);
            this.lblTuNgay.TabIndex = 1;
            this.lblTuNgay.Text = "Từ ngày:";
            // 
            // tabDoanhThuMon
            // 
            this.tabDoanhThuMon.Controls.Add(this.dgvDoanhThuMon);
            this.tabDoanhThuMon.Location = new System.Drawing.Point(4, 26);
            this.tabDoanhThuMon.Name = "tabDoanhThuMon";
            this.tabDoanhThuMon.Padding = new System.Windows.Forms.Padding(3);
            this.tabDoanhThuMon.Size = new System.Drawing.Size(752, 350);
            this.tabDoanhThuMon.TabIndex = 1;
            this.tabDoanhThuMon.Text = "Doanh thu theo món";
            this.tabDoanhThuMon.UseVisualStyleBackColor = true;
            // 
            // dgvDoanhThuMon
            // 
            this.dgvDoanhThuMon.AllowUserToAddRows = false;
            this.dgvDoanhThuMon.AllowUserToDeleteRows = false;
            this.dgvDoanhThuMon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDoanhThuMon.Location = new System.Drawing.Point(10, 10);
            this.dgvDoanhThuMon.Name = "dgvDoanhThuMon";
            this.dgvDoanhThuMon.ReadOnly = true;
            this.dgvDoanhThuMon.RowHeadersWidth = 51;
            this.dgvDoanhThuMon.Size = new System.Drawing.Size(730, 330);
            this.dgvDoanhThuMon.TabIndex = 0;
            // 
            // tabTop5
            // 
            this.tabTop5.Controls.Add(this.dgvTop5);
            this.tabTop5.Location = new System.Drawing.Point(4, 26);
            this.tabTop5.Name = "tabTop5";
            this.tabTop5.Size = new System.Drawing.Size(752, 350);
            this.tabTop5.TabIndex = 2;
            this.tabTop5.Text = "Top 5 món bán chạy";
            this.tabTop5.UseVisualStyleBackColor = true;
            // 
            // dgvTop5
            // 
            this.dgvTop5.AllowUserToAddRows = false;
            this.dgvTop5.AllowUserToDeleteRows = false;
            this.dgvTop5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTop5.Location = new System.Drawing.Point(10, 10);
            this.dgvTop5.Name = "dgvTop5";
            this.dgvTop5.ReadOnly = true;
            this.dgvTop5.RowHeadersWidth = 51;
            this.dgvTop5.Size = new System.Drawing.Size(730, 330);
            this.dgvTop5.TabIndex = 0;
            // 
            // FormThongKe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblTitle);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormThongKe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thống kê";
            this.Load += new System.EventHandler(this.FormThongKe_Load);
            this.tabDoanhThuNgay.ResumeLayout(false);
            this.tabDoanhThuNgay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoanhThuNgay)).EndInit();
            this.tabDoanhThuMon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoanhThuMon)).EndInit();
            this.tabTop5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabDoanhThuNgay;
        private System.Windows.Forms.DataGridView dgvDoanhThuNgay;
        private System.Windows.Forms.Button btnLoc;
        private System.Windows.Forms.DateTimePicker dtpDenNgay;
        private System.Windows.Forms.Label lblDenNgay;
        private System.Windows.Forms.DateTimePicker dtpTuNgay;
        private System.Windows.Forms.Label lblTuNgay;
        private System.Windows.Forms.TabPage tabDoanhThuMon;
        private System.Windows.Forms.DataGridView dgvDoanhThuMon;
        private System.Windows.Forms.TabPage tabTop5;
        private System.Windows.Forms.DataGridView dgvTop5;
    }
}
