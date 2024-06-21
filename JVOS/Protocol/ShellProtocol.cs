using Avalonia.Controls;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using JVOS.DataModel;
using JVOS.EmbededWindows;
using Newtonsoft.Json;
using SharpCompress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
                    WindowManager.OpenInWindow(new RunDialog());
                    return true;
                case "embeded":
                    if (args.Length != 2)
                    {
                        string s = "";
                        Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(typeof(WindowContentBase))).ForEach(x => s += x.FullName + "\n");
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", s));
                        return true;
                    }
                    var a = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(typeof(WindowContentBase))).Where(x => x.FullName.Equals(args[1])).FirstOrDefault();
                    if(a == null)
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", $"Class {args[1]} not found in assembly."));
                        return false;
                    }
                    var c = a.GetConstructors().Where(x => x.GetParameters().Where(x=>!x.IsOptional).Count() == 0).FirstOrDefault();
                    if(c == null)
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", $"Class {args[1]} dosen't contain empty constructor."));
                        return false;
                    }
                    int count = c.GetParameters().Count();
                    var d = (WindowContentBase)c.Invoke(new object[count]);
                    Communicator.OpenWindow(d);
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
                    if(args.Length == 1)
                    {
                        Communicator.BrowseFile(a =>
                        {
                            if (a.IsSuccessfull)
                                UserSession.CurrentUser.UserOptions.SetDesktopImage(new(Path.GetFullPath(a.Path.First())));
                        });
                    }
                    else if(args.Length == 2)
                        if (File.Exists(args[1]))
                            UserSession.CurrentUser.UserOptions.SetDesktopImage(new(args[1]));
                        else if(AssetLoader.Exists(new Uri(args[1])))
                            UserSession.CurrentUser.UserOptions.SetDesktopImage(new(AssetLoader.Open(new Uri(args[1]))));
                        else
                            Communicator.ShowMessageDialog(new MessageDialog("Shell", "File dosen't exists."));
                    return true;
                case "profile":
                    if (OperatingSystem.IsAndroid())
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", "This feature is not implemented on your platform."));
                        return false;
                    }
                    var toplevel2 = TopLevel.GetTopLevel(App.MainWindowInstance);
                    var files2 = toplevel2.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions()
                    {
                        AllowMultiple = false
                    }).GetAwaiter().GetResult();
                    if (files2.Count >= 1)
                        UserSession.CurrentUser.UserOptions.SetProfileImage(new(files2[0].Path.AbsolutePath));
                    return true;
                case "runtimeinfo":

                    return true;
                case "lockscreenimage":
                    if (OperatingSystem.IsAndroid())
                    {
                        Communicator.ShowMessageDialog(new MessageDialog("Shell", "This feature is not implemented on your platform."));
                        return false;
                    }
                    var toplevel3 = TopLevel.GetTopLevel(App.MainWindowInstance);
                    var files3 = toplevel3.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions()
                    {
                        AllowMultiple = false
                    }).GetAwaiter().GetResult();
                    if (files3.Count >= 1)
                        UserSession.CurrentUser.UserOptions.SetProfileImage(new(files3[0].Path.AbsolutePath));
                    return true;
                default:
                    Communicator.ShowMessageDialog(new MessageDialog("Shell", "Command not found"));
                    return false;
            }
        }
    }
}
