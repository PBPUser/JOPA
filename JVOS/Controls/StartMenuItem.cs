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
    public class StartMenuItem : Button
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);
        private static Brush TestSelectBackground = new SolidColorBrush(Color.FromArgb(128, 255, 0, 255), 1);
        static Bitmap DEFAULT_APP_ICON = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/app.png")));

        static StartMenuItem()
        {
            TitleProperty = AvaloniaProperty.RegisterAttached<StartMenuItem, Button, string>("Title", coerce: (a,b) =>
            {
                ((StartMenuItem)a).FormattedTitle = new FormattedText(b, CultureInfo.CurrentCulture, GetFlowDirection(a as StartMenuItem), new Typeface(FontFamily.Default, weight: FontWeight.Bold, style: FontStyle.Italic), 14, Brushes.Black);
                return b;
            });
            PathProperty = AvaloniaProperty.RegisterAttached<StartMenuItem, Button, string>("Path", coerce: (a,b) =>
            {
                if (b == null)
                    return "";
                ((StartMenuItem)a).Title = b.Split("\\").Last().Replace(".jnk", "");
                var shortcut = Shortcut.Load(b);
                Debug.WriteLine("Shortcut: " + shortcut.Base64Image);
                ((StartMenuItem)a).Icon = UserOptions.Base64ToImage(shortcut.Base64Image);
                return b;
            });
            IconProperty = AvaloniaProperty.RegisterAttached<StartMenuItem, Button, Bitmap?>("Icon");
            FormattedTitleProperty = AvaloniaProperty.RegisterAttached<StartMenuItem, Button, FormattedText>("FormattedTitle");
            MouseOverProperty = AvaloniaProperty.RegisterAttached<StartMenuItem, Button, bool>("MouseOver");
            AffectsRender<StartMenuItem>(
                MouseOverProperty,
                FormattedTitleProperty,
                IconProperty
                );
        }

        public StartMenuItem(string path)
        {
            Loaded += (a, b) => Height = 48;
            Path = path;
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

        public bool MouseOver
        {
            get => GetValue(MouseOverProperty);
            set => SetValue(MouseOverProperty, value);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            Communicator.RunPath(Path);
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
            context.FillRectangle(MouseOver ? TestSelectBackground : TransparentA1, new Rect(Bounds.Size), 8);
            context.DrawImage(Icon ?? DEFAULT_APP_ICON, new Rect(0, 0, Bounds.Size.Height, Bounds.Size.Height));
            context.DrawText(FormattedTitle, new Point(Bounds.Size.Height, (Bounds.Height - FormattedTitle.Height) / 2));
            base.Render(context);
        }

        public static readonly AttachedProperty<string> PathProperty;
        public static readonly AttachedProperty<Bitmap?> IconProperty;
        public static readonly AttachedProperty<string> TitleProperty;
        public static readonly AttachedProperty<FormattedText> FormattedTitleProperty;
        public static readonly AttachedProperty<bool> MouseOverProperty;
    }
}
