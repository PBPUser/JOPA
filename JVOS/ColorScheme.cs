using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public struct ColorHSV
    {
        public ColorHSV() 
        {
            Hue = 0;
            Saturation = 0;
            Value = 0;
            Alpha = 255;
        }

        public ColorHSV(Color color)
        {
            ColorHSV hsv = RgbToHsv(color);
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
            Alpha = hsv.Alpha;
        }

        public static ColorHSV RgbToHsv(Color RGB)
        {
            double r = ((double)RGB.R) / 255;
            double g = ((double)RGB.G) / 255;
            double b = ((double)RGB.B) / 255;
            double cmax = Math.Max(r, Math.Max(g, b));
            double cmin = Math.Min(r, Math.Min(g, b));
            double diff = cmax - cmin;
            double h = -1;
            double s = cmax == 0 ? 0 : (diff / cmax) * 100;
            double v = cmax * 100;
            if (cmax == cmin)
                h = 0;
            else if (cmax == r)
                h = (60 * ((g - b) / diff) + 360) % 360;
            else if (cmax == g)
                h = (60 * ((b - r) / diff) + 120) % 360;
            else if (cmax == b)
                h = (60 * ((r - g) / diff) + 240) % 360;
            ColorHSV color = new ColorHSV { Alpha = RGB.A, Hue = h, Saturation = s, Value = v };
            return color;
        }

        public static Color HsvToRgb(ColorHSV HSV)
        {
            double C = HSV.Saturation * HSV.Value / 10000d;
            double X = C * (1 - Math.Abs(((((int)(HSV.Hue + 0.5)) % 120) / 60d) - 1d));
            double m = (HSV.Value/100d) - C;
            //MessageBox.Show($"C={C}, X={X}, M={m}, H={HSV.Hue}, S={HSV.Saturation}, V={HSV.Value}");
            double R, G, B;
            if(HSV.Hue < 60)
            {
                R = C;
                G = X;
                B = 0;
            }
            else if(HSV.Hue < 120)
            {
                R = X;
                G = C;
                B = 0;
            }
            else if(HSV.Hue < 180)
            {
                R = 0;
                G = C;
                B = X;
            }
            else if(HSV.Hue < 240)
            {
                R = 0;
                G = X;
                B = C;
            }
            else if(HSV.Hue < 300)
            {
                R = X;
                G = 0;
                B = C;
            }
            else
            {
                R = C;
                G = 0;
                B = X;
            }
            R += m;
            G += m;
            B += m;
            R *= 255;
            G *= 255;
            B *= 255;
            //R += 0.5;
            //G += 0.5;
            //B += 0.5;
            Color c = new Color(HSV.Alpha, (byte)R, (byte)G, (byte)B);
            return c;
        }

        public byte Alpha;
        public double Hue;
        public double Saturation;
        public double Value;
    }

    public class ColorScheme
    {
        static ColorScheme()
        {
            ColorScheme.ApplyScheme(Current, false, true, false);
        }

        public ColorScheme()
        {

        }

        public static ColorScheme Current = new ColorScheme();

        public Color BasicColor = Color.FromRgb(40, 76, 100);
        public Color LightAccentBackgroundStart = Colors.Aqua;
        public Color LightAccentBackgroundEnd = Colors.Aqua;
        public Color LightAccentButtonStart = Colors.Aqua;
        public Color LightAccentButtonEnd = Colors.Aqua;
        public Color LightAccentForeground = Colors.Black;
        public Color LightAccentButtonForeground = Colors.Black;
        public Color LightAccentClaymorphismTop = Colors.Aquamarine;
        public Color LightAccentClaymorphismBottom = Colors.Aquamarine;
        public Color LightAccentShadow = Colors.Aquamarine;
        public Color LightAccentButtonClaymorphismTop = Colors.Aquamarine;
        public Color LightAccentButtonClaymorphismBottom = Colors.Aquamarine;
        public Color LightAccentButtonShadow = Colors.Aquamarine;
        public Color DarkAccentBackgroundStart = Colors.DarkCyan;
        public Color DarkAccentBackgroundEnd = Colors.DarkCyan;
        public Color DarkAccentForeground = Colors.White;
        public Color DarkAccentClaymorphismTop = Colors.CadetBlue;
        public Color DarkAccentClaymorphismBottom = Colors.CadetBlue;
        public Color DarkAccentShadow = Colors.CadetBlue;
        public Color DarkAccentButtonStart = Colors.DarkCyan;
        public Color DarkAccentButtonEnd = Colors.DarkCyan;
        public Color DarkAccentButtonForeground = Colors.White;
        public Color DarkAccentButtonClaymorphismTop = Colors.CadetBlue;
        public Color DarkAccentButtonClaymorphismBottom = Colors.CadetBlue;
        public Color DarkAccentButtonShadow = Colors.CadetBlue;
        public Color LightBasicBackgroundStart = Colors.White;
        public Color LightBasicBackgroundEnd = Colors.LightGray;
        public Color LightBasicForeground = Colors.Black;
        public Color LightBasicClaymorphismTop = Colors.Khaki;
        public Color LightBasicClaymorphismBottom = Colors.Khaki;
        public Color LightBasicShadow = Colors.Khaki;
        public Color LightBasicButtonStart = Colors.White;
        public Color LightBasicButtonEnd = Colors.LightGray;
        public Color LightBasicButtonForeground = Colors.Black;
        public Color LightBasicButtonClaymorphismTop = Colors.Khaki;
        public Color LightBasicButtonClaymorphismBottom = Colors.Khaki;
        public Color LightBasicButtonShadow = Colors.Khaki;
        public Color DarkBasicBackgroundStart = Colors.Gray;
        public Color DarkBasicBackgroundEnd = Colors.DarkGray;
        public Color DarkBasicForeground = Colors.White;
        public Color DarkBasicClaymorphismTop = Colors.DarkKhaki;
        public Color DarkBasicClaymorphismBottom = Colors.DarkKhaki;
        public Color DarkBasicShadow = Colors.DarkKhaki;
        public Color DarkBasicButtonStart = Colors.Gray;
        public Color DarkBasicButtonEnd = Colors.DarkGray;
        public Color DarkBasicButtonForeground = Colors.White;
        public Color DarkBasicButtonClaymorphismTop = Colors.DarkKhaki;
        public Color DarkBasicButtonClaymorphismBottom = Colors.DarkKhaki;
        public Color DarkBasicButtonShadow = Colors.DarkKhaki;
        public bool UseDarkScheme = false;
        public bool AccentTitle = false;
        public bool AccentBar = false;

        private const double BLUR_RADIUS_CLAYMORPHISM = 16;

        public static void ApplyScheme(ColorScheme scheme, bool UseDarkScheme, bool AccentTitle, bool AccentBar)
        {
            scheme = CreateColorSchemeFromColor(scheme.BasicColor);
            Current = scheme;
            Current.UseDarkScheme = UseDarkScheme;
            Current.AccentTitle = AccentTitle;
            Current.AccentBar = AccentBar;


            App.Current.Resources["BasicClaymorphismTop"] = !UseDarkScheme ? scheme.LightBasicClaymorphismTop : scheme.DarkBasicClaymorphismTop;
            App.Current.Resources["BasicClaymorphismBottom"] = !UseDarkScheme ? scheme.LightBasicClaymorphismBottom : scheme.DarkBasicClaymorphismBottom;
            App.Current.Resources["BasicShadow"] = !UseDarkScheme ? scheme.LightBasicShadow : scheme.DarkBasicShadow;
            App.Current.Resources["AccentClaymorphismTop"] = !UseDarkScheme ? scheme.LightAccentClaymorphismTop : scheme.DarkAccentClaymorphismTop;
            App.Current.Resources["AccentClaymorphismBottom"] = !UseDarkScheme ? scheme.LightAccentClaymorphismBottom : scheme.DarkAccentClaymorphismBottom;
            App.Current.Resources["AccentShadow"] = !UseDarkScheme ? scheme.LightAccentShadow : scheme.DarkAccentShadow;
            App.Current.Resources["ButtonAccentClaymorphismTop"] = !UseDarkScheme ? scheme.LightAccentButtonClaymorphismTop : scheme.DarkAccentButtonClaymorphismTop;
            App.Current.Resources["ButtonAccentClaymorphismBottom"] = !UseDarkScheme ? scheme.LightAccentButtonClaymorphismBottom : scheme.DarkAccentButtonClaymorphismBottom;
            App.Current.Resources["ButtonAccentShadow"] = !UseDarkScheme ? scheme.LightAccentButtonShadow : scheme.DarkAccentButtonShadow;
            App.Current.Resources["ButtonBasicClaymorphismTop"] = !UseDarkScheme ? scheme.LightBasicButtonClaymorphismTop : scheme.DarkBasicButtonClaymorphismTop;
            App.Current.Resources["ButtonBasicClaymorphismBottom"] = !UseDarkScheme ? scheme.LightBasicButtonClaymorphismBottom : scheme.DarkBasicButtonClaymorphismBottom;
            App.Current.Resources["ButtonBasicShadow"] = !UseDarkScheme ? scheme.LightBasicButtonShadow : scheme.DarkBasicButtonShadow;
            App.Current.Resources["BasicBackground"] = !UseDarkScheme ? new LinearGradientBrush() { GradientStops = new GradientStops() { new GradientStop(scheme.LightBasicBackgroundStart, 0), new GradientStop(scheme.LightBasicBackgroundEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(scheme.DarkBasicBackgroundStart, 0), new GradientStop(scheme.DarkBasicBackgroundEnd, 1) } };
            App.Current.Resources["AccentBackground"] = !UseDarkScheme ? new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.LightAccentBackgroundStart, 0), new GradientStop(scheme.LightAccentBackgroundEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.DarkAccentBackgroundStart, 0), new GradientStop(scheme.DarkAccentBackgroundEnd, 1) } };
            App.Current.Resources["ButtonBasicBackground"] = !UseDarkScheme ? new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.LightBasicButtonStart, 0), new GradientStop(scheme.LightBasicButtonEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.DarkBasicButtonStart, 0), new GradientStop(scheme.DarkBasicButtonEnd, 1) } };
            App.Current.Resources["ButtonAccentBackground"] = !UseDarkScheme ? new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.LightAccentButtonStart, 0), new GradientStop(scheme.LightAccentButtonEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.DarkAccentButtonStart, 0), new GradientStop(scheme.DarkAccentButtonEnd, 1) } };
            App.Current.Resources["BasicClaymorphismBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["BasicClaymorphismTop"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["BasicClaymorphismBottom"] }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["BasicShadow"] } });
            App.Current.Resources["AccentClaymorphismBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["AccentClaymorphismTop"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["AccentClaymorphismBottom"] }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["AccentShadow"] } });
            App.Current.Resources["ButtonBasicClaymorphismBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonBasicClaymorphismTop"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonBasicClaymorphismBottom"] }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonBasicShadow"] } });
            App.Current.Resources["ButtonAccentClaymorphismBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonAccentClaymorphismTop"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonAccentClaymorphismBottom"] }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonAccentShadow"] } });
            App.Current.Resources["BasicClaymorphismInnerBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["BasicClaymorphismBottom"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["BasicClaymorphismTop"] } });
            App.Current.Resources["AccentClaymorphismInnerBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["AccentClaymorphismBottom"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["AccentClaymorphismTop"] } });
            App.Current.Resources["ButtonBasicClaymorphismInnerBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonBasicClaymorphismBottom"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonBasicClaymorphismTop"] } });
            App.Current.Resources["ButtonAccentClaymorphismInnerBoxShadow"] = new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonAccentClaymorphismBottom"] }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)App.Current.Resources["ButtonAccentClaymorphismTop"] } });

            App.Current.Resources["BasicForeground"] = !UseDarkScheme ? new SolidColorBrush() { Color = scheme.LightBasicForeground } : new SolidColorBrush() { Color = scheme.DarkBasicForeground };
            App.Current.Resources["AccentForeground"] = !UseDarkScheme ? new SolidColorBrush() { Color = scheme.LightAccentForeground } : new SolidColorBrush() { Color = scheme.DarkAccentForeground };
            App.Current.Resources["ButtonBasicForeground"] = !UseDarkScheme ? new SolidColorBrush() { Color = scheme.LightBasicButtonForeground } : new SolidColorBrush() { Color = scheme.DarkBasicButtonForeground };
            App.Current.Resources["ButtonAccentForeground"] = !UseDarkScheme ? new SolidColorBrush() { Color = scheme.LightAccentButtonForeground } : new SolidColorBrush() { Color = scheme.DarkAccentButtonForeground };
            App.Current.Resources["BarClaymorphism"] = !AccentBar ? App.Current.Resources["BasicClaymorphism"] : App.Current.Resources["AccentClaymorphism"];
            App.Current.Resources["ButtonBarClaymorphism"] = !AccentBar ? App.Current.Resources["ButtonBasicClaymorphism"] : App.Current.Resources["ButtonAccentClaymorphism"];
            App.Current.Resources["BarBackground"] = !AccentBar ? App.Current.Resources["BasicBackground"] : App.Current.Resources["AccentBackground"];
            App.Current.Resources["ButtonBarBackground"] = !AccentBar ? App.Current.Resources["ButtonBasicBackground"] : App.Current.Resources["ButtonAccentBackground"];
            App.Current.Resources["BarForeground"] = !AccentBar ? App.Current.Resources["BasicForeground"] : App.Current.Resources["AccentForeground"];
            App.Current.Resources["ButtonBarForeground"] = !AccentBar ? App.Current.Resources["ButtonBasicForeground"] : App.Current.Resources["ButtonAccentForeground"];
            App.Current.Resources["BarClaymorphismBoxShadow"] = !AccentBar ? App.Current.Resources["BasicClaymorphismBoxShadow"] : App.Current.Resources["AccentClaymorphismBoxShadow"];
            App.Current.Resources["BarClaymorphismInnerBoxShadow"] = !AccentBar ? App.Current.Resources["BasicClaymorphismInnerBoxShadow"] : App.Current.Resources["AccentClaymorphismInnerBoxShadow"];
            App.Current.Resources["ButtonBarClaymorphismBoxShadow"] = !AccentBar ? App.Current.Resources["ButtonBasicClaymorphismBoxShadow"] : App.Current.Resources["ButtonAccentClaymorphismBoxShadow"];
            App.Current.Resources["ButtonBarClaymorphismInnerBoxShadow"] = !AccentBar ? App.Current.Resources["ButtonBasicClaymorphismInnerBoxShadow"] : App.Current.Resources["ButtonAccentClaymorphismInnerBoxShadow"];
            App.Current.Resources["TitleClaymorphism"] = !AccentTitle ? App.Current.Resources["BasicClaymorphism"] : App.Current.Resources["AccentClaymorphism"];
            App.Current.Resources["TitleBackground"] = !AccentTitle ? App.Current.Resources["BasicBackground"] : App.Current.Resources["AccentBackground"];
            App.Current.Resources["TitleForeground"] = !AccentTitle ? App.Current.Resources["BasicForeground"] : App.Current.Resources["AccentForeground"];
            App.Current.Resources["TitleClaymorphismBoxShadow"] = !AccentTitle ? App.Current.Resources["BasicClaymorphismBoxShadow"] : App.Current.Resources["AccentClaymorphismBoxShadow"];
            App.Current.Resources["TitleClaymorphismInnerBoxShadow"] = !AccentTitle ? App.Current.Resources["BasicClaymorphismInnerBoxShadow"] : App.Current.Resources["AccentClaymorphismInnerBoxShadow"];


        }

        public static ColorScheme CreateColorSchemeFromColor(Color color)
        {
            ColorScheme scheme = new ColorScheme();
            ColorHSV colorHSV = new ColorHSV(color);
            colorHSV.Value = Math.Max(10d, Math.Min(90d, colorHSV.Value));
            colorHSV.Saturation = Math.Max(10d, Math.Min(colorHSV.Saturation, 90d));
            scheme.BasicColor = ColorHSV.HsvToRgb(colorHSV);
            colorHSV.Value = colorHSV.Value >= 50d ? colorHSV.Value : 100d - colorHSV.Value;
            double darkbrightness = 100d - colorHSV.Value;
            double darkbasicbrightness = darkbrightness / 8;
            double lightbasicbrightness = (100 - darkbasicbrightness);

            double lightclaymorphismtopbrightness = Math.Min(lightbasicbrightness + 10, 100);
            double darkclaymorphismtopbrightness = Math.Min(darkbasicbrightness + 10, 100);
            double lightclaymorphismbottombrightness = Math.Max(lightclaymorphismtopbrightness - 20, 10);
            double darkclaymorphismbottonbrightness = Math.Max(darkclaymorphismtopbrightness - 20, 10);
            darkbasicbrightness = darkclaymorphismbottonbrightness + 10;

            double lightClaymorphismAccentTopBrigtness = Math.Min(colorHSV.Value + 10, 100);
            double lightClaymorphismAccentBottomBrigtness = Math.Max(lightClaymorphismAccentTopBrigtness - 20, 10);
            double darkClaymorphismAccentTopBrigtness = Math.Min(darkbrightness + 10, 100);
            double darkClaymorphismAccentBottomBrigtness = Math.Max(darkClaymorphismAccentTopBrigtness - 10, 10);
            darkbrightness = darkClaymorphismAccentTopBrigtness - 10;


            darkbasicbrightness += 10d;
            int hue = (int)(colorHSV.Hue + 0.5);
            double hueStart = (hue - 15) % 360;
            double hueEnd = (hue + 15) % 360;
            double claySat = colorHSV.Saturation - 10d;
            double buttSat = colorHSV.Saturation + 10d;
            scheme.LightAccentBackgroundStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = colorHSV.Saturation, Value = colorHSV.Value });
            scheme.LightAccentBackgroundEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = colorHSV.Saturation, Value = colorHSV.Value });
            scheme.LightAccentForeground = Colors.Black;
            scheme.LightAccentClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat, Value = lightClaymorphismAccentTopBrigtness });
            scheme.LightAccentClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat, Value = lightClaymorphismAccentBottomBrigtness });
            scheme.LightAccentShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat, Value = lightClaymorphismAccentBottomBrigtness-10 });

            scheme.DarkAccentBackgroundStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = colorHSV.Saturation, Value = darkbrightness + 4 });
            scheme.DarkAccentBackgroundEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = colorHSV.Saturation, Value = darkbrightness - 4 });
            scheme.DarkAccentForeground = Colors.White;
            scheme.DarkAccentClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat, Value = darkClaymorphismAccentTopBrigtness });
            scheme.DarkAccentClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat, Value = darkClaymorphismAccentBottomBrigtness });
            scheme.DarkAccentShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat, Value = darkClaymorphismAccentBottomBrigtness - 10 });

            scheme.LightBasicBackgroundStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = 10, Value = lightbasicbrightness + 4 });
            scheme.LightBasicBackgroundEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = 10, Value = lightbasicbrightness - 4 });
            scheme.LightBasicForeground = Colors.Black;
            scheme.LightBasicClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 5, Value = lightclaymorphismtopbrightness });
            scheme.LightBasicClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 5, Value = lightclaymorphismbottombrightness });
            scheme.LightBasicShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 5, Value = lightclaymorphismbottombrightness - 10 });

            scheme.DarkBasicBackgroundStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = 10, Value = darkbasicbrightness + 4 });
            scheme.DarkBasicBackgroundEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = 10, Value = darkbasicbrightness - 4 });
            scheme.DarkBasicForeground = Colors.White;
            scheme.DarkBasicClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 5, Value = darkclaymorphismtopbrightness  });
            scheme.DarkBasicClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 5, Value = darkclaymorphismbottonbrightness });
            scheme.DarkBasicButtonShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 5, Value = darkclaymorphismbottonbrightness - 10 });

            scheme.LightAccentButtonStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = buttSat, Value = colorHSV.Value });
            scheme.LightAccentButtonEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = buttSat, Value = colorHSV.Value });
            scheme.LightAccentButtonForeground = Colors.Black;
            scheme.LightAccentButtonClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat+10d, Value = lightClaymorphismAccentTopBrigtness });
            scheme.LightAccentButtonClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat+10d, Value = lightClaymorphismAccentBottomBrigtness });
            scheme.LightAccentButtonShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat+10d, Value = lightClaymorphismAccentBottomBrigtness - 10 });

            scheme.DarkAccentButtonStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = buttSat, Value = darkbrightness + 4 });
            scheme.DarkAccentButtonEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = buttSat, Value = darkbrightness - 4});
            scheme.DarkAccentButtonForeground = Colors.White;
            scheme.DarkAccentButtonClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat+10d, Value = darkClaymorphismAccentTopBrigtness });
            scheme.DarkAccentButtonClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat+10d, Value = darkClaymorphismAccentBottomBrigtness });
            scheme.DarkAccentButtonShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat+10d, Value = darkClaymorphismAccentBottomBrigtness - 10 });

            scheme.LightBasicButtonStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = 20, Value = lightbasicbrightness + 4 });
            scheme.LightBasicButtonEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = 20, Value = lightbasicbrightness - 4 });
            scheme.LightBasicButtonForeground = Colors.Black;
            scheme.LightBasicButtonClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 15, Value = lightclaymorphismtopbrightness });
            scheme.LightBasicButtonClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 15, Value = lightclaymorphismbottombrightness });
            scheme.LightBasicButtonShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = 15, Value = lightclaymorphismbottombrightness - 10 });

            scheme.DarkBasicButtonStart = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueStart, Saturation = claySat + 10d, Value = darkbasicbrightness + 4 });
            scheme.DarkBasicButtonEnd = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hueEnd, Saturation = claySat + 10d, Value = darkbasicbrightness - 4 });
            scheme.DarkBasicButtonForeground = Colors.White;
            scheme.DarkBasicButtonClaymorphismTop = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat - 10d, Value = darkclaymorphismtopbrightness });
            scheme.DarkBasicButtonClaymorphismBottom = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat - 10d, Value = darkclaymorphismbottonbrightness });
            scheme.DarkBasicButtonShadow = ColorHSV.HsvToRgb(new ColorHSV { Alpha = 255, Hue = hue, Saturation = claySat - 10d, Value = darkclaymorphismbottonbrightness - 10 });

            return scheme;
        }
    }
}
