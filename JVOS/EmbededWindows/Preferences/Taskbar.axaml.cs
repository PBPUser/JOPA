using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.Screens;
using System;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class Taskbar : UserControl, ISettingsPage
    {
        public Taskbar()
        {
            InitializeComponent();
            hideApplicationTooltips.IsChecked = UserOptions.Current.HideAppTooltips == false;
            hideApplicationTooltips.Click += (a, b) =>
            {
                UserOptions.Current.HideAppTooltips = hideApplicationTooltips.IsChecked == false;
                DesktopScreen.SetRunningAppButtonWidth(hideApplicationTooltips.IsChecked == false);
            };
        }

        public string Title => "Taskbar";

        public string InternalLink => "taskbar";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/taskbar.png")));
    }
}
