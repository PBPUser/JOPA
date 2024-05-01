using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using JVOS.Screens;
using JVOS.ApplicationAPI;
using ReactiveUI;
using Avalonia.Controls.Chrome;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using JVOS.Controls;
using Avalonia.Animation.Easings;
using System.Linq.Expressions;

namespace JVOS.Views
{
    public partial class MainView : UserControl, IWindowSpace
    {
        private int ScreenIndex = 0;
        private int IDIndex = 0;
        public static bool IsMobile = false;
        public static MainView? GLOBAL;
        public static List<DesktopScreen>? Desktops;
        public static LoadingScreen? Loading;
        public static LoginScreen? Login;
        public static DesktopScreen? CurrentDesktop;
        public MainView()
        {
            GLOBAL = this;
            Loading = new LoadingScreen();
            Login = new LoginScreen();
            WindowManager.Initialize();
            JVOSResourceBunch.SetResourceBunchToColorScheme();
            WindowManager.SetCurrentWindowSpace(this);
            int sleep = 5000;
#if DEBUG
            sleep /= 20;
#endif
            LanguageWorker.Test();
            UserOptions.Test();
            ColorScheme.ApplyScheme(ColorScheme.Current, false, true, false);
            InitializeComponent();
            this.SizeChanged += (a, b) =>
            {
                
                var isMobile = Math.Min(b.NewSize.Width, b.NewSize.Height) < 600;
                if(IsMobile != isMobile)
                {
                    IsMobile = isMobile;
                    if(CurrentScreen != null)
                        CurrentScreen.MobileModeStateSwitch(IsMobile);
                }
                JWindowGridController.UpdateSubjectsHWhenDesktopResized(b.NewSize.Width, b.NewSize.Height);
            };
            Loaded += (a, b) =>
            {
                JWindowGridController.UpdateSubjectsHWhenDesktopResized(Bounds.Width, Bounds.Height);
                switchScreen(Loading);
                new Thread(() =>
                {
                    Thread.Sleep(sleep);
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        SwitchScreen(Login);
                    });
                }).Start();
                InitializeAdaptiveController();
            };
        }

        private ScreenBase? CurrentScreen;
        
        private Overlays.XboxAccessController AdaptiveController = new() { ClipToBounds = false, Margin = new Thickness(0, 100), MinWidth = 1024, MaxWidth = 1024, MinHeight = 384, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
        private TranslateTransform AdaptiveControllerTranslate = new();
        public bool IsAdaptiveControllerShown = false;

        public void SwitchAdaptiveControllerState(bool? state = null)
        {
            if (state == null)
                state = !IsAdaptiveControllerShown;
            IsAdaptiveControllerShown = state == true;
            AdaptiveControllerTranslate.Y = state == true ? 0 : AdaptiveController.Bounds.Height + 100;
        }

        public void InitializeAdaptiveController()
        {
            basegrid.Children.Add(AdaptiveController);
            AdaptiveController.ZIndex = int.MaxValue;
            AdaptiveController.RenderTransform = AdaptiveControllerTranslate;
            AdaptiveController.Loaded += (a, b) =>
            {
                AdaptiveControllerTranslate.Y = AdaptiveController.Bounds.Height + 100;
                AdaptiveControllerTranslate.Transitions = new Transitions() { new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(333), Easing = new QuadraticEaseInOut(), Property = TranslateTransform.YProperty } };
            };
        }

        public static void SwitchScreen(ScreenBase newScreen)
        {
            GLOBAL?.switchScreen(newScreen);
            newScreen.ScreenShown();
        }
        private void switchScreen(ScreenBase newScreen)
        {
            if(!basegrid.Children.Contains(newScreen))
                basegrid.Children.Add(newScreen);
            if (CurrentScreen != null)
                CurrentScreen.ScreenOverlap();
            newScreen.ScreenShown();
            newScreen.MobileModeStateSwitch(IsMobile);
            CurrentScreen = newScreen;
            CurrentScreen.ZIndex = ScreenIndex++;
        }
        public void OpenOOBE()
        {
            /*OOBEScreen oobe = new OOBEScreen();
            oobe.desk = desktop;
            basegrid.Children.Add(oobe);
            Storyboard sb = new Storyboard();
            var anim1 = new DoubleAnimation { To = ActualHeight > 1280 ? ActualHeight : 1280, Duration = TimeSpan.FromMilliseconds(2000), EasingFunction = new BackEase() };
            var anim2 = new DoubleAnimation { To = ActualWidth > 720 ? ActualWidth : 720, Duration = TimeSpan.FromMilliseconds(2000), EasingFunction = new BackEase() };
            sb.Children.Add(anim1);
            sb.Children.Add(anim2);
            Storyboard.SetTarget(sb, this);
            Storyboard.SetTargetProperty(anim1, new PropertyPath(Window.WidthProperty));
            Storyboard.SetTargetProperty(anim2, new PropertyPath(Window.HeightProperty));
            sb.Begin();
            oobe.ShowLicense();*/
        }

        public void OpenPrepare()
        {
            /*OOBEScreen oobe = new OOBEScreen();
            oobe.desk = desktop;
            basegrid.Children.Add(oobe);
            Storyboard sb = new Storyboard();
            var anim1 = new DoubleAnimation { To = ActualHeight > 1280 ? ActualHeight : 1280, Duration = TimeSpan.FromMilliseconds(500), EasingFunction = new QuadraticEase() };
            var anim2 = new DoubleAnimation { To = ActualWidth > 720 ? ActualWidth : 720, Duration = TimeSpan.FromMilliseconds(500), EasingFunction = new QuadraticEase() };
            sb.Children.Add(anim1);
            sb.Children.Add(anim2);
            Storyboard.SetTarget(sb, this);
            Storyboard.SetTargetProperty(anim1, new PropertyPath(Window.WidthProperty));
            Storyboard.SetTargetProperty(anim2, new PropertyPath(Window.HeightProperty));
            sb.Begin();
            oobe.StartOOBEFinal();*/
        }

        private void OnAfterBurner()
        {
            isAfterburnerRunning = true;
        }

        private void OffAfterBurner()
        {
            isAfterburnerRunning = false;
        }

        private bool isAfterburnerRunning = false;

        public void ForceOnAfterBurner()
        {
            if (isAfterburnerRunning)
                return;
            OnAfterBurner();
        }

        public void ToggleAfterBurner()
        {
            if (isAfterburnerRunning)
                OffAfterBurner();
            else
                OnAfterBurner();
        }

        public async void L(int time)
        {
            await Task.Delay(TimeSpan.FromSeconds(new Random().Next(time / 2) + time / 2));
            //Dispatcher.Invoke(() => {
            //var loginHideAnimation = new DoubleAnimation { To = this.ActualWidth / 12.5, Duration = TimeSpan.FromSeconds(3), EasingFunction = new CubicEase() }; ;
            //var desktopFadeHideAnimation = new DoubleAnimation { To = 0, Duration = TimeSpan.FromSeconds(1), EasingFunction = new SineEase() };
            //loginHideAnimation.Completed += (s, e) => {
            //    this.login.Visibility = Visibility.Collapsed;
            //    //this.desktop.fade.BeginAnimation(OpacityProperty, desktopFadeHideAnimation);
            //};
            //desktopFadeHideAnimation.Completed += (s, e) =>
            //{
            //this.desktop.fade.Visibility = Visibility.Collapsed;
            //};

            //((ScaleTransform)login.RenderTransform).BeginAnimation(ScaleTransform.ScaleXProperty, loginHideAnimation);
            //((ScaleTransform)login.RenderTransform).BeginAnimation(ScaleTransform.ScaleYProperty, loginHideAnimation);
            //});
        }

        string installPath, fromPath;

        public void RequestInstallApp(string from, string to)
        {
            fromPath = from;
            installPath = to;
            //cancel_appinstall.IsEnabled = ok_appinstall.IsEnabled = true;
            //appInstall.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //cancel_appinstall.IsEnabled = ok_appinstall.IsEnabled = false;
            //appInstall.Visibility = Visibility.Collapsed;
        }

        private void PanelButton_Click(object sender, RoutedEventArgs e)
        {
            //if (JWindow.TopJWin != null && JWindow.TopJWin.@base.Child is UserControlWWE wwe)
            //.OnBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Applications.Insraller.CopyFilesRecursively(fromPath, installPath);
            //MessageBox.Show(Directory.GetFiles(fromPath).Count().ToString() + "-" + Directory.GetFiles(installPath).Count().ToString());
            //App.WebAppsManager.LoadApp(installPath);
            //UsersManager.SendNotification(new JVOS.Notification() { Program = "Package Installer", Text = "Check it out", Title = App.WebAppsManager.Applications.Last().Title + " was installed.", Buttons = new System.Collections.ObjectModel.ObservableCollection<NotifyButton>(new NotifyButton[] { new NotifyButton() { Title = "Run", Command = App.WebAppsManager.Applications.Last().AppPackageName } }) });
            //appInstall.Visibility = Visibility.Collapsed;
            //cancel_appinstall.IsEnabled = ok_appinstall.IsEnabled = false;
        }

        public void OpenWindow(IJWindowFrame jWindow)
        {
            jWindow.AllowMinimize = false;
            this.basegrid.Children.Add(jWindow as Control);
            (jWindow as Control).ZIndex = ZIndex++;
            //jWindow.ZIndex = ScreenIndex++;
            jWindow.ID = IDIndex++;
            jWindow.WindowSpace = this;
        }
        public void CloseWindow(IJWindowFrame jWindow)
        {
            jWindow.Close(() =>
            {
                basegrid.Children.Remove(jWindow as Control);
            });
        }

        public void BringToFront(IJWindowFrame window)
        {

        }

        public void CloseAllHubs()
        {
            throw new NotImplementedException();
        }
    }
}
