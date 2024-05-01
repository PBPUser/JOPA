using JVOS.Controls;
using JVOS.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.DataModel
{
    public class BarRunningInstance
    {
        public BarTooltip AssociatedBarTooltip;
        public IDisposable WidthDisposable;
        public EventHandler<ColorScheme> SchemeUpdate;

        public BarRunningInstance(BarTooltip barTooltip, IDisposable widthDisposable)
        {
            AssociatedBarTooltip = barTooltip;
            SchemeUpdate = (a, b) => {

            };
            WidthDisposable = widthDisposable;
        }
    }
}
