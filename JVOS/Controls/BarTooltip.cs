using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace JVOS.Controls
{
    public class BarTooltip : Control
    {
        static BarTooltip()
        {
            IconProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Control, Bitmap>("Icon");
            TitleProperty = AvaloniaProperty.RegisterAttached<BarTooltip, Control, string>("Title");
            AffectsRender<BarTooltip>(
                IconProperty, 
                TitleProperty
                );
        }

        public static readonly AttachedProperty<Bitmap> IconProperty;
        public static readonly AttachedProperty<string> TitleProperty;

        private void UpdateFormattedText()
        {
            fText = new(Title, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(FontFamily.Default), 14, Brushes.Black);
        }

        private FormattedText fText;

        public Bitmap Icon
        {
            get => (Bitmap)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set {
                SetValue(TitleProperty, value);
                UpdateFormattedText();
            }
        }

        public override void Render(DrawingContext context)
        {
            context.DrawImage(Icon, new Rect(4, 4, Bounds.Height - 8, Bounds.Height - 8));
            context.DrawText(fText, new Point(Bounds.Height, (Bounds.Height-fText.Height) / 2));
            base.Render(context);
        }
    }
}
