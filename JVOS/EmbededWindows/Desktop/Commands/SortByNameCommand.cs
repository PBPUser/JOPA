using JVOS.ApplicationAPI;
using JVOS.EmbededWindows.Preferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JVOS.EmbededWindows.Desktop.Commands
{
    public class SortByNameCommand : ICommand
    {
        public static readonly SortByNameCommand Instance = new();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            DesktopWindow.Current.IconPlacementHelper.Sort(DesktopWindow.Current.VM.Elements, Comparer<string>.Default);
            DesktopWindow.Current.VM.Refresh();
        }
    }
}
