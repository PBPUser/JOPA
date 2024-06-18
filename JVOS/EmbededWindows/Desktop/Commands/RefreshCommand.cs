using JVOS.ApplicationAPI;
using JVOS.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JVOS.EmbededWindows.Desktop.Commands
{
    internal class RefreshCommand : ICommand
    {
        public static readonly RefreshCommand Instance = new RefreshCommand();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            DesktopWindow.Current.VM.Refresh();
        }
    }
}
