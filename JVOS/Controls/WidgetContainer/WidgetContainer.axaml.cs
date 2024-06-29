using Avalonia.Controls;
using Avalonia.Media;
using JVOS.ApplicationAPI.Widgets;
using JVOS.DataModel;
using Newtonsoft.Json;
using System.IO;

namespace JVOS.Controls.WidgetContainer
{
    public partial class WidgetContainer : UserControl
    {
        public string FileName;
        public DesktopWidget Information;
        public TranslateTransform TranslateMove;
        WidgetContainerVM VM;

        public WidgetContainer(Widget widget, string filename, DesktopWidget information)
        {
            InitializeComponent();
            Information = information;
            FileName = filename;
            TranslateMove = new();
            TranslateMove.X = information.Position.X;
            TranslateMove.Y = information.Position.Y;
            RenderTransform = TranslateMove;
            DataContext = VM = new();
            VM.Title = widget.Name??"Widget";
            VM.Widget = widget;
            TranslateMove.X = information.Position.X;
            TranslateMove.Y = information.Position.Y;
            var thumb = new WidgetMoveThumb()
            {
                Container = this
            };
            root.Children.Insert(0, thumb);
            Grid.SetColumn(thumb, 1);

        }
    }
}
