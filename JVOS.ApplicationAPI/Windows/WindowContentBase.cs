using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Windows
{
    public abstract class WindowContentBase : UserControl, IDisposable
    {
        protected WindowContentBase()
        {

        }

        private string _Title = "";
        private Bitmap? _Icon = null;

        public WindowFrameBase Frame;

        private Subject<string> _TitleBinding = new Subject<string>();  
        private Subject<Bitmap?> _IconBinding = new Subject<Bitmap>();  

        public virtual bool AllowBringOnTop { get => true; }
        public virtual bool ShowOnTaskbar { get => true; }
        public virtual Size DefaultSize { get => new Size(800, 480); }
        public Subject<string> TitleBinding { get => _TitleBinding; }
        public Subject<Bitmap?> IconBinding { get => _IconBinding; }
        public string Title { get => _Title; set { _Title = value; _TitleBinding.OnNext(value); } }
        public Bitmap? Icon { get => _Icon; set { _Icon = value; _IconBinding.OnNext(value); } }

        public virtual string GetPanelId() => "";

        public void BringToFront()
        {
            Frame.BringToFront();
        }

        public virtual void Opened()
        {

        }

        public virtual void Deactivated()
        {

        }

        public virtual void Closed()
        {

        }

        public virtual void OnDispose()
        {

        }

        bool isDisposed = false;

        public void Dispose()
        {
            if (isDisposed)
                return;
            isDisposed = true;
            OnDispose();
        }
    }
}
