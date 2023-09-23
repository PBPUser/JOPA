using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class JWindowMoveThumb : Thumb
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);

        static JWindowMoveThumb()
        {
            ParentWindowProperty = AvaloniaProperty.RegisterAttached<JWindowMoveThumb, Thumb, JWindow>("ParentWindow");
        }


        public JWindow ParentWindow
        {
            get => GetValue(ParentWindowProperty);
            set => SetValue(ParentWindowProperty, value);
        }

        protected override void OnDragDelta(VectorEventArgs e)
        {
            if (ParentWindow == null)
                return;
            if(ParentWindow.StatePosition != JWindow.PositionState.Normal)
                ParentWindow.SwitchPositionState(JWindow.PositionState.Normal);
            ParentWindow.WindowPositionTransform.X += e.Vector.X;
            ParentWindow.WindowPositionTransform.Y += e.Vector.Y;
        }

        public override void Render(DrawingContext context)
        {
            context.FillRectangle(TransparentA1, new Rect(0, 0, ParentWindow.Width, 32));
            base.Render(context);
        }

        public static readonly AttachedProperty<JWindow> ParentWindowProperty;
    }
}
