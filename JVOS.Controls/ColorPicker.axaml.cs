using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Drawing;
using Brushes = Avalonia.Media.Brushes;
using Color = Avalonia.Media.Color;

namespace JVOS.Controls
{
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
            InitializePicker();
            Background = new SolidColorBrush(Colors.White);
            LostFocus += (a, b) =>
            {
                isPick1 = isPick2 = false;
            };
        }

        private LinearGradientBrush GRADIENT_MASK_PICKER = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0d, 0d, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1d, 0d, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(0, 255, 255, 255), 0),
                new GradientStop(Color.FromArgb(255, 255, 255, 255), 1)
            }
        };
        private LinearGradientBrush GRADIENT_GB_BLUE_PICKER = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0d, 0d, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0d, 1d, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(255, 0, 0, 255), 0),
                new GradientStop(Color.FromArgb(255, 0, 0, 0), 1),
            }
        };

        private LinearGradientBrush GRADIENT_GB_GREEN_PICKER = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0d, 1d, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0d, 0d, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(255, 0, 255, 0), 0),
                new GradientStop(Color.FromArgb(255, 0, 255, 255), 1),
            }
        };
        private LinearGradientBrush GRADIENT_AL_RED_PICKER = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0d, 0d, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0d, 1d, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(255, 255, 0, 0), 0),
                new GradientStop(Color.FromArgb(255, 0, 0, 0), 1),
            }
        };

        private LinearGradientBrush GRADIENT_GB_ALPHA_PICKER = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(1d, 0d, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0d, 0d, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(0, 255, 0, 0), 0),
                new GradientStop(Color.FromArgb(255, 255, 0, 0), 1),
            }
        };


        bool isPick1 = false;
        bool isPick2 = false;
        TextBlock tx = new TextBlock() { Background = Brushes.Black, Foreground = Brushes.White, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom };
        TranslateTransform translateTransform1 = new TranslateTransform();
        TranslateTransform translateTransform2 = new TranslateTransform();
        Border bPick1 = new Border() { Height = 20, Width = 20, CornerRadius = new CornerRadius(3), BorderThickness = new Thickness(3), BorderBrush = Brushes.Black };
        Border bPick2 = new Border() { Height = 20, Width = 20, CornerRadius = new CornerRadius(3), BorderThickness = new Thickness(3), BorderBrush = Brushes.Black };
        Border
            gbBorder1,
            gbBorder2,
            arBorder1,
            gbBoxShadow1,
            arBoxShadow1,
            gbBoxShadow2,
            arBoxShadow2;


        private void InitializePicker()
        {
            bPick1.RenderTransform = translateTransform1;
            bPick2.RenderTransform = translateTransform2;
            bPick1.HorizontalAlignment = bPick2.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            bPick1.VerticalAlignment = bPick2.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            Button b = new Button();
            b.Click += (a, x) =>
            {
                rootPicker.Children.Clear();
                InitializePicker();
            };
            Thickness t = new Thickness(8);
            gbBorder1 = new Border() { Margin = t, CornerRadius = new CornerRadius(8), Background = GRADIENT_GB_GREEN_PICKER, OpacityMask = GRADIENT_MASK_PICKER };
            gbBorder2 = new Border() { Margin = t, CornerRadius = new CornerRadius(8), Background = GRADIENT_GB_BLUE_PICKER };
            arBorder1 = new Border() { Margin = t, CornerRadius = new CornerRadius(8), Background = GRADIENT_AL_RED_PICKER, OpacityMask = GRADIENT_GB_ALPHA_PICKER };
            gbBoxShadow1 = new Border() { Margin = t, CornerRadius = new CornerRadius(8) };
            gbBoxShadow2 = new Border() { Margin = t, CornerRadius = new CornerRadius(8) };
            arBoxShadow1 = new Border() { Margin = t, CornerRadius = new CornerRadius(8) };
            arBoxShadow2 = new Border() { Margin = t, CornerRadius = new CornerRadius(8) };
            ActionSpecificButtonBase arAct1 = new ActionSpecificButtonBase(Color.FromArgb(255, 255, 255, 255), false);
            var xx = new Border()
            {
                Margin = t,
                CornerRadius = new CornerRadius(8),
                Background = GenCheckerboard(Colors.Gray, Colors.White, new RelativePoint(0, 0, RelativeUnit.Relative), new RelativePoint(0, 1, RelativeUnit.Relative), 20)
            };
            var xx2 = new Border()
            {
                Margin = t,
                CornerRadius = new CornerRadius(8),
                Background = GenCheckerboard(Colors.White, Colors.Gray, new RelativePoint(0, 0, RelativeUnit.Relative), new RelativePoint(0, 1, RelativeUnit.Relative), 20),
                OpacityMask = GenCheckerboard(Colors.Transparent, Colors.White, new RelativePoint(0, 0, RelativeUnit.Relative), new RelativePoint(1, 0, RelativeUnit.Relative), 20),
            }; 
            var xx1 = new Border()
            {
                Margin = t,
                CornerRadius = new CornerRadius(8),
                Background = GenCheckerboard(Colors.Gray, Colors.White, new RelativePoint(0, 0, RelativeUnit.Relative), new RelativePoint(0, 1, RelativeUnit.Relative), 20)
            };
            var xx12 = new Border()
            {
                Margin = t,
                CornerRadius = new CornerRadius(8),
                Background = GenCheckerboard(Colors.White, Colors.Gray, new RelativePoint(0, 0, RelativeUnit.Relative), new RelativePoint(0, 1, RelativeUnit.Relative), 20),
                OpacityMask = GenCheckerboard(Colors.Transparent, Colors.White, new RelativePoint(0, 0, RelativeUnit.Relative), new RelativePoint(1, 0, RelativeUnit.Relative), 20),
            };
            rootPicker.Children.Add(xx1);
            rootPicker.Children.Add(xx12);
            rootPicker.Children.Add(gbBorder2);
            rootPicker.Children.Add(gbBorder1);
            rootPicker.Children.Add(xx);
            rootPicker.Children.Add(xx2);
            rootPicker.Children.Add(arBorder1);
            var pick = new Border() { Margin = t, Background = new SolidColorBrush() { Color = Colors.Black, Opacity = 0.001 } };
            var pick2 = new Border() { Margin = t, Background = new SolidColorBrush() { Color = Colors.Black, Opacity = 0.001 } };
            pick.PointerReleased += (a, b) =>
            {
                isPick1 = false;
            };
            pick.PointerMoved += Pick_PointerMoved;
            pick.PointerPressed += (a, b) =>
            {
                isPick1 = true;
            };
            pick2.PointerReleased += (a, b) =>
            {
                isPick2 = false;
            };
            pick2.PointerMoved += Pick_PointerMoved;
            pick2.PointerPressed += (a, b) =>
            {
                isPick2 = true;
            };
            rootPicker.Children.Add(bPick1);
            rootPicker.Children.Add(bPick2);
            rootPicker.Children.Add(pick);
            rootPicker.Children.Add(pick2);
            Grid.SetColumn(xx, 1);
            Grid.SetColumn(xx2, 1);
            Grid.SetColumn(arBorder1, 1);
            Grid.SetColumn(bPick2, 1);
            Grid.SetColumn(pick2, 1);
        }

        private LinearGradientBrush GenGradient(Color startColor, RelativePoint startPoint, Color endColor, RelativePoint endPoint)
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

        private LinearGradientBrush GenGradient(Color startColor, Color endColor, Orientation orientation)
        {
            return GenGradient(startColor, new RelativePoint(0, 0, RelativeUnit.Relative), endColor, new RelativePoint(orientation == Orientation.Horizontal ? 1 : 0, orientation == Orientation.Horizontal ? 0 : 1, RelativeUnit.Relative));
        }

        private LinearGradientBrush GenCheckerboard(Color color1, Color color2, RelativePoint start, RelativePoint end, int width)
        {
            if (width < 1)
                width = 1;
            double step = 1 / (double)width;
            GradientStops stops = new GradientStops();
            bool isFirst = true;
            stops.Add(new GradientStop(color1, 0));
            for(int i = 1; i < width; i++)
            {
                stops.Add(new GradientStop(isFirst? color1 : color2, i * step));
                isFirst = !isFirst;
                stops.Add(new GradientStop(isFirst? color1 : color2, i * step));
            }
            stops.Add(new GradientStop(isFirst? color1 : color2, 1));

            return new LinearGradientBrush
            {
                StartPoint = start,
                EndPoint = end,
                GradientStops = stops
            };
        }

        byte
            r = 255,
            g = 255,
            b = 255,
            a = 255;

        public Color GetColor()
        {
            return Color.FromArgb(a, r, g, b);
        }

        private void Pick_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (isPick1 || isPick2)
            {
                var p = e.GetPosition(((Border)sender));
                var j = ((Border)sender).Bounds;
                var x = (((Math.Clamp(p.X, 0, j.Width) / j.Width)));
                var y = (((Math.Clamp(p.Y, 0, j.Height) / j.Height)));
                if (isPick1)
                {
                    g = (byte)(x * 255);
                    b = (byte)((1 - y) * 255);
                    translateTransform1.X = Math.Clamp(p.X, 0, j.Width);
                    translateTransform1.Y = Math.Clamp(p.Y, 0, j.Height);
                }
                if (isPick2)
                {
                    r = (byte)((1 - y) * 255);
                    a = (byte)((1 - x) * 255);
                    translateTransform2.X = Math.Clamp(p.X, 0, j.Width);
                    translateTransform2.Y = Math.Clamp(p.Y, 0, j.Height);
                }
                tx.Text = $"{p.X} {p.Y}";
            }
            var color = Color.FromArgb(a, r, g, b);
            ActionSpecificButtonBase actSpBtnBase = new ActionSpecificButtonBase(color, false);
            bPick1.Background = bPick2.Background = new LinearGradientBrush() { GradientStops = new GradientStops {
                new GradientStop(actSpBtnBase.GradientStartBackground, 0),
                new GradientStop(actSpBtnBase.GradientStopBackground, 1),
            } };
            bPick1.BoxShadow = !isPick1 ? actSpBtnBase.DefaultStateShadows : actSpBtnBase.HoldStateShadows;
            bPick2.BoxShadow = !isPick2 ? actSpBtnBase.DefaultStateShadows : actSpBtnBase.HoldStateShadows;
            bPick1.BorderThickness = bPick2.BorderThickness = new Thickness(1);
            bPick1.BorderBrush = bPick2.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B)));
            bPick1.CornerRadius = bPick2.CornerRadius = new CornerRadius(10);
            GRADIENT_GB_GREEN_PICKER = GenGradient(Color.FromArgb(a,r, 255, 255), Color.FromArgb(a,r, 255, 0), Orientation.Vertical);
            GRADIENT_GB_BLUE_PICKER = GenGradient(Color.FromArgb(a,r, 0, 255), Color.FromArgb(a,r, 0, 0), Orientation.Vertical);
            GRADIENT_AL_RED_PICKER = GenGradient(Color.FromArgb(255, 255, g, b), Color.FromArgb(255, 0, g, b), Orientation.Vertical);
            gbBorder1.Background = GRADIENT_GB_GREEN_PICKER;
            gbBorder2.Background = GRADIENT_GB_BLUE_PICKER;
            arBorder1.Background = GRADIENT_AL_RED_PICKER;
        }
    }
}
