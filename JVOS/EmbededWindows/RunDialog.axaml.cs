using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using System;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows
{
    public partial class RunDialog : UserControl, IJWindow
    {
        public RunDialog()
        {
            InitializeComponent();
            okBtn.Click += OkButton;
        }

        private void OkButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ((IJWindow)this).UpdateTitle(promptTb.Text);
        }

        private Subject<string> _title = new Subject<string>();
        private Subject<Bitmap> _icon = new Subject<Bitmap>();
        private string _titleValue = "";
        private Bitmap _iconValue;


        public void WhenLoaded()
        {
            ((IJWindow)this).UpdateTitle("Run");
            ((IJWindow)this).UpdateIcon(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/run.png"))));
            App.SendNotification("Under construction");

        }

        public IJWindow.WindowStartupLocation StartupLocation { get => IJWindow.WindowStartupLocation.BottomLeft; }
        public Subject<string> Title { get => _title; set => _title = value; }
        public Subject<Bitmap> Icon { get => _icon; set => _icon = value; }
        public string TitleValue { get => _titleValue; set => _titleValue = value; }
        public Bitmap IconValue { get => _iconValue; set => _iconValue = value; }
    }
}
