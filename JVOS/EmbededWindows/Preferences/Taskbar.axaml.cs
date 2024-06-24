using Avalonia.Controls;
using Avalonia.Layout;
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
            taskbarAlignBox.SelectedIndex = UserOptions.Current.TaskbarAlignment == HorizontalAlignment.Left ? 0 : 1;
            hideApplicationTooltips.Click += (a, b) =>
            {
                UserOptions.Current.HideAppTooltips = hideApplicationTooltips.IsChecked == false;
                DesktopScreen.SetRunningAppButtonWidth(hideApplicationTooltips.IsChecked == false);
                UserOptions.Current.SaveUser();
            };
            taskbarAlignBox.SelectionChanged += (a, b) =>
            {
                DesktopScreen.CurrentDesktop.SetBarAlign(taskbarAlignBox.SelectedIndex == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Center, true);
            };
        }

        public string Title => "Taskbar";

        public string InternalLink => "taskbar";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/taskbar.png")));
    }
}
