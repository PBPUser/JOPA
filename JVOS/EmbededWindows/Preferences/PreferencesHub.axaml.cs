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
using System.Reflection;
using SharpCompress;
using Avalonia.Animation;
using Avalonia.Media;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Threading;

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
            settingsViewr.Transitions = new Avalonia.Animation.Transitions();
            settingsViewr.Transitions.Add(new DoubleTransition()
            {
                Duration = TimeSpan.FromMilliseconds(175),
                Property = OpacityProperty
            });
            transform.Transitions = new();
            transform.Transitions.Add(new DoubleTransition()
            {
                Duration = TimeSpan.FromMilliseconds(175),
                Property = TranslateTransform.YProperty
            });
            settingsViewr.RenderTransform = transform;
        }
        public override string GetPanelId() => "jvos:settings";

        TranslateTransform transform = new TranslateTransform();

        List<ISettingsPage> pages = new List<ISettingsPage>();

        void AttachPages() {
            var pages = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(typeof(ISettingsPage)) && !x.IsInterface);
            foreach(var x in pages)
            {
                var constr = x.GetConstructors().Where(j => j.GetParameters().Length == 0);
                if (constr.Count() == 0)
                    continue;
                AttachPage((ISettingsPage)constr.First().Invoke(new object[0]));
            }
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
            settingsViewr.Opacity = 0;
            transform.Y = Bounds.Height / 4;

            Task.Run(() =>
            {
                Thread.Sleep(175);
                Dispatcher.UIThread.Invoke(() =>
                {
                    var pagec = page as Control;
                    settingsViewr.Content = pagec;
                    settingsViewr.Opacity = 1;
                    transform.Y = 0;
                    SettingsTitle.Text = page.Title;
                });
            });
        }
    }
}
