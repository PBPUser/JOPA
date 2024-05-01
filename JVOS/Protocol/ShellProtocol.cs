using Avalonia.Controls;
using JVOS.ApplicationAPI;
using JVOS.DataModel;
using JVOS.EmbededWindows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JVOS.Protocol
{
    public class ShellProtocol : IProtocol
    {
        public string Name => "shell";

        public bool Execute(string[] args)
        {
            switch (args[0])
            {
                case "run":
                    WindowManager.OpenInJWindow(new RunDialog());
                    return true;
                case "setres":
                case "setresource":
                    if (args.Length >= 3)
                    {
                        string[] rp = new string[args.Length - 2];
                        Array.Copy(args, 2, rp, 0, rp.Length);
                        ColorScheme.ResourceBunch.SetResource(args[1], string.Join(' ', rp));
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", "Resource has been set"));
                        return true;
                    }
                    return false;
                case "getres":
                case "getresource":
                    if (args.Length == 2)
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", ColorScheme.ResourceBunch.GetResource<string>(args[1])??"Resource not found"));
                        return true;
                    }
                    else if (args.Length == 1)
                    {
                        string s = "";
                        foreach (var x in App.Current.Resources)
                            s += $"{x.Key}={x.Value}\n";
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", s));
                        return true;
                    }
                    return false;
                case "wallpaper":
                    if (OperatingSystem.IsAndroid())
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", "This feature is not implemented on your platform."));
                        return false;
                    }
                    var toplevel = TopLevel.GetTopLevel(App.MainWindowInstance);
                    var files = toplevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions() {
                        AllowMultiple = false
                    }).GetAwaiter().GetResult();
                    if(files.Count >= 1)
                    {
                        UserSession.CurrentUser.UserOptions.SetWallpaper(files[0].Path.AbsolutePath);
                    }
                    return true;
                case "addappshort":
                    if(args.Length >= 3)
                    {
                        string name = args[1];
                        string[] commandx = new string[args.Length -2];
                        Array.Copy(args, 2, commandx, 0, commandx.Length);
                        Shortcut s = new Shortcut(string.Join(" ", commandx), name, "");
                        File.WriteAllText(UserOptions.Current.MenuDirectory + $"\\{name}.json", JsonConvert.SerializeObject(s));
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", "Shortcut created"));
                    }
                    else
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", "Too few arguments to create shortcut.\nExample shell://addappshort ShortcutName [args]"));
                    }
                    return true;
                default:
                    Communicator.ShowMessageDialog(new MessageDialog("Shell", "Command not found"));
                    return false;
            }
        }
    }
}
