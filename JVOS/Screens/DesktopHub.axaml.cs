using Avalonia.Controls;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using System.Collections.Generic;

namespace JVOS.Screens
{
    public partial class DesktopHub : UserControl, IWindowSpace
    {
        public DesktopHub()
        {
            InitializeComponent();
        }

        private List<WindowFrameBase> WindowFrames = new List<WindowFrameBase>();
        public DesktopScreen ParentScreen;

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
            this.windowCanvas.Children.Add((Control)window);
            ParentScreen.AttachBarApplication(window);
            window.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            window.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            window.WindowLoaded += (a,b) => BringToFront(window);
            window.WindowLoaded += (a, b) =>
            {
                //var width = ((Control)this).Bounds.Width;
                //var height = ((Control)this).Bounds.Height;
                //var wwidth = ((Control)window).Bounds.Width;
                //var wheight = ((Control)window).Bounds.Height;
                //double xP = LatestWindowX += 24;
                //double yP = LatestWindowY += 24;
                //switch (window.ChildWindow.StartupLocation)
                //{
                //    case IJWindow.WindowStartupLocation.CenterLeft:
                //    case IJWindow.WindowStartupLocation.CenterRight:
                //    case IJWindow.WindowStartupLocation.Center:
                //        yP = (height - wheight) / 2;
                //        break;
                //    case IJWindow.WindowStartupLocation.BottomLeft:
                //    case IJWindow.WindowStartupLocation.BottomCenter:
                //    case IJWindow.WindowStartupLocation.BottomRight:
                //        yP = height - wheight - yP;
                //        break;
                //}
                //switch (window.ChildWindow.StartupLocation)
                //{
                //    case IJWindow.WindowStartupLocation.TopCenter:
                //    case IJWindow.WindowStartupLocation.BottomCenter:
                //    case IJWindow.WindowStartupLocation.Center:
                //        xP = (width - wwidth) / 2;
                //        break;
                //    case IJWindow.WindowStartupLocation.TopRight:
                //    case IJWindow.WindowStartupLocation.CenterRight:
                //    case IJWindow.WindowStartupLocation.BottomRight:
                //        xP = width - wwidth - xP;
                //        break;

                //}
                //window.SetPosition((int)xP, (int)yP);
                //if (LatestWindowY + 256 > yP)
                //    LatestWindowY = 96;
                //if (LatestWindowX + 128 > ((Control)this).Bounds.Width)
                //    LatestWindowX = 96;
            };
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
        }

        public void MinimizeWindow(WindowFrameBase window)
        {
            if (window.Minimized)
                return;
            //window.Minimized = true;
        }

        public void CloseAllHubs()
        {
            
        }
    }
}
