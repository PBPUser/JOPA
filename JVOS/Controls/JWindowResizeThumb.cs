using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using JVOS.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class JWindowResizeThumb : Thumb
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);
        static JWindowResizeThumb()
        {
            ParentWindowProperty = AvaloniaProperty.RegisterAttached<JWindowResizeThumb, Thumb, SystemWindowFrame>("ParentWindow");
        }

        public SystemWindowFrame ParentWindow
        {
            get => GetValue(ParentWindowProperty);
            set => SetValue(ParentWindowProperty, value);
        }

        protected override void OnDragDelta(VectorEventArgs e)
        {
            e.Handled = true;
            if (ParentWindow == null)
                return;
            double deltaVertical, deltaHorizontal;

            switch (VerticalAlignment)
            {
                case Avalonia.Layout.VerticalAlignment.Bottom:

                    deltaVertical = Math.Min(-e.Vector.Y, ParentWindow.Bounds.Height - ParentWindow.MinHeight);
                    ParentWindow.Height = Math.Max(32, ParentWindow.Bounds.Height - deltaVertical);
                    break;
                case Avalonia.Layout.VerticalAlignment.Top:
                    deltaVertical = Math.Min(e.Vector.Y, ParentWindow.Bounds.Height - ParentWindow.MinHeight);
                    var deltaHeight = ParentWindow.Bounds.Height;
                    //ParentWindow.WindowPositionTransform.Y -= deltaVertical;
                    ParentWindow.Height = Math.Max(32, ParentWindow.Bounds.Height - deltaVertical);
                    var d = ParentWindow.Height = Math.Max(32, ParentWindow.Bounds.Height - deltaVertical);
                    deltaHeight = deltaHeight - d;
                    ParentWindow.WindowTranslateMove.Y += deltaHeight;
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case Avalonia.Layout.HorizontalAlignment.Left:
                    deltaHorizontal = Math.Min(e.Vector.X, ParentWindow.Bounds.Width - ParentWindow.MinWidth);
                    var deltaWidth = ParentWindow.Bounds.Width;
                    //ParentWindow.WindowPositionTransform.X -= e.Vector.X;
                    var d = ParentWindow.Width = Math.Max(32, ParentWindow.Bounds.Width - deltaHorizontal);
                    deltaWidth = deltaWidth - d;
                    ParentWindow.WindowTranslateMove.X += deltaWidth;
                    break;
                case Avalonia.Layout.HorizontalAlignment.Right:
                    deltaHorizontal = Math.Min(-e.Vector.X, ParentWindow.Bounds.Width - ParentWindow.MinWidth);
                    ParentWindow.Width = Math.Max(32, ParentWindow.Bounds.Width - deltaHorizontal);
                    break;
                default:
                    break;
            }
        }

        public override void Render(DrawingContext context)
        {
            context.FillRectangle(TransparentA1, new Rect(Bounds.Size));
        }

        public static readonly AttachedProperty<SystemWindowFrame> ParentWindowProperty;
    }
}
