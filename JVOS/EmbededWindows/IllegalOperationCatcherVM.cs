using JVOS.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.EmbededWindows
{
    public class IllegalOperationCatcherVM : ViewModelBase
    {
        public static string IllegialText = "This program has performed an illegal operation \nand will be shut down. \n\nIf the problem persists, contact the program \nvendor.";

        public IllegalOperationCatcherVM()
        {

        }

        private string details = "";
        public string Details
        {
            get => details;
            set => this.RaiseAndSetIfChanged(ref details, value);
        }
    }
}
