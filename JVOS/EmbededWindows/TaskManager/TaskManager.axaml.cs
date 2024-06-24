using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using Newtonsoft.Json;
using System;
using System.Runtime.Loader;
using System.Text.Json.Serialization;

namespace JVOS.EmbededWindows.TaskManager
{
    public partial class TaskManager : WindowContentBase
    {
        TaskManagerVM VM;

        public TaskManager()
        {
            InitializeComponent();
            DataContext = VM = new();
            refreshBtn.Click += (a, b) => VM.Refresh();
            endBtn.Click += (a, b) =>
            {
                if (listProcess.SelectedItem == null)
                    return;
                try
                {
                    ((AssemblyLoadContext)listProcess.SelectedItem).Unload();
                }
                catch(Exception e)
                {
                    Communicator.ShowMessageDialog(new MessageDialog("Task Manager", JsonConvert.SerializeObject(e)));
                }
            };
            Title = "Task Manager";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/taskmgr.png")));
        }
    }
}
