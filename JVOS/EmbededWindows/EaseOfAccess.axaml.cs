using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.Controls;
using System;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows
{
    public partial class EaseOfAccess : UserControl, IJWindow
    {
        public EaseOfAccess()
        {
            InitializeComponent();
            apply.Click += (a, b) =>
            {
                ColorScheme.ApplyScheme(ColorScheme.Current, this.useDarkMode.IsChecked == true, useAccentTitle.IsChecked == true, useAccentBar.IsChecked == true);
            };
            rootStack.Children.Insert(1, new JVOS.Controls.DualPanelColorPicker() { Padding = new Avalonia.Thickness(8), Color = Colors.Violet, CornerRadius = new Avalonia.CornerRadius(8) });
        }

        public void WhenLoaded()
        {
            ((IJWindow)this).UpdateTitle("Ease of access");
            ((IJWindow)this).UpdateIcon(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Lockscreen/easeofaccess.png"))));
            App.SendNotification("Tess");
        }

        private Subject<string> _title = new Subject<string>();
        private Subject<Bitmap> _icon = new Subject<Bitmap>();
        private string _titleValue = "";
        private Bitmap _iconValue;
        private IJWindowFrame JWindowFrame;

        public IJWindow.WindowStartupLocation StartupLocation { get => IJWindow.WindowStartupLocation.Center; }
        public Subject<string> Title { get => _title; set => _title = value; }
        public Subject<Bitmap> Icon { get => _icon; set => _icon = value; }
        public string TitleValue { get => _titleValue; set => _titleValue = value; }
        public Bitmap IconValue { get => _iconValue; set => _iconValue = value; }
        public IJWindowFrame WindowFrame { get => JWindowFrame; set => JWindowFrame = value; }
    }
}
