using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.DataModel;
using JVOS.EmbededWindows;
using JVOS.Screens;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class DesktopIcon : Thumb
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);
        private static Brush TestSelectBackground = new SolidColorBrush(Color.FromArgb(128, 255, 0, 255), 1);
        private static Brush MaskForTitle;
        static Bitmap FOLDER_ICON = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/folder.png")));
        static Bitmap FILE_ICON = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/file.png")));

        static DesktopIcon()
        {
            MaskForTitle = new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(0.8, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops = new GradientStops()
                    {
                        new GradientStop(Colors.Black, 0),
                        new GradientStop(Colors.Transparent, 1)
                    }
            };
            TransitionsTransform =
            [
                new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(175) },
                new DoubleTransition() { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(175) },
            ];
            TitleProperty = AvaloniaProperty.RegisterAttached<DesktopIcon, Thumb, string>("Title", coerce: (a,b) =>
            {
                ((DesktopIcon)a).ReloadFormattedTitle(text: b);
                return b;
            });
            PathProperty = AvaloniaProperty.RegisterAttached<DesktopIcon, Thumb, string>("Path", coerce: (a,b) =>
            {
                List<string> j = b.Split("\\").Last().Split('.').ToList();
                if(j.Count > 1 && !Directory.Exists(b))
                    j.RemoveAt(j.Count - 1);
                ((DesktopIcon)a).Title = String.Join('.', j);
                ((DesktopIcon)a).IsDirectory = Directory.Exists(b);
                if (b.EndsWith(".jnk") && File.Exists(b))
                {
                    var shortcut = Shortcut.Load(b);
                    ((DesktopIcon)a).Icon = UserOptions.Base64ToImage(shortcut.Base64Image);
                }
                return b;
            });
            IsDirProperty = AvaloniaProperty.RegisterAttached<DesktopIcon, Thumb, bool>("IsDirectory");
            IconProperty = AvaloniaProperty.RegisterAttached<DesktopIcon, Thumb, Bitmap?>("Icon");
            FormattedTitleProperty = AvaloniaProperty.RegisterAttached<DesktopIcon, Thumb, FormattedText>("FormattedTitle");
            MouseOverProperty = AvaloniaProperty.RegisterAttached<DesktopIcon, Thumb, bool>("MouseOver");
            TextBrushProperty = AvaloniaProperty.RegisterAttached<DesktopIcon, Thumb, IBrush>("TextBrush", coerce: (a,b) =>
            {
                ((DesktopIcon)a).ReloadFormattedTitle(Color: b);
                return b;
            });
            AffectsRender<DesktopIcon>(
                MouseOverProperty,
                IconProperty,
                FormattedTitleProperty,
                IsDirProperty
                );
        }

        TranslateTransform tt;
        static Transitions TransitionsTransform;

        public DesktopIcon(string path)
        {
            Loaded += (a, b) => Height = 96;
            Loaded += (a, b) => Width = 80;
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            Path = path;
            var x = DesktopWindow.Current.IconPlacementHelper.GetPlacementForItem(path);
            tt = new() { 
                X = x.X * 80,
                Y = x.Y * 104,
                Transitions = TransitionsTransform
            };
            RenderTransform = tt;
        }

        private void ReloadFormattedTitle(string? text = null, IBrush? Color = null)
        {
            FormattedTitle = new FormattedText(
                text??Title??"", 
                CultureInfo.CurrentCulture, 
                GetFlowDirection(this), 
                new Typeface(
                    FontFamily.Default, 
                    weight: FontWeight.Bold, 
                    style: FontStyle.Italic
                    ), 
                14, 
                Color??TextBrush??Brushes.Magenta
                );
        }

        public string Path
        {
            get => GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public IBrush TextBrush
        {
            get => GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }
        public Bitmap? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public FormattedText FormattedTitle
        {
            get => GetValue(FormattedTitleProperty);
            set => SetValue(FormattedTitleProperty, value);
        }
        public bool IsDirectory
        {
            get => GetValue(IsDirProperty);
            set => SetValue(IsDirProperty, value);
        }

        public bool MouseOver
        {
            get => GetValue(MouseOverProperty);
            set => SetValue(MouseOverProperty, value);
        }

        protected override void OnDragCompleted(VectorEventArgs e)
        {
            DesktopWindow.Current.IconPlacementHelper.SetPlacementForItem(new System.Drawing.Point((int)((tt.X + 40) / 80), (int)((tt.Y + 52) / 104)), Path);
            var x = DesktopWindow.Current.IconPlacementHelper.GetPlacementForItem(Path);
            tt.X = 80 * x.X;
            tt.Y = 104 * x.Y;
            base.OnDragCompleted(e);
        }

        protected override void OnDragDelta(VectorEventArgs e)
        {
            tt.X += e.Vector.X;
            tt.Y += e.Vector.Y;
        }

        ulong timestampMD = 0;

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if(e.Timestamp-timestampMD < 1000)
            {
                Communicator.RunPath(Path);
            }
            timestampMD = e.Timestamp;
            base.OnPointerReleased(e);
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            MouseOver = true;
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            MouseOver = false;
            base.OnPointerExited(e);
        }

        public override void Render(DrawingContext context)
        {
            context.FillRectangle(MouseOver ? TestSelectBackground : TransparentA1, new Rect(Bounds.Size), 12);
            context.DrawImage(IsDirectory ? FOLDER_ICON : (Icon ?? FILE_ICON), new Rect(Bounds.Size - new Size(0, 20)));

            if (FormattedTitle.Width > Bounds.Size.Width)
                context.PushOpacityMask(MaskForTitle, new Rect(Bounds.Size));
            
            context.DrawText(FormattedTitle, new Point(
                FormattedTitle.Width > Bounds.Size.Width ? 0 : (Bounds.Width - FormattedTitle.Width) / 2, 
                Bounds.Height - FormattedTitle.Height
                ));
            base.Render(context);
        }

        public static readonly AttachedProperty<string> PathProperty;
        public static readonly AttachedProperty<bool> IsDirProperty;
        public static readonly AttachedProperty<Bitmap?> IconProperty;
        public static readonly AttachedProperty<string> TitleProperty;
        public static readonly AttachedProperty<FormattedText> FormattedTitleProperty;
        public static readonly AttachedProperty<bool> MouseOverProperty;
        public static readonly AttachedProperty<IBrush> TextBrushProperty;
    }
}
