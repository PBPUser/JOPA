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
    public class RecommendMenuItem : Button
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);
        private static Brush TestSelectBackground = new SolidColorBrush(Color.FromArgb(128, 255, 0, 255), 1);
        static Bitmap DEFAULT_APP_ICON = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/app.png")));

        static RecommendMenuItem()
        {
            TitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Button, string>("Title", coerce: (a, b) =>
            {
                ((RecommendMenuItem)a).FormattedTitle = new FormattedText(b, CultureInfo.CurrentCulture, GetFlowDirection(a as RecommendMenuItem), new Typeface(FontFamily.Default, weight: FontWeight.Bold, style: FontStyle.Italic), 16, Brushes.Black);
                return b;
            });
            SubtitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Button, string>("Subtitle", coerce: (a, b) =>
            {
                ((RecommendMenuItem)a).FormattedSubtitle = new FormattedText(b ??"", CultureInfo.CurrentCulture, GetFlowDirection(a as RecommendMenuItem), new Typeface(FontFamily.Default, weight: FontWeight.Bold, style: FontStyle.Italic), 14, Brushes.DarkGray);
                return b;
            });
            PathProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Button, string>("Path", coerce: (a,b) =>
            {
                if (b == null)
                    return "";
                ((RecommendMenuItem)a).Title = b.Split("\\").Last().Replace(".jnk", "");
                var shortcut = Shortcut.Load(b);
                ((RecommendMenuItem)a).Icon = UserOptions.Base64ToImage(shortcut.Base64Image);
                ((RecommendMenuItem)a).Subtitle = shortcut.Description;
                return b;
            });
            IconProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Button, Bitmap?>("Icon");
            FormattedTitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Button, FormattedText>("FormattedTitle");
            FormattedSubtitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Button, FormattedText>("FormattedSubtitle");
            MouseOverProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Button, bool>("MouseOver");
            AffectsRender<RecommendMenuItem>(
                MouseOverProperty,
                FormattedTitleProperty,
                FormattedSubtitleProperty,
                IconProperty
                );
        }

        public RecommendMenuItem(string path)
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
        public string Subtitle
        {
            get => GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
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
        public FormattedText FormattedSubtitle
        {
            get => GetValue(FormattedSubtitleProperty);
            set => SetValue(FormattedSubtitleProperty, value);
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
            if(FormattedTitle != null)
            context.DrawText(FormattedTitle, new Point(Bounds.Size.Height, (Bounds.Height / 2) - 3 - FormattedTitle.Height));
            if(FormattedSubtitle != null)
            context.DrawText(FormattedSubtitle, new Point(Bounds.Size.Height, (Bounds.Height / 2) + 3));
            base.Render(context);
        }

        public static readonly AttachedProperty<string> PathProperty;
        public static readonly AttachedProperty<Bitmap?> IconProperty;
        public static readonly AttachedProperty<string> TitleProperty;
        public static readonly AttachedProperty<string> SubtitleProperty;
        public static readonly AttachedProperty<FormattedText> FormattedTitleProperty;
        public static readonly AttachedProperty<FormattedText> FormattedSubtitleProperty;
        public static readonly AttachedProperty<bool> MouseOverProperty;
    }
}
