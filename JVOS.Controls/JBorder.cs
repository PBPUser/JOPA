using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    internal class JBorder : Border
    {
        static JBorder()
        {
            AffectsRender<JBorder>(BlurBehindProperty);
        }

        public static readonly StyledProperty<bool> BlurBehindProperty = AvaloniaProperty<bool>.Register<JBorder, bool>("BlurBehind", false);

        public bool BlurBehind { 
            get => GetValue(BlurBehindProperty); 
            set => SetValue(BlurBehindProperty, value); 
        }

        
    }
}
