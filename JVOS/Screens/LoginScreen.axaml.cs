using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using JVOS.EmbededWindows;
using JVOS.Hubs;
using JVOS.Views;
using ReactiveUI;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;

namespace JVOS.Screens
{
    public partial class LoginScreen : ScreenBase
    {
        public LoginScreen()
        {
            InitializeComponent();
            InitializeLockscreen();
            InitializeLoginscreen();
            InitializeUserSessions();
        }

        ScaleTransform? LoginTranslate;
        ScaleTransform? ImageTranslate;
        TranslateTransform? LockscreenTranslate;
        TranslateTransform? HubTranslate;
        LanguageSwitcherHub? LanguageHub;
        IHub? CurrentOpenedHub;

        public void OpenHub(IHub Hub)
        {
            if (!(Hub is UserControl))
                return;
            if (CurrentOpenedHub != null)
                return;
            CurrentOpenedHub = Hub;
            menus.IsVisible = true;
            menuChildHoster.Child = ((UserControl)CurrentOpenedHub);
            HubTranslate.Y = menuChildHoster.Bounds.Height + 8;
            HubTranslate.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(250), Property = TranslateTransform.YProperty });
            HubTranslate.Y = 0;
            CurrentOpenedHub.OnOpened(EventArgs.Empty);
            CurrentOpenedHub.Closed += (a, b) =>
            {
                if (b == IHub.CloseReason.CloseReason)
                    CloseHub();
            };
        }

        public void CloseHub()
        {
            if (CurrentOpenedHub == null)
                return;
            CurrentOpenedHub.OnClosed(IHub.CloseReason.Hide);
            HubTranslate.Y = menuChildHoster.Bounds.Height + 8;
            new Thread(() =>
            {
                Thread.Sleep(250);
                Dispatcher.UIThread.Invoke(() =>
                {
                    CurrentOpenedHub = null;
                    menus.IsVisible = false;
                    HubTranslate.Transitions.Clear();
                });
            }).Start();
        }

        private void InitializeLockscreen()
        {
            double startY = 0;
            bool isPoinerPressed = false;
            LockscreenTranslate = new TranslateTransform();
            lockScreenJV.RenderTransform = LockscreenTranslate;
            LockscreenTranslate.Transitions = new Transitions();
            lock_screen.Transitions = new Transitions();
            borderFront.PointerMoved += (a, b) =>
            {
                if (!isPoinerPressed)
                    return;
                LockscreenTranslate.Y = Math.Min(0,b.GetPosition(this).Y - startY);
                lock_screen.Opacity = (this.Bounds.Height + LockscreenTranslate.Y) / this.Bounds.Height;
                //lock_screen.Opacity = LockscreenTranslate.Y / this.Height;
            };
            borderFront.PointerPressed += (a, b) =>
            {
                isPoinerPressed = true;
                LockscreenTranslate.Transitions.Clear();
                lock_screen.Transitions.Clear();
                startY = b.GetPosition(this).Y;
            };
            borderFront.PointerReleased += (a, b) =>
            {
                isPoinerPressed = false;
                if(LockscreenTranslate.Y < -this.Bounds.Height / 2)
                {
                    SetLockscreenState(false);
                }
                else
                {
                    LockscreenTranslate.Transitions.Clear();
                    LockscreenTranslate.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(-LockscreenTranslate.Y), Property = TranslateTransform.YProperty });
                    lock_screen.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(-LockscreenTranslate.Y), Property = OpacityProperty });
                    LockscreenTranslate.Y = 0;
                    lock_screen.Opacity = 1;
                }
                
            };
        }

        private void InitializeLoginscreen()
        {
            LanguageHub = new LanguageSwitcherHub();
            LoginTranslate = new ScaleTransform() { Transitions = new Transitions(), ScaleX = 0.75, ScaleY = 0.75 };
            ImageTranslate = new ScaleTransform() { Transitions = new Transitions(), ScaleX = 1, ScaleY = 1 };
            password_box.Watermark = "Password";
            HubTranslate = new TranslateTransform() { Transitions = new Transitions() };
            menuChildHoster.RenderTransform = HubTranslate;
            subsign_page.Transitions = new Transitions();
            LoginTranslate.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(200), Property = ScaleTransform.ScaleXProperty });
            LoginTranslate.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(200), Property = ScaleTransform.ScaleYProperty });
            ImageTranslate.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(500), Property = ScaleTransform.ScaleXProperty });
            ImageTranslate.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(500), Property = ScaleTransform.ScaleYProperty });
            subsign_page.Opacity = 0;
            sign_page.RenderTransform = LoginTranslate;
            loginImage.RenderTransform = ImageTranslate;
            subsign_page.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(200), Property = OpacityProperty });
            SizeChanged += (a, b) =>
            {
                if (!b.HeightChanged && !b.WidthChanged)
                    return;
                bool isMobileView = Bounds.Width < Bounds.Height;
                SetMobileView(isMobileView);
            };
            bool isMobileView = Bounds.Width < Bounds.Height;
            SetMobileView(isMobileView);
            menuBorder.Transitions = new Transitions();
            menuBorder.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(500), Property = WidthProperty });
            this.closeMenuBorder.PointerReleased += (a, b) =>
            {
                CloseHub();
            };
            this.languageSwitchBtn.Click += (a, b) =>
            {
                OpenHub(LanguageHub);
            };
            this.easeOfAccessBtn.Click += (a, b) =>
            {
                WindowManager.OpenInJWindow(new EaseOfAccess());
            };
            loginWithoutPasswordBtn.Click += (a, b) =>
            {
                var session = UserSession.CreateOrGetSession(UserOptions.Current);
                session.Login();
            };
        }

        private void InitializeUserSessions()
        {
            UserOptions.SetLockscreenUserOptions(UserOptions.Users.First());
            foreach(UserOptions u in UserOptions.Users)
            {
                Button b = new Button() { Content = u.Username, Height = 64 };
                this.usersStack.Children.Add(b);
                b.Click += (a, b) =>
                {
                    UserOptions.SetLockscreenUserOptions(u);
                };
            }
            UserSession.UserLogin += (a, b) =>
            {
                MainView.SwitchScreen(b.DesktopScreen);
            };
            UserSession.UserLogout += (a, b) =>
            {
                MainView.SwitchScreen(this);
            };
        }

        private bool IsLockscreenShown = true;
        private bool? IsMobileView = null;

        private void SetMobileView(bool enable)
        {
            if (IsMobileView == enable)
                return;
            IsMobileView = enable;
            userSwitchBtn.IsVisible = enable;
            usersStack.IsVisible = !enable;
            menuChildHoster.HorizontalAlignment = enable ? Avalonia.Layout.HorizontalAlignment.Stretch : Avalonia.Layout.HorizontalAlignment.Right;
            menuBorder.HorizontalAlignment = enable ? Avalonia.Layout.HorizontalAlignment.Center : Avalonia.Layout.HorizontalAlignment.Right;
        }

        private void SetLockscreenState(bool visible)
        {
            if (IsLockscreenShown == visible)
                return;
            IsLockscreenShown = visible;
            borderFront.IsVisible = visible;
            LockscreenTranslate.Transitions.Clear();
            LockscreenTranslate.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(200), Property = TranslateTransform.YProperty });
            lock_screen.Transitions.Add(new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(200), Property = OpacityProperty });
            LockscreenTranslate.Y = visible ? 0 : -this.Bounds.Height;
            lock_screen.Opacity = visible ? 1 : 0;
            subsign_page.Opacity = visible ? 0 : 1;
            LoginTranslate.ScaleX = visible ? 0.75 : 1;
            LoginTranslate.ScaleY = visible ? 0.75 : 1;
            ImageTranslate.ScaleX = visible ? 1 : 1.25;
            ImageTranslate.ScaleY = visible ? 1 : 1.25;
            if (!visible)
            {
                new Thread(() =>
                {
                    Thread.Sleep(30000);
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        SetLockscreenState(true);
                    });
                }).Start();
            }

        }
    }
}
