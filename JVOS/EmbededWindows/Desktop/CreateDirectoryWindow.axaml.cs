using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using JVOS.DataModel;
using Newtonsoft.Json;
using System;
using System.IO;

namespace JVOS.EmbededWindows.Desktop
{
    public partial class CreateShortcutWindow : WindowContentBase
    {
        public CreateShortcutWindow()
        {
            InitializeComponent();
            Title = "JVOS";
            Bitmap? icon = null;
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/add.png")));
            okBtn.Click += (a, b) =>
            {
                string str = UserOptions.Current.GetPath("Desktop") + "\\" + tb.Text  + ".jnk";
                if (File.Exists(str))
                {
                    Communicator.ShowMessageDialog(new MessageDialog("Warning", "Shortcut with name " + tb.Text + " already exists."));
                }
                else
                {
                    File.WriteAllText(str, JsonConvert.SerializeObject(new Shortcut(tx.Text ?? "", icon == null ? "" : UserOptions.ImageToBase64(icon), "")));
                    this.Frame.Close();
                    DesktopWindow.Current.VM.Refresh();
                }
            };
            browse.Click += (a, b) =>
            {
                var files = TopLevel.GetTopLevel(this).StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                }).GetAwaiter().GetResult();
                if (files.Count != 1)
                    return;
                try
                {
                    using var fs = files[0].OpenReadAsync().GetAwaiter().GetResult();
                    icon = new Bitmap(fs);
                    img.Source = icon;
                }
                catch
                {
                    Communicator.ShowMessageDialog(new MessageDialog("Error", "Bitmap failed to load"));
                    return;
                }
            };
            cancelBtn.Click += (a, b) => this.Frame.Close();
        }
    }
}
