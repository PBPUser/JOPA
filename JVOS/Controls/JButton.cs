using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using JVOS.Views;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class JButton : Button
    {
        public JButton()
        {
            Transitions = new Avalonia.Animation.Transitions();
            Transitions.Add(new DoubleTransition { Duration = TimeSpan.FromMilliseconds(150), Property = AnimationPressedProperty, Easing = new SineEaseIn() });
            Transitions.Add(new DoubleTransition { Duration = TimeSpan.FromMilliseconds(150), Property = AnimationHoverProperty, Easing = new SineEaseIn() });
        }

        static JButton()
        {
            AffectsRender<JButton>(
                BackgroundProperty,
                BorderBrushProperty,
                ContentProperty,
                BorderThicknessProperty,
                CornerRadiusProperty,
                BoxShadowProperty,
                IsPressedProperty,
                IsPointerOverProperty,
                AnimationPressedProperty,
                AnimationHoverProperty,
                IsFocusedProperty);
        }

        bool
            IsPointerOver = false, IsMouseDown = false;

        public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
            AvaloniaProperty.Register<JButton, BoxShadows>(nameof(BoxShadow));
        public static readonly StyledProperty<Brush> HovergroundProeprty =
            AvaloniaProperty.Register<JButton, Brush>(nameof(Hoverground));

        private static readonly StyledProperty<double> AnimationPressedProperty =
            AvaloniaProperty.Register<JButton, double>(nameof(AnimationPressed));
        private static readonly StyledProperty<double> AnimationHoverProperty =
            AvaloniaProperty.Register<JButton, double>(nameof(AnimationHover));

        private double AnimationPressed
        {
            get => (double)GetValue(AnimationPressedProperty);
            set => SetValue(AnimationPressedProperty, value);
        }
        private double AnimationHover
        {
            get => (double)GetValue(AnimationHoverProperty);
            set => SetValue(AnimationHoverProperty, value);
        }

        public Brush Hoverground
        {
            get => (Brush)GetValue(HovergroundProeprty);
            set => SetValue(HovergroundProeprty, value);
        }

        public BoxShadows BoxShadow
        {
            get => (BoxShadows)GetValue(BoxShadowProperty);
            set => SetValue(BoxShadowProperty, value);
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            IsPointerOver = true;
            AnimationHover = 1;
            InvalidateVisual();
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            InvalidateVisual();
            AnimationHover = 0;
            AnimationPressed = 0;
            IsPointerOver = false;
            IsMouseDown = false;
            base.OnPointerExited(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            InvalidateVisual();
            AnimationPressed = 1;
            IsMouseDown = true;
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            InvalidateVisual();
            AnimationPressed = 0;
            IsMouseDown = false;
            base.OnPointerReleased(e);
        }

        public override void Render(DrawingContext context)
        {
            var val = new ScaleTransform(1 - .1 * AnimationPressed, 1 - .1 * AnimationPressed).Value;
            context.PushTransform(new TranslateTransform(Bounds.Width * 0.05 * AnimationPressed, Bounds.Height * 0.05 * AnimationPressed).Value);
            context.PushTransform(val);
            var j = new Rect(Bounds.Size);
            Vector[] vecs = new Vector[] { new Vector(CornerRadius.TopLeft, CornerRadius.TopLeft), new Vector(CornerRadius.TopRight, CornerRadius.TopLeft), new Vector(CornerRadius.BottomLeft, CornerRadius.BottomLeft), new Vector(CornerRadius.BottomRight, CornerRadius.BottomRight) };
            var rc = new RoundedRect(j, vecs[0], vecs[1], vecs[2], vecs[3]);
            var pushOpc1 = context.PushOpacity(1-AnimationHover);
            context.DrawRectangle(new SolidColorBrush(Colors.Black, .00001), null, rc);
            context.DrawRectangle(Background, null, rc);
            pushOpc1.Dispose();
            var pushOpc2 = context.PushOpacity(1 - AnimationHover);
            context.DrawRectangle(Hoverground, null, rc);
            pushOpc2.Dispose();
            
            context.DrawRectangle(null, new Pen(BorderBrush, BorderThickness.Left, null, PenLineCap.Round, PenLineJoin.Miter, 10), rc, BoxShadow);
            if (Content is Control)
            {
                var marginC = Padding;
                RoundedRect rcc = new RoundedRect(new Rect(marginC.Left, marginC.Top, rc.Rect.Size.Width - marginC.Left - marginC.Right, rc.Rect.Size.Height - marginC.Top - marginC.Bottom));
                if(Content is Image)
                    context.DrawRectangle(new VisualBrush(Content as Image), null, rc);
                else if(Content is Control)
                    context.DrawRectangle(new VisualBrush(Content as Visual), null, rc);
            }
            else if(Content as string != null)
            {
                var text = new FormattedText((string)Content, System.Globalization.CultureInfo.CurrentCulture, FlowDirection, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground);
                context.DrawText(text, new Point((Bounds.Width - text.Width) / 2, (Bounds.Height - text.Height) / 2));
            }
            else if(Content != null)
            {
                App.MainWindowInstance.Title = (Content.ToString() + (Content is Control));
            }
            base.Render(context);
        }
    }
}
