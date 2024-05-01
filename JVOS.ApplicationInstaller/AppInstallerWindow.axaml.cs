using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using System.Reactive.Subjects;
using ZstdSharp.Unsafe;

namespace JVOS.ApplicationInstaller
{
    public partial class AppInstallerWindow : UserControl, IJWindow
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
        }

        public IJWindow.WindowStartupLocation StartupLocation => IJWindow.WindowStartupLocation.Center;

        private Subject<string> _title = new Subject<string>();
        private Subject<Bitmap> _icon = new Subject<Bitmap>();

        private string title = "Installer";
        private Bitmap icon;

        private IJWindowFrame _frame;

        public Subject<string> Title { get => _title; set => _title = value; }
        public string TitleValue { get => title; set => throw new NotImplementedException(); }
        public Subject<Bitmap> Icon { get => _icon; set => throw new NotImplementedException(); }
        public Bitmap IconValue { get => icon; set { Icon.OnNext(icon); icon = value; } }
        public IJWindowFrame WindowFrame { get => _frame; set => _frame = value; }

        public void WhenLoaded()
        {
            ((IJWindow)this).UpdateTitle("Application Installer");
            ((IJWindow)this).UpdateIcon(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.ApplicationInstaller/Assets/icon.png"))));
        }

        public string GetPanelId()
        {
            return "jvos.appinstaller:mainwindow";
        }
    }
}
