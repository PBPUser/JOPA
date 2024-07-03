using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI.App;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Windows
{
    public abstract class WindowFrameBase : UserControl
    {
        public WindowFrameBase(WindowContentBase windowContent, IWindowSpace windowSpace)
        {
            WindowContent = windowContent;
            WindowSpace = windowSpace;
            WindowContent.Frame = this;
            Communicator.OpenFrame(this);
        }

        public WindowFrameBase(WindowOpenRequest request, IWindowSpace windowSpace) : this(request.Window, windowSpace) {
            request.Communicatior.AddWindow(this);
        }


        public event EventHandler<bool> ActivateStateChanged;
        bool isLoaded = false;
        private AppCommunicator? ApplicationCommunicator = null;
        public int ID { get; set; } = -1;
        public IWindowSpace WindowSpace;
        public WindowContentBase WindowContent { get; set; }
        public virtual Bitmap? Icon => null;
        public virtual string Title => "undefined";
        public virtual bool Minimized { get; set; }
        public virtual bool IsActivated { get; set; }

        public virtual void SetPosition(double x, double y)
        {

        }

        private void SendRequestToCloseWindow()
        {
            if(ApplicationCommunicator != null) {
                ApplicationCommunicator.CloseJWindow(this);
            }
            else {
                Communicator.CloseWindow(this);
            }
        }

        public virtual void ToggleVisibilityState(bool? isVisible = null)
        {

        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            SubscribeFrameToContent();
            OnLoaded();
            base.OnLoaded(e);
        }

        public virtual void Close(Action? action = null) {
            SendRequestToCloseWindow();
            action?.Invoke();
        }

        public virtual void SubscribeFrameToContent()
        {

        }

        public virtual void SetFrameVisibility(bool visible)
        {

        }

        public virtual bool GetFrameVisibility()
        {
            return false;
        }

        public virtual void Activated()
        {
            ActivateStateChanged?.Invoke(this, true);
        }

        public virtual void Deactivated()
        {
            ActivateStateChanged?.Invoke(this, false);
            WindowContent.Deactivated();
        }

        public virtual void ChangeState(WindowFrameState FrameState)
        {

        }

        private void OnLoaded()
        {
            if (isLoaded)
                return;
            if(WindowLoaded != null)
                WindowLoaded(this, EventArgs.Empty);
            isLoaded = true;
        }

        public virtual string GetPanelId()
        {
            return DateTime.Now.ToString();
        }

        public void BringToFront()
        {
            ToggleVisibilityState(true);
            WindowSpace.BringToFront(this);
        }

        public virtual WindowFrameState GetState()
        {
            return WindowFrameState.Default;
        }

        public virtual void Minimize()
        {

        }

        public event EventHandler<EventArgs>? WindowLoaded = null;
    }

    public enum WindowFrameState
    {
        Maximized = 0,
        Default = 1
    }
}
