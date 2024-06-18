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
    public class WindowMoveThumb : Thumb
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);

        static WindowMoveThumb()
        {
            ParentWindowProperty = AvaloniaProperty.RegisterAttached<WindowMoveThumb, Thumb, SystemWindowFrame>("ParentWindow");
        }

        public WindowMoveThumb()
        {
            Loaded += (a, b) => Height = 32;
        }

        public SystemWindowFrame ParentWindow
        {
            get => GetValue(ParentWindowProperty);
            set => SetValue(ParentWindowProperty, value);
        }

        protected override void OnDragDelta(VectorEventArgs e)
        {
            if (ParentWindow == null)
            {
                return;
            }
            if(ParentWindow.FrameState != ApplicationAPI.Windows.WindowFrameState.Default)
                ParentWindow.ChangeState(ApplicationAPI.Windows.WindowFrameState.Default);
            ParentWindow.WindowTranslateMove.X += e.Vector.X;
            ParentWindow.WindowTranslateMove.Y += e.Vector.Y;
        }

        public override void Render(DrawingContext context)
        {
            context.FillRectangle(TransparentA1, new Rect(Bounds.Size));
            base.Render(context);
        }

        public static readonly AttachedProperty<SystemWindowFrame> ParentWindowProperty;
    }
}
