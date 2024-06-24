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
    internal class OpenWallpaperSettingsCommand : ICommand
    {
        public static readonly OpenWallpaperSettingsCommand Instance = new();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Communicator.OpenWindow(new PreferencesHub("desktop"));
        }
    }
}
