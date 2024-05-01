using Avalonia.Media;

namespace JVOS
{
    public struct Theme
    {
        public Color BaseColor;
        public bool DarkScheme;
        public bool AccentTaskbar;
        public bool AccentTitlebars;

        public Theme(Color BaseColor, bool DarkScheme, bool AccentTaskbar, bool AccentTitlebars)
        {
            this.BaseColor = BaseColor;
            this.DarkScheme = DarkScheme;
            this.AccentTaskbar = AccentTaskbar;
            this.AccentTitlebars = AccentTitlebars;
        }

    }
}