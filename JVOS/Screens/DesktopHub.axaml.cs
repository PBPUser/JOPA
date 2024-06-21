using Avalonia;
using Avalonia.Controls;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using System.Collections.Generic;
using System.Diagnostics;

namespace JVOS.Screens
{
    public partial class DesktopHub : UserControl, IWindowSpace
    {
        public DesktopHub()
        {
            InitializeComponent();
            DataContext = VM = new DesktopHubViewModel();
        }

        private List<WindowFrameBase> WindowFrames = new List<WindowFrameBase>();
        public DesktopScreen ParentScreen;
        public DesktopHubViewModel VM;

        public void CloseWindow(WindowFrameBase window)
        {
            WindowFrames.Remove(window);
            ParentScreen.DeattachBarApplication(window);
            this.windowCanvas.Children.Remove((Control)window);
        }

        private int
            LatestWindowX = 96, LatestWindowY = 96;

        public void OpenWindow(WindowFrameBase window)
        {
            WindowFrames.Add(window);
            window.WindowSpace = this;
            this.windowCanvas.Children.Add(window);
            ParentScreen.AttachBarApplication(window);
            window.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            window.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            window.WindowLoaded += (a,b) => BringToFront(window);
            DesktopScreen.CurrentDesktop.CloseAllHubs();
        }

        private int TopZIndex = 0;
        private WindowFrameBase? TopWindow;

        public void BringToFront(WindowFrameBase window)
        {
            if (window == TopWindow)
                return;
            if(TopWindow != null)
                TopWindow.IsActivated = false;
            TopWindow = window;
            if(TopWindow.Minimized)
                TopWindow.Minimized = false;
            if(window.WindowContent.AllowBringOnTop)
                window.ZIndex = TopZIndex++;
            TopWindow.IsActivated = true;
            DesktopScreen.CurrentDesktop.CloseAllHubs();
        }

        public void MinimizeWindow(WindowFrameBase window)
        {
            if (window.Minimized)
                return;
        }

        public void CloseAllHubs()
        {
            
        }
    }
}
