using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI.Windows;
using JVOS.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.EmbededWindows
{
    public class WindowJumpListVM : ViewModelBase
    {
        public WindowJumpListVM()
        {

        }

        private bool visible = false;
        private bool showAppItem = true;
        private bool showPinItem = true;
        private string appTitle = "";
        private string pinTitle = "";
        private Bitmap? appIcon = null;
        private WindowFrameBase? frame = null;

        public void SetWindowFrame(WindowFrameBase frame)
        {
            this.frame = frame;
            PinTitle = "Pin";
            AppIcon = frame.Icon;
            AppTitle = frame.Title;
        }

        internal void CloseWindow()
        {
            if (frame != null)
                frame.Close();
        }

        public bool Opended
        {
            get => visible;
            set => this.RaiseAndSetIfChanged(ref visible, value);
        }

        public bool ShowAppItem
        {
            get => showAppItem;
            set => this.RaiseAndSetIfChanged(ref showAppItem, value);
        }
        public bool ShowPinItem
        {
            get => showPinItem;
            set => this.RaiseAndSetIfChanged(ref showPinItem, value);
        }
        public string PinTitle
        {
            get => pinTitle;
            set => this.RaiseAndSetIfChanged(ref pinTitle, value);
        }
        public string AppTitle
        {
            get => appTitle;
            set => this.RaiseAndSetIfChanged(ref appTitle, value);
        }
        public Bitmap? AppIcon
        {
            get => appIcon;
            set => this.RaiseAndSetIfChanged(ref appIcon, value);
        }
    }
}
