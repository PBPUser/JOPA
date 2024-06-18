using Avalonia.Metadata;
using DynamicData;
using JVOS.ApplicationAPI.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.App
{
    public class AppCommunicator
    {

        public enum ApplicationShutdownPolicy
        {
            ApplicationControlled,
            MainWindowClosed,
            LatestWindowClosed
        }

        private List<WindowFrameBase> windows = new();

        public ApplicationShutdownPolicy AppShutdownPolicy = ApplicationShutdownPolicy.MainWindowClosed;
        public App app;
        public AssemblyLoadContext assemblyContext;
        public Assembly assembly;
        public AppManifest manifest;

        public static bool OpenApplication(string path, string runActivity, object[]? args = null) {
            App? app;
            if (LoadApp(path, args, out app))
            {
                app.OnActivity(runActivity, args);
                //app.Communicator.CheckAndUnloadApp();
                return true;
            }
            return false;
        }

        public static bool ExecuteProtocol(string path, string runProtocol, object[]? args = null)
        {
            App? app;
            if(LoadApp(path, args, out app))
            {
                app.OnProtocolExec(runProtocol, args);
                app.Communicator.CheckAndUnloadApp();
                return true;
            }
            return false;
        }
        
        private static bool LoadApp(string path, object[]? args, out App? app)
        {
            return Communicator.LoadApp(path, args, out app);
        }


        public AppCommunicator()
        {

        }

        public void OpenJWindow(WindowContentBase jwindow)
        {
            Communicator.OpenWindow(jwindow);
        }

        public void CloseJWindow(WindowFrameBase window)
        {
            Communicator.CloseWindow(window);
            this.windows.Remove(window);
            CheckAndUnloadApp();
        }

        public void CheckAndUnloadApp()
        {
            if (windows.Count == 0 && AppShutdownPolicy != ApplicationShutdownPolicy.ApplicationControlled)
            {
                UnloadAndCloseApplication();
                return;
            }
        }

        public void UnloadAndCloseApplication()
        {
            var win = windows;
            foreach (var x in win)
                CloseJWindow(x);
            assemblyContext.Unload();
            Communicator.ShowMessageDialog(new MessageDialog("Test", $"Application {manifest.Name} closed"));
            app = null;
        }

        public void AddWindow(WindowFrameBase jWindow)
        {
            windows.Add(jWindow);
        }
    }
}
