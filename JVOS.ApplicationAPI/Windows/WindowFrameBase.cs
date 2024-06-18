using Avalonia.Controls;
using Avalonia.Interactivity;
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

        private AppCommunicator? ApplicationCommunicator = null;
        public int ID { get; set; } = -1;
        public IWindowSpace WindowSpace;
        public WindowContentBase WindowContent { get; set; }
        public virtual bool Minimized { get; set; }
        public virtual bool IsActivated { get; set; }

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

        }

        public virtual void Deactivated()
        {

        }

        public virtual void ChangeState(WindowFrameState FrameState)
        {

        }

        private void OnLoaded()
        {
            if(WindowLoaded != null)
                WindowLoaded(this, EventArgs.Empty);
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

        public event EventHandler<EventArgs>? WindowLoaded = null;
    }

    public enum WindowFrameState
    {
        Maximized = 0,
        Default = 1
    }
}
