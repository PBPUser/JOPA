using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Hub;
using JVOS.EmbededWindows.Preferences;
using JVOS.Hubs;
using JVOS.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using static JVOS.Screens.DesktopScreen.HubManager.HubConfig;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class HubsAdd : UserControl, ISettingsPage
    {
        static Bitmap DEFAULT_ICON = new Bitmap(AssetLoader.Open(new("avares://JVOS/Assets/Taskbar/exhubs.png")));

        public HubsAdd()
        {
            InitializeComponent();
            AvailableHubs.ItemsSource = ApplicationManager.HubProviders;
            this.addBtn.Click += AddBtnClick;
            this.rmBtn.Click += RemBtnClick;
            this.dwBtn.Click += DownBtnClick;
            this.upBtn.Click += UpBtnClick;
            Desktop = DesktopScreen.CurrentDesktop;
            AvailableHubs.ItemTemplate = HubDataTemplate;
            UpdateExList();
            UpdateAvailableList();
        }

        static FuncDataTemplate<HubConfigurationStructure> HubDataTemplate = new FuncDataTemplate<HubConfigurationStructure>((a, b) =>
        {
            DockPanel dock = new();
            Image icon = new() { Width = 64, Height = 64 };
            icon.Source = a.Icon ?? DEFAULT_ICON;
            TextBlock blockTitle = new(){ Text = a.HubName };
            blockTitle.Classes.Add("h3");
            TextBlock blockDescription = new(){ Text = a.Description };
            TextBlock blockProvider = new(){ Text = a.Provider, FontStyle = Avalonia.Media.FontStyle.Italic };
            DockPanel.SetDock(icon, Dock.Left);
            DockPanel.SetDock(blockTitle, Dock.Top);
            DockPanel.SetDock(blockDescription, Dock.Top);
            DockPanel.SetDock(blockProvider, Dock.Top);
            dock.Children.Add(icon);
            dock.Children.Add(blockTitle);
            dock.Children.Add(blockDescription);
            dock.Children.Add(blockProvider);
            return dock;
        });
        private void DownBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var selectedbox = ExistingHubsTabs.SelectedIndex == 0 ? LeftExistingHubs : ExistingHubsTabs.SelectedIndex == 1 ? CenterExistingHubs : RightExistingHubs;
            if (selectedbox.SelectedIndex == -1 || selectedbox.SelectedIndex >= selectedbox.ItemCount - 1)
                return;
            var selectedlistc = ExistingHubsTabs.SelectedIndex == 0 ? Desktop.HubMan.Config.Left : ExistingHubsTabs.SelectedIndex == 1 ? Desktop.HubMan.Config.Center : Desktop.HubMan.Config.Right;
            var selectedlist = ExistingHubsTabs.SelectedIndex == 0 ? Desktop.HubMan.LeftDesktopHubs : ExistingHubsTabs.SelectedIndex == 1 ? Desktop.HubMan.CenterDesktopHubs : Desktop.HubMan.RightDesktopHubs;
            var hub = selectedlist[selectedbox.SelectedIndex];
            var s = ExistingHubsTabs.SelectedIndex == 0 ? HorizontalAlignment.Left : ExistingHubsTabs.SelectedIndex == 1 ? HorizontalAlignment.Center : HorizontalAlignment.Right;
            var hubc = selectedlistc[selectedbox.SelectedIndex];
            Desktop.MoveHub(hub, hubc, s, selectedbox.SelectedIndex + 1);
            UpdateExList();
        }

        private void UpBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var selectedbox = ExistingHubsTabs.SelectedIndex == 0 ? LeftExistingHubs : ExistingHubsTabs.SelectedIndex == 1 ? CenterExistingHubs : RightExistingHubs;
            if (selectedbox.SelectedIndex <= 0)
                return;
            var selectedlistc = ExistingHubsTabs.SelectedIndex == 0 ? Desktop.HubMan.Config.Left : ExistingHubsTabs.SelectedIndex == 1 ? Desktop.HubMan.Config.Center : Desktop.HubMan.Config.Right;
            var selectedlist = ExistingHubsTabs.SelectedIndex == 0 ? Desktop.HubMan.LeftDesktopHubs : ExistingHubsTabs.SelectedIndex == 1 ? Desktop.HubMan.CenterDesktopHubs : Desktop.HubMan.RightDesktopHubs;
            var hub = selectedlist[selectedbox.SelectedIndex];
            var s = ExistingHubsTabs.SelectedIndex == 0 ? HorizontalAlignment.Left : ExistingHubsTabs.SelectedIndex == 1 ? HorizontalAlignment.Center : HorizontalAlignment.Right;
            var hubc = selectedlistc[selectedbox.SelectedIndex];
            Desktop.MoveHub(hub, hubc, s, selectedbox.SelectedIndex - 1);
            UpdateExList();
        }

        private void RemBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var selectedbox = ExistingHubsTabs.SelectedIndex == 0 ? LeftExistingHubs : ExistingHubsTabs.SelectedIndex == 1 ? CenterExistingHubs : RightExistingHubs;
            if (selectedbox.SelectedIndex == -1)
                return;
            var selectedlist = ExistingHubsTabs.SelectedIndex == 0 ? Desktop.HubMan.LeftDesktopHubs : ExistingHubsTabs.SelectedIndex == 1 ? Desktop.HubMan.CenterDesktopHubs : Desktop.HubMan.RightDesktopHubs;
            var selectedlistc = ExistingHubsTabs.SelectedIndex == 0 ? Desktop.HubMan.Config.Left : ExistingHubsTabs.SelectedIndex == 1 ? Desktop.HubMan.Config.Center : Desktop.HubMan.Config.Right;
            var hub = selectedlist[selectedbox.SelectedIndex];
            var hubc = selectedlistc[selectedbox.SelectedIndex];
            Desktop.RemoveHub(hubc, hub);
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
            var hubp = types[AvailableHubs.SelectedIndex];
            var s = ExistingHubsTabs.SelectedIndex == 0 ? HorizontalAlignment.Left : ExistingHubsTabs.SelectedIndex == 1 ? HorizontalAlignment.Center : HorizontalAlignment.Right;
            Desktop.AddHub(hubp,s);
            UpdateExList();
        }

        List<HubConfigurationStructure> types;

        private void UpdateAvailableList()
        {
            AvailableHubs.ItemsSource = new HubConfigurationStructure[0];
            types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(typeof(HubProvider))).Where(x => x.GetConstructors().Where(x => x.GetParameters().Length == 0).Count() > 0).Select(x => TypeToHubInformationStructure(x, "")).ToList();
            
            types.AddRange(Directory
                .GetDirectories(PlatformSpecifixController.GetLocalFilePath("Hubs"))
                .Where(x => File.Exists(Path.Combine(x, "manifest.jvon")))
                .Select(y => Path.GetRelativePath(Directory.GetCurrentDirectory(), y))
                .SelectMany(z => GetHubConfigurationStructures(z)));
            AvailableHubs.ItemsSource = types;
        }

        private DesktopScreen.HubManager.HubConfig.HubConfigurationStructure[] GetHubConfigurationStructures(string path)
        {
            var manifest = HubManager.GetHubManifest(Path.Combine(path, "manifest.jvon"));
            return manifest.ProvidenNames.Select(x => GetExternalHubConfig(x, path, "", "", null)).ToArray();
        }

        private DesktopScreen.HubManager.HubConfig.HubConfigurationStructure GetExternalHubConfig(string hubName, string path, string provider, string description, Bitmap? image )
        {
            return new DesktopScreen.HubManager.HubConfig.HubConfigurationStructure()
            {
                HubName = hubName,
                Path = path,
                Description = description,
                Provider = provider,
                Icon = image
            };
        }

        private DesktopScreen.HubManager.HubConfig.HubConfigurationStructure TypeToHubInformationStructure(Type t, string path)
        {
            return new DesktopScreen.HubManager.HubConfig.HubConfigurationStructure()
            {
                HubName = t.ToString(),
                Path = path,
                Icon = ((Bitmap?)t.GetField("Icon", BindingFlags.Static)?.GetValue(null)),
                Description= (string)t.GetField("Description", BindingFlags.Static)?.GetValue(null),
                Provider = "Built-in to JVOS"
            };
        }

        public void UpdateExList()
        {
            LeftExistingHubs.ItemsSource = CenterExistingHubs.ItemsSource = RightExistingHubs.ItemsSource = new string[0];
            LeftExistingHubs.ItemsSource = Desktop.HubMan.LeftDesktopHubs;
            CenterExistingHubs.ItemsSource = Desktop.HubMan.CenterDesktopHubs;
            RightExistingHubs.ItemsSource = Desktop.HubMan.RightDesktopHubs;
        }

        
    }
}
