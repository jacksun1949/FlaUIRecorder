using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlaUIRecorder.CodeProvider.Common
{
    public static class KeyPressCodeGenerator
    {
        private static readonly HashSet<string> ModifierKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CTRL", "CONTROL", "SHIFT", "ALT"
        };

        public static bool IsCompoundKey(string keyName)
        {
            return !string.IsNullOrEmpty(keyName) && keyName.Contains("+");
        }

        public static string BuildCSharpKeyPress(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return string.Empty;

            if (!IsCompoundKey(keyName))
                return $"FlaUI.Core.Input.Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.{NormalizeVirtualKey(keyName)});";

            var parts = keyName.Split('+').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToArray();
            if (parts.Length == 0)
                return string.Empty;

            var modifiers = new List<string>();
            string mainKey = null;

            foreach (var part in parts)
            {
                if (ModifierKeys.Contains(part))
                    modifiers.Add(MapModifier(part));
                else
                    mainKey = NormalizeVirtualKey(part);
            }

            if (string.IsNullOrEmpty(mainKey))
                mainKey = NormalizeVirtualKey(parts[parts.Length - 1]);

            if (modifiers.Count == 0)
                return $"FlaUI.Core.Input.Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.{mainKey});";

            var withChain = string.Join(", ", modifiers.Select(m => $"FlaUI.Core.WindowsAPI.VirtualKeyShort.{m}"));
            return $"FlaUI.Core.Input.Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.{mainKey}).With({withChain});";
        }

        public static string BuildPowerShellKeyPress(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return string.Empty;

            if (!IsCompoundKey(keyName))
                return $"[FlaUI.Core.Input.Keyboard]::Press([FlaUI.Core.WindowsAPI.VirtualKeyShort]::{NormalizeVirtualKey(keyName)})";

            var csharp = BuildCSharpKeyPress(keyName);
            return csharp;
        }

        private static string MapModifier(string modifier)
        {
            switch (modifier.ToUpperInvariant())
            {
                case "CTRL":
                case "CONTROL":
                    return "CONTROL";
                case "SHIFT":
                    return "SHIFT";
                case "ALT":
                    return "MENU";
                default:
                    return modifier.ToUpperInvariant();
            }
        }

        public static string NormalizeVirtualKey(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return keyName;

            switch (keyName.ToUpperInvariant())
            {
                case "RETURN":
                    return "ENTER";
                case "ESCAPE":
                    return "ESCAPE";
                case "PAGEUP":
                    return "PRIOR";
                case "PAGEDOWN":
                    return "NEXT";
                case "LEFT":
                    return "LEFT";
                case "RIGHT":
                    return "RIGHT";
                case "UP":
                    return "UP";
                case "DOWN":
                    return "DOWN";
                case "HOME":
                    return "HOME";
                case "END":
                    return "END";
                case "DELETE":
                    return "DELETE";
                case "BACK":
                case "BACKSPACE":
                    return "BACK";
                case "SPACE":
                    return "SPACE";
                default:
                    if (keyName.Length == 1)
                        return keyName.ToUpperInvariant();
                    return keyName.ToUpperInvariant();
            }
        }
    }
}
