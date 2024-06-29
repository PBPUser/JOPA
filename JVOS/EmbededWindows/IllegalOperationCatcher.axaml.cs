using Avalonia.Controls;
using Avalonia.Platform;
using JVOS.ApplicationAPI.Windows;

namespace JVOS.EmbededWindows
{
    public partial class IllegalOperationCatcher : WindowContentBase
    {
        IllegalOperationCatcherVM VM;

        public IllegalOperationCatcher(string details = "", string title = "")
        {
            InitializeComponent();
            DataContext = VM = new();
            VM.Details = details;
            Title = string.IsNullOrEmpty(title) ? "<unknown/>" : title;
            Icon = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new("avares://JVOS/Assets/errorjv.png")));
        }
    }
}
