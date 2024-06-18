using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using System;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows
{
    public partial class Message : WindowContentBase
    {
        public Message()
        {
            InitializeComponent();
            Title = "Message";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/message.png")));
        }
        public override Size DefaultSize => new(512, 280);
    }
}
