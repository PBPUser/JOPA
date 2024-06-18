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
    internal class FilesVM : ViewModelBase
    {
        public FilesVM(string path = "", string filter = "*", bool showFiles=true)
        {
            ShowFiles = showFiles;
            this.filter = filter;
            LoadDirectory(path);
        }

        public void GoToDir(string path)
        {
            if(!path.EndsWith("\\"))
                path+= "\\";
            LoadDirectory(path);
        }

        private void LoadDirectory(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                path = "PC";
            if(Directory.Exists(path))
            {
                Elements = new List<string>();
                var x = Directory.GetDirectories(path, filter).ToList();
                if(ShowFiles)
                    x.AddRange(Directory.GetFiles(path, filter));
                Elements = x;
                Path = internalPath = path;
            }
            else if(path == "PC")
            {
                Elements = new List<string>();
                Elements = Environment.GetLogicalDrives().ToList();
            }
            else
            {
                Path = internalPath;
            }
        }

        private List<string> elements = new();
        private List<string> tree = new();
        private string filter = "*";
        private string path = "";
        private string internalPath = "";
        private bool ShowFiles = true;

        public string Path
        {
            get => path;
            set => this.RaiseAndSetIfChanged(ref path, value);
        }

        public List<string> Elements
        {
            get => elements;
            set => this.RaiseAndSetIfChanged(ref elements, value);
        }

        public List<string> Tree
        {
            get => tree;
            set => this.RaiseAndSetIfChanged(ref tree, value);
        }
    }
}
