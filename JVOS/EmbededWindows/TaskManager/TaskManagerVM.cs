using JVOS.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.EmbededWindows.TaskManager
{
    public class TaskManagerVM : ViewModelBase
    {
        public TaskManagerVM() { 
            
        }

        public void Refresh()
        {
            LoadContexts = AssemblyLoadContext.All.ToList();
        }

        private List<AssemblyLoadContext> loadContexts;

        public List<AssemblyLoadContext> LoadContexts
        {
            get => loadContexts;
            set => this.RaiseAndSetIfChanged(ref loadContexts, value);
        }
    }
}
