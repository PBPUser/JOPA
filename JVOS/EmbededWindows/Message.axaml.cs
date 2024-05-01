using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using System;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows
{
    public partial class Message : UserControl, IJWindow
    {
        public Message()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            ((IJWindow)this).UpdateTitle("Message");
            ((IJWindow)this).UpdateIcon(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/message.png"))));
            base.OnLoaded(e);
        }

        private Subject<string> _titleSubject = new Subject<string>();
        private Subject<Bitmap> _iconSubject = new Subject<Bitmap>();
        private Bitmap _icon;
        private string _title = "Message";
        private IJWindowFrame _windowFrame;

        public IJWindow.WindowStartupLocation StartupLocation => IJWindow.WindowStartupLocation.Center;

        public string GetPanelId() => "jvos.system:message";

        public Subject<string> Title { get => _titleSubject; set => _titleSubject = value; }
        public string TitleValue { get => _title; set => _title = value; }
        public Subject<Bitmap> Icon { get => _iconSubject; set => _iconSubject = value; }
        public Bitmap IconValue { get => _icon; set => _icon = value; }
        public IJWindowFrame WindowFrame { get => _windowFrame; set => _windowFrame = value; }
    }
}
