﻿using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Simple;
using Avalonia.Threading;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.App;
using JVOS.Controls;
using JVOS.DataModel;
using JVOS.EmbededWindows;
using JVOS.Protocol;
using JVOS.ViewModels;
using JVOS.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using Application = Avalonia.Application;

namespace JVOS;

public partial class App : Application
{
    public static MainWindow MainWindowInstance;

    public bool IsGodot = false;

    public static void SendNotification(string message, double time = 3000,bool isDebug = true)
    {
        
        bool doReturn = isDebug;
#if DEBUG
        doReturn = false;
#endif
        if (MainView.GLOBAL == null || doReturn)
        {
            if(MainView.GLOBAL == null)
            {
                new Window() { Content = new TextBlock() { Text = message } }.ShowDialog(null);
                return;
            }
            return;
        }
        TextBlock NotificationText = new TextBlock() { Margin = new Thickness(4, 0), Text = message, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left };
        Border Border = new Border() { Height = 2, Width = 0, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Background = isDebug ? (Brush)App.Current.Resources["BasicBackground"] : (Brush)App.Current.Resources["AccentBackground"], Transitions = new Avalonia.Animation.Transitions() };
        Grid NotificationGrid = new Grid() { Children = { NotificationText, Border }, Transitions = new Transitions(), Height = 0, Background = new SolidColorBrush() { Color = Color.FromArgb(32, 0, 0, 0) } };
        Border.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(time), Property = Border.WidthProperty });
        Border Border2 = new Border() { CornerRadius = new CornerRadius(12), Child = NotificationGrid, ClipToBounds = true, Transitions = new Transitions() };
        NotificationGrid.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(200), Property = Grid.HeightProperty });
        Border2.Transitions.Add(new ThicknessTransition() { Duration = TimeSpan.FromMilliseconds(200), Property = Grid.MarginProperty });
        
        NotificationGrid.Height = 32;
        if(MainView.GLOBAL.notificationPlace != null)
            if(MainView.GLOBAL.notificationPlace.Children.Count == 1)
                Border2.Margin = new Thickness(8, 8, 8, 8);
            else
                Border2.Margin = new Thickness(8, 0, 8, 8);
        new Thread(() =>
        {
            Thread.Sleep(200);
            Dispatcher.UIThread.Invoke(() =>
            {
                Border.Width = NotificationGrid.Bounds.Width;
            });
            Thread.Sleep((int)time);
            Dispatcher.UIThread.Invoke(() =>
            {
                NotificationGrid.Height = 0;
                Border2.Margin = new Thickness(0);
            });
            Thread.Sleep(200);
            Dispatcher.UIThread.Invoke(() =>
            {
                MainView.GLOBAL.notificationPlace.Children.Remove(Border2);
            });
        }).Start();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Preload();
    }

    public void Preload()
    {
        Communicator.BrowsePathRequest += Browse;
        Communicator.CommandRun += CommandRun;
        Communicator.ShowDialog += ShowDialogCommunicate;
        Communicator.PathRun += PathRun;
        Communicator.AppLoader = new AppLoader();
        ApplicationManager.AppsLoaded += AppsLoaoded;
        ProtocolWorker.LoadProtocolWorker();

        AppDomain.CurrentDomain.UnhandledException += (a, b) =>
        {
            Communicator.OpenWindow(new IllegalOperationCatcher($"{b.ExceptionObject} \nTerminating: {b.IsTerminating}"));
        };

        ApplicationManager.Load();
    }

    private void Browse(object? sender, DialogFileSystemBrowsingEventArgs e)
    {
        Communicator.OpenWindow(new FileBrowser(e.IsDirectory ? FileBrowser.SelectMode.Directory : FileBrowser.SelectMode.File, e.DoAfter, e.Filter));
    }

    private void PathRun(object? sender, string e)
    {
        if (File.Exists(e))
        {
            string[] pext = e.Split("\\").Last().Split(".");
            if (pext.Length == 1)
                return;
            string ext = pext.Last();
            if(ext == "jnk")
            {
                Shortcut? s = Shortcut.Load(e);
                if(s == null)
                {
                    Communicator.ShowMessageDialog(new MessageDialog("Error", "Shotcut has invalid data."));
                }
                else
                {
                    Communicator.RunCommand(s.Command);
                }
                return;
            }
            string fext = UserOptions.Current.GetPath($"AppData\\JVOS\\FileAssociation\\{ext}");
            if (File.Exists(fext))
            {
                string[] s = File.ReadAllText(fext).Split(":-:");
                UserOptions.Current.RecentManager.AddRecent(e);
                if (!AppCommunicator.OpenApplication(s[0], s[1], new object[]
                {
                    e
                }))
                    Communicator.ShowMessageDialog(new MessageDialog("App protocol", "Failed to run application " + s[0]));
            }
            else
                Communicator.ShowMessageDialog(new MessageDialog(e, $"Association to open {ext} file type not found."));
        }
        else if (Directory.Exists(e))
        {
            Communicator.OpenWindow(new FileBrowser(path: e));
            UserOptions.Current.RecentManager.AddRecent(e);
        }
    }

    private void AppsLoaoded(object? sender, EventArgs e)
    {
        
    }

    private void ShowDialogCommunicate(object? sender, IMessageDialog dialog)
    {
        var jMessage = new Message();
        jMessage.title.Text = dialog.Title;
        jMessage.message.Text = dialog.Message;
        if(dialog.Icon!= null)
            jMessage.icon.Source = dialog.Icon;
        List<EventHandler<ColorScheme>> EventHandleUpdate = new();
        foreach (var x in dialog.Buttons)
        {
            var btn = new JButton() { Content = x.Title, Width = 100 };
            btn.Click += (a, b) => x.Run();
            jMessage.buttonStack.Children.Add(btn);
        }
        if(dialog.Buttons.Count == 0)
        {
            var btn = new JButton() { Content = "OK", Width = 100 };
            btn.Click += (a, b) => WindowManager.CloseWindowFrame(jMessage.Frame);
            jMessage.buttonStack.Children.Add(btn);
        }
        WindowManager.OpenInWindow(jMessage);
    }

    private void CommandRun(object? sender, string e)
    {
        if (e == null)
            e = "";
        e.Replace(":\\\\", "://");
        string[] eSplit = e.Split("://");
        if(eSplit.Length >= 2)
        {
            string[] jSplit = new string[eSplit.Length - 1];
            Array.Copy(eSplit, 1, jSplit, 0, jSplit.Length);
            var protocol = ProtocolWorker.Protocols.Where(x => x.Name == eSplit[0]);
            if(protocol.Count() == 0)
                Communicator.ShowMessageDialog(new MessageDialog("Shell", $"Invalid protocol {eSplit[0]}"));
            else 
                protocol.First().Execute(String.Join("://", jSplit).Split(' '));
        }
        else
        {
            Communicator.ShowMessageDialog(new MessageDialog("Shell", $"Invalid command {e}\nUsage: protocol_name://arguments"));
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
