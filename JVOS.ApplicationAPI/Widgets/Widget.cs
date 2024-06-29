using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using JVOS.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Widgets
{
    public class Widget : UserControl, IDisposable
    {
        public ExternalWidgetLoadContext? LoadContext = null;
        DispatcherTimer? UpdatingTimer = null;

        public void SetUpdatingTimer(ulong? cycleLength = null)
        {
            UpdatingTimer?.Stop();
            if (cycleLength == null)
                return;
            UpdatingTimer = new() {
                Interval = TimeSpan.FromMilliseconds(cycleLength??1000000)
            };
            UpdatingTimer.Tick += (a, b) => Update();
            UpdatingTimer.Start();
        }

        public virtual Size DefaultSize=>new(200, 200);

        public virtual void Unloading()
        {

        }

        bool isDisposed = false;

        public void Dispose()
        {
            if (isDisposed)
                return;
            Unloading();
            isDisposed = true;
            UpdatingTimer?.Stop();
            Communicator.ShowMessageDialog(new MessageDialog("Widgets Manager", $"{GetType().FullName} unloaded"));
            LoadContext?.UnloadWidget(this);
        }

        public virtual void Update()
        {

        }
    }
}
