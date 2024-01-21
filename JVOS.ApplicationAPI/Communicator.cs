using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public static class Communicator
    {
        public static event EventHandler<IJWindow>? WindowOpenRequest;
        public static event EventHandler<IJWindowFrame>? WindowCloseRequest;
        public static event EventHandler<(Bitmap?, string, string, string)>? RegisterRunnable;
        public static event EventHandler<(Bitmap?, string, string, string)>? RunApp;
        public static event EventHandler<(Bitmap?, string, string)>? NotificationShown;
        public static event EventHandler<(string, Action<string, string>)>? ProtocolRegister;
        public static event EventHandler<string>? RegisterApp;
        public static event EventHandler<string>? CommandRun;
        public static event EventHandler<(Bitmap? bitmap, string title, string message, List<(string, Action?)> buttons)>? ShowDialog;

        public static void OpenWindow(IJWindow window)
        {
            if (WindowOpenRequest != null)
                WindowOpenRequest.Invoke(null, window);
        }
        public static void CloseWindow(IJWindowFrame window)
        {
            if (WindowCloseRequest != null)
                WindowCloseRequest.Invoke(null, window);
        }

        public static void AttachRunnable((Bitmap?, string, string, string) runnable)
        {
            if (RegisterRunnable != null) 
                RegisterRunnable.Invoke(null, runnable);
        }

        public static void RegisterApplication(string path)
        {
            if(RegisterApp != null)
                RegisterApp.Invoke(null, path);
        }

        public static void RunRunnable((Bitmap?, string, string, string) runnable)
        {
            if(RunApp != null)
                RunApp.Invoke(null, runnable);
        }

        public static void ShowNotification((Bitmap?, string, string) notification)
        {
            if(NotificationShown != null)
                NotificationShown.Invoke(null, notification);
        }

        public static void ShowMessageDialog((Bitmap? bitmap, string title, string message, List<(string, Action?)> buttons) message)
        {
            if (ShowDialog != null)
                ShowDialog.Invoke(null, message);
        }

        public static void RegisterProtocol((string name, Action<string, string> action) protocol)
        {
            if (ProtocolRegister != null)
                ProtocolRegister.Invoke(null, protocol);
        }

        public static void RunCommand(string direction)
        {
            if (CommandRun != null)
                CommandRun.Invoke(null, direction);
        }

    }
    public class Runnable
    {
        public Runnable(string Name, string Internal, Bitmap? icon = null)
        {
            this.Name = Name;
            this.InternalName = Internal;
            this.Icon = icon;
        }

        public string Name;
        public string InternalName;
        public Bitmap? Icon;

        public virtual void Run()
        {

        }
    }
}
