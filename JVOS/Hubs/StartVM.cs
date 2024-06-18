using JVOS.DataModel;
using JVOS.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Hubs
{
    internal class StartVM : ViewModelBase
    {
        public StartVM()
        {
            string dirPath = UserOptions.Current.GetPath("AppData\\StartMenu\\All\\");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            ListAll = Directory.GetFiles(dirPath).ToList();
        }

        public void Refresh()
        {
            string dirPath = UserOptions.Current.GetPath("AppData\\StartMenu\\All\\");
            ListAll = new();
            var x = Directory.GetFiles(dirPath).ToList();
            ListAll = x;
            if(ListAll.Count > 0)
                RefreshRecommended();
        }

        void RefreshRecommended()
        {
            ListRecommended = new();
            var list = new List<string>();
            int seed = new Random().Next();
            for(int i =0; i < 6; i++)
            {
                int r = new Random(seed + i).Next(0, ListAll.Count - 1);
                list.Add(ListAll[r]);
            }
            ListRecommended = list;
        }

        private List<string> pinned = new();
        private List<string> recommended = new();
        private List<string> all = new();
        private List<string> extendedrec = new();

        public List<string> ListRecommended
        {
            get => recommended;
            set => this.RaiseAndSetIfChanged(ref recommended, value);
        }

        public List<string> ListAll { 
            get => all;
            set
            {
                this.RaiseAndSetIfChanged(ref all, value);
            }
        }
    }
}
