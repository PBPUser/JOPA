using Avalonia.Controls;
using Avalonia.Controls.Templates;
using JVOS.DataModel;
using JVOS.ViewModels;
using JVOS.Widgets;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.EmbededWindows.Desktop
{
    public class AddWidgetWindowVM : ViewModelBase
    {
        static AddWidgetWindowVM()
        {
            WidgetWindowVMDataTemplate = new((a, b) =>
            {
                DockPanel panel = new();
                Image preview = new() { Source = a.Preview, Width = 320, Height = 180 };
                Image icon = new() { Source = a.Icon, Width = 28, Height = 28 };
                TextBlock title = new() { Text = a.Title };
                TextBlock description = new() { Text = a.ClassName };

                panel.Children.Add(preview);
                panel.Children.Add(icon);
                panel.Children.Add(title);
                panel.Children.Add(description);

                DockPanel.SetDock(preview, Dock.Bottom);
                DockPanel.SetDock(title, Dock.Top);

                return panel;
            });
        }

        public static FuncDataTemplate<WidgetPreview> WidgetWindowVMDataTemplate;

        public AddWidgetWindowVM()
        {
            Refresh();
        }

        private List<WidgetPreview> previews = new();

        public void Refresh()
        {
            Previews = new List<WidgetPreview>();

            List<WidgetPreview> previews = new();
            previews.AddRange(WidgetManager.GetWidgetPreviews());
            Previews = previews;
        }

        public List<WidgetPreview> Previews
        {
            get => previews;
            set => this.RaiseAndSetIfChanged(ref previews, value);
        }
    }
}
