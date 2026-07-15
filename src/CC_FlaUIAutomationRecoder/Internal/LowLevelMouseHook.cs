using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CC_FlaUIAutomationRecoder.Internal
{
    /// <summary>
    /// Point struct matching the EventHook POINT layout (int fields, lowercase).
    /// </summary>
    public struct HookPoint
    {
        public int x;
        public int y;

        public HookPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// Windows mouse message constants matching EventHook.MouseMessages values.
    /// </summary>
    public static class MouseMessage
    {
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_MOUSEWHEEL = 0x020A;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_RBUTTONUP = 0x0205;
        public const uint WM_MBUTTONDOWN = 0x0207;
        public const uint WM_MBUTTONUP = 0x0208;
    }

    public class MouseEventData
    {
        public uint Message { get; set; }
        public HookPoint Point { get; set; }
    }

    public class LowLevelMouseHook : IDisposable
    {
        private const int WH_MOUSE_LL = 14;

        private IntPtr _hookId = IntPtr.Zero;
        private MouseHookNativeMethods.LowLevelMouseProc _proc;
        private bool _disposed;

        public event Action<MouseEventData> MouseEvent;

        public bool IsRunning => _hookId != IntPtr.Zero;

        public void Start()
        {
            if (_hookId != IntPtr.Zero)
                return;

            _proc = HookCallback;
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _hookId = MouseHookNativeMethods.SetWindowsHookEx(
                    WH_MOUSE_LL,
                    _proc,
                    MouseHookNativeMethods.GetModuleHandle(curModule.ModuleName),
                    0);
            }

            if (_hookId == IntPtr.Zero)
                throw new InvalidOperationException("Failed to install mouse hook.");
        }

        public void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                MouseHookNativeMethods.UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
            _proc = null;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var msg = (uint)wParam;
                var point = Marshal.PtrToStructure<HookPoint>(lParam);

                try
                {
                    MouseEvent?.Invoke(new MouseEventData
                    {
                        Message = msg,
                        Point = point
                    });
                }
                catch (Exception ex)
                {
                    RecorderErrorLog.RecordError(ex, "LowLevelMouseHook.Callback");
                }
            }

            return MouseHookNativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _disposed = true;
            }
        }
    }

    internal static class MouseHookNativeMethods
    {
        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
