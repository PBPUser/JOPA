using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JVOS.EmbededWindows.Desktop.Commands
{
    internal class CreateDirectoryCommand : ICommand
    {
        public static readonly CreateDirectoryCommand Instance = new CreateDirectoryCommand();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Communicator.OpenWindow(new CreateDirectoryWindow(true));
        }
    }
}
