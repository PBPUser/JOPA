using JVOS.ApplicationAPI.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IWindowSpace
    {
        public virtual Size GetSpaceSize() => new Size(0, 0);

        public void OpenWindow(WindowFrameBase window);
        public void MinimizeWindow(WindowFrameBase window);
        public void CloseWindow(WindowFrameBase window);
        public void BringToFront(WindowFrameBase window);
        public void CloseAllHubs();
    }
}
