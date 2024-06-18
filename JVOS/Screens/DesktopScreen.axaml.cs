using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using JVOS.ApplicationAPI.Hub;
using JVOS.Controls;
using JVOS.DataModel;
using JVOS.EmbededWindows;
using JVOS.Hubs;
using JVOS.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Reflection;
using System.Runtime.Loader;
using DynamicData;
using SharpCompress;

namespace JVOS.Screens
{
    public partial class DesktopScreen : ScreenBase, IWindowSpace
    {
        public static DesktopScreen? CurrentDesktop;

        public DesktopScreen()
        {
            InitializeComponent();
            AddWindowHub(new DesktopHub() { ParentScreen = this });
            OHubXTransition = new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseOut() };
            OHubYTransition = new DoubleTransition() { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseOut() };
            CHubXTransition = new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseIn() };
            CHubYTransition = new DoubleTransition() { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseIn() };
            
            
            //LanguageSwitcher = (LanguageSwitcherHub)AttachHub(new LanguageSwitcherHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            
            
            //Clock = (ClockHub)AttachHub(new ClockHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            //Panorama = (PanoramaHub)AttachHub(new PanoramaHub(), VerticalAlignment.Stretch, HorizontalAlignment.Left, Orientation.Horizontal);
            //LanguageSwitcher = (LanguageSwitcherHub)AttachHub(new LanguageSwitcherHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            //Start = (StartHub)AttachHub(new StartHub(), VerticalAlignment.Bottom, HorizontalAlignment.Center, Orientation.Vertical);
            //Clock._setLeft.Click += (a, b) => SetBarAlign(HorizontalAlignment.Left);
            //Clock._setCenter.Click += (a, b) => SetBarAlign(HorizontalAlignment.Center);
            appsPlace.RenderTransform = AppsBarTransform;
            widgetsBtn.RenderTransform = BtnBarTransform;
            AppsBarTransform.Transitions = new Transitions();
            AppsBarTransform.Transitions.Add(AppBarTransition);
            BtnBarTransform.Transitions = AppsBarTransform.Transitions;
            widgetsBtnPlace.Transitions = new Transitions();
            widgetsBtnPlace.Transitions.Add(WidgetPlaceTransition);
            OHubTransitions = new Transitions();
            OHubTransitions.Add(OHubXTransition);
            OHubTransitions.Add(OHubYTransition);
            CHubTransitions = new Transitions();
            CHubTransitions.Add(CHubXTransition);
            CHubTransitions.Add(CHubYTransition);
            keyBtn.Click += (a, b) => MainView.GLOBAL.SwitchAdaptiveControllerState();
            Loaded += (a, b) => Communicator.OpenWindow(new DesktopWindow());
            CurrentDesktop = this;
            LoadHubs();
        }

        DoubleTransition WidgetPlaceTransition = new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(500), Property = WidthProperty, Easing = new SineEaseIn() };
        DoubleTransition AppBarTransition = new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(500), Property = TranslateTransform.XProperty, Easing = new SineEaseIn() };

        TranslateTransform AppsBarTransform = new TranslateTransform();
        TranslateTransform BtnBarTransform = new TranslateTransform();

        DoubleTransition? PreviousImageTransition;
        Image? PreviousImage;
        TranslateTransform? PreviousImageTransform;
        DoubleTransition? BarTransition;
        DoubleTransition? OHubXTransition;
        DoubleTransition? OHubYTransition;
        DoubleTransition? CHubXTransition;
        DoubleTransition? CHubYTransition;

        Transitions OHubTransitions;
        Transitions CHubTransitions;

        LanguageSwitcherHub LanguageSwitcher;
        PanoramaHub Panorama;
        ClockHub Clock;
        StartHub Start;
        Subject<HorizontalAlignment> HorizontalBarSubjectAlignment = new();


        HorizontalAlignment BarAlignment = HorizontalAlignment.Center;

        private void SetBarAlign(HorizontalAlignment horizontalAlignment)
        {
            if (horizontalAlignment == HorizontalAlignment.Stretch || horizontalAlignment == HorizontalAlignment.Right || horizontalAlignment == BarAlignment) return;
            HorizontalBarSubjectAlignment.OnNext(horizontalAlignment);
            BarAlignment = horizontalAlignment;
            double xAppsPos = appsPlace.PointToScreen(new Point(0, 0)).X - Bounds.Left - 8;
            double xWidgetPos = widgetsBtn.PointToScreen(new Point(0, 0)).X - Bounds.Left - 8;
            this.barRandWordTitle.Text = $"{xAppsPos}";
            switch (BarAlignment)
            {
                case HorizontalAlignment.Center:
                    new Thread(() =>
                    {
                        Thread.Sleep(500);
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            AppBarTransition.Duration = TimeSpan.FromMilliseconds(0);
                            AppsBarTransform.X = 0;
                            BtnBarTransform.X = 0;
                            widgetsBtnPlace.Children.Remove(widgetsBtn);
                            leftStack.Children.Add(widgetsBtn);
                            appsPlace.HorizontalAlignment = HorizontalAlignment.Center;
                        });
                    }).Start();
                    AppBarTransition.Duration = TimeSpan.FromMilliseconds(500);
                    widgetsBtnPlace.Width = 0;
                    AppsBarTransform.X = (barBorder.Bounds.Width- appsPlace.Bounds.Width)/2;
                    BtnBarTransform.X = -xWidgetPos- (barBorder.Bounds.Width - appsPlace.Bounds.Width) / 2;
                    break;
                case HorizontalAlignment.Left:
                    new Thread(() =>
                    {
                        Thread.Sleep(500);
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            AppBarTransition.Duration = TimeSpan.FromMilliseconds(0);
                            AppsBarTransform.X = 0;
                            BtnBarTransform.X = 0;
                            leftStack.Children.Remove(widgetsBtn);
                            widgetsBtnPlace.Children.Add(widgetsBtn);
                            appsPlace.HorizontalAlignment = HorizontalAlignment.Left;
                        });
                    }).Start();
                    widgetsBtnPlace.Width = this.widgetsBtn.Bounds.Width;
                    AppBarTransition.Duration = TimeSpan.FromMilliseconds(500);
                    AppsBarTransform.X = -xAppsPos;
                    BtnBarTransform.X = 198;
                    break;
            }
        }

        private static void GetRandomString(EventHandler<string> @event)
        {
            new Thread(() =>
            {
            }).Start();
        }

        public Dictionary<WindowFrameBase, BarRunningInstance> RunningApplicationInstances = new();

        public void AttachBarApplication(WindowFrameBase jWindowFrame)
        {
            jWindowFrame.WindowLoaded += (a, b) =>
            {
                foreach(var x in RunningApplicationInstances)
                {
                    if(x.Value.AssociatedBarTooltip.PanelID == jWindowFrame.GetPanelId()) {
                        x.Value.AssociatedBarTooltip.AddJWindowFrame(jWindowFrame);
                        RunningApplicationInstances.Add(jWindowFrame, x.Value);
                        return;
                    }
                }
                var bt = new BarTooltip() { 
                    Icon = jWindowFrame.WindowContent.Icon, 
                    ClipToBounds = false, 
                    Margin = new Thickness(4), 
                    VerticalAlignment = VerticalAlignment.Center, 
                    Height = 56, 
                    Width = 0, 
                    Title = jWindowFrame.WindowContent.Title
                };
                BarRunningInstance barRunningInstance = new BarRunningInstance(bt, bt.Bind(WidthProperty, RunningAppWidthSubject));
                ColorScheme.Updated += barRunningInstance.SchemeUpdate;
                bt.Transitions = new Transitions { 
                    new DoubleTransition { 
                        Property = WidthProperty, 
                        Duration = TimeSpan.FromMilliseconds(175)
                    }, 
                    new DoubleTransition { 
                        Property = BarTooltip.ShadowPosProperty, 
                        Duration = TimeSpan.FromMilliseconds(175) 
                    }, 
                    new DoubleTransition { 
                        Property = BarTooltip.ScaleProperty, 
                        Duration = TimeSpan.FromMilliseconds(175) 
                    }, 
                    bt.DeltaTransition, 
                    new DoubleTransition { 
                        Duration = TimeSpan.FromMilliseconds(2000), 
                        Property = BarTooltip.StartAnimationProperty 
                    } 
                };
                bt.AddJWindowFrame(jWindowFrame);
                if(jWindowFrame.WindowContent.ShowOnTaskbar)
                    runnedApps.Children.Add(bt);
                bt.Width = RunningAppWidth;
                RunningApplicationInstances.Add(jWindowFrame, barRunningInstance);
                bt.AllWindowsClosed += (a, b) =>
                {
                    DeattachBarTooltip(barRunningInstance);
                };
            };
        }

        public void DeattachBarApplication(WindowFrameBase jWindowFrame)
        {
            BarRunningInstance x;
            if (RunningApplicationInstances.TryGetValue(jWindowFrame, out x))
            {
                RunningApplicationInstances.Remove(jWindowFrame);
                x.AssociatedBarTooltip.RemoveJWindowFrame(jWindowFrame);
            }
        }

        public void DeattachBarTooltip(BarRunningInstance instance)
        {
            instance.AssociatedBarTooltip.Width = 0;
            new Task(() =>
            {
                Task.Delay(175).Wait();
                Dispatcher.UIThread.Invoke(() =>
                {
                    runnedApps.Children.Remove(instance.AssociatedBarTooltip);
                    ColorScheme.Updated -= instance.SchemeUpdate;
                });
            }).Start();
        }

        public override void MobileModeStateSwitch(bool enabled)
        {
            runnedApps.IsVisible = !enabled;
            topBarBorder.IsVisible = enabled;
        }

        private int TopHub = 3;

        public void ToggleHub(HubWindow hub, bool? forced = null)
        {
            if (forced != null)
                hub.IsOpen = forced.Value;
            else
                hub.IsOpen = !hub.IsOpen;
            if (hub.IsOpen)
            {
                hub.OnOpened(EventArgs.Empty);
                (hub.RenderTransform as TranslateTransform).Transitions = OHubTransitions;
                hub.ZIndex = TopHub++;
                foreach (var Hubinfo in HubMan.LeftDesktopHubs)
                {
                    var xHub = Hubinfo.RuntimeHubInformation == null ? Hubinfo.InternalHubInformation.Value.Window : Hubinfo.RuntimeHubInformation.Value.Window;
                    if (xHub.IsOpen == false)
                        continue;
                    if (xHub == hub)
                        continue;
                    if(xHub.VerticalAlignment == hub.VerticalAlignment && xHub.HorizontalAlignment == hub.HorizontalAlignment)
                        ToggleHub(xHub, false);
                }
                if (hub.AnimationOrientation == Orientation.Horizontal)
                    (hub.RenderTransform as TranslateTransform).X = 0;
                else
                    (hub.RenderTransform as TranslateTransform).Y = 0;
            }
            else
            {
                hub.OnClosed(HubWindow.CloseReason.Hide);
                (hub.RenderTransform as TranslateTransform).Transitions = CHubTransitions;
                if (hub.AnimationOrientation == Orientation.Horizontal)
                    (hub.RenderTransform as TranslateTransform).X = hub.HorizontalAlignment == HorizontalAlignment.Left ? -hub.Bounds.Width : hub.Bounds.Width;
                else
                    (hub.RenderTransform as TranslateTransform).Y = hub.VerticalAlignment == VerticalAlignment.Top ? -hub.Bounds.Height : (hub.Bounds.Height + 88);

            }
        }

        private void UpdateStrings()
        {
            GetRandomString((a, b) =>
            {
                this.barRandWordTitle.Text = b;
            });
            GetRandomString((a, b) =>
            {
                this.barRandWordSubtitle.Text = b;
            });
        }

        public override void ScreenShown()
        {
            SetCurrentWindowHub(0);
            UpdateStrings();
            if(UserOptions.Current.DesktopBitmap != null)
                SetBackground(UserOptions.Current.DesktopBitmap, false);
            base.ScreenShown();
        }

        public void CloseWindow(WindowFrameBase window)
        {
            _currentWindowSpace?.CloseWindow(window);
        }

        private bool isBackgroundAnimationPlaying = false;

        public void SetBackground(Bitmap image,bool animate)
        {
            if (isBackgroundAnimationPlaying)
                return;
            isBackgroundAnimationPlaying = true;
            var trans = new TranslateTransform() { Y = -Bounds.Height };
            var img = new Image() { Source = image, RenderTransform = trans };
            backgroundImageHoster.Children.Add(img);
            var transiton = new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new QuadraticEaseOut() };
            trans.Transitions = new Avalonia.Animation.Transitions() { transiton };
            trans.Y = 0;
            if (PreviousImageTransition != null)
            {
                PreviousImageTransition.Easing = new QuadraticEaseIn();
                PreviousImageTransform.Y = Bounds.Height;
            }

            Task.Run(() =>
            {
                Task.Delay(500);
                isBackgroundAnimationPlaying = false;
                Dispatcher.UIThread.Invoke(() =>
                {
                    if(PreviousImage != null)
                        backgroundImageHoster.Children.Remove(PreviousImage);
                    PreviousImage = img;
                    PreviousImageTransition = transiton;
                    PreviousImageTransform = trans;
                });
            });
        }

        public void OpenWindow(WindowFrameBase window)
        {
            _currentWindowSpace?.OpenWindow(window);
        }

        public void MinimizeWindow(WindowFrameBase window)
        {
            _currentWindowSpace?.MinimizeWindow(window);
        }

        private IWindowSpace? _currentWindowSpace;
        public List<IWindowSpace> WindowHubs = new List<IWindowSpace>();

        public void AddWindowHub(IWindowSpace windowSpace)
        {
            if (!(windowSpace is Control))
                return;
            var winSpace = windowSpace as Control;
            this.WindowHubs.Add(windowSpace);
            this.desktopCanvasHoster.Children.Add(winSpace);
        }

        public void SetCurrentWindowHub(int index)
        {
            if (index >= WindowHubs.Count  || index <= -1)
                return;
            _currentWindowSpace = WindowHubs[index];
            WindowManager.SetCurrentWindowSpace(_currentWindowSpace);
        }

        private void LoadHubs()
        {
            HubMan = new(this);

            HubMan.Config.Left.ForEach(x => LoadHub(x, HorizontalAlignment.Left));
            HubMan.Config.Center.ForEach(x => LoadHub(x, HorizontalAlignment.Center));
            HubMan.Config.Right.ForEach(x => LoadHub(x, HorizontalAlignment.Right));
        }

        public void MoveHub(HubManager.DesktopHubInfo hubInfo, HubManager.HubConfig.HubConfigurationStructure structure, HorizontalAlignment newPlacement, int index)
        {
            HubMan.MoveHub(ref hubInfo, newPlacement, index);
            HubMan.Config.MoveHub(ref structure, newPlacement, index);
        }

        public void AddHub(HubManager.HubConfig.HubConfigurationStructure config, HorizontalAlignment placement)
        {
            LoadHub(config, placement);
            HubMan.Config.AddHub(config, placement);
        }

        public void RemoveHub(HubManager.HubConfig.HubConfigurationStructure config, HubManager.DesktopHubInfo info)
        {
            HubMan.DeattachHub(ref info);
            HubMan.Config.RemHub(ref config);
        }

        public void LoadHub(HubManager.HubConfig.HubConfigurationStructure config, HorizontalAlignment placement)
        {
            Assembly assembly;
            ConstructorInfo? constctuctor;
            bool internalAssembly = String.IsNullOrEmpty(config.Path);
            if (internalAssembly)
            {

                assembly = Assembly.GetExecutingAssembly();
                constctuctor = assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(HubProvider)))
                    .Where(x => x.ToString().Equals(config.HubName))
                    .SelectMany(x => x.GetConstructors().Where(x => x.GetParameters().Length == 0))
                    .FirstOrDefault();
                
                if(constctuctor == null)
                {
                    Communicator.ShowMessageDialog(new MessageDialog(config.HubName, "Hub not found"));
                    return;
                }
                var hubProvider = (HubProvider)constctuctor.Invoke(null);
                HubMan.AttachHub(new HubManager.DesktopHubInfo()
                {
                    InternalHubInformation = new HubManager.InternalHubInformation(hubProvider)
                }, 
                placement);
            }
            else
            {
                RuntimeHubInformation? hubInfo;
                string? s;
                if(!JVOS.ApplicationAPI.Hub.HubManager.LoadHub(config.Path, config.HubName, out hubInfo, out s))
                {
#if DEBUG
                    throw new Exception(s);
#endif
                    return;
                }
                HubMan.AttachHub(new HubManager.DesktopHubInfo()
                {
                    RuntimeHubInformation = hubInfo
                }, placement);
            }
        }

        private static int TopZIndex = 0;

        public void BringToFront(WindowFrameBase window)
        {
            ((Control)window).ZIndex = TopZIndex++;
        }

        public void CloseAllHubs()
        {

        }

        public static Subject<double> RunningAppWidthSubject = new Subject<double>();
        public static double RunningAppWidth = 0;

        public static void SetRunningAppButtonWidth(bool value)
        {
            RunningAppWidth = value ? 192 : 56;
            RunningAppWidthSubject.OnNext(RunningAppWidth);
        }


        public HubManager HubMan;

        public class HubManager
        {
            public HubConfig Config;
            DesktopScreen Parent;

            public HubManager(DesktopScreen parent)
            {
                Parent = parent;
                Config = HubConfig.Load(this);
                if (!File.Exists(UserOptions.Current.GetPath("Appdata\\HubsConfig.jvon")))
                    Config.Save();
            }

            public class HubConfig
            {
                [JsonIgnore]
                public HubManager? Parent;

                public HubConfig()
                {

                }

                private static HubConfig GetDefault(HubManager parent)
                {
                    return new HubConfig()
                    {
                        Center = new List<HubConfigurationStructure>() {
                            new HubConfigurationStructure()
                            {
                                HubName = typeof(StartHubProvider).ToString()
                            }
                        },
                        Right = new List<HubConfigurationStructure>()
                        {
                            new HubConfigurationStructure()
                            {
                                HubName = typeof(ClockHubProvider).ToString()
                            }
                        },
                        Parent = parent
                        
                    };
                }

                public List<HubConfigurationStructure> Left = new();
                public List<HubConfigurationStructure> Center = new();
                public List<HubConfigurationStructure> Right = new();

                public void AddHub(HubConfigurationStructure hubConfigurationStructure, HorizontalAlignment placement)
                {
                    switch (placement) {
                        case HorizontalAlignment.Left:
                            Left.Add(hubConfigurationStructure);
                            break;
                        case HorizontalAlignment.Center:
                            Center.Add(hubConfigurationStructure);
                            break;
                        case HorizontalAlignment.Right:
                            Right.Add(hubConfigurationStructure);
                            break;
                    }
                    Save();
                }

                public void RemHub(ref HubConfigurationStructure hubConfigurationStructure)
                {
                    Left.Remove(hubConfigurationStructure);
                    Center.Remove(hubConfigurationStructure);
                    Right.Remove(hubConfigurationStructure);
                    Save();
                }

                public void MoveHub(ref HubConfigurationStructure hubConfigurationStructure, HorizontalAlignment placement, int index)
                {
                    RemHub(ref hubConfigurationStructure);
                    if(index != -1)
                    {
                        switch (placement) {
                            case HorizontalAlignment.Left: Left.Insert(index, hubConfigurationStructure); break;
                            case HorizontalAlignment.Center: Center.Insert(index, hubConfigurationStructure); break;
                            case HorizontalAlignment.Right: Right.Insert(index, hubConfigurationStructure); break;
                        }
                        Save();
                    }
                    else
                    {
                        AddHub(hubConfigurationStructure, placement);
                    }
                }

                public void Save()
                {
                    string path = UserOptions.Current.GetPath("AppData\\HubsConfig.jvon");
                    File.WriteAllText(path, JsonConvert.SerializeObject(this));
                }

                public static HubConfig Load(HubManager parent)
                {
                    string path = UserOptions.Current.GetPath("AppData\\HubsConfig.jvon");
                    if (File.Exists(path))
                    {
                        HubConfig? s = JsonConvert.DeserializeObject<HubConfig>(File.ReadAllText(path));
                        if(s == null)
                            return GetDefault(parent);
                        s.Parent = parent;
                        return s;
                    }
                    
                    return GetDefault(parent);
                }

                public struct HubConfigurationStructure
                {
                    public HubConfigurationStructure(string path, string hubname)
                    {
                        Path = path;
                        HubName = hubname;
                    }

                    public HubConfigurationStructure()
                    {
                        Path = "";
                        HubName = "";
                    }

                    public override string ToString()
                    {
                        return HubName;
                    }

                    public string Path = "";
                    public string HubName = "";
                }
            }

            public List<DesktopHubInfo> LeftDesktopHubs = new();
            public List<DesktopHubInfo> CenterDesktopHubs = new();
            public List<DesktopHubInfo> RightDesktopHubs = new();

            public struct DesktopHubInfo
            {
                public RuntimeHubInformation? RuntimeHubInformation;
                public InternalHubInformation? InternalHubInformation;
                public JButton Button;
                public IDisposable? CenterHubHorizontalSubscription;

                public override string ToString()
                {
                    return RuntimeHubInformation == null ?
                        InternalHubInformation.Value.Provider.ToString() :
                        RuntimeHubInformation.Value.Provider.ToString();
                }
            }

            public struct InternalHubInformation {
                public InternalHubInformation(HubProvider provider)
                {
                    Provider = provider;
                    Window = provider.CreateHub();
                }

                public HubProvider Provider;
                public HubWindow Window;
            }

            public void LoadAndAddExternalHub(string path, string hubInternalName, HorizontalAlignment placement)
            {
                RuntimeHubInformation? runtimeHubInformation;
                string? error;

                if (!JVOS.ApplicationAPI.Hub.HubManager.LoadHub(path, hubInternalName, out runtimeHubInformation, out error))
                {
                    Communicator.ShowMessageDialog(new MessageDialog($"{hubInternalName}@{path}", error));
                    return;
                }
                DesktopHubInfo info = new() { RuntimeHubInformation = runtimeHubInformation.Value };
                AttachHub(info, placement);
            }

            public void RefreshHubs()
            {
                LeftDesktopHubs.ForEach(x => RefreshHub(x));
                CenterDesktopHubs.ForEach(x => RefreshHub(x));
                RightDesktopHubs.ForEach(x => RefreshHub(x));
            }

            public void RefreshHub(DesktopHubInfo info)
            {
                var hubProvider = info.InternalHubInformation != null ? info.InternalHubInformation.Value.Provider : info.RuntimeHubInformation.Value.Provider;
                hubProvider.UpdateButtonContent(ref info.Button);
            }

            public void AttachHub(DesktopHubInfo info, HorizontalAlignment placement)
            {
                var hubProvider = info.InternalHubInformation != null ? info.InternalHubInformation.Value.Provider : info.RuntimeHubInformation.Value.Provider;
                var hubWindow = info.InternalHubInformation != null ? info.InternalHubInformation.Value.Window : info.RuntimeHubInformation.Value.Window;
                info.Button = new();
                hubProvider.CreateButtonContent(ref info.Button);
                
                hubWindow.VerticalAlignment = hubWindow.GetDefaultVerticalAlignment();
                if (placement == HorizontalAlignment.Center)
                    info.CenterHubHorizontalSubscription = hubWindow.Bind(HorizontalAlignmentProperty, Parent.HorizontalBarSubjectAlignment);
                else
                    hubWindow.HorizontalAlignment = placement;


                TranslateTransform Transform = new();
                hubWindow.RenderTransform = Transform;
                hubWindow.Margin = new Thickness(0, 0, 0, 88);

                hubWindow.Loaded += (a, b) =>
                {
                    if (hubWindow.AnimationOrientation == Orientation.Horizontal)
                    {
                        if (hubWindow.HorizontalAlignment == HorizontalAlignment.Left)
                            Transform.X = -hubWindow.Bounds.Width;
                        else
                            Transform.X = hubWindow.Bounds.Width;
                    }
                    else
                    {
                        if (hubWindow.GetDefaultVerticalAlignment() == VerticalAlignment.Top)
                            Transform.Y = -hubWindow.Bounds.Height;
                        else
                            Transform.Y = hubWindow.Bounds.Height + 88;
                    }
                    hubWindow.SizeChanged += (a, b) =>
                    {
                        Transform.Transitions = Parent.CHubTransitions;
                        if (b.PreviousSize.Width != b.NewSize.Width && !hubWindow.IsOpen && Orientation.Horizontal == hubWindow.AnimationOrientation)
                        {
                            if (hubWindow.HorizontalAlignment == HorizontalAlignment.Left)
                                Transform.X = -hubWindow.Bounds.Width;
                            else
                                Transform.X = hubWindow.Bounds.Width;
                        }
                        else if (b.PreviousSize.Height != b.NewSize.Height && !hubWindow.IsOpen && Orientation.Vertical == hubWindow.AnimationOrientation)
                        {
                            if (hubWindow.VerticalAlignment == VerticalAlignment.Top)
                                Transform.Y = -hubWindow.Bounds.Height;
                            else
                                Transform.Y = hubWindow.Bounds.Height + 88;
                        }
                    };
                };

                info.Button.Click += (a, b) => Parent.ToggleHub(hubWindow);
                info.Button.Classes.AddRange(new string[] { "Bar", placement == HorizontalAlignment.Right ? "Hub" : "Mid" });
                AddHubToPanelAndList(ref info, placement);
                Parent.baseGrid.Children.Add(hubWindow);
            }

            public void MoveHub(ref DesktopHubInfo info, HorizontalAlignment newPlacement, int newIndex = -1)
            {
                RemoveHubFromPanelAndList(ref info);
                AddHubToPanelAndList(ref info, newPlacement, newIndex);
                info.CenterHubHorizontalSubscription?.Dispose();
                var hubWindow = info.InternalHubInformation != null ? info.InternalHubInformation.Value.Window : info.RuntimeHubInformation.Value.Window;
                if (newPlacement == HorizontalAlignment.Center)
                    info.CenterHubHorizontalSubscription = hubWindow.Bind(HorizontalAlignmentProperty, Parent.HorizontalBarSubjectAlignment);
                else
                    hubWindow.HorizontalAlignment = newPlacement;
            }

            private void AddHubToPanelAndList(ref DesktopHubInfo desktopHubInfo, HorizontalAlignment placement, int index = -1)
            {
                if(index == -1)
                {
                    switch (placement)
                    {
                        case HorizontalAlignment.Center:
                            CenterDesktopHubs.Add(desktopHubInfo);
                            Parent.centerStack.Children.Add(desktopHubInfo.Button);
                            break;
                        case HorizontalAlignment.Right:
                            RightDesktopHubs.Add(desktopHubInfo);
                            Parent.rightStack.Children.Add(desktopHubInfo.Button);
                            break;
                        case HorizontalAlignment.Left:
                            LeftDesktopHubs.Add(desktopHubInfo);
                            Parent.leftStack.Children.Add(desktopHubInfo.Button);
                            break;
                    }
                }
                else
                {
                    switch (placement)
                    {
                        case HorizontalAlignment.Center:
                            CenterDesktopHubs.Insert(index,desktopHubInfo);
                            Parent.centerStack.Children.Insert(index+1, desktopHubInfo.Button);
                            break;
                        case HorizontalAlignment.Right:
                            RightDesktopHubs.Insert(index, desktopHubInfo);
                            Parent.rightStack.Children.Insert(index + 1, desktopHubInfo.Button);
                            break;
                        case HorizontalAlignment.Left:
                            LeftDesktopHubs.Insert(index, desktopHubInfo);
                            Parent.leftStack.Children.Insert(index + 1, desktopHubInfo.Button);
                            break;
                    }
                }
            }

            private void RemoveHubFromPanelAndList(ref DesktopHubInfo desktopHubInfo)
            {
                if (LeftDesktopHubs.Contains(desktopHubInfo))
                {
                    LeftDesktopHubs.Remove(desktopHubInfo);
                    Parent.leftStack.Children.Remove(desktopHubInfo.Button);
                }
                else if (CenterDesktopHubs.Contains(desktopHubInfo))
                {
                    CenterDesktopHubs.Remove(desktopHubInfo);
                    Parent.centerStack.Children.Remove(desktopHubInfo.Button);
                }
                else
                {
                    RightDesktopHubs.Remove(desktopHubInfo);
                    Parent.rightStack.Children.Remove(desktopHubInfo.Button);
                }
            }

            public void DeattachHub(ref DesktopHubInfo info)
            {
                var hubWindow = info.InternalHubInformation != null ? info.InternalHubInformation.Value.Window : info.RuntimeHubInformation.Value.Window;
                RemoveHubFromPanelAndList(ref info);
                info.CenterHubHorizontalSubscription?.Dispose();
                Parent.baseGrid.Children.Remove(hubWindow);
                if(info.RuntimeHubInformation != null)
                    JVOS.ApplicationAPI.Hub.HubManager.UnloadHub(info.RuntimeHubInformation.Value);
            }
        }
    }
}
