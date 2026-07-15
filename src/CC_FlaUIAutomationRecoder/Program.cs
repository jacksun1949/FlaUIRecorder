using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CC_FlaUIAutomationRecoder.Internal;

namespace CC_FlaUIAutomationRecoder
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Global exception handlers — must be set before Application.Run
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            EnableDpiAwareness();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleCrash(e.Exception, "UI Thread");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                HandleCrash(ex, e.IsTerminating ? "Fatal Background Thread" : "Unhandled");
            }
        }

        private static void HandleCrash(Exception ex, string source)
        {
            try
            {
                // Log full details
                RecorderErrorLog.RecordError(ex, source);
                RecorderErrorLog.FlushToFile();

                var msg = new StringBuilder();
                msg.AppendLine($"A critical error occurred ({source}).");
                msg.AppendLine();
                msg.AppendLine($"Error: {ex.GetType().FullName}");
                msg.AppendLine($"Message: {ex.Message}");
                msg.AppendLine();
                msg.AppendLine($"Log file: {RecorderErrorLog.LogFilePath}");
                msg.AppendLine();
                msg.AppendLine("The application will now close. Please check the log file for full details.");
                msg.AppendLine();
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    // Show first few lines of stack trace
                    var lines = ex.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    msg.AppendLine("Stack trace (first lines):");
                    foreach (var line in lines.Take(8))
                        msg.AppendLine($"  {line.Trim()}");
                }

                MessageBox.Show(msg.ToString(), "CC_FlaUIAutomationRecoder - Critical Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                // Last resort — can't even show the error dialog
                MessageBox.Show($"Fatal error: {ex?.Message}\n\n{ex?.StackTrace}",
                    "CC_FlaUIAutomationRecoder - Fatal Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Environment.Exit(1);
            }
        }

        private static void EnableDpiAwareness()
        {
            try
            {
                if (NativeMethods.SetProcessDpiAwarenessContext(NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
                    return;
            }
            catch (Exception ex)
            {
                RecorderErrorLog.WriteRaw($"[DPI] SetProcessDpiAwarenessContext failed: {ex.Message}");
            }

            try
            {
                NativeMethods.SetProcessDpiAwareness(NativeMethods.PROCESS_PER_MONITOR_DPI_AWARE);
            }
            catch (Exception ex)
            {
                RecorderErrorLog.WriteRaw($"[DPI] SetProcessDpiAwareness failed: {ex.Message}");
            }
        }
    }
}
