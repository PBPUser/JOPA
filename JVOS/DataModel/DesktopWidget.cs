using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.DataModel
{
    public class DesktopWidget
    {
        public DesktopWidget() { 
        
        }

        public string ClassName = "";
        public string DllName = "";
        public ulong AutoUpdateCycleLength = 1000;
        public bool AutoUpdate = false;
        public Size Size = new(200, 200);
        public Point Position = new(0, 0);
    }
}
