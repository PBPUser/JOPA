using Avalonia.Media;

namespace JVOS
{
    public struct Theme
    {
        public Color BaseColor = Color.FromRgb(40, 76, 100);
        public bool AutoColor = false;
        public bool DarkScheme = false;
        public bool AccentTaskbar = false;
        public bool AccentTitlebars = false;

        public Theme()
        {
            BaseColor = Color.FromRgb(40, 76, 100);
        }

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