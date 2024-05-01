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
            var x = new JVOS.Controls.DualPanelColorPicker() { Padding = new Avalonia.Thickness(8), Color = Colors.Violet, CornerRadius = new Avalonia.CornerRadius(8) };
            apply.Click += (a, b) =>
            {
                ColorScheme.ApplyScheme(ColorScheme.CreateColorSchemeFromColor(x.Color.Value), this.useDarkMode.IsChecked == true, useAccentTitle.IsChecked == true, useAccentBar.IsChecked == true);
                UserOptions.Current.ColorScheme = ColorScheme.Current;
                UserOptions.Save();
            };
            hideApplicationTooltips.IsChecked = UserOptions.Current.HideAppTooltips == false;
            hideApplicationTooltips.Click += (a, b) =>
            {
                UserOptions.Current.HideAppTooltips = hideApplicationTooltips.IsChecked == false;
                DesktopScreen.SetRunningAppButtonWidth(hideApplicationTooltips.IsChecked == false);
            };
            rootStack.Children.Insert(0, x);
        }

        public string Title => "Colors";

        public string InternalLink => "colors";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/colors.png")));
    }
}
