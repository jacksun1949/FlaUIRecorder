using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CC_FlaUIAutomationRecoder.Internal
{
    public enum KeyEventType
    {
        Down = 0x0100,  // WM_KEYDOWN
        Up = 0x0101     // WM_KEYUP
    }

    public class KeyEventData
    {
        public string KeyName { get; set; }
        public KeyEventType EventType { get; set; }
    }

    public class LowLevelKeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private IntPtr _hookId = IntPtr.Zero;
        private KeyboardHookNativeMethods.LowLevelKeyboardProc _proc;
        private bool _disposed;

        public event Action<KeyEventData> KeyEvent;

        public bool IsRunning => _hookId != IntPtr.Zero;

        public void Start()
        {
            if (_hookId != IntPtr.Zero)
                return;

            _proc = HookCallback;
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _hookId = KeyboardHookNativeMethods.SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    _proc,
                    KeyboardHookNativeMethods.GetModuleHandle(curModule.ModuleName),
                    0);
            }

            if (_hookId == IntPtr.Zero)
                throw new InvalidOperationException("Failed to install keyboard hook.");
        }

        public void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                KeyboardHookNativeMethods.UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
            _proc = null;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var vkCode = Marshal.ReadInt32(lParam);
                var eventType = (int)wParam;

                if (eventType == WM_KEYDOWN || eventType == WM_SYSKEYDOWN ||
                    eventType == WM_KEYUP || eventType == WM_SYSKEYUP)
                {
                    var keyName = GetKeyName(vkCode);
                    try
                    {
                        KeyEvent?.Invoke(new KeyEventData
                        {
                            KeyName = keyName,
                            EventType = (eventType == WM_KEYDOWN || eventType == WM_SYSKEYDOWN)
                                ? KeyEventType.Down
                                : KeyEventType.Up
                        });
                    }
                    catch (Exception ex)
                    {
                        RecorderErrorLog.RecordError(ex, "LowLevelKeyboardHook.Callback");
                    }
                }
            }

            return KeyboardHookNativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static string GetKeyName(int vkCode)
        {
            // Use GetKeyNameText for accurate key names (handles L/R modifiers, etc.)
            var scanCode = KeyboardHookNativeMethods.MapVirtualKey((uint)vkCode, 0);
            // Extend scan code for extended keys (Right Alt, Right Ctrl, etc.)
            var isExtended = false;
            switch (vkCode)
            {
                case 0x21: // VK_PRIOR
                case 0x22: // VK_NEXT
                case 0x23: // VK_END
                case 0x24: // VK_HOME
                case 0x25: // VK_LEFT
                case 0x26: // VK_UP
                case 0x27: // VK_RIGHT
                case 0x28: // VK_DOWN
                case 0x2D: // VK_INSERT
                case 0x2E: // VK_DELETE
                    isExtended = true;
                    break;
            }

            if (isExtended)
                scanCode |= 0x100;

            var sb = new StringBuilder(256);
            var result = KeyboardHookNativeMethods.GetKeyNameText((int)(scanCode << 16), sb, sb.Capacity);
            if (result > 0)
                return sb.ToString();

            // Fallback: translate common virtual key codes
            return VkCodeToName(vkCode);
        }

        private static string VkCodeToName(int vkCode)
        {
            switch (vkCode)
            {
                case 0x08: return "Back";
                case 0x09: return "Tab";
                case 0x0D: return "Return";
                case 0x10: return "Shift";
                case 0x11: return "Control";
                case 0x12: return "Alt";
                case 0x13: return "Pause";
                case 0x14: return "Capital";
                case 0x1B: return "Escape";
                case 0x20: return "Space";
                case 0x21: return "PageUp";
                case 0x22: return "PageDown";
                case 0x23: return "End";
                case 0x24: return "Home";
                case 0x25: return "Left";
                case 0x26: return "Up";
                case 0x27: return "Right";
                case 0x28: return "Down";
                case 0x2C: return "Snapshot";
                case 0x2D: return "Insert";
                case 0x2E: return "Delete";
                case 0x5B: return "LWin";
                case 0x5C: return "RWin";
                case 0x5D: return "Apps";
                case 0x6A: return "Multiply";
                case 0x6B: return "Add";
                case 0x6D: return "Subtract";
                case 0x6E: return "Decimal";
                case 0x6F: return "Divide";
                case 0x90: return "Numlock";
                case 0x91: return "Scroll";
                case 0xA0: return "LShift";
                case 0xA1: return "RShift";
                case 0xA2: return "LControl";
                case 0xA3: return "RControl";
                case 0xA4: return "LAlt";
                case 0xA5: return "RAlt";
                default:
                    if (vkCode >= 0x30 && vkCode <= 0x39) // 0-9
                        return ((char)vkCode).ToString();
                    if (vkCode >= 0x41 && vkCode <= 0x5A) // A-Z
                        return ((char)vkCode).ToString();
                    if (vkCode >= 0x60 && vkCode <= 0x69) // Numpad 0-9
                        return "NumPad" + (vkCode - 0x60);
                    if (vkCode >= 0x70 && vkCode <= 0x7B) // F1-F12
                        return "F" + (vkCode - 0x6F);
                    if (vkCode >= 0xBA && vkCode <= 0xC0) // OEM keys
                        return VkOemToChar(vkCode);
                    return "VK_" + vkCode.ToString("X2");
            }
        }

        private static string VkOemToChar(int vkCode)
        {
            var buf = new StringBuilder(2);
            var scanCode = KeyboardHookNativeMethods.MapVirtualKey((uint)vkCode, 0);
            KeyboardHookNativeMethods.ToUnicode((uint)vkCode, scanCode, IntPtr.Zero, buf, buf.Capacity, 0);
            return buf.Length > 0 ? buf.ToString() : "OEM_" + vkCode.ToString("X2");
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

    internal static class KeyboardHookNativeMethods
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetKeyNameText(int lParam, [Out] StringBuilder lpString, int nSize);

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint wVirtKey, uint wScanCode, IntPtr lpKeyState, [Out] StringBuilder pwszBuff, int cchBuff, uint wFlags);
    }
}
