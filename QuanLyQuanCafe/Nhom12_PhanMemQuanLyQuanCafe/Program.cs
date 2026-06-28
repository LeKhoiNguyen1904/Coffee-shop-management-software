using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using System.Drawing;
using System.Data;

namespace Nhom12_PhanMemQuanLyQuanCafe
{
    internal static class Program
    {
        public static bool DebugMode = false;

        [STAThread]
        static void Main()
        {
#if DEBUG
            DebugMode = true;
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                var conn = DatabaseHelper.Connection;
                if (conn.State == ConnectionState.Open)
                {
                    Application.Run(new FormDangNhap());
                }
                else
                {
                    ShowConnectionError(new Exception("Không thể mở kết nối database"));
                }
            }
            catch (Exception ex)
            {
                ShowConnectionError(ex);
            }
        }

        private static void ShowConnectionError(Exception ex)
        {
            MessageBox.Show(
                $"Không thể kết nối database!\n\n" +
                $"Lỗi: {ex.Message}\n\n" +
                $"Vui lòng kiểm tra:\n" +
                $"1. Oracle Database đang chạy\n" +
                $"2. Đã chạy script SETUP_COMPLETE_ORACLE.sql\n" +
                $"3. Connection string trong DatabaseHelper.cs\n" +
                $"4. Oracle Managed Data Access Client installed\n" +
                $"5. Kiểm tra SID và port trong connection string",
                "Lỗi kết nối",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject as Exception);
        }

        private static void HandleUnhandledException(Exception ex)
        {
            string errorMessage = $"Đã xảy ra lỗi không mong muốn:\n\n{ex?.Message}\n\n" +
                                $"Ứng dụng sẽ đóng. Vui lòng khởi động lại.";

            if (DebugMode)
            {
                errorMessage += $"\n\nChi tiết lỗi:\n{ex?.StackTrace}";
            }

            MessageBox.Show(errorMessage, "Lỗi ứng dụng", MessageBoxButtons.OK, MessageBoxIcon.Error);

            System.Diagnostics.Debug.WriteLine($"UNHANDLED EXCEPTION: {ex}");

            Application.Exit();
        }
    }
}