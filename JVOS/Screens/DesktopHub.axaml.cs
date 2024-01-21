using Avalonia.Controls;
using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;

namespace JVOS.Screens
{
    public partial class DesktopHub : UserControl, IWindowSpace
    {
        public DesktopHub()
        {
            InitializeComponent();
        }

        private List<IJWindowFrame> JWindowFrames = new List<IJWindowFrame>();
        public DesktopScreen ParentScreen;

        public void CloseWindow(IJWindowFrame window)
        {
            JWindowFrames.Remove(window);
            ParentScreen.DeattachBarApplication(window);
            this.windowCanvas.Children.Remove((Control)window);
        }

        private int
            LatestWindowX = 96, LatestWindowY = 96;

        public void OpenWindow(IJWindowFrame window)
        {
            JWindowFrames.Add(window);
            window.WindowSpace = this;
            this.windowCanvas.Children.Add((Control)window);
            ParentScreen.AttachBarApplication(window);
            window.ChildWindowSet += (a, b) =>
            {
                var width = ((Control)this).Bounds.Width;
                var height = ((Control)this).Bounds.Height;
                var wwidth = ((Control)window).Bounds.Width;
                var wheight = ((Control)window).Bounds.Height;
                double xP = LatestWindowX += 24;
                double yP = LatestWindowY += 24;
                switch (window.ChildWindow.StartupLocation)
                {
                    case IJWindow.WindowStartupLocation.CenterLeft:
                    case IJWindow.WindowStartupLocation.CenterRight:
                    case IJWindow.WindowStartupLocation.Center:
                        yP = (height - wheight) / 2;
                        break;
                    case IJWindow.WindowStartupLocation.BottomLeft:
                    case IJWindow.WindowStartupLocation.BottomCenter:
                    case IJWindow.WindowStartupLocation.BottomRight:
                        yP = height - wheight - yP;
                        break;
                }
                switch (window.ChildWindow.StartupLocation)
                {
                    case IJWindow.WindowStartupLocation.TopCenter:
                    case IJWindow.WindowStartupLocation.BottomCenter:
                    case IJWindow.WindowStartupLocation.Center:
                        xP = (width - wwidth) / 2;
                        break;
                    case IJWindow.WindowStartupLocation.TopRight:
                    case IJWindow.WindowStartupLocation.CenterRight:
                    case IJWindow.WindowStartupLocation.BottomRight:
                        xP = width - wwidth - xP;
                        break;

                }
                window.SetPosition((int)xP, (int)yP);
                if (LatestWindowY + 256 > yP)
                    LatestWindowY = 96;
                if (LatestWindowX + 128 > ((Control)this).Bounds.Width)
                    LatestWindowX = 96;
            };
        }

        private int TopZIndex = 0;

        public void BringToFront(IJWindowFrame window)
        {
            ((Control)window).ZIndex = TopZIndex++;
        }

        public void CloseAllHubs()
        {
            
        }
    }
}
