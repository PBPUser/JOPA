using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.EmbededWindows.Preferences;
using JVOS.Hubs;
using JVOS.Screens;
using System;
using System.Linq;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class HubsAdd : UserControl, ISettingsPage
    {
        public HubsAdd()
        {
            InitializeComponent();
            AvailableHubs.ItemsSource = ApplicationManager.HubProviders;
            this.addBtn.Click += AddBtnClick;
            this.rmBtn.Click += RemBtnClick;
            Desktop = DesktopScreen.CurrentDesktop;
            UpdateExList();
        }

        private void RemBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(ExistingHubs.SelectedIndex != -1)
            Desktop.RemoveHub(ExistingHubs.SelectedIndex);
            UpdateExList();
        }

        DesktopScreen Desktop;

        public string Title => "Hubs";

        public string InternalLink => "hubs";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Taskbar/exhubs.png")));

        private void AddBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (AvailableHubs.SelectedIndex == -1)
                return;
            var hubp = ApplicationManager.HubProviders[AvailableHubs.SelectedIndex];
            Desktop.AddExternalHub(hubp);
            UpdateExList();
            Desktop.SaveExternalHubsList();
        }

        public void UpdateExList()
        {
            ExistingHubs.ItemsSource = new string[0];
            ExistingHubs.ItemsSource = Desktop.ExternalHubsList;
        }

        
    }
}
