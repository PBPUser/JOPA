using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using JVOS.ApplicationAPI;
using JVOS.Protocol;
using JVOS.Screens;
using System;
using System.Linq;

namespace JVOS.EmbededWindows.Preferences
{
    public partial class DesktopPage : UserControl, ISettingsPage
    {
        DesktopSettingsVM VM;

        public DesktopPage()
        {
            InitializeComponent();
            var enumList = Enum.GetValues(typeof(Stretch)).Cast<Stretch>().ToArray();
            stetching.SelectedIndex = enumList.IndexOf((Stretch)App.Current.Resources["DesktopStretch"]);

            stetching.ItemsSource = enumList;
            stetching.SelectionChanged += (a, b) =>
            {
                if (stetching.SelectedIndex == -1)
                    return;
                App.Current.Resources["DesktopStretch"] = enumList[stetching.SelectedIndex];
                UserOptions.Current.SetUserValue("DesktopStretch", enumList[stetching.SelectedIndex]);
            };
            DataContext = VM = new DesktopSettingsVM();
            includedImagesList.ItemTemplate = DesktopImageDataTemplate;
            includedImagesList.PointerReleased += IncludedImagesList_PointerReleased;
            browse.Click += (a, b) => Communicator.RunCommand("shell://wallpaper");
        }

        private void IncludedImagesList_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            if (includedImagesList.SelectedItem == null)
                return;
            UserOptions.Current.SetDesktopImage(new (AssetLoader.Open((Uri)includedImagesList.SelectedItem)));
        }

        static FuncDataTemplate<Uri> DesktopImageDataTemplate = new FuncDataTemplate<Uri>((a, b) =>
        {
            Grid g = new Grid() { MaxWidth = 128, MaxHeight = 128, Margin = new(4) };
            Border border = new Border();
            border.Classes.Add("Transparent");
            border.Classes.Add("Outer");
            g.Children.Add(new Image
            {
                Source = new Bitmap(AssetLoader.Open(a))
            });
            g.Children.Add(border);
            return g;
        });

        public string Title => "Desktop";

        public string InternalLink => "desktop";

        public Bitmap Icon => new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/desktop.png")));
    }
}
