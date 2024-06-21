using Avalonia.Media;

namespace JVOS
{
    public struct Theme
    {
        public Color BaseColor = Color.FromRgb(40, 76, 100);
        public bool AutoColor;
        public bool DarkScheme;
        public bool AccentTaskbar;
        public bool AccentTitlebars;

        public Theme(Color BaseColor, bool autoColor, bool DarkScheme, bool AccentTaskbar, bool AccentTitlebars)
        {
            this.AutoColor = autoColor;
            this.BaseColor = BaseColor;
            this.DarkScheme = DarkScheme;
            this.AccentTaskbar = AccentTaskbar;
            this.AccentTitlebars = AccentTitlebars;
        }

    }
}