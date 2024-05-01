using Avalonia.Controls;
using Avalonia.Layout;
using DynamicData.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public class IHub : UserControl
    {
        public Orientation AnimationOrientation { get; set; }
        public enum CloseReason { Hide, CloseReason }
        public bool IsOpen = false;


        public event EventHandler<EventArgs>? Opened;
        public event EventHandler<CloseReason>? Closed;
        public event EventHandler<object>? Removed;
        public event EventHandler<object>? ButtonContentChanged;

        public void OnAdded()
        {

        }

        public void OnRemoved()
        {
            Removed?.Invoke(this, EventArgs.Empty);
        }

        public void OnClosed(CloseReason Reason)
        {
            Closed?.Invoke(this, Reason);
        }

        public void OnOpened(EventArgs e)
        {
            Opened?.Invoke(this, e);
        }

        public void OnButtonContentChanged(object obj)
        {
            if(ButtonContentChanged != null)
                ButtonContentChanged.Invoke(this, obj);
        }
    }
}
