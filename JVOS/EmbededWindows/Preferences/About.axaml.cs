using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.Screens;
using System;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class About : UserControl, ISettingsPage
    {
        public About()
        {
            InitializeComponent();
        }

        public string Title => "About";

        public string InternalLink => "about";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/app.png")));
    }
}
