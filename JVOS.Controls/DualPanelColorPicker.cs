using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AColor = Avalonia.Media.Color;

namespace JVOS.Controls
{
    public class DualPanelColorPicker : TemplatedControl
    {
        static DualPanelColorPicker()
        {
            AffectsRender<DualPanelColorPicker>(
                BackgroundProperty,
                BorderBrushProperty,
                BorderThicknessProperty,
                CornerRadiusProperty,
                ShadowsProperty,
                ColorProperty,
                ClaymorphismShadowsProperty,
                OrientationProperty,
                OffsetProperty,
                PickerRadiusProperty,
                UseDarkModeProperty,
                IsFocusedProperty);
        }

        public DualPanelColorPicker()
        {
            
        }

        public event EventHandler<Color?> ColorChanged;

        public static readonly StyledProperty<Color?> ColorProperty = AvaloniaProperty.Register<DualPanelColorPicker, Color?>(nameof(Color), Colors.White, coerce: ValidateColor);
        public static readonly StyledProperty<BoxShadows?> ShadowsProperty = AvaloniaProperty.Register<DualPanelColorPicker, BoxShadows?>(nameof(Shadows));
        public static readonly StyledProperty<Thickness> OffsetProperty = AvaloniaProperty.Register<DualPanelColorPicker, Thickness>(nameof(Offset), new Thickness(0), coerce: ValidateOffset);
        public static readonly StyledProperty<bool> UseDarkModeProperty = AvaloniaProperty.Register<DualPanelColorPicker, bool>(nameof(UseDarkMode));
        public static readonly StyledProperty<bool?> ClaymorphismShadowsProperty = AvaloniaProperty.Register<DualPanelColorPicker, bool?>(nameof(ClaymorphismShadows));
        public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<DualPanelColorPicker, double>(nameof(Size), 128d);
        public static readonly StyledProperty<double> PickerRadiusProperty = AvaloniaProperty.Register<DualPanelColorPicker, double>(nameof(PickerRadius), 20);
        public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<DualPanelColorPicker, Orientation>(nameof(Orientation), Orientation.Horizontal, coerce: ValidateOrientation);
        private static readonly StyledProperty<Point> ARPickerPositionProperty = AvaloniaProperty.Register<DualPanelColorPicker, Point>(nameof(ARPickerPosition));
        private static readonly StyledProperty<Point> GBPickerPositionProperty = AvaloniaProperty.Register<DualPanelColorPicker, Point>(nameof(GBPickerPosition));

        public Color? Color
        {
            get => GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public BoxShadows? Shadows
        {
            get => GetValue(ShadowsProperty);
            set => SetValue(ShadowsProperty, value);
        }

        public bool UseDarkMode
        {
            get => GetValue(UseDarkModeProperty);
            set => SetValue(UseDarkModeProperty, value);
        }
        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public double PickerRadius
        {
            get => GetValue(PickerRadiusProperty);
            set => SetValue(PickerRadiusProperty, value);
        }

        public double Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public bool? ClaymorphismShadows
        {
            get => GetValue(ClaymorphismShadowsProperty);
            set => SetValue(ClaymorphismShadowsProperty, value);
        }
        private Point ARPickerPosition
        {
            get => GetValue(ARPickerPositionProperty);
            set => SetValue(ARPickerPositionProperty, value);
        }
        private Point GBPickerPosition
        {
            get => GetValue(GBPickerPositionProperty);
            set => SetValue(GBPickerPositionProperty, value);
        }
        public Thickness Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        private static LinearGradientBrush
            CheckerboardColoredPositiveRows = GenCheckerboard(Colors.Gray, Colors.White, Orientation.Horizontal, 16),
            CheckerboardColoredNegativeRows = GenCheckerboard(Colors.White, Colors.Gray, Orientation.Horizontal, 16),
            CheckerboardMask = GenCheckerboard(Colors.Transparent, Colors.White, Orientation.Vertical, 16),
            CCheckerboardColoredPositiveRows = GenCheckerboard(Colors.Gray, Colors.White, Orientation.Horizontal, 4),
            CCheckerboardColoredNegativeRows = GenCheckerboard(Colors.White, Colors.Gray, Orientation.Horizontal, 4),
            CCheckerboardMask = GenCheckerboard(Colors.Transparent, Colors.White, Orientation.Vertical, 4),
            MaskAlpha = GenGradient(Colors.Black, Colors.Transparent, Orientation.Horizontal);

        private LinearGradientBrush
            ARRed = GenGradient(AColor.FromRgb(255, 0, 0), AColor.FromRgb(0, 0, 0), Orientation.Vertical),
            ARGreen = GenGradient(AColor.FromRgb(0, 255, 255), AColor.FromRgb(0, 255, 0), Orientation.Vertical),
            ARBlue = GenGradient(AColor.FromRgb(0, 0, 255), AColor.FromRgb(0, 0, 0), Orientation.Vertical);

        private ActionSpecificButtonBase
            ARRightASBB = new ActionSpecificButtonBase(AColor.FromArgb(255, 255, 0, 0), AColor.FromArgb(255, 0, 0, 0), false),
            ARLeftASBB = new ActionSpecificButtonBase(AColor.FromArgb(255, 0, 0, 0), AColor.FromArgb(0, 0, 0, 0), false),
            GBRightASBB = new ActionSpecificButtonBase(AColor.FromArgb(255, 0, 0, 255), AColor.FromArgb(255, 0, 255, 0), false),
            GBLeftASBB = new ActionSpecificButtonBase(AColor.FromArgb(255, 0, 255, 255), AColor.FromArgb(255, 0, 255, 0), false);

        private double
            PaneWidth = 0,
            PaneHeight = 0,
            Pane1X = 0,
            Pane2X = 0,
            Pane1Y = 0,
            Pane2Y = 0;

        private bool
            ARColorPickerHold = false,
            GBColorPickerHold = false;

        private void OnColorChanged()
        {
            if (ColorChanged != null)
                ColorChanged.Invoke(this, Color);
        }

        private void UpdateGeometry()
        {
            Pane1X = Padding.Left;
            Pane1Y = Padding.Top;

            if (Orientation == Orientation.Horizontal)
            {
                PaneWidth = ((Bounds.Width) - (2 * (Padding.Left + Padding.Right))) / 2;
                PaneHeight = Bounds.Height - Padding.Top - Padding.Bottom;
                Pane2X = (Padding.Left * 2) + PaneWidth + Padding.Right;
                Pane2Y = Pane1Y;
            }
            else
            {
                PaneWidth = Bounds.Width - Padding.Left - Padding.Right;
                PaneHeight = ((Bounds.Height / 2) - (2 * (Padding.Top) + Padding.Bottom)) / 2;
                Pane2Y = (Padding.Top * 2) + PaneHeight + Padding.Bottom;
                Pane2X = Pane1X;
            }
            UpdatePickersPosition();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            UpdateGeometry();
            UpdateGradients();
            InvalidateVisual();
            base.OnLoaded(e);
        }

        private void UpdateGradients()
        {
            if (Color == null)
                return;
            GBRightASBB = new ActionSpecificButtonBase(AColor.FromArgb(Color.Value.A, Color.Value.R, 0, 255), AColor.FromArgb(Color.Value.A, Color.Value.R, 255, 0), false);
            GBLeftASBB = new ActionSpecificButtonBase(AColor.FromArgb(Color.Value.A, Color.Value.R, 255, 255), AColor.FromArgb(Color.Value.A, Color.Value.R, 255, 0), false);
            ARRed = GenGradient(AColor.FromArgb(255, 255, Color.Value.G, Color.Value.B), AColor.FromArgb(255, 0, Color.Value.G, Color.Value.B), Orientation.Vertical);
            ARGreen = GenGradient(AColor.FromArgb(Color.Value.A, Color.Value.R, 255, 255), AColor.FromArgb(Color.Value.A, Color.Value.R, 255, 0), Orientation.Vertical);
            ARBlue = GenGradient(AColor.FromArgb(Color.Value.A, Color.Value.R, 0, 255), AColor.FromArgb(Color.Value.A, Color.Value.R, 0, 0), Orientation.Vertical);
        }

        private void UpdatePickersPosition()
        {
            if (Color == null)
            {
                ARPickerPosition = new Point(Pane1X, Pane1Y);
                GBPickerPosition = new Point(Pane2X, Pane2Y);
                return;
            }
            double
                ARX = Pane2X + (PaneWidth * (1 -(((double)Color.Value.A) / 255d))),
                ARY = Pane2Y + (PaneHeight * (((double)Color.Value.R) / 255d)),
                GBX = Pane1X + (PaneWidth * ((((double)Color.Value.G) / 255d))),
                GBY = Pane1Y + (PaneHeight * (1 -(((double)Color.Value.B) / 255d)));
            ARPickerPosition = new Point(ARX, ARY);
            GBPickerPosition = new Point(GBX, GBY);
        }

        private static Color? ValidateColor(AvaloniaObject sender, Color? value)
        {
            ((DualPanelColorPicker)sender).UpdateGradients();
            ((DualPanelColorPicker)sender).UpdatePickersPosition();
            ((DualPanelColorPicker)sender).OnColorChanged();
            return value;
        }
        private static Orientation ValidateOrientation(AvaloniaObject sender, Orientation value)
        {
            ((DualPanelColorPicker)sender).UpdateGeometry();
            return value;
        }
        private static Thickness ValidateOffset(AvaloniaObject sender, Thickness value)
        {
            ((DualPanelColorPicker)sender).UpdateGeometry();
            return value;
        }

        private static LinearGradientBrush GenGradient(Color startColor, RelativePoint startPoint, Color endColor, RelativePoint endPoint)
        {
            return new LinearGradientBrush()
            {
                StartPoint = startPoint,
                EndPoint = endPoint,
                GradientStops = new GradientStops()
                {
                    new GradientStop(startColor, 0),
                    new GradientStop(endColor, 1),
                }
            };
        }

        private static LinearGradientBrush GenGradient(Color startColor, Color endColor, Orientation orientation)
        {
            return GenGradient(startColor, new RelativePoint(0, 0, RelativeUnit.Relative), endColor, new RelativePoint(orientation == Orientation.Horizontal ? 1 : 0, orientation == Orientation.Horizontal ? 0 : 1, RelativeUnit.Relative));
        }

        private static LinearGradientBrush GenCheckerboard(Color color1, Color color2, RelativePoint start, RelativePoint end, int cells)
        {
            if (cells < 1)
                cells = 1;
            double step = 1 / (double)cells;
            GradientStops stops = new GradientStops();
            bool isFirst = true;
            stops.Add(new GradientStop(color1, 0));
            for (int i = 1; i < cells; i++)
            {
                stops.Add(new GradientStop(isFirst ? color1 : color2, i * step));
                isFirst = !isFirst;
                stops.Add(new GradientStop(isFirst ? color1 : color2, i * step));
            }
            stops.Add(new GradientStop(isFirst ? color1 : color2, 1));

            return new LinearGradientBrush
            {
                StartPoint = start,
                EndPoint = end,
                GradientStops = stops
            };
        }

        private static LinearGradientBrush GenCheckerboard(Color color1, Color color2, Orientation orientation, int cells)
        {
            return GenCheckerboard(color1, color2, new RelativePoint(0, 0, RelativeUnit.Relative), new RelativePoint(orientation == Orientation.Horizontal ? 1 : 0, orientation == Orientation.Horizontal ? 0 : 1, RelativeUnit.Relative), cells);
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            UpdateGeometry();
            base.OnSizeChanged(e);
        }

        public override sealed void Render(DrawingContext context)
        {
            var bounds = new Rect(Bounds.Size);
            context.DrawRectangle(Background, null, new RoundedRect(bounds, CornerRadius));
            var pane1 = new RoundedRect(new Rect(Pane1X, Pane1Y, PaneWidth, PaneHeight), CornerRadius);
            var pane2 = new RoundedRect(new Rect(Pane2X, Pane2Y, PaneWidth, PaneHeight), CornerRadius);
            DrawCheckerboard(context, pane1);
            DrawCheckerboard(context, pane2);
            Draw2DGradientedBorder(context, ARGreen, GBLeftASBB.DefaultStateShadows, ARBlue, GBRightASBB.DefaultStateShadows, MaskAlpha, pane1);
            Draw2DGradientedBorder(context, Brushes.Transparent, ARLeftASBB.DefaultStateShadows, ARRed, ARRightASBB.DefaultStateShadows, MaskAlpha, pane2);
            if(Color != null)
            {
                var pr = (PickerRadius / 2);
                var rc1 = new RoundedRect(new Rect(GBPickerPosition.X + Pane1X - pr, GBPickerPosition.Y + Pane1Y - pr, PickerRadius, PickerRadius), pr, pr);
                var rc2 = new RoundedRect(new Rect(ARPickerPosition.X + Pane2X - pr, ARPickerPosition.Y + Pane2Y - pr, PickerRadius, PickerRadius), pr, pr);
                DrawCheckerboardCircle(context, ARPickerPosition, PickerRadius);
                DrawCheckerboardCircle(context, GBPickerPosition, PickerRadius);
                context.DrawEllipse(new SolidColorBrush(Color.Value), null, ARPickerPosition, PickerRadius, PickerRadius);
                context.DrawEllipse(new SolidColorBrush(Color.Value), null, GBPickerPosition, PickerRadius, PickerRadius);
            }
            base.Render(context);
        }

        private static void DrawCheckerboard(DrawingContext context, RoundedRect rect)
        {
            var cr = new CornerRadius(rect.RadiiTopLeft.X, rect.RadiiTopRight.X, rect.RadiiBottomLeft.X, rect.RadiiBottomRight.X);
            var rcRb = new RoundedRect(new Rect(0, 0, rect.Rect.Width, rect.Rect.Height), cr);
            var clip = context.PushTransform(new TranslateTransform(rect.Rect.Left, rect.Rect.Top).Value);
            context.DrawRectangle(CheckerboardColoredNegativeRows, null, rcRb);
            var cbp1 = context.PushOpacityMask(CheckerboardMask, rcRb.Rect);
            context.DrawRectangle(CheckerboardColoredPositiveRows, null, rcRb);
            cbp1.Dispose();
            clip.Dispose();
        }


        private static void DrawCheckerboardCircle(DrawingContext context, Point mid, double radius)
        {
            var transform = context.PushTransform(new TranslateTransform(mid.X - radius, mid.Y - radius).Value);
            context.DrawEllipse(CCheckerboardColoredNegativeRows, null, new Point(radius, radius), radius, radius);
            var cbp1 = context.PushOpacityMask(CCheckerboardMask, new Rect(0, 0, radius * 2, radius * 2));
            context.DrawEllipse(CCheckerboardColoredPositiveRows, null, new Point(radius, radius), radius, radius);
            cbp1.Dispose();
            transform.Dispose();
        }

        private static void Draw2DGradientedBorder(DrawingContext context, IBrush gradient1, BoxShadows? shadows1, IBrush gradient2, BoxShadows? shadows2, IBrush maskGradient2, RoundedRect rect)
        {
            var cr = new CornerRadius(rect.RadiiTopLeft.X, rect.RadiiTopRight.X, rect.RadiiBottomLeft.X, rect.RadiiBottomRight.X);
            var rcRb = new RoundedRect(new Rect(0, 0, rect.Rect.Width, rect.Rect.Height), cr);
            var clip = context.PushTransform(new TranslateTransform(rect.Rect.Left, rect.Rect.Top).Value);
            context.DrawRectangle(gradient1, null, rcRb, shadows1 ?? default(BoxShadows));
            var cbp1 = context.PushOpacityMask(maskGradient2, rcRb.Rect);
            context.DrawRectangle(gradient2, null, rcRb, shadows2 ?? default(BoxShadows));
            cbp1.Dispose();
            clip.Dispose();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            var pos = e.GetPosition(this);
            GBColorPickerHold = (pos.X >= Pane1X && pos.Y >= Pane1Y && pos.X <= Pane1X + PaneWidth && pos.Y <= Pane1Y + PaneHeight);
            ARColorPickerHold = (pos.X >= Pane2X && pos.Y >= Pane2Y && pos.X <= Pane2X + PaneWidth && pos.Y <= Pane2Y + PaneHeight);
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            ARColorPickerHold = GBColorPickerHold = false;
            base.OnPointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            var pos = e.GetPosition(this);
            if (ARColorPickerHold)
            {
                double x = (pos.X - Pane2X) / PaneWidth;
                double y = (pos.Y - Pane2Y) / PaneHeight;
                byte a = (byte)(Math.Clamp(1 - x, 0, 1) * 255);
                byte r = (byte)(Math.Clamp(y, 0, 1) * 255);
                var c = Color ?? AColor.FromRgb(255, 255, 255);
                Color = AColor.FromArgb(a, r, c.G, c.B);
            }
            if (GBColorPickerHold)
            {
                double x = (pos.X - Pane1X) / PaneWidth;
                double y = (pos.Y - Pane1Y) / PaneHeight;
                byte g = (byte)(Math.Clamp(x, 0, 1) * 255);
                byte b = (byte)(Math.Clamp(1-y, 0, 1) * 255);
                var c = Color ?? AColor.FromRgb(255, 255, 255);
                Color = AColor.FromArgb(c.A, c.R, g, b);
            }
            base.OnPointerMoved(e);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Horizontal)
                return new Size(
                        (Size + Padding.Left + Padding.Right) * 2,
                        (Size + Padding.Top + Padding.Bottom)
                    );
            return new Size(
                        (Size + Padding.Left + Padding.Right),
                        (Size + Padding.Top + Padding.Bottom) * 2
                );
        }
    }
}
