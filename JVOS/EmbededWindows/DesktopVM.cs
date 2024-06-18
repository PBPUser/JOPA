using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using JVOS.ApplicationAPI;
using JVOS.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.EmbededWindows
{
    public class DesktopVM :ViewModelBase
    {
        public DesktopVM()
        {

            string path = UserOptions.Current.GetPath("Desktop");
            FileSystemWatcher fsWatch = new()
            {
                Path = path
            };
            var x = Directory.GetFiles(path).ToList();
            x.AddRange(Directory.GetDirectories(path).ToList());
            Elements = x;
            fsWatch.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fsWatch.Created += (a,b) => Refresh();
            fsWatch.Renamed += (a, b) =>
            {
                Elements = _elements.Select(x => x == b.OldFullPath ? b.FullPath : x).ToList();
            };
            fsWatch.Deleted += (a, b) => Refresh();
            fsWatch.EnableRaisingEvents = true;
        }

        public void Refresh()
        {
            string path = UserOptions.Current.GetPath("Desktop");
            Elements = new List<string>();
            var x = Directory.GetFiles(path).ToList();
            x.AddRange(Directory.GetDirectories(path).ToList());
            x.Remove(path + "\\desktop.json");
            Elements = x;

        }

        private List<string> _elements = new();

        public List<string> Elements
        {
            get => _elements;
            set => this.RaiseAndSetIfChanged(ref _elements, value);
        }
    }
}
