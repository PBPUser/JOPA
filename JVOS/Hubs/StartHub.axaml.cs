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
            _refreshButton.Content = "<";

            _runButton.Click += RunBtnClick;
            _filesButton.Click += FilesBtnClick;
            _allButton.Click += SwitchApps;
            _moreButton.Click += SwitchRecents;
            _pinButton.Click += SwitchHome;
            _nazadRecommendBtn.Click += SwitchHome;
            _refreshButton.Click += RefreshBtnClick;
            _preferencesButton.Click += PreferencesBtnClick;

            translateTransform.Transitions = new Avalonia.Animation.Transitions() { new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(1000), Property = TranslateTransform.XProperty, Easing = new ElasticEaseOut() { }  } };
            clippedGrid.RenderTransform = translateTransform;
            DataContext = VM = new StartVM();
            AllApplications.ItemTemplate = StartItemDataTemplate;
            RecomendedApplications.ItemTemplate = MoreRecommendations.ItemTemplate = RecomItemDataTemplate;
            Opened += (a, b) =>
            {
                VM.Refresh();
                OpenPage(0);
            };
            VM.Refresh();
        }

        private void SwitchHome(object? sender, RoutedEventArgs e) => OpenPage(0);
        private void SwitchApps(object? sender, RoutedEventArgs e) => OpenPage(1);
        private void SwitchRecents(object? sender, RoutedEventArgs e) => OpenPage(2);

        static FuncDataTemplate<string> StartItemDataTemplate = new((value, namescope) =>
        {
            return new StartMenuItem(value);
        });
        static FuncDataTemplate<object> RecomItemDataTemplate = new((value, namescope) =>
        {
            return new RecommendMenuItem(value);
        });

        private void PreferencesBtnClick(object? sender, RoutedEventArgs e)
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

        private void RefreshBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            VM.Refresh();
        }

        public void OpenPage(int page = 0)
        {
            switch (page)
            {
                case 0:
                    translateTransform.X = 0;
                    break;
                case 1:
                    translateTransform.X = -clippyGrid.Bounds.Width;
                    MoreRecentsGrid.IsHitTestVisible = MoreRecentsGrid.IsVisible = false;
                    AllAppsGrid.IsHitTestVisible = AllAppsGrid.IsVisible = true;
                    break;
                case 2:
                    translateTransform.X = -clippyGrid.Bounds.Width;
                    MoreRecentsGrid.IsHitTestVisible = MoreRecentsGrid.IsVisible = true;
                    AllAppsGrid.IsHitTestVisible = AllAppsGrid.IsVisible = false;
                    break;
            }
        }

        private void FilesBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowManager.OpenInWindow(new FileBrowser());
            WindowManager.CloseAllHubsInActiveWindowSpace();
        }

        private void RunBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowManager.OpenInWindow(new RunDialog());
            WindowManager.CloseAllHubsInActiveWindowSpace();
        }
    }
}
