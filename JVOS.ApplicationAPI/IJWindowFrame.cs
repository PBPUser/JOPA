using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IJWindowFrame
    {
        public IWindowSpace? WindowSpace { get; set; }
        public IJWindow ChildWindow { get; set; }
        public bool AllowMinimize { get; set; }
        public int ID { get; set; }

        public event EventHandler<EventArgs> Closing;
        public event EventHandler<IJWindow> ChildWindowSet;
        public void SetPosition(int x, int y)
        {

        }

        public void BringToFront()
        {
            WindowSpace.BringToFront(this);
        }

        public virtual void Close(Action? action = null)
        {

        }

        string GetPanelId();
    }
}
