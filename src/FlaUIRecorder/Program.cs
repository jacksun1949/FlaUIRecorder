using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlaUIRecorder.Internal;

namespace FlaUIRecorder
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            EnableDpiAwareness();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void EnableDpiAwareness()
        {
            try
            {
                if (NativeMethods.SetProcessDpiAwarenessContext(NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
                    return;
            }
            catch
            {
                // Fall through to shcore fallback
            }

            try
            {
                NativeMethods.SetProcessDpiAwareness(NativeMethods.PROCESS_PER_MONITOR_DPI_AWARE);
            }
            catch
            {
                // Final fallback: app.manifest declares DPI awareness
            }
        }
    }
}
