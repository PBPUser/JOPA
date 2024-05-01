using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using JVOS.ApplicationAPI;
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
    public partial class StartHub : IHub
    {
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
        }

        private void OnHub(object? sender, RoutedEventArgs e)
        {
            WindowManager.OpenInJWindow(new PreferencesHub());
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
            WindowManager.OpenInJWindow(new EaseOfAccess());
            WindowManager.CloseAllHubsInActiveWindowSpace();
        }

        private void OnRun(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowManager.OpenInJWindow(new RunDialog());
            WindowManager.CloseAllHubsInActiveWindowSpace();
        }
    }
}
