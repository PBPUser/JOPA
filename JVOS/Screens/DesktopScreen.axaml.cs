using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using JVOS.ApplicationAPI;
using JVOS.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JVOS.Screens
{
    public partial class DesktopScreen : ScreenBase, IWindowSpace
    {
        public DesktopScreen()
        {
            InitializeComponent();
            AttachHandlers();
            AddWindowHub(new DesktopHub());
            OHubXTransition = new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseOut() };
            OHubYTransition = new DoubleTransition() { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseOut() };
            CHubXTransition = new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseIn() };
            CHubYTransition = new DoubleTransition() { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new CubicEaseIn() };
            LanguageSwitcher = (LanguageSwitcherHub)AttachHub(new LanguageSwitcherHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            Clock = (ClockHub)AttachHub(new ClockHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            Panorama = (PanoramaHub)AttachHub(new PanoramaHub(), VerticalAlignment.Stretch, HorizontalAlignment.Left, Orientation.Horizontal);
            LanguageSwitcher = (LanguageSwitcherHub)AttachHub(new LanguageSwitcherHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            Start = (StartHub)AttachHub(new StartHub(), VerticalAlignment.Bottom, HorizontalAlignment.Center, Orientation.Vertical);
            Clock._setLeft.Click += (a, b) => SetBarAlign(HorizontalAlignment.Left);
            Clock._setCenter.Click += (a, b) => SetBarAlign(HorizontalAlignment.Center);
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

        HorizontalAlignment BarAlignment = HorizontalAlignment.Center;

        private void SetBarAlign(HorizontalAlignment horizontalAlignment)
        {
            if (horizontalAlignment == HorizontalAlignment.Stretch || horizontalAlignment == HorizontalAlignment.Right || horizontalAlignment == BarAlignment) return;
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
                var client = new System.Net.Http.HttpClient();
                var task = client.GetStringAsync("https://random-word-api.herokuapp.com/word");
                task.Wait(3000);
                string str = task.Result;
                if (String.IsNullOrEmpty(str))
                    str = "mjssjngno";
                else
                    str = str.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("i", "j");
                Dispatcher.UIThread.Invoke(() => @event.Invoke(null, (string)str));
            }).Start();
        }

        private void AttachHandlers()
        {
            widgetsBtn.Click += (a, b) =>
            {
                ToggleHub(Panorama);
            };
            clockBtn.Click += (a, b) =>
            {
                ToggleHub(Clock);
            };
            langBtn.Click += (a, b) =>
            {
                ToggleHub(LanguageSwitcher);
            };
            startBtn.Click += (a, b) =>
            {
                ToggleHub(Start);
            };
        }

        private int TopHub = 3;

        public void ToggleHub(IHub hub, bool? forced = null)
        {
            if (forced != null)
                hub.IsOpen = forced.Value;
            else
                hub.IsOpen = !hub.IsOpen;
            if (hub.IsOpen)
            {
                (hub.RenderTransform as TranslateTransform).Transitions = OHubTransitions;
                hub.ZIndex = TopHub++;
                foreach (IHub xHub in Hubs)
                {
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

        public void CloseWindow(IJWindowFrame window)
        {
            _currentWindowSpace?.CloseWindow(window);
        }

        private bool isBackgroundAnimationPlaying = false;

        private void SetBackground(Bitmap image,bool animate)
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

        public void OpenWindow(IJWindowFrame window)
        {
            _currentWindowSpace?.OpenWindow(window);
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

        private List<IHub> Hubs = new List<IHub>();

        public IHub AttachHub(IHub hub, VerticalAlignment vAlign, HorizontalAlignment hAlign, Orientation AnimationOrientation = Orientation.Vertical)
        {
            hub.VerticalAlignment = vAlign;
            hub.HorizontalAlignment = hAlign;
            hub.Margin = new Avalonia.Thickness(0, 0, 0, 88);
            hub.AnimationOrientation = AnimationOrientation;
            TranslateTransform Transform = new TranslateTransform();
            hub.RenderTransform = Transform;
            baseGrid.Children.Add(hub);
            hub.Loaded += (a, b) =>
            {
                if(AnimationOrientation == Orientation.Horizontal)
                {
                    if(hAlign == HorizontalAlignment.Left)
                        Transform.X = -hub.Bounds.Width;
                    else
                        Transform.X = hub.Bounds.Width;
                }
                else
                {
                    if (vAlign == VerticalAlignment.Top)
                        Transform.Y = -hub.Bounds.Height;
                    else
                        Transform.Y = hub.Bounds.Height + 88;
                }
                hub.SizeChanged += (a, b) =>
                {
                    Transform.Transitions = CHubTransitions;
                    if (b.PreviousSize.Width != b.NewSize.Width && !hub.IsOpen && Orientation.Horizontal == AnimationOrientation)
                    {
                        if (hAlign == HorizontalAlignment.Left)
                            Transform.X = -hub.Bounds.Width;
                        else
                            Transform.X = hub.Bounds.Width;
                    }
                    else if (b.PreviousSize.Height != b.NewSize.Height && !hub.IsOpen && Orientation.Vertical == AnimationOrientation)
                    {
                        if (vAlign == VerticalAlignment.Top)
                            Transform.Y = -hub.Bounds.Height;
                        else
                            Transform.Y = hub.Bounds.Height + 88;
                    }
                };
            };
            Hubs.Add(hub);
            return hub;
        }
    }
}
