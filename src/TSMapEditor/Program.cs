using I18n;
using System;
using System.Drawing;
using System.Windows.Forms;
using TSMapEditor.Rendering;

namespace TSMapEditor
{
    static class Program
    {
        public static string[] args;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Program.args = args;

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Environment.CurrentDirectory = Application.StartupPath.Replace('\\', '/');
            new GameClass().Run();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private static void HandleException(Exception ex)
        {
            MessageBox.Show(String.Format(Shared.The_map_editor_failed_to_launch,ex.Message,ex.StackTrace));
        }

        public static void DisableExceptionHandler()
        {
            Application.ThreadException -= Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }
    }
}
