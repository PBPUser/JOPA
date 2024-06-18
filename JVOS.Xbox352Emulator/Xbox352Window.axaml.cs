using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;

namespace JVOS.Xbox352Emulator
{
    public partial class Xbox352Window : WindowContentBase
    {
        public Xbox352Window()
        {
            InitializeComponent();
            MinWidth = 800;
            MinHeight = 480;
            Title = "Real Xbox 352 Emulator (Trial)";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.Xbox352Emulator/Assets/icon.png")));
            bg.Source = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.Xbox352Emulator/Assets/xbox352.png")));
            loadBtn.Click += (a, b) =>
            {
                Communicator.ShowMessageDialog(new MessageDialog("Xbox 352 emulator", "THIS IS NOT VALID XB\nOX 352 IMAGE, INSEART REAL XB\nOX 342 IMAGE.", new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.Xbox352Emulator/Assets/icon.png")))));
            };
        }
    }
}
