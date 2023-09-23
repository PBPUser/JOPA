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
            HubXTransition = new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new SineEaseIn() };
            HubYTransition = new DoubleTransition() { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(500), Easing = new SineEaseIn() };
            LanguageSwitcher = (LanguageSwitcherHub)AttachHub(new LanguageSwitcherHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            Clock = (ClockHub)AttachHub(new ClockHub(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Orientation.Vertical);
            Panorama = (PanoramaHub)AttachHub(new PanoramaHub(), VerticalAlignment.Stretch, HorizontalAlignment.Left, Orientation.Horizontal);
        }

        DoubleTransition? PreviousImageTransition;
        Image? PreviousImage;
        TranslateTransform? PreviousImageTransform;
        DoubleTransition? HubXTransition;
        DoubleTransition? HubYTransition;

        LanguageSwitcherHub LanguageSwitcher;
        PanoramaHub Panorama;
        ClockHub Clock;

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
        }

        public void ToggleHub(IHub hub, bool? forced = null)
        {
            if (forced != null)
                hub.IsOpen = forced.Value;
            else
                hub.IsOpen = !hub.IsOpen;
            if (hub.IsOpen)
            {
                if (hub.AnimationOrientation == Orientation.Horizontal)
                    (hub.RenderTransform as TranslateTransform).X = 0;
                else
                    (hub.RenderTransform as TranslateTransform).Y = 0;
            }
            else
            {
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
                    Transform.Transitions = new Transitions { HubXTransition };
                }
                else
                {
                    if (vAlign == VerticalAlignment.Top)
                        Transform.Y = -hub.Bounds.Height;
                    else
                        Transform.Y = hub.Bounds.Height + 88;
                    Transform.Transitions = new Transitions { HubYTransition };
                }
            };
            return hub;
        }
    }
}
