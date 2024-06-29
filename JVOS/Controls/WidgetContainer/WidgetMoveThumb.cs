using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls.WidgetContainer
{
    public class WidgetMoveThumb : Thumb
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);

        static WidgetMoveThumb()
        {
            ContainerProperty = AvaloniaProperty.RegisterAttached<WidgetMoveThumb, Thumb, WidgetContainer>("ParentWindow");
        }

        public WidgetMoveThumb()
        {
        }

        public WidgetContainer Container
        {
            get => GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }

        protected override void OnDragDelta(VectorEventArgs e)
        {
            if (Container == null)
                return;
            Container.TranslateMove.X += e.Vector.X;
            Container.TranslateMove.Y += e.Vector.Y;
        }

        protected override void OnDragCompleted(VectorEventArgs e)
        {
            if (Container == null)
                return;
            Container.Information.Position.X = (int)Container.TranslateMove.X;
            Container.Information.Position.Y = (int)Container.TranslateMove.Y;
            File.WriteAllText(Container.FileName, JsonConvert.SerializeObject(Container.Information));
        }

        public override void Render(DrawingContext context)
        {
            context.FillRectangle(TransparentA1, new Rect(Bounds.Size));
            base.Render(context);
        }

        public static readonly AttachedProperty<WidgetContainer> ContainerProperty;
    }
}
