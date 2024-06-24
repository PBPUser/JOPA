using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.Screens;
using System;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class ColorsPage : UserControl, ISettingsPage
    {
        public ColorsPage()
        {
            InitializeComponent();
            var x = new JVOS.Controls.DualPanelColorPicker() { Padding = new Avalonia.Thickness(8), Color = ColorScheme.Current.BasicColor, CornerRadius = new Avalonia.CornerRadius(8) };
            useAutoColor.Click += (a, b) =>
            {
                bool use = useAutoColor.IsChecked == true;
                UserOptions.Current.Theme.AutoColor = use;
                x.IsHitTestVisible = x.IsVisible = !use;
                UserOptions.Current.ReloadAutoColor();
            };
            useAccentBar.Click += (a, b) =>
            {
                bool use = useAccentBar.IsChecked == true;
                UserOptions.Current.Theme.AccentTaskbar = use;
                UserOptions.Current.ReloadAutoColor(false);
            };
            useDarkMode.Click += (a, b) =>
            {
                bool use = useDarkMode.IsChecked == true;
                UserOptions.Current.Theme.DarkScheme = use;
                UserOptions.Current.ReloadAutoColor(false);
            };
            useAccentTitle.Click += (a, b) =>
            {
                bool use = useAccentTitle.IsChecked == true;
                UserOptions.Current.Theme.AccentTitlebars = use;
                UserOptions.Current.ReloadAutoColor(false);
            };
            x.PointerReleased += (a, b) =>
            {
                UserOptions.Current.Theme.BaseColor = x.Color.Value;
                UserOptions.Current.ReloadAutoColor();
            };
            x.IsHitTestVisible = x.IsVisible = !UserOptions.Current.Theme.AutoColor;
            useDarkMode.IsChecked = UserOptions.Current.Theme.DarkScheme;
            useAccentBar.IsChecked = UserOptions.Current.Theme.AccentTaskbar;
            useAccentTitle.IsChecked = UserOptions.Current.Theme.AccentTitlebars;
            useAutoColor.IsChecked = UserOptions.Current.Theme.AutoColor;
            coloringStack.Children.Insert(2, x);
        }

        public string Title => "Colors";

        public string InternalLink => "colors";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/colors.png")));
    }
}
