using Avalonia.Controls;
using JVOS.ApplicationAPI;
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

        public void CloseWindow(IJWindowFrame window)
        {
            JWindowFrames.Remove(window);
            this.windowCanvas.Children.Remove((Control)window);
        }

        public void OpenWindow(IJWindowFrame window)
        {
            JWindowFrames.Add(window);
            this.windowCanvas.Children.Add((Control)window);
        }
    }
}
