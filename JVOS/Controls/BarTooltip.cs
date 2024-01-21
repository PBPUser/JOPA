using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class BarTooltip : Thumb
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);

        static BarTooltip()
        {
            IconProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, Bitmap>("Icon");
            TitleProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, string>("Title");
            ActiveBoxShadowsProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, BoxShadows>("ActiveBoxShadows");
            BoxShadowsProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, BoxShadows>("BoxShadows");
            ScaleProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, double>("Scale");
            ShadowPosProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, double>("ShadowPos");
            DragDeltaXProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, double>("DragDeltaX");
            StartAnimationProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Thumb, double>("StartAnimation");
            AffectsRender<BarTooltip>(
                IconProperty, 
                TitleProperty,
                ScaleProperty,
                ActiveBoxShadowsProperty,
                BoxShadowsProperty,
                DragDeltaXProperty,
                StartAnimationProperty
                );
        }
        
        public BarTooltip()
        {
            StartAnimation = 0;
            Loaded += (a, b) =>
            {
                StartAnimation = 1;
            };
        }


        public static readonly AttachedProperty<BoxShadows> ActiveBoxShadowsProperty;
        public static readonly AttachedProperty<BoxShadows> BoxShadowsProperty;
        public static readonly AttachedProperty<Bitmap> IconProperty;
        public static readonly AttachedProperty<string> TitleProperty;
        public static readonly AttachedProperty<double> ScaleProperty;
        public static readonly AttachedProperty<double> DragDeltaXProperty;
        public static readonly AttachedProperty<double> ShadowPosProperty;
        public static readonly AttachedProperty<double> StartAnimationProperty;

        public DoubleTransition DeltaTransition = new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(175), Property = DragDeltaXProperty };

        private void UpdateFormattedText()
        {
            fText = new(Title, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(FontFamily.Default), 14, Brushes.Black);
        }

        private FormattedText fText;

        private double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        public double StartAnimation
        {
            get => (double)GetValue(StartAnimationProperty);
            set => SetValue(StartAnimationProperty, value);
        }
        private double DragDeltaX
        {
            get => (double)GetValue(DragDeltaXProperty);
            set => SetValue(DragDeltaXProperty, value);
        }
        private double ShadowPos
        {
            get => (double)GetValue(ShadowPosProperty);
            set => SetValue(ShadowPosProperty, value);
        }
        
        public BoxShadows ActiveBoxShadows
        {
            get => (BoxShadows)GetValue(ActiveBoxShadowsProperty);
            set => SetValue(ActiveBoxShadowsProperty, value);
        }

        public BoxShadows BoxShadows
        {
            get => (BoxShadows)GetValue(BoxShadowsProperty);
            set => SetValue(BoxShadowsProperty, value);
        }

        public Bitmap Icon
        {
            get => (Bitmap)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private static int TopZIndex = 1;

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set {
                SetValue(TitleProperty, value);
                UpdateFormattedText();
            }
        }

        int prevIndex = 0;
        double startDeltaX = 0;

        protected override void OnDragDelta(VectorEventArgs e)
        {
            var c = (startDeltaX - Bounds.X);
            var r = e.Vector.X - (startDeltaX - Bounds.X);
            var s = ((StackPanel)Parent);
            var rt = 0d;
            double deltaX = Math.Clamp(r, -Bounds.Left, ((Control)Parent).Bounds.Width - Bounds.Width - Bounds.Left);
            
            DragDeltaX = deltaX - rt;
            base.OnDragDelta(e);
        }

        protected override void OnDragCompleted(VectorEventArgs e)
        {
            var s = ((StackPanel)Parent);
            double deltaX = Math.Clamp(e.Vector.X, -Bounds.Left, ((Control)Parent).Bounds.Width - Bounds.Width - Bounds.Left);
            var rt = Bounds.X;
            int j = (int)((deltaX) / Bounds.Width);
            if (Math.Abs(deltaX) > Bounds.Width / 2)
            {
                s.Children.Remove(this);
                s.Children.Insert(Math.Clamp(prevIndex + j, 0, s.Children.Count), this);
            }
            Debug.WriteLine(Bounds.X - rt);
            DragDeltaX = DragDeltaX % (Bounds.Width+Margin.Left+Margin.Right);
            new Task(() =>
            {
                Thread.Sleep(10);
                Dispatcher.UIThread.Invoke(() =>
                {
                    DeltaTransition.Duration = TimeSpan.FromMilliseconds(175);
                    DragDeltaX = 0;
                });
            }).Start();
            base.OnDragCompleted(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            Scale = 1;
            startDeltaX = Bounds.X;
            ShadowPos = 1;
            prevIndex = ((StackPanel)Parent).Children.IndexOf(this);
            ZIndex = TopZIndex++;
            DeltaTransition.Duration = TimeSpan.Zero;
            base.OnPointerPressed(e);
        }
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            Scale = 0;
            ShadowPos = 0;
            base.OnPointerReleased(e);
        }

        public override void Render(DrawingContext context)
        {
            context.DrawRectangle(TransparentA1, null, new Rect(Bounds.Size));
            var dp = 1 - (0.2 * Scale);
            var d = context.PushTransform(new ScaleTransform(dp, dp).Value);
            context.DrawImage(Icon, new Rect(4 + (Scale * 0.1 * (Bounds.Width) * StartAnimation) + DragDeltaX * (1 / dp), 4 + (Scale * 0.1 * Bounds.Height * StartAnimation), Bounds.Height - 8, Bounds.Height - 8));
            context.DrawText(fText, new Point(Bounds.Height + (Scale * 0.1 * (Bounds.Width)) + DragDeltaX * (1 / dp), (Bounds.Height-fText.Height) / 2 + (Scale * 0.1 * Bounds.Height)));
            d.Dispose();
            var j = context.PushOpacity(1 - ShadowPos);
            context.DrawRectangle(null, null, new Rect(new Point(DragDeltaX, 0), Bounds.Size), 12, 12, this.BoxShadows);
            j.Dispose();
            var jt = context.PushOpacity(ShadowPos);
            context.DrawRectangle(null, null, new Rect(new Point(DragDeltaX, 0), Bounds.Size), 12, 12, this.ActiveBoxShadows);
            jt.Dispose();

            base.Render(context);
        }
    }
}
