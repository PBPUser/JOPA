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
            apply.Click += (a, b) =>
            {
                ColorScheme.ApplyScheme(ColorScheme.CreateColorSchemeFromColor(x.Color.Value), this.useDarkMode.IsChecked == true, useAccentTitle.IsChecked == true, useAccentBar.IsChecked == true);
                UserOptions.Current.Theme.BaseColor = x.Color.Value;
                UserOptions.Save();
            };
            useAutoColor.Click += (a, b) =>
            {
                bool use = useAutoColor.IsChecked == true;
                UserOptions.Current.Theme.AutoColor = use;
                ColorScheme.ApplyScheme(ColorScheme.CreateColorSchemeFromColor(use ? ColorScheme.ColorFromBitmap(UserOptions.ImageToBytes(UserOptions.Current.DesktopBitmap??new Bitmap(AssetLoader.Open(new("avares://JVOS/Assets/Shell/app.png"))))) : x.Color.Value), this.useDarkMode.IsChecked == true, useAccentTitle.IsChecked == true, useAccentBar.IsChecked == true);
            };
            rootStack.Children.Insert(0, x);
        }

        public string Title => "Colors";

        public string InternalLink => "colors";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/colors.png")));
    }
}
