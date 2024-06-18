using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class WidthBasedUniformGrid : UniformGrid
    {
        static WidthBasedUniformGrid()
        {

        }

        public static readonly StyledProperty<double> 
            ObjectMinWidthProperty = AvaloniaProperty.Register<WidthBasedUniformGrid, double>("ObjectMinWidth", 64d, coerce: ValidateNewWidth);
        public static readonly StyledProperty<double> 
            ObjectMinHeightProperty = AvaloniaProperty.Register<WidthBasedUniformGrid, double>("ObjectMinHeight", 64d, coerce: ValidateNewHeight);

        private static double ValidateNewWidth(AvaloniaObject obj, double x)
        {
            var s = (obj as WidthBasedUniformGrid);
            if (s == null)
                return x;
            s.Columns = (int)(s.Bounds.Width / x);
            return x;
        }

        private static double ValidateNewHeight(AvaloniaObject obj, double x)
        {
            var s = (obj as WidthBasedUniformGrid);
            if (s == null)
                return x;
            s.Height = (int)(s.Bounds.Height / x);
            return x;
        }

        public double ObjectMinWidth
        {
            get => (double)GetValue(ObjectMinWidthProperty);
            set => SetValue(ObjectMinWidthProperty, value);
        }

        public double ObjectMinHeight
        {
            get => (double)GetValue(ObjectMinHeightProperty);
            set => SetValue(ObjectMinHeightProperty, value);
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            Columns = (int)(e.NewSize.Width / ObjectMinWidth);
            Rows = (int)(e.NewSize.Height / ObjectMinHeight);
            base.OnSizeChanged(e);
        }
    }
}
