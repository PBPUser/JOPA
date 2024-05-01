using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using System.Reactive.Subjects;
using System;
using JVOS.Controls;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class PreferencesHub : UserControl, IJWindow
    {
        public PreferencesHub()
        {
            InitializeComponent();
            AttachPages();
        }
        public string GetPanelId() => "jvos.system:perfs";

        void AttachPages()
        {
            AttachPage(new ColorsPage());
            AttachPage(new HubsAdd());
        }

        void AttachPage(ISettingsPage page)
        {
            var stack = new StackPanel()
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal
            };
            stack.Children.Add(new Image { Source = page.Icon, Width = 24, Height = 24 });
            stack.Children.Add(new TextBlock() { Text = page.Title });
            JButton button = new JButton()
            {
                Content = stack,
                Height = 32
            };
            button.Click += (a, b) => OpenPage(page);
            settingsColumns.Children.Add(button);
        }

        void OpenPage(ISettingsPage page)
        {
            if(!(page is Control))
                return;
            var pagec = page as Control;
            settingsViewr.Content = pagec;
            SettingsTitle.Text = page.Title;
        }

        public void WhenLoaded()
        {
            ((IJWindow)this).UpdateTitle("Preferences");
            ((IJWindow)this).UpdateIcon(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/preferences.png"))));
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
