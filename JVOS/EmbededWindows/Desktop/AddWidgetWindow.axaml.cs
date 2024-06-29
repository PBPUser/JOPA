using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI.Windows;
using JVOS.DataModel;
using Newtonsoft.Json;
using System;
using System.IO;

namespace JVOS.EmbededWindows.Desktop
{
    public partial class AddWidgetWindow : WindowContentBase
    {
        AddWidgetWindowVM VM;

        public AddWidgetWindow()
        {
            InitializeComponent();
            Title = "Add Widgets";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/add.png")));
            okBtn.Click += (a, b) =>
            {
                Frame.Close();
                if(widgetsList.SelectedItem != null)
                {
                    var wp = widgetsList.SelectedItem as WidgetPreview;
                    var dw = new DesktopWidget()
                    {
                        AutoUpdate = wp.AutoUpdating,
                        AutoUpdateCycleLength = wp.AutoUpdateCycleLength,
                        ClassName = wp.ClassName,
                        DllName = wp.DllName
                    };
                    string str = $"{UserOptions.Current.GetPath("Desktop")}\\Widget ({DateTime.Now.ToBinary()}).jwi";
                    File.WriteAllText(str, JsonConvert.SerializeObject(dw));
                    DesktopWindow.Current.VM.Refresh();
                }
            };
            DataContext = VM = new();
            cancelBtn.Click += (a, b) => this.Frame.Close();
        }
    }
}
