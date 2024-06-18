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
    public class FileIcon : Control
    {
        private static Brush TransparentA1 = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0), 1);
        private static Brush TestSelectBackground = new SolidColorBrush(Color.FromArgb(128, 255, 0, 255), 1);
        static Bitmap FOLDER_ICON = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/folder.png")));
        static Bitmap FILE_ICON = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/file.png")));

        static FileIcon()
        {
            TitleProperty = AvaloniaProperty.RegisterAttached<FileIcon, Control, string>("Title", coerce: (a,b) =>
            {
                ((FileIcon)a).FormattedTitle = new FormattedText(b, CultureInfo.CurrentCulture, GetFlowDirection(a as FileIcon), new Typeface(FontFamily.Default, weight: FontWeight.Bold, style: FontStyle.Italic), 14, Brushes.Black);
                return b;
            });
            PathProperty = AvaloniaProperty.RegisterAttached<FileIcon, Control, string>("Path", coerce: (a,b) =>
            {
                if (b == null)
                    b = "";
                ((FileIcon)a).Title = b.Split("\\").Last();
                ((FileIcon)a).IsDirectory = Directory.Exists(b);
                if (b.EndsWith(".jnk") && File.Exists(b))
                {
                    var shortcut = Shortcut.Load(b);
                    ((FileIcon)a).Icon = UserOptions.Base64ToImage(shortcut.Base64Image);
                }
                return b;
            });
            IsDirProperty = AvaloniaProperty.RegisterAttached<FileIcon, Control, bool>("IsDirectory");
            IconProperty = AvaloniaProperty.RegisterAttached<FileIcon, Control, Bitmap?>("Icon");
            FormattedTitleProperty = AvaloniaProperty.RegisterAttached<FileIcon, Control, FormattedText>("FormattedTitle");
            AffectsRender<FileIcon>(
                IconProperty,
                IsDirProperty
                );
        }

        public FileIcon(string path)
        {
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
        public bool IsDirectory
        {
            get => GetValue(IsDirProperty);
            set => SetValue(IsDirProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            context.DrawImage(IsDirectory ? FOLDER_ICON : (Icon ?? FILE_ICON), new Rect(0, 0, Bounds.Height, Bounds.Height));
            context.DrawText(FormattedTitle, new Point(Bounds.Height, (Bounds.Height - FormattedTitle.Height) / 2));
            base.Render(context);
        }

        public static readonly AttachedProperty<string> PathProperty;
        public static readonly AttachedProperty<bool> IsDirProperty;
        public static readonly AttachedProperty<Bitmap?> IconProperty;
        public static readonly AttachedProperty<string> TitleProperty;
        public static readonly AttachedProperty<FormattedText> FormattedTitleProperty;
    }
}
