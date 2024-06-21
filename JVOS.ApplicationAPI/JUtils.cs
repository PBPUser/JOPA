using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public static class JUtils
    {
        public static Color GetMidColorFromBitmap(Bitmap bitmap)
        {
            using WriteableBitmap writeableBitmap = new(bitmap.PixelSize, bitmap.Dpi, bitmap.Format, bitmap.AlphaFormat);
            using var lc = writeableBitmap.Lock();
            writeableBitmap.CopyPixels(lc, bitmap.AlphaFormat??Avalonia.Platform.AlphaFormat.Opaque);
            return Colors.AliceBlue;
        }
    }
}
