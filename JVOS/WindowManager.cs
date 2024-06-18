using Avalonia.Controls;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using JVOS.Screens;
using JVOS.Views;
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

        static WindowManager()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            Communicator.WindowCloseRequest += (a, b) => CloseWindowFrame(b);
            Communicator.WindowOpenRequest += (a, b) => OpenInWindow(b);
            Communicator.FrameOpenRequest += (a, b) => OpenWindow(b);
            Communicator.WindowOpen += (a, b) => OpenInWindow(b);
        }

        public static IWindowSpace? WindowSpace;

        static List<WindowFrameBase> FramesQueue = new();

        public static void SetCurrentWindowSpace(IWindowSpace windowSpace)
        {
            WindowSpace = windowSpace;
            while(FramesQueue.Count > 0)
            {
                FramesQueue[0].WindowSpace = windowSpace;
                OpenWindow(FramesQueue[0]);
                FramesQueue.RemoveAt(0);
            }
        }

        public static void OpenWindow(WindowFrameBase window)
        {
            if(WindowSpace == null)
            {
                FramesQueue.Add(window);
                return;
            }
            WindowSpace?.OpenWindow(window);
        }

        public static SystemWindowFrame OpenInWindow(WindowContentBase windowContent, Action? whenClosed = null)
        {
            SystemWindowFrame jwin = new SystemWindowFrame(windowContent, WindowSpace) { };
            return jwin;
        }

        public static SystemWindowFrame OpenInWindow(WindowOpenRequest request, Action? whenClosed = null)
        {
            SystemWindowFrame jwin = new SystemWindowFrame(request, WindowSpace) { };
            return jwin;
        }

        public static void CloseWindowFrame(WindowFrameBase window)
        {
            WindowSpace?.CloseWindow(window);
        }

        public static void CloseAllHubsInActiveWindowSpace()
        {
            WindowSpace?.CloseAllHubs();
        }
    }
}
