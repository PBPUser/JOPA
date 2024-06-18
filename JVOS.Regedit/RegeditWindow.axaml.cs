using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI.Windows;
using System;

namespace JVOS.Regedit
{
    public partial class RegeditWindow : WindowContentBase
    {
        public RegeditWindow()
        {
            InitializeComponent();
            Width = 800;
            Height = 480;

            Title = "Registry Editor";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.Regedit/Assets/Icon.png")));

            regeditImage.Stretch = Avalonia.Media.Stretch.Fill;
            regeditImage.Source = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.Regedit/Assets/Registry.png")));
        }
    }
}
