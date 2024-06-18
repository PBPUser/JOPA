using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using JVOS.ApplicationAPI;
using JVOS.Screens;
using System;
using System.Linq;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class DesktopPage : UserControl, ISettingsPage
    {
        public DesktopPage()
        {
            InitializeComponent();
            var enumList = Enum.GetValues(typeof(Stretch)).Cast<Stretch>().ToArray();
            stetching.SelectedIndex = enumList.IndexOf((Stretch)App.Current.Resources["DesktopStretch"]);
            stetching.ItemsSource = enumList;
            stetching.SelectionChanged += (a, b) =>
            {
                if (stetching.SelectedIndex == -1)
                    return;
                App.Current.Resources["DesktopStretch"] = enumList[stetching.SelectedIndex];
                UserOptions.Current.SetUserValue("DesktopStretch", enumList[stetching.SelectedIndex]);
            };
            browse.Click += (a, b) => Communicator.RunCommand("shell://wallpaper");
        }

        public string Title => "Desktop";

        public string InternalLink => "desktop";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/desktop.png")));
    }
}
