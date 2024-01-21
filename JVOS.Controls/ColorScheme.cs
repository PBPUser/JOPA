using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using JVOS.Controls;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

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

        internal static ColorHSV SetBrightness(ColorHSV hsv, bool dark)
        {
            var darkBrightness = hsv.Value > 50 ? 100 - hsv.Value : hsv.Value;
            return hsv with { Value = dark ? darkBrightness : 100 - darkBrightness };
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

    public struct ActionSpecificButtonBase
    {
        public Color GradientStartBackground;
        public Color GradientStopBackground;
        public Color InsetTopShadowColor;
        public Color InsetBottomShadowColor;
        public Color ShadowColor;
        public Color Foreground;

        public BoxShadows DefaultStateShadows;
        public BoxShadows HoldStateShadows;

        public ActionSpecificButtonBase(Color baseColor, bool UseDarkTheme)
        {
            var baseColorHSV = ColorHSV.SetBrightness(ColorHSV.RgbToHsv(baseColor), UseDarkTheme);
            baseColorHSV.Saturation = Math.Clamp(baseColorHSV.Saturation, 10, 95);
            baseColorHSV.Value = Math.Clamp(baseColorHSV.Value, 15, 85);
            var gradient1 = baseColorHSV;
            var gradient2 = baseColorHSV;
            gradient1.Value += 5;
            gradient2.Value -= 5;
            gradient1.Hue = (gradient1.Hue - 10) % 360;
            gradient2.Hue = (gradient1.Hue + 10) % 360;
            var iTopShadow = gradient1;
            var iBottomShadow = gradient2;
            var shadow = baseColorHSV;
            shadow.Value /= 2;
            shadow.Alpha /= 2;
            iTopShadow.Saturation -= 10;
            iBottomShadow.Saturation -= 10;
            iTopShadow.Value += 10;
            iBottomShadow.Value -= 10;
            GradientStartBackground = ColorHSV.HsvToRgb(gradient1);
            GradientStopBackground = ColorHSV.HsvToRgb(gradient2);
            InsetTopShadowColor = ColorHSV.HsvToRgb(iTopShadow);
            InsetBottomShadowColor = ColorHSV.HsvToRgb(iBottomShadow);
            ShadowColor = ColorHSV.HsvToRgb(shadow);
            Foreground = UseDarkTheme ? Colors.White : Colors.Black;

            DefaultStateShadows = new BoxShadows(
                new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetTopShadowColor },
                new BoxShadow[]
                {
                    new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetBottomShadowColor },
                    new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = false, OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM/2, OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM /2, Color = ShadowColor }
                }
                );
            HoldStateShadows = new BoxShadows(
                new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetBottomShadowColor },
                new BoxShadow[]
                {
                    new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetTopShadowColor },
                }
                );
        }

        public ActionSpecificButtonBase(Color color1, Color color2, bool InvertOffsets)
        {
            var baseColorHSVTop = ColorHSV.RgbToHsv(color1);
            var baseColorHSVBottom = ColorHSV.RgbToHsv(color2);
            var iTopShadow = baseColorHSVTop;
            var iBottomShadow = baseColorHSVBottom;
            var shadow = baseColorHSVBottom;
            shadow.Value /= 4;
            shadow.Alpha /= 2;
            iTopShadow.Alpha /= 2;
            iBottomShadow.Value /= 2;
            iBottomShadow.Alpha /= 2;
            iTopShadow.Saturation = Math.Clamp(iTopShadow.Saturation - 10, 0, 100);
            iBottomShadow.Saturation = Math.Clamp(iBottomShadow.Saturation - 10, 0, 100);
            iTopShadow.Value = Math.Clamp(iTopShadow.Value * 1.5, 0, 100);
            iBottomShadow.Value = Math.Clamp(iBottomShadow.Value - 10, 0, 100);
            GradientStartBackground = ColorHSV.HsvToRgb(baseColorHSVTop);
            GradientStopBackground = ColorHSV.HsvToRgb(baseColorHSVBottom);
            InsetTopShadowColor = ColorHSV.HsvToRgb(iTopShadow);
            InsetBottomShadowColor = ColorHSV.HsvToRgb(iBottomShadow);
            ShadowColor = ColorHSV.HsvToRgb(shadow);
            Foreground = Colors.Black;


            DefaultStateShadows = new BoxShadows(
                new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetBottomShadowColor },
                new BoxShadow[]
                {
                    new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetTopShadowColor },
                    new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = false, OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM/2, OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM /2, Color = ShadowColor }
                }
                );
            HoldStateShadows = new BoxShadows(
                new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetBottomShadowColor },
                new BoxShadow[]
                {
                    new BoxShadow() { Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM, IsInset = true, OffsetX = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2, Color = InsetTopShadowColor },
                }
                );
        }
    }

    public class ColorScheme
    {
        public class FSWatcher
        {
            private string Path;
            private string Filter;
            private PhysicalFileProvider _fileProvider;
            private IChangeToken? _token;
            private IDisposable? _changeCallback;
            public event EventHandler<EventArgs> Changed;

            public FSWatcher(string path, string filter)
            {
                Path = path;
                Filter = filter;
                _fileProvider = new PhysicalFileProvider(path);
            }

            public void Start()
            {
                _token = _fileProvider.Watch(Filter);
                _changeCallback = _token.RegisterChangeCallback(Notify, default);
            }

            private void Notify(object? state)
            {
                Start();
                OnChanged();
            }

            private void OnChanged()
            {
                if(Changed != null)
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        try
                        {
                            Changed.Invoke(this, EventArgs.Empty);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                    });
            }

            public void Stop()
            {
                if(_changeCallback != null)
                {
                    _changeCallback.Dispose();
                    _changeCallback = null;
                }
                if (_token != null)
                    _token = null;
            }
        }

        private const string ColorSchemeUnixLoc = "~/.local/share/jvos/colorscheme.json";

        static ColorScheme()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var dir = $"{Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.local/share/jvos/";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var loc = ColorSchemeUnixLoc.Replace("~", Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile));
                if (!File.Exists(loc))
                {
                    ColorScheme.ApplyScheme(Current, false, true, false);
                    File.WriteAllText(loc, JsonConvert.SerializeObject(Current));
                }
                else
                {
                    var x = JsonConvert.DeserializeObject<ColorScheme>(File.ReadAllText(loc));
                    if(x == null)
                    {
                        ColorScheme.ApplyScheme(Current, false, true, false);
                        File.WriteAllText(loc, JsonConvert.SerializeObject(Current));
                    }
                    else
                    {
                        ColorScheme.ApplyScheme(x);
                    }
                }
                FSWatcher watcher = new FSWatcher(dir, "colorscheme.json");
                watcher.Changed += (a, b) => {
                    Console.WriteLine("File Changed");
                        var x = JsonConvert.DeserializeObject<ColorScheme>(File.ReadAllText(loc));
                        ColorScheme.ApplyScheme(x);
                };
                watcher.Start();
            }
            else
            {
                ColorScheme.ApplyScheme(Current, false, true, false);
            }
        }


        public ColorScheme()
        {

        }

        public static void Refresh()
        {
            ApplyScheme(Current);
        }

        private static void OnUpdated(ColorScheme scheme)
        {
            if (Updated != null)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    Updated.Invoke(null, scheme);
                });
            }
        }

        public static event EventHandler<ColorScheme> Updated;

        public static ResourceBunch ResourceBunch = new();
        public static ColorScheme Current = new();

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

        public BoxShadows ButtonBarClaymorphismBoxShadow;
        public BoxShadows ButtonBarClaymorphismInnerBoxShadow;
        

        
        public bool UseDarkScheme = false;
        public bool AccentTitle = false;
        public bool AccentBar = false;

        internal const double BLUR_RADIUS_CLAYMORPHISM = 16;

        

        public static void ApplyScheme(ColorScheme scheme, bool? UseDarkScheme = null, bool? AccentTitle = null, bool? AccentBar = null)
        {
            bool useDark = false, accentTitle = false, accentBar = false;
            if(UseDarkScheme != null && AccentBar != null && AccentTitle != null)
            {
                scheme = CreateColorSchemeFromColor(scheme.BasicColor);
                useDark=scheme.UseDarkScheme = UseDarkScheme ?? scheme.UseDarkScheme;
                accentTitle=scheme.AccentTitle = AccentTitle ?? scheme.AccentTitle;
                accentBar=scheme.AccentBar = AccentBar ?? scheme.AccentBar;
                Current = scheme;
            }
            else
            {
                useDark = UseDarkScheme ?? scheme.UseDarkScheme;
                accentTitle = AccentTitle ?? scheme.AccentTitle;
                accentBar = AccentBar ?? scheme.AccentBar;
                Current = scheme;
            }


            ResourceBunch.SetResource("BasicClaymorphismTop", !useDark ? scheme.LightBasicClaymorphismTop : scheme.DarkBasicClaymorphismTop);
            ResourceBunch.SetResource("BasicClaymorphismBottom", !useDark ? scheme.LightBasicClaymorphismBottom : scheme.DarkBasicClaymorphismBottom);
            ResourceBunch.SetResource("BasicShadow", !useDark ? scheme.LightBasicShadow : scheme.DarkBasicShadow);
            ResourceBunch.SetResource("AccentClaymorphismTop", !useDark ? scheme.LightAccentClaymorphismTop : scheme.DarkAccentClaymorphismTop);
            ResourceBunch.SetResource("AccentClaymorphismBottom", !useDark ? scheme.LightAccentClaymorphismBottom : scheme.DarkAccentClaymorphismBottom);
            ResourceBunch.SetResource("AccentShadow", !useDark ? scheme.LightAccentShadow : scheme.DarkAccentShadow);
            ResourceBunch.SetResource("ButtonAccentClaymorphismTop", !useDark ? scheme.LightAccentButtonClaymorphismTop : scheme.DarkAccentButtonClaymorphismTop);
            ResourceBunch.SetResource("ButtonAccentClaymorphismBottom", !useDark ? scheme.LightAccentButtonClaymorphismBottom : scheme.DarkAccentButtonClaymorphismBottom);
            ResourceBunch.SetResource("ButtonAccentShadow", !useDark ? scheme.LightAccentButtonShadow : scheme.DarkAccentButtonShadow);
            ResourceBunch.SetResource("ButtonBasicClaymorphismTop", !useDark ? scheme.LightBasicButtonClaymorphismTop : scheme.DarkBasicButtonClaymorphismTop);
            ResourceBunch.SetResource("ButtonBasicClaymorphismBottom", !useDark ? scheme.LightBasicButtonClaymorphismBottom : scheme.DarkBasicButtonClaymorphismBottom);
            ResourceBunch.SetResource("ButtonBasicShadow", !useDark ? scheme.LightBasicButtonShadow : scheme.DarkBasicButtonShadow);
            ResourceBunch.SetResource("BasicBackground", !useDark ? new LinearGradientBrush() { GradientStops = new GradientStops() { new GradientStop(scheme.LightBasicBackgroundStart, 0), new GradientStop(scheme.LightBasicBackgroundEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(scheme.DarkBasicBackgroundStart, 0), new GradientStop(scheme.DarkBasicBackgroundEnd, 1) } });
            ResourceBunch.SetResource("AccentBackground", !useDark ? new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.LightAccentBackgroundStart, 0), new GradientStop(scheme.LightAccentBackgroundEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.DarkAccentBackgroundStart, 0), new GradientStop(scheme.DarkAccentBackgroundEnd, 1) } });
            ResourceBunch.SetResource("ButtonBasicBackground", !useDark ? new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.LightBasicButtonStart, 0), new GradientStop(scheme.LightBasicButtonEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.DarkBasicButtonStart, 0), new GradientStop(scheme.DarkBasicButtonEnd, 1) } });
            ResourceBunch.SetResource("ButtonAccentBackground", !useDark ? new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.LightAccentButtonStart, 0), new GradientStop(scheme.LightAccentButtonEnd, 1) } } : new LinearGradientBrush() { GradientStops = new GradientStops{ new GradientStop(scheme.DarkAccentButtonStart, 0), new GradientStop(scheme.DarkAccentButtonEnd, 1) } });
            ResourceBunch.SetResource("BasicClaymorphismBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("BasicClaymorphismTop") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = (Color)ResourceBunch.GetResource<Color>("BasicClaymorphismBottom") }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("BasicShadow") } }));
            ResourceBunch.SetResource("AccentClaymorphismBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("AccentClaymorphismTop") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("AccentClaymorphismBottom") }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("AccentShadow") } }));
            ResourceBunch.SetResource("ButtonBasicClaymorphismBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonBasicClaymorphismTop") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonBasicClaymorphismBottom") }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonBasicShadow") } }));
            ResourceBunch.SetResource("ButtonAccentClaymorphismBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonAccentClaymorphismTop") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonAccentClaymorphismBottom") }, new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonAccentShadow") } }));
            ResourceBunch.SetResource("BasicClaymorphismInnerBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("BasicClaymorphismBottom") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("BasicClaymorphismTop") } }));
            ResourceBunch.SetResource("AccentClaymorphismInnerBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("AccentClaymorphismBottom") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("AccentClaymorphismTop") } }));
            ResourceBunch.SetResource("ButtonBasicClaymorphismInnerBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonBasicClaymorphismBottom") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonBasicClaymorphismTop") } }));
            ResourceBunch.SetResource("ButtonAccentClaymorphismInnerBoxShadow", new BoxShadows(new BoxShadow() { OffsetX = BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonAccentClaymorphismBottom") }, new BoxShadow[] { new BoxShadow() { OffsetX = -BLUR_RADIUS_CLAYMORPHISM / 2, OffsetY = -BLUR_RADIUS_CLAYMORPHISM / 2, IsInset = true, Blur = BLUR_RADIUS_CLAYMORPHISM, Color = ResourceBunch.GetResource<Color>("ButtonAccentClaymorphismTop") } }));

            ResourceBunch.SetResource("BasicForeground", !useDark ? new SolidColorBrush() { Color = scheme.LightBasicForeground } : new SolidColorBrush() { Color = scheme.DarkBasicForeground });
            ResourceBunch.SetResource("AccentForeground", !useDark ? new SolidColorBrush() { Color = scheme.LightAccentForeground } : new SolidColorBrush() { Color = scheme.DarkAccentForeground });
            ResourceBunch.SetResource("ButtonBasicForeground", !useDark ? new SolidColorBrush() { Color = scheme.LightBasicButtonForeground } : new SolidColorBrush() { Color = scheme.DarkBasicButtonForeground });
            ResourceBunch.SetResource("ButtonAccentForeground", !useDark ? new SolidColorBrush() { Color = scheme.LightAccentButtonForeground } : new SolidColorBrush() { Color = scheme.DarkAccentButtonForeground });
            ResourceBunch.SetResource("BarClaymorphism", !accentBar ? ResourceBunch.GetResource<object>("BasicClaymorphism") : ResourceBunch.GetResource<object>("AccentClaymorphism"));
            ResourceBunch.SetResource("ButtonBarClaymorphism", !accentBar ? ResourceBunch.GetResource<object>("ButtonBasicClaymorphism") : ResourceBunch.GetResource<object>("ButtonAccentClaymorphism"));
            ResourceBunch.SetResource("BarBackground", !accentBar ? ResourceBunch.GetResource<object>("BasicBackground") : ResourceBunch.GetResource<object>("AccentBackground"));
            ResourceBunch.SetResource("ButtonBarBackground", !accentBar ? ResourceBunch.GetResource<object>("ButtonBasicBackground") : ResourceBunch.GetResource<object>("ButtonAccentBackground"));
            ResourceBunch.SetResource("BarForeground", !accentBar ? ResourceBunch.GetResource<object>("BasicForeground") : ResourceBunch.GetResource<object>("AccentForeground"));
            ResourceBunch.SetResource("ButtonBarForeground", !accentBar ? ResourceBunch.GetResource<object>("ButtonBasicForeground") : ResourceBunch.GetResource<object>("ButtonAccentForeground"));
            ResourceBunch.SetResource("BarClaymorphismBoxShadow", !accentBar ? ResourceBunch.GetResource<object>("BasicClaymorphismBoxShadow") : ResourceBunch.GetResource<object>("AccentClaymorphismBoxShadow"));
            ResourceBunch.SetResource("BarClaymorphismInnerBoxShadow", !accentBar ? ResourceBunch.GetResource<object>("BasicClaymorphismInnerBoxShadow") : ResourceBunch.GetResource<object>("AccentClaymorphismInnerBoxShadow"));
            ResourceBunch.SetResource("ButtonBarClaymorphismBoxShadow", !accentBar ? ResourceBunch.GetResource<object>("ButtonBasicClaymorphismBoxShadow") : ResourceBunch.GetResource<object>("ButtonAccentClaymorphismBoxShadow"));
            ResourceBunch.SetResource("ButtonBarClaymorphismInnerBoxShadow", !accentBar ? ResourceBunch.GetResource<object>("ButtonBasicClaymorphismInnerBoxShadow") : ResourceBunch.GetResource<object>("ButtonAccentClaymorphismInnerBoxShadow"));
            ResourceBunch.SetResource("TitleClaymorphism", !accentTitle ? ResourceBunch.GetResource<object>("BasicClaymorphism") : ResourceBunch.GetResource<object>("AccentClaymorphism"));
            ResourceBunch.SetResource("TitleBackground", !accentTitle ? ResourceBunch.GetResource<object>("BasicBackground") : ResourceBunch.GetResource<object>("AccentBackground"));
            ResourceBunch.SetResource("TitleForeground", !accentTitle ? ResourceBunch.GetResource<object>("BasicForeground") : ResourceBunch.GetResource<object>("AccentForeground"));
            ResourceBunch.SetResource("TitleClaymorphismBoxShadow", !accentTitle ? ResourceBunch.GetResource<object>("BasicClaymorphismBoxShadow") : ResourceBunch.GetResource<object>("AccentClaymorphismBoxShadow"));
            ResourceBunch.SetResource("TitleClaymorphismInnerBoxShadow", !accentTitle ? ResourceBunch.GetResource<object>("BasicClaymorphismInnerBoxShadow") : ResourceBunch.GetResource<object>("AccentClaymorphismInnerBoxShadow"));

            scheme.ButtonBarClaymorphismBoxShadow = !accentBar ? ResourceBunch.GetResource<BoxShadows>("ButtonBasicClaymorphismBoxShadow") : ResourceBunch.GetResource<BoxShadows>("ButtonAccentClaymorphismBoxShadow");
            scheme.ButtonBarClaymorphismInnerBoxShadow = !accentBar ? ResourceBunch.GetResource<BoxShadows>("ButtonBasicClaymorphismInnerBoxShadow") : ResourceBunch.GetResource<BoxShadows>("ButtonAccentClaymorphismInnerBoxShadow");


            ResourceBunch.SetResource("DefaultButtonStyle", CreateStyle<JButton>(
                (JButton.ActiveBoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonBarClaymorphismInnerBoxShadow")),
                (JButton.BoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonBarClaymorphismBoxShadow")),
                (JButton.BackgroundProperty, ResourceBunch.GetResource<Brush>("ButtonBasicBackground")),
                (JButton.HovergroundProperty, ResourceBunch.GetResource<Brush>("ButtonBasicBackground")),
                (JButton.OvergroundProperty, ResourceBunch.GetResource<Brush>("ButtonBasicBackground")),
                (JButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                (JButton.VerticalContentAlignmentProperty, VerticalAlignment.Center),
                (JButton.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (JButton.PaddingProperty, new Thickness(8, 4)),
                (JButton.MinHeightProperty, 32d),
                (JButton.ClipToBoundsProperty, false),
                (JButton.ForegroundProperty, ResourceBunch.GetResource<Brush>("ButtonBasicForeground"))
                ));
            ResourceBunch.SetResource("HighlightButtonStyle", CreateStyle<JButton>("highlight",
                (JButton.ActiveBoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonAccentClaymorphismInnerBoxShadow")),
                (JButton.BoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonAccentClaymorphismBoxShadow")),
                (JButton.BackgroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HovergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.OvergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                (JButton.VerticalContentAlignmentProperty, VerticalAlignment.Center),
                (JButton.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (JButton.PaddingProperty, new Thickness(8, 4)),
                (JButton.ClipToBoundsProperty, false),
                (JButton.ForegroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentForeground"))
                )); 
            ResourceBunch.SetResource("RedButtonStyle", CreateStyle<JButton>("highlight",
                (JButton.ActiveBoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonAccentClaymorphismInnerBoxShadow")),
                (JButton.BoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonAccentClaymorphismBoxShadow")),
                (JButton.BackgroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HovergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.OvergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                (JButton.VerticalContentAlignmentProperty, VerticalAlignment.Center),
                (JButton.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (JButton.PaddingProperty, new Thickness(8, 4)),
                (JButton.ClipToBoundsProperty, false),
                (JButton.ForegroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentForeground"))
                ));
            ResourceBunch.SetResource("BarButtonStyle", CreateStyle<JButton>("bar",
                (JButton.ActiveBoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonClaymorphismInnerBoxShadow")),
                (JButton.BoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonBarClaymorphismBoxShadow")),
                (JButton.BackgroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HovergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.OvergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                (JButton.VerticalContentAlignmentProperty, VerticalAlignment.Center),
                (JButton.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (JButton.PaddingProperty, new Thickness(8, 4)),
                (JButton.ClipToBoundsProperty, false),
                (JButton.ForegroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentForeground"))
                ));
            ResourceBunch.SetResource("TitleButtonStyle", CreateStyle<JButton>("title",
                (JButton.ActiveBoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonClaymorphismInnerBoxShadow")),
                (JButton.BoxShadowsProperty, ResourceBunch.GetResource<BoxShadows>("ButtonBarClaymorphismBoxShadow")),
                (JButton.BackgroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HovergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.OvergroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentBackground")),
                (JButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                (JButton.VerticalContentAlignmentProperty, VerticalAlignment.Center),
                (JButton.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (JButton.PaddingProperty, new Thickness(8, 4)),
                (JButton.ClipToBoundsProperty, false),
                (JButton.ForegroundProperty, ResourceBunch.GetResource<Brush>("ButtonAccentForeground"))
                ));
            ResourceBunch.SetResource("OuterBorderStyle", CreateStyle<Border>("outer",
                (Border.BoxShadowProperty, ResourceBunch.GetResource<BoxShadows>("BarClaymorphismBoxShadow")),
                (Border.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (Border.PaddingProperty, new Thickness(8, 4))
                ));
            ResourceBunch.SetResource("InnerBorderStyle", CreateStyle<Border>("inner",
                (Border.BoxShadowProperty, ResourceBunch.GetResource<BoxShadows>("BarClaymorphismInnerBoxShadow")),
                (Border.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (Border.PaddingProperty, new Thickness(8, 4))
                ));
            ResourceBunch.SetResource("DialogBoderStyle", CreateStyle<Border>("dialog",
                (Border.BoxShadowProperty, ResourceBunch.GetResource<BoxShadows>("BarClaymorphismBoxShadow")),
                (Border.CornerRadiusProperty, new CornerRadius(8, 8, 8, 8)),
                (Border.PaddingProperty, new Thickness(8, 4)),
                (Border.BackgroundProperty, ResourceBunch.GetResource<Brush>("BasicBackground"))
                ));
            ResourceBunch.SetResource("TextBlockDefaultStyle", CreateStyle<TextBlock>(
                (TextBlock.ForegroundProperty, ResourceBunch.GetResource<Brush>("BasicForeground"))
                ));
            ResourceBunch.SetResource("TextBlockTitleStyle", CreateStyle<TextBlock>("title",
                (TextBlock.ForegroundProperty, ResourceBunch.GetResource<Brush>("BasicForeground")),
                (TextBlock.FontSizeProperty, 20d),
                (TextBlock.FontWeightProperty, FontWeight.Bold)
                ));
            ResourceBunch.SetResource("TextBlockSubtitleStyle", CreateStyle<TextBlock>("subtitle",
                (TextBlock.ForegroundProperty, ResourceBunch.GetResource<Brush>("BasicForeground")),
                (TextBlock.FontSizeProperty, 16d)
                ));
            ResourceBunch.SetResource("RedActionButtonStyle", CreateActionButtonStyle(Colors.Red, useDark, "redact"));
            ResourceBunch.SetResource("GreenActionButtonStyle", CreateActionButtonStyle(Colors.Green, useDark, "greenact"));
            ResourceBunch.SetResource("YellowActionButtonStyle", CreateActionButtonStyle(Colors.Yellow, useDark, "yellowact"));
            ResourceBunch.SetResource("BlueActionButtonStyle", CreateActionButtonStyle(Colors.Blue, useDark, "blueact"));
            OnUpdated(scheme);

        }

        private static Style CreateActionButtonStyle(Color colorBase, bool darkMode, string? classes = null)
        {
            var j = new ActionSpecificButtonBase(colorBase, darkMode);
            if (classes == null)
                return CreateStyle<JButton>(
                        (JButton.BoxShadowsProperty, j.DefaultStateShadows),
                        (JButton.ActiveBoxShadowsProperty, j.HoldStateShadows),
                        (JButton.ForegroundProperty, new SolidColorBrush(j.Foreground)),
                        (JButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                        (JButton.VerticalContentAlignmentProperty, VerticalAlignment.Center),
                        (JButton.ClipToBoundsProperty, false),
                        (JButton.BackgroundProperty, new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(j.GradientStartBackground, 0), new GradientStop(j.GradientStopBackground, 1) } }),
                        (JButton.OvergroundProperty, new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(j.GradientStartBackground, 0), new GradientStop(j.GradientStopBackground, 1) } }),
                        (JButton.HovergroundProperty, new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(j.GradientStartBackground, 0), new GradientStop(j.GradientStopBackground, 1) } })
                    );
            else
                return CreateStyle<JButton>(classes,
                        (JButton.BoxShadowsProperty, j.DefaultStateShadows),
                        (JButton.ActiveBoxShadowsProperty, j.HoldStateShadows),
                        (JButton.ForegroundProperty, new SolidColorBrush(j.Foreground)),
                        (JButton.ClipToBoundsProperty, false),
                        (JButton.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                        (JButton.VerticalContentAlignmentProperty, VerticalAlignment.Center),
                        (JButton.BackgroundProperty, new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(j.GradientStartBackground, 0), new GradientStop(j.GradientStopBackground, 1) } }),
                        (JButton.OvergroundProperty, new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(j.GradientStartBackground, 0), new GradientStop(j.GradientStopBackground, 1) } }),
                        (JButton.HovergroundProperty, new LinearGradientBrush() { GradientStops = new GradientStops { new GradientStop(j.GradientStartBackground, 0), new GradientStop(j.GradientStopBackground, 1) } })
                    );
        }

        private static Style CreateStyle<T>(params (AvaloniaProperty, object?)[] x)
        {
            var style = new Style(x => x.OfType(typeof(T)));
            foreach (var s in x)
                style.Setters.Add(new Setter(s.Item1, s.Item2));
            return style;
        }

        private static Style CreateStyle<T>(string Class, params (AvaloniaProperty, object?)[] x)
        {
            var style = new Style(x => x.OfType(typeof(T)).Class(Class));
            foreach (var s in x)
                style.Setters.Add(new Setter(s.Item1, s.Item2));
            return style;
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
