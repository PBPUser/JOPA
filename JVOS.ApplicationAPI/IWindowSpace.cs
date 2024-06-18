using JVOS.ApplicationAPI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IWindowSpace
    {
        public void OpenWindow(WindowFrameBase window);
        public void MinimizeWindow(WindowFrameBase window);
        public void CloseWindow(WindowFrameBase window);
        public void BringToFront(WindowFrameBase window);
        public void CloseAllHubs();
    }
}
