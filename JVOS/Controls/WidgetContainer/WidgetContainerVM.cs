using JVOS.ApplicationAPI.Widgets;
using JVOS.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls.WidgetContainer
{
    internal class WidgetContainerVM : ViewModelBase
    {
        public WidgetContainerVM()
        {

        }

        private string title = "";
        public string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, String.Join('\n', value.ToCharArray()));
        }

        private Widget widget;
        public Widget Widget
        {
            get => widget;
            set => this.RaiseAndSetIfChanged(ref widget, value);
        }
    }
}
