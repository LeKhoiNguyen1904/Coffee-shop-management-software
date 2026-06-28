namespace Nhom12_PhanMemQuanLyQuanCafe
{
    partial class FormAuditLog
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvAuditLog;
        private System.Windows.Forms.DataGridView dgvStatistics;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabAuditLog;
        private System.Windows.Forms.TabPage tabStatistics;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.ComboBox cboTableName;
        private System.Windows.Forms.Label lblActionType;
        private System.Windows.Forms.ComboBox cboActionType;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnViewDetail;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTotalRecords;

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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabAuditLog = new System.Windows.Forms.TabPage();
            this.dgvAuditLog = new System.Windows.Forms.DataGridView();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.lblTableName = new System.Windows.Forms.Label();
            this.cboTableName = new System.Windows.Forms.ComboBox();
            this.lblActionType = new System.Windows.Forms.Label();
            this.cboActionType = new System.Windows.Forms.ComboBox();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblTotalRecords = new System.Windows.Forms.Label();
            this.tabStatistics = new System.Windows.Forms.TabPage();
            this.dgvStatistics = new System.Windows.Forms.DataGridView();
            this.btnViewDetail = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabAuditLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditLog)).BeginInit();
            this.panelFilter.SuspendLayout();
            this.tabStatistics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabAuditLog);
            this.tabControl.Controls.Add(this.tabStatistics);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1500, 1000);
            this.tabControl.TabIndex = 0;
            // 
            // tabAuditLog
            // 
            this.tabAuditLog.Controls.Add(this.dgvAuditLog);
            this.tabAuditLog.Controls.Add(this.panelFilter);
            this.tabAuditLog.Controls.Add(this.lblTotalRecords);
            this.tabAuditLog.Location = new System.Drawing.Point(4, 29);
            this.tabAuditLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAuditLog.Name = "tabAuditLog";
            this.tabAuditLog.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAuditLog.Size = new System.Drawing.Size(1492, 967);
            this.tabAuditLog.TabIndex = 0;
            this.tabAuditLog.Text = "Audit Log";
            this.tabAuditLog.UseVisualStyleBackColor = true;
            // 
            // dgvAuditLog
            // 
            this.dgvAuditLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAuditLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuditLog.Location = new System.Drawing.Point(9, 194);
            this.dgvAuditLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvAuditLog.Name = "dgvAuditLog";
            this.dgvAuditLog.RowHeadersWidth = 51;
            this.dgvAuditLog.Size = new System.Drawing.Size(1470, 724);
            this.dgvAuditLog.TabIndex = 0;
            // 
            // panelFilter
            // 
            this.panelFilter.Controls.Add(this.lblTableName);
            this.panelFilter.Controls.Add(this.cboTableName);
            this.panelFilter.Controls.Add(this.lblActionType);
            this.panelFilter.Controls.Add(this.cboActionType);
            this.panelFilter.Controls.Add(this.lblFromDate);
            this.panelFilter.Controls.Add(this.dtpFrom);
            this.panelFilter.Controls.Add(this.lblToDate);
            this.panelFilter.Controls.Add(this.dtpTo);
            this.panelFilter.Controls.Add(this.lblUsername);
            this.panelFilter.Controls.Add(this.txtUsername);
            this.panelFilter.Controls.Add(this.btnFilter);
            this.panelFilter.Controls.Add(this.btnRefresh);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(4, 5);
            this.panelFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1484, 154);
            this.panelFilter.TabIndex = 1;
            // 
            // lblTableName
            // 
            this.lblTableName.Location = new System.Drawing.Point(15, 15);
            this.lblTableName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTableName.Name = "lblTableName";
            this.lblTableName.Size = new System.Drawing.Size(97, 35);
            this.lblTableName.TabIndex = 0;
            this.lblTableName.Text = "Bảng:";
            // 
            // cboTableName
            // 
            this.cboTableName.Location = new System.Drawing.Point(120, 12);
            this.cboTableName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboTableName.Name = "cboTableName";
            this.cboTableName.Size = new System.Drawing.Size(223, 28);
            this.cboTableName.TabIndex = 1;
            // 
            // lblActionType
            // 
            this.lblActionType.Location = new System.Drawing.Point(375, 15);
            this.lblActionType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblActionType.Name = "lblActionType";
            this.lblActionType.Size = new System.Drawing.Size(120, 35);
            this.lblActionType.TabIndex = 2;
            this.lblActionType.Text = "Hành động:";
            // 
            // cboActionType
            // 
            this.cboActionType.Location = new System.Drawing.Point(495, 12);
            this.cboActionType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboActionType.Name = "cboActionType";
            this.cboActionType.Size = new System.Drawing.Size(178, 28);
            this.cboActionType.TabIndex = 3;
            // 
            // lblFromDate
            // 
            this.lblFromDate.Location = new System.Drawing.Point(15, 61);
            this.lblFromDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(97, 35);
            this.lblFromDate.TabIndex = 4;
            this.lblFromDate.Text = "Từ ngày:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(120, 59);
            this.dtpFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(223, 26);
            this.dtpFrom.TabIndex = 5;
            // 
            // lblToDate
            // 
            this.lblToDate.Location = new System.Drawing.Point(375, 61);
            this.lblToDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(120, 35);
            this.lblToDate.TabIndex = 6;
            this.lblToDate.Text = "Đến ngày:";
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(495, 59);
            this.dtpTo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(223, 26);
            this.dtpTo.TabIndex = 7;
            // 
            // lblUsername
            // 
            this.lblUsername.Location = new System.Drawing.Point(15, 108);
            this.lblUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(97, 35);
            this.lblUsername.TabIndex = 8;
            this.lblUsername.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(120, 105);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(223, 26);
            this.txtUsername.TabIndex = 9;
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(375, 101);
            this.btnFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(120, 39);
            this.btnFilter.TabIndex = 10;
            this.btnFilter.Text = "Lọc";
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(510, 101);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 39);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblTotalRecords
            // 
            this.lblTotalRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalRecords.Location = new System.Drawing.Point(9, 926);
            this.lblTotalRecords.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalRecords.Name = "lblTotalRecords";
            this.lblTotalRecords.Size = new System.Drawing.Size(300, 31);
            this.lblTotalRecords.TabIndex = 2;
            this.lblTotalRecords.Text = "Tổng số: 0 bản ghi";
            // 
            // tabStatistics
            // 
            this.tabStatistics.Controls.Add(this.dgvStatistics);
            this.tabStatistics.Location = new System.Drawing.Point(4, 29);
            this.tabStatistics.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabStatistics.Size = new System.Drawing.Size(1492, 967);
            this.tabStatistics.TabIndex = 1;
            this.tabStatistics.Text = "Thống kê";
            this.tabStatistics.UseVisualStyleBackColor = true;
            // 
            // dgvStatistics
            // 
            this.dgvStatistics.ColumnHeadersHeight = 29;
            this.dgvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStatistics.Location = new System.Drawing.Point(4, 5);
            this.dgvStatistics.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvStatistics.Name = "dgvStatistics";
            this.dgvStatistics.RowHeadersWidth = 51;
            this.dgvStatistics.Size = new System.Drawing.Size(1484, 957);
            this.dgvStatistics.TabIndex = 0;
            // 
            // btnViewDetail
            // 
            this.btnViewDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewDetail.Location = new System.Drawing.Point(1050, 939);
            this.btnViewDetail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnViewDetail.Name = "btnViewDetail";
            this.btnViewDetail.Size = new System.Drawing.Size(135, 46);
            this.btnViewDetail.TabIndex = 1;
            this.btnViewDetail.Text = "Xem chi tiết";
            this.btnViewDetail.Click += new System.EventHandler(this.btnViewDetail_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(1200, 939);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(135, 46);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Xuất file";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1350, 939);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(135, 46);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormAuditLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1500, 1000);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnViewDetail);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnClose);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormAuditLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audit Log - Nhật ký kiểm toán";
            this.Load += new System.EventHandler(this.FormAuditLog_Load);
            this.tabControl.ResumeLayout(false);
            this.tabAuditLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditLog)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            this.tabStatistics.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
