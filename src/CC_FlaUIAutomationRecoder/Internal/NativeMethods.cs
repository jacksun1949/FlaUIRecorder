using System;
using System.Runtime.InteropServices;

namespace CC_FlaUIAutomationRecoder.Internal
{
    public static class NativeMethods
    {
        public const int SW_RESTORE = 9;
        public const int WM_HOTKEY = 0x0312;
        public const int WM_DPICHANGED = 0x02E0;

        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;

        // Hotkey IDs for RecorderForm
        public const int HOTKEY_PAUSE = 1;
        public const int HOTKEY_COMMENT = 2;
        public const int HOTKEY_WAIT = 3;
        public const int HOTKEY_ASSERT = 4;
        public const int HOTKEY_CANCEL = 5;
        public const int HOTKEY_ELEMENT_TREE = 6;

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDpiAwarenessContext(IntPtr dpiContext);

        [DllImport("shcore.dll", SetLastError = true)]
        public static extern int SetProcessDpiAwareness(int value);

        public static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
        public const int PROCESS_PER_MONITOR_DPI_AWARE = 2;
    }
}
