using JVOS.ApplicationAPI;
using JVOS.EmbededWindows.TaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JVOS.Screens.TaskbarCommands
{
    internal class OpenTaskmanagerCommand : ICommand
    {
        public static readonly OpenTaskmanagerCommand Instance = new OpenTaskmanagerCommand();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Communicator.OpenWindow(new TaskManager());
        }
    }
}
