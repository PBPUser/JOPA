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
    public class RecommendMenuItem : Control
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);
        private static Brush TestSelectBackground = new SolidColorBrush(Color.FromArgb(128, 255, 0, 255), 1);
        public static Bitmap DEFAULT_APP_ICON = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/app.png")));

        static RecommendMenuItem()
        {
            TitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, string>("Title", coerce: (a, b) =>
            {
                ((RecommendMenuItem)a).FormattedTitle = new FormattedText(b??"", CultureInfo.CurrentCulture, GetFlowDirection(a as RecommendMenuItem), new Typeface(FontFamily.Default, weight: FontWeight.Bold, style: FontStyle.Italic), 16, (a as RecommendMenuItem).TextColor);
                return b??"";
            });
            TextColorProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, IBrush>("TextColor", coerce: (a, b) =>
            {
                b = b ?? TestSelectBackground;
                var x = ((RecommendMenuItem)a);
                x.FormattedTitle = new FormattedText(x.Title ?? "", CultureInfo.CurrentCulture, GetFlowDirection(x), new Typeface(FontFamily.Default, weight: FontWeight.Bold, style: FontStyle.Italic), 16, b);
                x.FormattedSubtitle = new FormattedText(x.Subtitle ?? "", CultureInfo.CurrentCulture, GetFlowDirection(x), new Typeface(FontFamily.Default, weight: FontWeight.Bold, style: FontStyle.Italic), 16, b);
                return b;
            });
            SubtitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, string>("Subtitle", coerce: (a, b) =>
            {
                ((RecommendMenuItem)a).FormattedSubtitle = new FormattedText(b ??"", CultureInfo.CurrentCulture, GetFlowDirection(a as RecommendMenuItem), new Typeface(FontFamily.Default, style: FontStyle.Italic), 14, (a as RecommendMenuItem).TextColor);
                return b??"";
            });
            PathProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, string>("Path", coerce: (a, b) =>
            {
                if (b == null)
                    return "";
                var shortcut = Shortcut.Load(b);
                if (String.IsNullOrEmpty(shortcut.OverwriteName))
                    ((RecommendMenuItem)a).Title = b.Split("\\").Last().Replace(".jnk", "");
                else
                    ((RecommendMenuItem)a).Title = shortcut.OverwriteName;
                ((RecommendMenuItem)a).Icon = UserOptions.Base64ToImage(shortcut.Base64Image);
                ((RecommendMenuItem)a).Subtitle = shortcut.Description;
                return b;
            });
            RecentProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, Recent>("Recent", coerce: (a, b) =>
            {
                if (b.Path.EndsWith(".jnk"))
                {
                    var shortcut = Shortcut.Load(b.Path);
                    if (String.IsNullOrEmpty(shortcut.OverwriteName))
                        ((RecommendMenuItem)a).Title = b.Path.Split("\\").Last().Replace(".jnk", "");
                    else
                        ((RecommendMenuItem)a).Title = shortcut.OverwriteName;
                    ((RecommendMenuItem)a).Icon = UserOptions.Base64ToImage(shortcut.Base64Image);
                    ((RecommendMenuItem)a).Subtitle = shortcut.Description;
                }
                else
                {
                    ((RecommendMenuItem)a).Title = b.Path.Split("\\").Last();
                    ((RecommendMenuItem)a).Subtitle = b.Date.ToString("MMM dd t");
                }
                return b;
            });
            IconProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, Bitmap?>("Icon");
            FormattedTitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, FormattedText>("FormattedTitle");
            FormattedSubtitleProperty = AvaloniaProperty.RegisterAttached<RecommendMenuItem, Control, FormattedText>("FormattedSubtitle");
            AffectsRender<RecommendMenuItem>(
                FormattedTitleProperty,
                FormattedSubtitleProperty,
                IconProperty
                );
        }

        public RecommendMenuItem(object data)
        {
            Loaded += (a, b) => Height = 48;
            if (data is string)
                Path = (string)data;
            else if (data is Recent)
                Recent = (Recent)data;
        }

        public Recent Recent
        {
            get => GetValue(RecentProperty);
            set => SetValue(RecentProperty, value);
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
        public IBrush TextColor
        {
            get => GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            Communicator.RunPath(Path);
            base.OnPointerReleased(e);
        }
        public override void Render(DrawingContext context)
        {
            context.DrawImage(Icon ?? DEFAULT_APP_ICON, new Rect(0, 0, Bounds.Size.Height, Bounds.Size.Height));
            if (FormattedTitle != null)
                context.DrawText(FormattedTitle, new Point(Bounds.Size.Height, (Bounds.Height / 2) - 3 - FormattedTitle.Height));
            if (FormattedSubtitle != null)
            {
                var s = context.PushOpacity(0.5);
                context.DrawText(FormattedSubtitle, new Point(Bounds.Size.Height, (Bounds.Height / 2) + 3));
                s.Dispose();
            }
            base.Render(context);
        }

        public static readonly AttachedProperty<string> PathProperty;
        public static readonly AttachedProperty<Recent> RecentProperty;
        public static readonly AttachedProperty<Bitmap?> IconProperty;
        public static readonly AttachedProperty<string> TitleProperty;
        public static readonly AttachedProperty<IBrush> TextColorProperty;
        public static readonly AttachedProperty<string> SubtitleProperty;
        public static readonly AttachedProperty<FormattedText> FormattedTitleProperty;
        public static readonly AttachedProperty<FormattedText> FormattedSubtitleProperty;
    }
}
