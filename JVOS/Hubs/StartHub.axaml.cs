using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Hub;
using JVOS.Controls;
using JVOS.DataModel;
using JVOS.EmbededWindows;
using JVOS.EmbededWindows.Preferences;
using JVOS.Screens;
using JVOS.Views;
using Newtonsoft.Json;
using System;
using System.IO;

namespace JVOS.Hubs
{
    public partial class StartHub : HubWindow
    {
        StartVM VM;

        public StartHub()
        {
            InitializeComponent();
            _runButton.Click += OnRun;
            _thmButton.Click += OnThm;
            _allButton.Click += OnAll;
            _pinButton.Click += OnPin;
            _refButton.Click += OnRef;
            _hubButton.Click += OnHub;

            translateTransform.Transitions = new Avalonia.Animation.Transitions() { new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(1000), Property = TranslateTransform.XProperty, Easing = new ElasticEaseOut() { }  } };
            clippedGrid.RenderTransform = translateTransform;
            DataContext = VM = new StartVM();
            AllApplications.ItemTemplate = StartItemDataTemplate;
            RecomendedApplications.ItemTemplate = RecomItemDataTemplate;
            Opened += (a, b) => VM.Refresh();
            VM.Refresh();
        }

        static FuncDataTemplate<string> StartItemDataTemplate = new((value, namescope) =>
        {
            return new StartMenuItem(value);
        });
        static FuncDataTemplate<string> RecomItemDataTemplate = new((value, namescope) =>
        {
            return new RecommendMenuItem(value);
        });

        private void OnHub(object? sender, RoutedEventArgs e)
        {
            WindowManager.OpenInWindow(new PreferencesHub());
        }

        TranslateTransform translateTransform = new TranslateTransform();

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            clippedGrid.Width = clippyGrid.Bounds.Width * 2;
            base.OnSizeChanged(e);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            
            clippedGrid.Width = clippyGrid.Bounds.Width * 2;
            base.OnLoaded(e);
        }

        private void OnRef(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            VM.Refresh();
        }

        private void OnPin(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            translateTransform.X = 0;
        }

        private void OnAll(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            translateTransform.X = -clippyGrid.Bounds.Width;
        }

        private void OnThm(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowManager.OpenInWindow(new EaseOfAccess());
            WindowManager.CloseAllHubsInActiveWindowSpace();
        }

        private void OnRun(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowManager.OpenInWindow(new RunDialog());
            WindowManager.CloseAllHubsInActiveWindowSpace();
        }
    }
}
