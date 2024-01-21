using Avalonia.Controls;
using JVOS.ApplicationAPI;
using JVOS.EmbededWindows;
using JVOS.Views;

namespace JVOS.Hubs
{
    public partial class StartHub : IHub
    {
        public StartHub()
        {
            InitializeComponent();
            _runButton.Click += OnRun;
            _thmButton.Click += OnThm;
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
