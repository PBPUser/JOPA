using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.Screens;
using System;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class DisplayPage : UserControl, ISettingsPage
    {
        public DisplayPage()
        {
            InitializeComponent();
        }

        public string Title => "Display";

        public string InternalLink => "display";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/monitor.png")));
    }
}
