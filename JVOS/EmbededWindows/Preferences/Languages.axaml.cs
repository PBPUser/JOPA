using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.Screens;
using System;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class Languages : UserControl, ISettingsPage
    {
        public Languages()
        {
            InitializeComponent();
        }

        public string Title => "Languages";

        public string InternalLink => "languages";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/language.png")));
    }
}
