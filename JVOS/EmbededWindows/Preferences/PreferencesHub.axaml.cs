using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using System.Reactive.Subjects;
using System;
using JVOS.Controls;
using JVOS.ApplicationAPI.Windows;
using System.Collections.Generic;
using ReactiveUI;
using System.Linq;
using System.Runtime.InteropServices;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class PreferencesHub : WindowContentBase
    {
        public PreferencesHub(string? page = null)
        {
            InitializeComponent();
            AttachPages();
            ISettingsPage? ipage = pages.FindAll(x => x.InternalLink == page).FirstOrDefault();
            if (ipage == null)
                ipage = pages[0];
            Loaded += (a,b) => OpenPage(ipage);
            Title = "Settings";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/preferences.png")));
        }
        public override string GetPanelId() => "jvos:settings";

        List<ISettingsPage> pages = new List<ISettingsPage>();

        void AttachPages() {
            AttachPage(new DesktopPage());
            AttachPage(new ColorsPage());
            AttachPage(new DisplayPage());
            AttachPage(new HubsAdd());
            AttachPage(new Applications());
        }

        void AttachPage(ISettingsPage page) {
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
            pages.Add(page);
        }

        void OpenPage(ISettingsPage page) {
            if(!(page is Control))
                return;
            var pagec = page as Control;
            settingsViewr.Content = pagec;
            SettingsTitle.Text = page.Title;
        }
    }
}
