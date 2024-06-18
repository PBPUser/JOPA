using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using System.Reactive.Subjects;
using ZstdSharp.Unsafe;

namespace JVOS.ApplicationInstaller
{
    public partial class AppInstallerWindow : WindowContentBase
    {
        public AppInstallerWindow()
        {
            InitializeComponent();
            installBtn.Click += (a, b) =>
            {
                var topLevel = TopLevel.GetTopLevel(this);
                var files = topLevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions() { AllowMultiple = false, Title = "Select package" }).GetAwaiter().GetResult();
                if(files.Count == 1)
                {
                    var fileInfo = new FileInfo(files[0].Path.AbsolutePath);
                    if (!SharpCompress.Archives.SevenZip.SevenZipArchive.IsSevenZipFile(fileInfo))
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Application Installer", "Selected file is not JOPA Application."));
                        return;
                    }
                    installGrid.IsVisible = true;
                    installBtn.IsVisible = false;
                }
            };
            Title = "Application Installer";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.ApplicationInstaller/Assets/icon.png")));
        }


        public override string GetPanelId()
        {
            return "jvos.appinstaller:mainwindow";
        }
    }
}
