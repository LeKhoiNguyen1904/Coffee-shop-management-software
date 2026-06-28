using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    public partial class FormAuditLog : Form
    {
        public FormAuditLog()
        {
            InitializeComponent();
        }

        private void FormAuditLog_Load(object sender, EventArgs e)
        {
            InitializeControls();
            LoadAuditLog();
            LoadTableNames();
            LoadStatistics();
        }

        private void InitializeControls()
        {
            // DataGridView
            dgvAuditLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAuditLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAuditLog.ReadOnly = true;
            dgvAuditLog.AllowUserToAddRows = false;

            // DateTimePicker
            dtpFrom.Value = DateTime.Now.AddDays(-7);
            dtpTo.Value = DateTime.Now;

            // ComboBox
            cboActionType.Items.AddRange(new string[] { "Tất cả", "INSERT", "UPDATE", "DELETE" });
            cboActionType.SelectedIndex = 0;
        }

        private void LoadAuditLog()
        {
            try
            {
                string query = @"SELECT ID, USERNAME, ACTION_TYPE, TABLE_NAME, RECORD_ID,
                                       SUBSTR(OLD_VALUE, 1, 100) AS OLD_VALUE_SHORT,
                                       SUBSTR(NEW_VALUE, 1, 100) AS NEW_VALUE_SHORT,
                                       TO_CHAR(ACTION_TIME, 'YYYY-MM-DD HH24:MI:SS') AS ACTION_TIME,
                                       IP_ADDRESS
                                FROM AUDIT_LOG
                                ORDER BY ACTION_TIME DESC
                                FETCH FIRST 100 ROWS ONLY";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvAuditLog.DataSource = dt;

                // Đặt tên cột
                if (dgvAuditLog.Columns.Count > 0)
                {
                    dgvAuditLog.Columns["ID"].HeaderText = "ID";
                    dgvAuditLog.Columns["USERNAME"].HeaderText = "Người dùng";
                    dgvAuditLog.Columns["ACTION_TYPE"].HeaderText = "Hành động";
                    dgvAuditLog.Columns["TABLE_NAME"].HeaderText = "Bảng";
                    dgvAuditLog.Columns["RECORD_ID"].HeaderText = "Record ID";
                    dgvAuditLog.Columns["OLD_VALUE_SHORT"].HeaderText = "Giá trị cũ";
                    dgvAuditLog.Columns["NEW_VALUE_SHORT"].HeaderText = "Giá trị mới";
                    dgvAuditLog.Columns["ACTION_TIME"].HeaderText = "Thời gian";
                    dgvAuditLog.Columns["IP_ADDRESS"].HeaderText = "IP Address";
                }

                lblTotalRecords.Text = $"Tổng số: {dt.Rows.Count} bản ghi";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải audit log: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTableNames()
        {
            try
            {
                string query = @"SELECT DISTINCT TABLE_NAME 
                                FROM AUDIT_LOG 
                                WHERE TABLE_NAME IS NOT NULL
                                ORDER BY TABLE_NAME";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                cboTableName.Items.Add("Tất cả");
                foreach (DataRow row in dt.Rows)
                {
                    cboTableName.Items.Add(row["TABLE_NAME"].ToString());
                }
                cboTableName.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadStatistics()
        {
            try
            {
                string query = @"SELECT 
                                   TABLE_NAME,
                                   ACTION_TYPE,
                                   COUNT(*) AS SO_LAN
                                FROM AUDIT_LOG
                                GROUP BY TABLE_NAME, ACTION_TYPE
                                ORDER BY TABLE_NAME, ACTION_TYPE";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvStatistics.DataSource = dt;

                if (dgvStatistics.Columns.Count > 0)
                {
                    dgvStatistics.Columns["TABLE_NAME"].HeaderText = "Bảng";
                    dgvStatistics.Columns["ACTION_TYPE"].HeaderText = "Hành động";
                    dgvStatistics.Columns["SO_LAN"].HeaderText = "Số lần";
                }
            }
            catch { }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"SELECT ID, USERNAME, ACTION_TYPE, TABLE_NAME, RECORD_ID,
                                       SUBSTR(OLD_VALUE, 1, 100) AS OLD_VALUE_SHORT,
                                       SUBSTR(NEW_VALUE, 1, 100) AS NEW_VALUE_SHORT,
                                       TO_CHAR(ACTION_TIME, 'YYYY-MM-DD HH24:MI:SS') AS ACTION_TIME,
                                       IP_ADDRESS
                                FROM AUDIT_LOG
                                WHERE 1=1";

                var parameters = new System.Collections.Generic.List<OracleParameter>();

                // Filter by table
                if (cboTableName.SelectedIndex > 0)
                {
                    query += " AND TABLE_NAME = :table_name";
                    parameters.Add(new OracleParameter("table_name", cboTableName.SelectedItem.ToString()));
                }

                // Filter by action type
                if (cboActionType.SelectedIndex > 0)
                {
                    query += " AND ACTION_TYPE = :action_type";
                    parameters.Add(new OracleParameter("action_type", cboActionType.SelectedItem.ToString()));
                }

                // Filter by date range
                query += " AND ACTION_TIME BETWEEN :from_date AND :to_date";
                parameters.Add(new OracleParameter("from_date", dtpFrom.Value.Date));
                parameters.Add(new OracleParameter("to_date", dtpTo.Value.Date.AddDays(1).AddSeconds(-1)));

                // Filter by username
                if (!string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    query += " AND UPPER(USERNAME) LIKE :username";
                    parameters.Add(new OracleParameter("username", "%" + txtUsername.Text.ToUpper() + "%"));
                }

                query += " ORDER BY ACTION_TIME DESC FETCH FIRST 100 ROWS ONLY";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
                dgvAuditLog.DataSource = dt;
                lblTotalRecords.Text = $"Tổng số: {dt.Rows.Count} bản ghi";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lọc dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAuditLog();
            LoadStatistics();
        }

        private void btnViewDetail_Click(object sender, EventArgs e)
        {
            if (dgvAuditLog.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int id = Convert.ToInt32(dgvAuditLog.SelectedRows[0].Cells["ID"].Value);
                
                string query = @"SELECT ID, USERNAME, ACTION_TYPE, TABLE_NAME, RECORD_ID,
                                       OLD_VALUE, NEW_VALUE,
                                       TO_CHAR(ACTION_TIME, 'YYYY-MM-DD HH24:MI:SS') AS ACTION_TIME,
                                       IP_ADDRESS
                                FROM AUDIT_LOG
                                WHERE ID = :id";
                DataTable dt = DatabaseHelper.ExecuteQuery(query, new OracleParameter("id", id));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string detail = $"ID: {row["ID"]}\n" +
                                  $"Người dùng: {row["USERNAME"]}\n" +
                                  $"Hành động: {row["ACTION_TYPE"]}\n" +
                                  $"Bảng: {row["TABLE_NAME"]}\n" +
                                  $"Record ID: {row["RECORD_ID"]}\n" +
                                  $"Thời gian: {row["ACTION_TIME"]}\n" +
                                  $"IP: {row["IP_ADDRESS"]}\n\n" +
                                  $"Giá trị cũ:\n{row["OLD_VALUE"]}\n\n" +
                                  $"Giá trị mới:\n{row["NEW_VALUE"]}";

                    MessageBox.Show(detail, "Chi tiết Audit Log", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem chi tiết: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    
                    // Header
                    for (int i = 0; i < dgvAuditLog.Columns.Count; i++)
                    {
                        sb.Append(dgvAuditLog.Columns[i].HeaderText);
                        if (i < dgvAuditLog.Columns.Count - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();

                    // Data
                    foreach (DataGridViewRow row in dgvAuditLog.Rows)
                    {
                        for (int i = 0; i < dgvAuditLog.Columns.Count; i++)
                        {
                            sb.Append(row.Cells[i].Value?.ToString().Replace(",", ";"));
                            if (i < dgvAuditLog.Columns.Count - 1)
                                sb.Append(",");
                        }
                        sb.AppendLine();
                    }

                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                    MessageBox.Show("Xuất file thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
