﻿using JVOS.ApplicationAPI;
using JVOS.EmbededWindows.Preferences;
using JVOS.EmbededWindows.TaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JVOS.Screens.TaskbarCommands
{
    internal class OpenBarSettingsCommand : ICommand
    {
        public static readonly OpenBarSettingsCommand Instance = new();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Communicator.OpenWindow(new PreferencesHub("taskbar"));
        }
    }
}
