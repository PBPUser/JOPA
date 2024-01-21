using Avalonia.Controls;
using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public static class WindowManager
    {
        static bool isInitialized = false;

        public static void Initialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            Communicator.WindowCloseRequest += (a, b) =>
            {
                CloseJWindow(b);
            };
            Communicator.WindowOpenRequest += (a, b) =>
            {
                OpenInJWindow(b);
            };
        }

        public static IWindowSpace? WindowSpace;
         
        public static void SetCurrentWindowSpace(IWindowSpace windowSpace)
        {
            WindowSpace = windowSpace;
        }

        public static void OpenJWindow(JWindow window)
        {
            WindowSpace?.OpenWindow(window);
        }

        public static void OpenInJWindow(IJWindow windowContent)
        {
            JWindow jwin = new JWindow() {  };
            OpenJWindow(jwin);
            jwin.SetChild((UserControl)windowContent);
        }

        public static void CloseJWindow(IJWindowFrame window)
        {
            WindowSpace?.CloseWindow(window);
        }

        public static void CloseAllHubsInActiveWindowSpace()
        {
            WindowSpace?.CloseAllHubs();
        }
    }
}
