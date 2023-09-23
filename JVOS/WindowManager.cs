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
        public static IWindowSpace WindowSpace;
         
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
            jwin.SetChild((UserControl)windowContent);
            OpenJWindow(jwin);
        }

        public static void CloseJWindow(IJWindowFrame window)
        {
            WindowSpace?.CloseWindow(window);
        }
    }
}
