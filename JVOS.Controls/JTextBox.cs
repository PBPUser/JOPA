using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class JTextBox : TemplatedControl
    {
        public JTextBox()
        {
            
        }

        static JTextBox()
        {
            AffectsRender<JTextBox>(
                BackgroundProperty,
                BorderBrushProperty,
                TextProperty,
                BorderThicknessProperty,
                CornerRadiusProperty,
                BoxShadowsProperty,
                ActiveBoxShadowsProperty,
                ActiveBrushProperty,
                IsFocusedProperty);
        }

        TextPresenter? textPresenter = null;
        ScrollViewer? scrollViewer = null;

        public static readonly StyledProperty<IBrush?> ActiveBrushProperty =
            AvaloniaProperty.Register<JTextBox, IBrush?>(nameof(ActiveBrush));

        public static readonly StyledProperty<BoxShadows?> BoxShadowsProperty =
                    AvaloniaProperty.Register<JTextBox, BoxShadows?>(nameof(BoxShadows));

        public static readonly StyledProperty<BoxShadows?> ActiveBoxShadowsProperty =
                    AvaloniaProperty.Register<JTextBox, BoxShadows?>(nameof(ActiveBoxShadows));

        public static readonly StyledProperty<string?> TextProperty =
            TextBlock.TextProperty.AddOwner<JTextBox>(new(
                coerce: CoerceText,
                defaultBindingMode: BindingMode.TwoWay,
                enableDataValidation: true));
        
        [Content]
        public string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public IBrush? ActiveBrush
        {
            get => GetValue(ActiveBrushProperty);
            set => SetValue(ActiveBrushProperty, value);
        }

        public BoxShadows? BoxShadows
        {
            get => GetValue(BoxShadowsProperty);
            set => SetValue(BoxShadowsProperty, value);
        }
        public BoxShadows? ActiveBoxShadows
        {
            get => GetValue(ActiveBoxShadowsProperty);
            set => SetValue(ActiveBoxShadowsProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
        }

        private static string? CoerceText(AvaloniaObject sender, string? value)
        {
            var textBox = (JTextBox)sender;
            //if (!textBox._isUndoingRedoing)
                //textBox.SnapshotUndoRedo();
            
            return value;
        }
    }
}
