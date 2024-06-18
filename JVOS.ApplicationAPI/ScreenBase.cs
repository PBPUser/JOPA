using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public class ScreenBase : UserControl
    {
        public virtual void ScreenShown() {

        }

        public virtual void ScreenOverlap() { 
            
        }

        public virtual void MobileModeStateSwitch(bool enabled)
        {

        }

    }
}
