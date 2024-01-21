    using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace JVOS.Controls
{
    public class JButton : Button
    {
        public JButton()
        {
            Transitions = MASTER_TRANSITIONS;
        }

        static JButton()
        {
            var gradStops = new GradientStops();
            gradStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 0));
            gradStops.Add(new GradientStop(Color.FromArgb(128, 255, 255, 255), 0.1));
            gradStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.2));
            DEFAULT_GLAREBRUSH = new LinearGradientBrush()
            {
                GradientStops = gradStops,
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative)
            };
            DEFAULT_ACTIVEBRUSH.Color = Color.FromArgb(0, 0, 192, 255);
            DEFAULT_HOVERBRUSH.Color = Color.FromArgb(0, 0, 128, 255);
            AffectsRender<JButton>(
                BackgroundProperty,
                BorderBrushProperty,
                ContentProperty,
                BorderThicknessProperty,
                CornerRadiusProperty,
                BoxShadowsProperty,
                ActiveBoxShadowsProperty,
                HovergroundProperty,
                OvergroundProperty,
                GlareProperty,
                GlarebrushProperty,
                OverProperty,
                HoverProperty,
                IsFocusedProperty);
            MASTER_TRANSITIONS.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(100), Property = OverProperty });
            MASTER_TRANSITIONS.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(100), Property = HoverProperty });
            MASTER_TRANSITIONS.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(300), Property = GlareProperty });
        }

        private static LinearGradientBrush DEFAULT_GLAREBRUSH = new LinearGradientBrush();
        private static SolidColorBrush DEFAULT_ACTIVEBRUSH = new SolidColorBrush();
        private static SolidColorBrush DEFAULT_HOVERBRUSH = new SolidColorBrush();
        private static Transitions MASTER_TRANSITIONS = new Transitions();

        public enum RenderBoxShadow { AfterChild, BeforeChild }

        public static readonly StyledProperty<RenderBoxShadow> RenderShadowProperty = AvaloniaProperty.Register<JButton, RenderBoxShadow>(nameof(RenderShadow));
        public static readonly StyledProperty<BoxShadows> BoxShadowsProperty = AvaloniaProperty.Register<JButton, BoxShadows>(nameof(BoxShadows));
        public static readonly StyledProperty<BoxShadows> ActiveBoxShadowsProperty = AvaloniaProperty.Register<JButton, BoxShadows>(nameof(ActiveBoxShadows));
        public static readonly StyledProperty<Brush> HovergroundProperty = AvaloniaProperty.Register<JButton, Brush>(nameof(Hoverground), DEFAULT_HOVERBRUSH);
        public static readonly StyledProperty<Brush> GlarebrushProperty = AvaloniaProperty.Register<JButton, Brush>(nameof(Glarebrush), DEFAULT_GLAREBRUSH);
        public static readonly StyledProperty<Brush> OvergroundProperty = AvaloniaProperty.Register<JButton, Brush>(nameof(Overground), DEFAULT_ACTIVEBRUSH);

        private static readonly StyledProperty<double> OverProperty = AvaloniaProperty.Register<JButton, double>(nameof(Over));
        private static readonly StyledProperty<double> HoverProperty = AvaloniaProperty.Register<JButton, double>(nameof(Hover));
        private static readonly StyledProperty<double> GlareProperty = AvaloniaProperty.Register<JButton, double>(nameof(Glare));

        public RenderBoxShadow RenderShadow
        {
            get => GetValue(RenderShadowProperty);
            set => SetValue(RenderShadowProperty, value);
        }

        public BoxShadows BoxShadows
        {
            get => GetValue(BoxShadowsProperty);
            set => SetValue(BoxShadowsProperty, value);
        }

        public BoxShadows ActiveBoxShadows
        {
            get => GetValue(ActiveBoxShadowsProperty);
            set => SetValue(ActiveBoxShadowsProperty, value);
        }

        public Brush Hoverground
        {
            get => GetValue(HovergroundProperty);
            set => SetValue(HovergroundProperty, value);
        }
        public Brush Glarebrush
        {
            get => GetValue(GlarebrushProperty);
            set => SetValue(GlarebrushProperty, value);
        }
        public Brush Overground
        {
            get => GetValue(OvergroundProperty);
            set => SetValue(OvergroundProperty, value);
        }

        private double Over
        {
            get => GetValue(OverProperty);
            set => SetValue(OverProperty, value);
        }

        private double Hover
        {
            get => GetValue(HoverProperty);
            set => SetValue(HoverProperty, value);
        }

        private double Glare
        {
            get => GetValue(GlareProperty);
            set => SetValue(GlareProperty, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            Over = 1;
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            Over = 0;
            base.OnPointerReleased(e);
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            Hover = 1;
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            Hover = 0;
            base.OnPointerExited(e);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size bounds = new Size(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            if (Content is String)
            {
                var ft = new FormattedText(Content as string, CultureInfo.CurrentUICulture, FlowDirection, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground);
                return new Size(ft.Width, ft.Height) + bounds;
            }
            else if (Content is Control)
            {
                var ct = Content as Control;
                return ct.DesiredSize + bounds;
            }
            return base.MeasureOverride(availableSize);
        }

        public sealed override void Render(DrawingContext context)
        {
            var rrc = new RoundedRect(new Rect(0, 0, Bounds.Width, Bounds.Height), CornerRadius);
            bool drawAfter = RenderShadow != RenderBoxShadow.BeforeChild;
            var opDV = context.PushOpacity(0.0001);
            context.DrawRectangle(Brushes.Black, null, rrc, default(BoxShadows));
            opDV.Dispose();
            var opNHov = context.PushOpacity(1 - Over);
            context.DrawRectangle(Background, new Pen(BorderBrush, BorderThickness.Left), rrc, drawAfter ? default(BoxShadows) : BoxShadows);
            opNHov.Dispose();
            var opHov = context.PushOpacity(Hover);
            context.DrawRectangle(Hoverground, new Pen(BorderBrush, BorderThickness.Left), rrc, drawAfter ? default(BoxShadows) : BoxShadows);
            opHov.Dispose();
            var opAct = context.PushOpacity(Over);
            context.DrawRectangle(Overground, new Pen(BorderBrush, BorderThickness.Left), rrc, drawAfter ? default(BoxShadows) : ActiveBoxShadows);
            opAct.Dispose();
            var scaleCtrlValue = 1 - (0.1 * Over);
            var scaleCtrl = context.PushTransform(new ScaleTransform(scaleCtrlValue, scaleCtrlValue).Value);
            Point childLoc = new Point(0, 0);
            if (Content is String)
            {
                FormattedText? text;
                text = new FormattedText(Content as string, CultureInfo.CurrentUICulture, FlowDirection, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground);
                childLoc = GetChildLocation(new Size(text.Width, text.Height));
                context.DrawText(text, childLoc + (new Point(text.Width, text.Height) * (1 - scaleCtrlValue)));
            }
            else if (Content is Image)
            {
                var c = Content as Image;
                childLoc = GetChildLocation(new Size(c.Bounds.Width, c.Bounds.Height));
                var bl = context.PushTransform(new TranslateTransform((1-scaleCtrlValue)*((Bounds.Width - Padding.Left - Padding.Right)/2), (1 - scaleCtrlValue) * ((Bounds.Height - Padding.Top - Padding.Bottom) / 2)).Value);
                context.DrawImage(c.Source, new Rect(Padding.Left, Padding.Top, Bounds.Width - Padding.Left - Padding.Right, Bounds.Height - Padding.Top - Padding.Bottom));    
                bl.Dispose();
            }
            else if (Content is Control)
            {
                var c = Content as Control;
                c.Render(context);

            }
            scaleCtrl.Dispose();
            if (Glare > 0 && Glare < 1)
            {
                var opGla = context.PushOpacity(1 - Math.Abs(Glare * 2 - 1));
                Brush b = DEFAULT_GLAREBRUSH;
                TransformGroup g = new TransformGroup();
                g.Children.Add(new TranslateTransform(Glare * Bounds.Width, Glare * Bounds.Height));
                g.Children.Add(new ScaleTransform(1 + Glare, 1 + Glare));
                b.Transform = g;
                context.DrawRectangle(DEFAULT_GLAREBRUSH, new Pen(null, BorderThickness.Left), rrc, default(BoxShadows));
                opGla.Dispose();
            }
            if (drawAfter)
            {
                var xopNHov = context.PushOpacity(Math.Max(1 - (Over * 2), 0));
                context.DrawRectangle(null, new Pen(BorderBrush, BorderThickness.Left), rrc, BoxShadows);
                xopNHov.Dispose();
                var xopAct = context.PushOpacity(Math.Max(Over * 2 - 1, 0));
                context.DrawRectangle(null, new Pen(BorderBrush, BorderThickness.Left), rrc, ActiveBoxShadows);
                xopAct.Dispose();
            }
            base.Render(context);
        }

        private Point GetChildLocation(Size childSize)
        {
            double x = HorizontalContentAlignment switch
            {
                HorizontalAlignment.Left => Padding.Left + BorderThickness.Left,
                HorizontalAlignment.Right => Bounds.Width - Padding.Right - BorderThickness.Right - childSize.Width,
                HorizontalAlignment.Center => (Bounds.Width - childSize.Width) / 2,
                _ => 0
            };
            double y = VerticalContentAlignment switch
            {
                VerticalAlignment.Top => Padding.Top + BorderThickness.Top,
                VerticalAlignment.Bottom => Bounds.Height - Padding.Bottom - BorderThickness.Bottom - childSize.Height,
                VerticalAlignment.Center => (Bounds.Height - childSize.Height) / 2,
                _ => 0
            };
            return new Point(x, y);
        }

        public void DoGlare()
        {
            if (Glare > 0)
                Glare = 0;
            else
                Glare = 1;
        }
    }
}
