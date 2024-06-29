using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using System;
using System.IO;

namespace JVOS.EmbededWindows.Desktop
{
    public partial class CreateDirectoryWindow : WindowContentBase
    {
        public CreateDirectoryWindow(bool createDir)
        {
            InitializeComponent();
            Title = "JVOS";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/add.png")));
            okBtn.Click += (a, b) =>
            {
                string str = UserOptions.Current.GetPath("Desktop") + "\\" + tb.Text;
                if (Directory.Exists(str))
                {
                    Communicator.ShowMessageDialog(new MessageDialog("Warning", "Directory with name " + tb.Text + " already exists."));
                }
                else
                {
                    Directory.CreateDirectory(str);
                    DesktopWindow.Current.VM.Refresh();
                    this.Frame.Close();
                }
            };
            cancelBtn.Click += (a, b) => this.Frame.Close();
        }
    }
}
