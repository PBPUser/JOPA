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
        public static event EventHandler<IJWindow>? WindowCloseRequest;
        public static event EventHandler<(Bitmap?, string, string)>? RegisterRunnable;
        public static event EventHandler<(Bitmap?, string, string)>? RunApp;
        public static event EventHandler<string>? RegisterApp;

        public static void OpenWindow(IJWindow window)
        {
            if (WindowOpenRequest != null)
                WindowOpenRequest.Invoke(null, window);
        }
        public static void CloseWindow(IJWindow window)
        {
            if (WindowCloseRequest != null)
                WindowCloseRequest.Invoke(null, window);
        }

        public static void AttachRunnable((Bitmap?, string, string) runnable)
        {
            if (RegisterRunnable != null) 
                RegisterRunnable.Invoke(null, runnable);
        }

        public static void RegisterApplication(string path)
        {
            if(RegisterApp != null)
                RegisterApp.Invoke(null, path);
        }

        public static void RunRunnable((Bitmap?, string, string) runnable)
        {
            if(RunApp != null)
                RunApp.Invoke(null, runnable);
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
