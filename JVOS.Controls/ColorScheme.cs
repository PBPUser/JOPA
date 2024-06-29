using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using JVOS.Controls;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Color = Avalonia.Media.Color;

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

        public ColorHSV WithHue(double hue)
        {
            return this with { Hue = hue };
        }

        public ColorHSV WithValue(double value)
        {
            return this with { Value = value };
        }
        public ColorHSV WithAlpha(byte alpha)
        {
            return this with { Alpha = alpha };
        }

        public ColorHSV WithSaturation(double saturation)
        {
            return this with { Saturation = saturation };
        }

        public ColorHSV AddHue(double hue)
        {
            return this with { Hue = Math.Clamp(this.Hue + hue, 0, 360) };
        }

        public ColorHSV AddValue(double value)
        {
            return this with { Value = this.Value + value };
        }

        public ColorHSV AddSaturation(double saturation)
        {
            return this with { Saturation = this.Saturation + saturation };
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
            HSV.Hue += 720;
            HSV.Hue = HSV.Hue % 360;
            HSV.Value = Math.Clamp(HSV.Value, 0, 100);
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

    public struct ClaymorphismLayer
    {
        public ColorHSV BaseColor;
        public Color Foreground;
        public Color TopGradient;
        public Color BottomGradient;
        public Color TopInsetShadow;
        public Color BottomInsetShadow;
        public Color BottomShadow;

        public Brush Background;
        public BoxShadows BoxShadows;
        public BoxShadows ButtonBoxShadows;

        public bool Dark;
        public bool Inner;

        ClaymorphismLayer(ColorHSV colorHSV, bool dark, bool inner, bool bwbg = false)
        {
            Dark = dark;
            Inner = inner;
            BaseColor = colorHSV;

            Foreground = dark ? Colors.White : Colors.Black;

            TopGradient = ColorHSV.HsvToRgb(colorHSV.AddHue(-10).WithValue(bwbg ? (dark ? 10: 90) : colorHSV.Value).WithSaturation(bwbg ? 0 : colorHSV.Saturation));
            BottomGradient = ColorHSV.HsvToRgb(colorHSV.AddHue(10).WithValue(bwbg ? (dark ? 5 : 80) : colorHSV.Value).WithSaturation(bwbg ? 5 : colorHSV.Saturation));
            if (!inner)
            {
                TopInsetShadow = ColorHSV.HsvToRgb(colorHSV.AddValue(10).WithSaturation(bwbg?0:colorHSV.Saturation).WithAlpha(bwbg ? (byte)(colorHSV.Alpha / 4) : (byte)(colorHSV.Alpha / 2)));
                BottomInsetShadow = ColorHSV.HsvToRgb(colorHSV.AddValue(-10).WithAlpha(bwbg ? (byte)(colorHSV.Alpha / 4) : (byte)(colorHSV.Alpha / 2)));
                BottomShadow = ColorHSV.HsvToRgb(colorHSV with { Value = colorHSV.Value - 15, Alpha = (byte)(colorHSV.Alpha * 0.5) });
            }
            else
            {
                TopInsetShadow = ColorHSV.HsvToRgb(colorHSV with { Value = colorHSV.Value - 10 });
                BottomInsetShadow = ColorHSV.HsvToRgb(colorHSV.AddValue(10).WithSaturation(bwbg?0:colorHSV.Saturation).WithAlpha(bwbg?(byte)(colorHSV.Alpha/4):(byte)(colorHSV.Alpha/2)));
                BottomShadow = Colors.Transparent;
            }

            Background = new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops()
                {
                    new GradientStop(){ Color = TopGradient, Offset = 0 },
                    new GradientStop(){ Color = BottomGradient, Offset = 1 }
                }
            };

            BoxShadows = new BoxShadows(
                new BoxShadow()
                {
                    Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM,
                    OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2,
                    OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2,
                    IsInset = true,
                    Color = TopInsetShadow
                },
                new BoxShadow[] {
                    new BoxShadow()
                    {
                        Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM,
                        OffsetX = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2,
                        OffsetY = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 2,
                        IsInset = true,
                        Color = BottomInsetShadow
                    },
                    new BoxShadow()
                    {
                        Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM /2,
                        OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 6,
                        OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 3,
                        IsInset = false,
                        Color = BottomShadow
                    }
                }
                );

            ButtonBoxShadows = new BoxShadows(
                new BoxShadow()
                {
                    Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 4,
                    OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 8,
                    OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 8,
                    IsInset = true,
                    Color = TopInsetShadow
                },
                new BoxShadow[] {
                    new BoxShadow()
                    {
                        Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 4,
                        OffsetX = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 8,
                        OffsetY = -ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 8,
                        IsInset = true,
                        Color = BottomInsetShadow
                    },
                    new BoxShadow()
                    {
                        Blur = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 4,
                        OffsetX = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 8,
                        OffsetY = ColorScheme.BLUR_RADIUS_CLAYMORPHISM / 4,
                        IsInset = false,
                        Color = BottomShadow
                    }
                }
                );

        }

        public static void GetLightDarkColors(ColorHSV input, out ColorHSV outputLight, out ColorHSV outputDark)
        {
            outputLight = input.Value >= 50 ? input : input with { Value = 100 - input.Value };
            outputDark = input.Value < 50 ? input : input with { Value = 100 - input.Value };
        }

        public static double ClampMod(double value, double min, double max)
        {
            return (value + max - min) % (max - min) + min;
        }

        public static double ClampHue(double value) => ClampMod(value, 0, 360);

        public static void GenerateClaymorphismLayer(ColorHSV BaseColor, bool IsInner, out ClaymorphismLayer LightLayer, out ClaymorphismLayer DarkLayer, bool? bwbg = null)
        {
            BaseColor.Saturation = Math.Min(BaseColor.Saturation, 90);
            BaseColor.Value = Math.Clamp(BaseColor.Value, 15, 90);

            ColorHSV DarkColor, LightColor;

            GetLightDarkColors(BaseColor, out LightColor, out DarkColor);

            LightLayer = new ClaymorphismLayer(LightColor, false, IsInner, bwbg == true);
            DarkLayer = new ClaymorphismLayer(DarkColor, true, IsInner, bwbg == true);
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

        public Color DesktopForeground = Colors.Chocolate;

        public ClaymorphismLayer AlphaLightInner, AlphaLightOuter, AlphaDarkInner, AlphaDarkOuter;
        public ClaymorphismLayer BettaLightInner, BettaLightOuter, BettaDarkInner, BettaDarkOuter;
        public ClaymorphismLayer BasicLightInner, BasicLightOuter, BasicDarkInner, BasicDarkOuter;
        public ClaymorphismLayer LayerLightInner, LayerLightOuter, LayerDarkInner, LayerDarkOuter;
        public ClaymorphismLayer DemiiLightInner, DemiiLightOuter, DemiiDarkInner, DemiiDarkOuter;
        public ClaymorphismLayer AcentLightInner, AcentLightOuter, AcentDarkInner, AcentDarkOuter;
        
        public bool UseDarkScheme = false;
        public bool AccentTitle = false;
        public bool AccentBar = false;
        public bool UseBlur = true;

        internal const double BLUR_RADIUS_CLAYMORPHISM = 32;

        private static void ApplyClaymorhismLayer(bool useDark, ClaymorphismLayer light, ClaymorphismLayer dark, string postfix)
        {
            ResourceBunch.SetResource("ButtonShadows_" + postfix, !useDark ? light.ButtonBoxShadows : dark.ButtonBoxShadows);
            ResourceBunch.SetResource("BoxShadows_" + postfix, !useDark ? light.BoxShadows : dark.BoxShadows);
            ResourceBunch.SetResource("Foreground_" + postfix, !useDark ? light.Foreground : dark.Foreground);
            ResourceBunch.SetResource("Background_" + postfix, !useDark ? light.Background : dark.Background);
        }

        private static void ApplyClaymorhismLayer(bool accent, string basicPostfix, string accentPostfix, string postfix)
        {
            ResourceBunch.SetResource($"ButtonShadows_{postfix}", !accent ? ResourceBunch.GetResource<BoxShadows>($"ButtonShadows_{basicPostfix}") : ResourceBunch.GetResource<BoxShadows>($"ButtonShadows_{accentPostfix}"));
            ResourceBunch.SetResource($"BoxShadows_{postfix}", !accent ? ResourceBunch.GetResource<BoxShadows>($"BoxShadows_{basicPostfix}") : ResourceBunch.GetResource<BoxShadows>($"BoxShadows_{accentPostfix}"));
            ResourceBunch.SetResource($"Foreground_{postfix}", !accent ? ResourceBunch.GetResource<Brush>($"Foreground_{basicPostfix}") : ResourceBunch.GetResource<Brush>($"Foreground_{accentPostfix}"));
            ResourceBunch.SetResource($"Background_{postfix}", !accent ? ResourceBunch.GetResource<Brush>($"Background_{basicPostfix}") : ResourceBunch.GetResource<Brush>($"Background_{accentPostfix}"));
        }

        public static void ApplyScheme(ColorScheme scheme, bool? UseDarkScheme = null, bool? AccentTitle = null, bool? AccentBar = null, bool? UseBlur = null, bool reloadScheme = false)
        {
            bool useDark = false, accentTitle = false, accentBar = false, useBlur = true;
            if(reloadScheme)
            {
                scheme = CreateColorSchemeFromColor(scheme.BasicColor);
                useDark=scheme.UseDarkScheme = UseDarkScheme ?? scheme.UseDarkScheme;
                accentTitle=scheme.AccentTitle = AccentTitle ?? scheme.AccentTitle;
                accentBar=scheme.AccentBar = AccentBar ?? scheme.AccentBar;
                useBlur = scheme.UseBlur = UseBlur ?? scheme.UseBlur;
                Current = scheme;
            }
            else
            {
                useDark = UseDarkScheme ?? scheme.UseDarkScheme;
                accentTitle = AccentTitle ?? scheme.AccentTitle;
                accentBar = AccentBar ?? scheme.AccentBar;
                useBlur = UseBlur ?? scheme.UseBlur;
                Current = scheme;
            }



            ResourceBunch.SetResource("TopShadow", new LinearGradientBrush() { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative), GradientStops = new GradientStops() { new GradientStop(Colors.Black, 0), new GradientStop(new Color(0, 0, 0, 0), 1) } });

            ApplyClaymorhismLayer(useDark, scheme.AlphaLightInner, scheme.AlphaDarkInner, "AlphaInner");
            ApplyClaymorhismLayer(useDark, scheme.AlphaLightOuter, scheme.AlphaDarkOuter, "AlphaOuter");

            ApplyClaymorhismLayer(useDark, scheme.BettaLightInner, scheme.BettaDarkInner, "BetaInner");
            ApplyClaymorhismLayer(useDark, scheme.BettaLightOuter, scheme.BettaDarkOuter, "BetaOuter");

            ApplyClaymorhismLayer(useDark, scheme.BasicLightInner, scheme.BasicDarkInner, "BasicInner");
            ApplyClaymorhismLayer(useDark, scheme.BasicLightOuter, scheme.BasicDarkOuter, "BasicOuter");

            ApplyClaymorhismLayer(useDark, scheme.LayerLightInner, scheme.LayerDarkInner, "LayerInner");
            ApplyClaymorhismLayer(useDark, scheme.LayerLightOuter, scheme.LayerDarkOuter, "LayerOuter");

            ApplyClaymorhismLayer(useDark, scheme.DemiiLightInner, scheme.DemiiDarkInner, "DemiInner");
            ApplyClaymorhismLayer(useDark, scheme.DemiiLightOuter, scheme.DemiiDarkOuter, "DemiOuter");

            ApplyClaymorhismLayer(useDark, scheme.AcentLightInner, scheme.AcentDarkInner, "AccentInner");
            ApplyClaymorhismLayer(useDark, scheme.AcentLightOuter, scheme.AcentDarkOuter, "AccentOuter");

            ApplyClaymorhismLayer(accentBar, "AlphaInner", "BetaInner", "BarInner");
            ApplyClaymorhismLayer(accentBar, "AlphaOuter", "BetaOuter", "BarOuter");

            ApplyClaymorhismLayer(accentBar, "BasicInner", "DemiInner", "BarLayer1Inner");
            ApplyClaymorhismLayer(accentBar, "BasicOuter", "DemiOuter", "BarLayer1Outer");

            ApplyClaymorhismLayer(accentBar, "LayerInner", "AccentInner", "BarLayer2Inner");
            ApplyClaymorhismLayer(accentBar, "LayerOuter", "AccentInner", "BarLayer2Outer");

            ApplyClaymorhismLayer(accentTitle, "DemiInner", "AccentInner", "TitleInner");
            ApplyClaymorhismLayer(accentTitle, "DemiOuter", "AccentOuter", "TitleOuter");

            ApplyClaymorhismLayer(accentTitle, "BasicInner", "BasicInner", "InactiveTitleInner");
            ApplyClaymorhismLayer(accentTitle, "BasicOuter", "BasicOuter", "InactiveTitleOuter");

            ResourceBunch.SetResource("Foreground_Desktop", new SolidColorBrush(scheme.DesktopForeground));

            OnUpdated(scheme);
        }

        public static ColorScheme CreateColorSchemeFromColor(Color color)
        {
            ColorScheme scheme = new ColorScheme();

            ColorHSV BaseColor = ColorHSV.RgbToHsv(color);

            scheme.BasicColor = ColorHSV.HsvToRgb(BaseColor);

            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 20, Alpha = 192 }, true, out scheme.AlphaLightInner, out scheme.AlphaDarkInner, true);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 20, Alpha = 192 }, false, out scheme.AlphaLightOuter, out scheme.AlphaDarkOuter, true);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 50, Alpha = 192 }, true, out scheme.BettaLightInner, out scheme.BettaDarkInner);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 50, Alpha = 192 }, false, out scheme.BettaLightOuter, out scheme.BettaDarkOuter);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 20 }, true, out scheme.BasicLightInner, out scheme.BasicDarkInner, true);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 20 }, false, out scheme.BasicLightOuter, out scheme.BasicDarkOuter, true);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 40 }, true, out scheme.LayerLightInner, out scheme.LayerDarkInner);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 40 }, false, out scheme.LayerLightOuter, out scheme.LayerDarkOuter);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 60 }, true, out scheme.DemiiLightInner, out scheme.DemiiDarkInner);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 60 }, false, out scheme.DemiiLightOuter, out scheme.DemiiDarkOuter);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 80 }, true, out scheme.AcentLightInner, out scheme.AcentDarkInner);
            ClaymorphismLayer.GenerateClaymorphismLayer(BaseColor with { Saturation = 80 }, false, out scheme.AcentLightOuter, out scheme.AcentDarkOuter);

            return scheme;
        }

        public static Color ColorFromBitmap(Bitmap bitmap, out Color foreground)
        {
            foreground = Colors.BlueViolet;
            if (bitmap == null)
                return Colors.Black;
            var x = Path.GetTempFileName() + ".png";
            bitmap.Save(x, 100);
            using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(x);
            File.Delete(x);
            byte[] pixels = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixels);
            long r=0, g=0, b=0;
            long c = pixels.Length / 4;
            for(int i =0; i < c; i++)
            {
                r += pixels[i*4];
                g += pixels[i*4+1];
                b += pixels[i*4+2];
            }
            pixels = new byte[0];
            long j = (r + g + b) / (c * 3);
            foreground = j < 128 ? Colors.White : Colors.Black;
            return Color.FromRgb((byte)(r / c), (byte)(g / c), (byte)(b / c));
        }
    }
}
