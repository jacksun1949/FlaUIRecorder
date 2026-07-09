using System.Drawing;
using System.Windows.Forms;

namespace FlaUIRecorder.Internal
{
    public enum AppTheme
    {
        Light,
        Dark,
        HighContrast
    }

    public static class ThemeManager
    {
        public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

        public static void ApplyTheme(Form form, AppTheme theme)
        {
            CurrentTheme = theme;
            ApplyToControl(form, theme);
        }

        public static void ApplyToControl(Control control, AppTheme theme)
        {
            var colors = GetColors(theme);
            control.BackColor = colors.Back;
            control.ForeColor = colors.Fore;

            foreach (Control child in control.Controls)
            {
                if (child is MenuStrip || child is StatusStrip || child is ToolStrip)
                    continue;
                ApplyToControl(child, theme);
            }
        }

        private static (Color Back, Color Fore) GetColors(AppTheme theme)
        {
            switch (theme)
            {
                case AppTheme.Dark:
                    return (Color.FromArgb(32, 32, 32), Color.FromArgb(230, 230, 230));
                case AppTheme.HighContrast:
                    return (Color.Black, Color.Yellow);
                default:
                    return (Color.FromArgb(245, 245, 245), SystemColors.ControlText);
            }
        }
    }
}
