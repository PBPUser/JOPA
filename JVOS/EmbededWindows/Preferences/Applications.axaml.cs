using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.Screens;
using System;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class Applications : UserControl, ISettingsPage
    {
        public Applications()
        {
            InitializeComponent();
        }

        public string Title => "Applications";

        public string InternalLink => "applications";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/app.png")));
    }
}
