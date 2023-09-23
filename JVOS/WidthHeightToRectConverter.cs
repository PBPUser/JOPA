using Avalonia.Data.Converters;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public class WidthHeightToRectConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;
            double width = (double)values[0];
            double height = (double)values[1];
            return new Rect(0, 0, width, height);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
