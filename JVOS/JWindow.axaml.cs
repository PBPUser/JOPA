using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using JVOS.Controls;
using System;
using System.Threading;
using System.Timers;
using JVOS.ApplicationAPI;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JVOS
{
    public partial class JWindow : UserControl, IDisposable, IJWindowFrame
    {
        static JWindow()
        {
            TitleProperty = AvaloniaProperty.RegisterAttached<JWindow, UserControl, string>("Title");
            IconProperty = AvaloniaProperty.RegisterAttached<JWindow, UserControl, Image>("Icon");
        }

        public JWindow()
        {
            InitializeComponent();
            WindowTransformGroup = new TransformGroup();
            WindowPositionTransform = new TranslateTransform();
            WindowTransformGroup.Children.Add(WindowPositionTransform);
            WindowGridPositionTransform = new TranslateTransform();
            this.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            this.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            this.RenderTransform = WindowTransformGroup;
            Transitions = new Transitions();
            InitializeAnimationTransforms();
            Loaded += (a, b) =>
            {
                PlayOpenAnimation();
            };
            mxButton.PointerReleased += (a, b) =>
            {
                Maximize();
            };
            clButton.PointerReleased += (a, b) =>
            {
                if (WindowSpace == null)
                    Close();
                else
                    WindowSpace.CloseWindow(this);
            };
        }

        const int AnimationLength = 500;

        public event EventHandler<EventArgs>? WindowClosing = null;
        public event EventHandler<WindowState>? WindowStateChanged = null;
        public IWindowSpace? WindowSpace {
            get => _iWindowSpace;
            set => _iWindowSpace = value;
        }
        
        public int ID
        {
            get => _id;
            set => _id = value;
        }

        private int _id = -1;
        private IWindowSpace? _iWindowSpace;

        private TransformGroup WindowTransformGroup;
        private TransformGroup WindowAnimationGroup;
        private RotateTransform WindowAnimationsRotateTransform;
        private ScaleTransform WindowAnimationsScaleTransform;
        private SkewTransform WindowAnimationsSkewTransform;
        private TranslateTransform WindowAnimationsTranslateTransform;
        private DoubleTransition WindowOpacityTransition;
        public TranslateTransform WindowPositionTransform;
        public TranslateTransform WindowGridPositionTransform;

        private (IDisposable, IDisposable?)?
            HorizontalBindings = null,
            VerticalBindings = null;

        private (IDisposable, IDisposable)?
            ChildSubscription = null;

        private bool _allowMinimize = true;
        private IJWindow? JWindowChild = null;

        private double
            WindowWidth = 640,
            WindowHeight = 400;

        private PositionState PreMaximizedState = PositionState.Normal;
        public PositionState StatePosition = PositionState.Normal;
        public WindowState StateWindow = WindowState.Normal;

        public static readonly AttachedProperty<string> TitleProperty;
        public static readonly AttachedProperty<Image> IconProperty;

        public enum PositionState { Normal, Maximized, LeftSide, RightSide, TopSide, BottomSide, TopLeft, BottomLeft, TopRight, BottomRight }
        public enum WindowState { Normal, Minimized };
        public bool AllowMinimize
        {
            get => _allowMinimize;
            set => _allowMinimize = value;
        }

        private bool _isAnimationsActive = false;
        private bool isSubscribed = false;

        private void SubscribeTitleIcon()
        {
            if (isSubscribed)
                return;
            isSubscribed = true;
            title.Bind(TextBlock.TextProperty, JWindowChild.Title);
        }

        private void SetAnimationState(bool Active, double? timeL = null)
        {
            if (_isAnimationsActive == Active)
                return;
            _isAnimationsActive = Active;
            if (timeL == null)
                timeL = AnimationLength;
            if (Active)
            {
                var time = TimeSpan.FromMilliseconds(timeL.Value);
                WindowTransformGroup.Children.Add(WindowAnimationGroup);
                Transitions.Add(WindowOpacityTransition);
                WindowAnimationsScaleTransform.Transitions = new Avalonia.Animation.Transitions() { new DoubleTransition { Duration = time, Property = ScaleTransform.ScaleXProperty }, new DoubleTransition { Duration = time, Property = ScaleTransform.ScaleYProperty } };
                WindowAnimationsSkewTransform.Transitions = new Avalonia.Animation.Transitions() { new DoubleTransition { Duration = time, Property = SkewTransform.AngleXProperty }, new DoubleTransition { Duration = time, Property = SkewTransform.AngleYProperty } };
                WindowAnimationsTranslateTransform.Transitions = new Avalonia.Animation.Transitions() { new DoubleTransition { Duration = time, Property = TranslateTransform.XProperty }, new DoubleTransition { Duration = time, Property = TranslateTransform.YProperty } };
                WindowAnimationsRotateTransform.Transitions = new Avalonia.Animation.Transitions() { new DoubleTransition { Duration = time, Property = RotateTransform.AngleProperty }, new DoubleTransition { Duration = time, Property = RotateTransform.CenterXProperty }, new DoubleTransition { Duration = time, Property = RotateTransform.CenterYProperty } };
            }
            else
            {
                WindowTransformGroup.Children.Remove(WindowAnimationGroup);
                Transitions.Remove(WindowOpacityTransition);
                WindowAnimationsScaleTransform.Transitions = null;
                WindowAnimationsSkewTransform.Transitions = null;
                WindowAnimationsTranslateTransform.Transitions = null;
                WindowAnimationsRotateTransform.Transitions = null;
            }
        }

        private void InitializeAnimationTransforms()
        {
            WindowOpacityTransition = new DoubleTransition() { Duration = TimeSpan.FromMilliseconds(AnimationLength), Property = OpacityProperty };
            WindowAnimationsScaleTransform = new ScaleTransform();
            WindowAnimationsSkewTransform = new SkewTransform();
            WindowAnimationsTranslateTransform = new TranslateTransform();
            WindowAnimationsRotateTransform = new RotateTransform();
            WindowAnimationGroup = new TransformGroup() { Children = { WindowAnimationsTranslateTransform, WindowAnimationsScaleTransform, WindowAnimationsSkewTransform, WindowAnimationsRotateTransform } };
        }

        public void SwitchPositionState(PositionState state, bool cursorCaptured = false)
        {
            if (state == StatePosition)
                return;
            App.SendNotification($"{StatePosition} -> {state}", 3000, true);
            if (HorizontalBindings != null)
                JWindowGridController.Unsubscribe(HorizontalBindings.Value);
            if (VerticalBindings != null)
                JWindowGridController.Unsubscribe(VerticalBindings.Value);
            if (PositionState.Maximized == state)
                PreMaximizedState = StatePosition;
            StatePosition = state;
            resizeLS.IsVisible = HResizeAllowed(HorizontalAlignment.Left);
            resizeRS.IsVisible = HResizeAllowed(HorizontalAlignment.Right);
            resizeST.IsVisible = VResizeAllowed(VerticalAlignment.Top);
            resizeSB.IsVisible = VResizeAllowed(VerticalAlignment.Bottom);
            resizeLB.IsVisible = resizeLS.IsVisible && resizeSB.IsVisible;
            resizeRB.IsVisible = resizeRS.IsVisible && resizeSB.IsVisible;
            resizeLT.IsVisible = resizeLS.IsVisible && resizeST.IsVisible;
            resizeRT.IsVisible = resizeRS.IsVisible && resizeST.IsVisible;
            App.SendNotification($"{StatePosition} -> {HResizeAllowed(HorizontalAlignment.Left)}");
            switch (state) {
                case PositionState.Normal:
                    SetGridMode(false);
                    HorizontalBindings = null;
                    VerticalBindings = null;
                    return;
                case PositionState.Maximized:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Stretch);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Stretch);
                    return;
                case PositionState.LeftSide:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Left);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Stretch);
                    return;
                case PositionState.RightSide:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Right);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Stretch);
                    return;
                case PositionState.TopSide:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Stretch);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Top);
                    return;
                case PositionState.BottomSide:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Stretch);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Bottom);
                    return;
                case PositionState.TopLeft:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Left);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Top);
                    return;
                case PositionState.TopRight:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Right);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Top);
                    return;
                case PositionState.BottomLeft:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Left);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Bottom);
                    return;
                case PositionState.BottomRight:
                    SetGridMode(true);
                    HorizontalBindings = JWindowGridController.SubscribeToHorizontal(this, HorizontalAlignment.Right);
                    VerticalBindings   = JWindowGridController.SubscribeToVertical(this, VerticalAlignment.Bottom);
                    return;
            }
        }

        public bool HResizeAllowed(HorizontalAlignment horizontalAlignment)
        {
            if (horizontalAlignment == HorizontalAlignment.Left)
                return StatePosition switch
                {
                    PositionState.TopRight => true,
                    PositionState.BottomRight => true,
                    PositionState.RightSide => true,
                    PositionState.Normal => true,
                    _ => false
                };
            if (horizontalAlignment == HorizontalAlignment.Right)
                return StatePosition switch
                {
                    PositionState.TopLeft => true,
                    PositionState.BottomLeft => true,
                    PositionState.LeftSide => true,
                    PositionState.Normal => true,
                    _ => false
                };
            return false;
        }
        public bool VResizeAllowed(VerticalAlignment verticalAlignment)
        {
            if (verticalAlignment == VerticalAlignment.Top)
                return StatePosition switch
                {
                    PositionState.BottomLeft => true,
                    PositionState.BottomRight => true,
                    PositionState.BottomSide => true,
                    PositionState.Normal => true,
                    _ => false
                };
            if (verticalAlignment == VerticalAlignment.Bottom)
                return StatePosition switch
                {
                    PositionState.TopLeft => true,
                    PositionState.TopRight => true,
                    PositionState.TopSide => true,
                    PositionState.Normal => true,
                    _ => false
                };
            return false;
        }

        public bool GridModeEnabled = false;

        private void SetGridMode(bool enable)
        {
            if(GridModeEnabled == enable) return;
            GridModeEnabled = enable;
            if (enable)
            {
                WindowWidth = Bounds.Width;
                WindowHeight = Bounds.Height;
            }
            else
            {
                Width = WindowWidth;
                Height = WindowHeight;
            }
            WindowTransformGroup.Children.Remove(enable ? WindowPositionTransform : WindowGridPositionTransform);
            WindowTransformGroup.Children.Add(enable ? WindowGridPositionTransform : WindowPositionTransform);
        }

        public void SetChild(UserControl userControl)
        {
            if (ChildSubscription != null)
            {
                ChildSubscription.Value.Item1.Dispose();
                ChildSubscription.Value.Item2.Dispose();
            }
            ChildSubscription = null;
            childHost.Children.Clear();
            if (userControl is IJWindow)
            {
                var jWnd = userControl as IJWindow;
                JWindowChild = jWnd;
                ChildSubscription = SubscribeToBindings(jWnd);
                jWnd.WhenLoaded();
            }
            this.childHost.Children.Add(userControl);
            
        }

        private (IDisposable, IDisposable) SubscribeToBindings(IJWindow jwindow)
        {
            IDisposable binding = title.Bind(TextBlock.TextProperty, jwindow.Title);
            IDisposable binding2 = title.Bind(Image.SourceProperty, jwindow.Icon);
            return (binding, binding2);
        }

        private void Restore()
        {
            if (StateWindow == WindowState.Normal)
                return;
            StateWindow = WindowState.Normal;
            OnWindowStateChanged(StateWindow);
        }
        private void Minimize()
        {
            if (StateWindow == WindowState.Minimized)
                return;
            StateWindow = WindowState.Minimized;
            OnWindowStateChanged(StateWindow);
        }

        public void Maximize()
        {
            if (StatePosition == PositionState.Maximized)
                PlayRestoreStateAnimation(() => SwitchPositionState(PositionState.Normal));
            else
                PlayMaximizeStateAnimation(() => SwitchPositionState(PositionState.Maximized));
        }

        private void OnWindowStateChanged(WindowState windowState)
        {
            if(WindowStateChanged != null)
                WindowStateChanged(this, windowState);
        }

        private void PlayOpenAnimation()
        {
            WindowAnimationsRotateTransform.Angle = 30;
            WindowAnimationsScaleTransform.ScaleX = 0.75;
            WindowAnimationsScaleTransform.ScaleY = 0.75;
            Opacity = 0;
            SetAnimationState(true);
            WindowAnimationsRotateTransform.Angle = 0;
            WindowAnimationsScaleTransform.ScaleX = 1;
            WindowAnimationsScaleTransform.ScaleY = 1;
            Opacity = 1;
        }

        private void PlayCloseAnimation(Action actionThen)
        {
            SetAnimationState(true);
            WindowAnimationsRotateTransform.Angle = 30;
            WindowAnimationsScaleTransform.ScaleX = 0.75;
            WindowAnimationsScaleTransform.ScaleY = 0.75;
            Opacity = 0;
            new Thread(() =>
            {
                Thread.Sleep(AnimationLength);
                Dispatcher.UIThread.Invoke(actionThen);
            }).Start();
        }

        private void PlayMaximizeStateAnimation(Action actionThen)
        {
            this.RenderTransformOrigin = RelativePoint.Center;
            SetAnimationState(true, 500);
            WindowAnimationsScaleTransform.ScaleX = (Parent as Control).Bounds.Width / Bounds.Width;
            WindowAnimationsScaleTransform.ScaleY = (Parent as Control).Bounds.Height / Bounds.Height;
            WindowAnimationsTranslateTransform.X = -WindowPositionTransform.X;
            WindowAnimationsTranslateTransform.Y = -WindowPositionTransform.Y;
            WindowAnimationsRotateTransform.Angle = 360;
            new Thread(() =>
            {
                Thread.Sleep(250);
                Dispatcher.UIThread.Invoke(() =>
                {
                });
                Thread.Sleep(250);
                Dispatcher.UIThread.Invoke(() =>
                {
                    SetAnimationState(false);
                    WindowAnimationsScaleTransform.ScaleX = 1;
                    WindowAnimationsScaleTransform.ScaleY = 1;
                    WindowAnimationsRotateTransform.Angle = 0;
                    WindowAnimationsTranslateTransform.X = 0;
                    WindowAnimationsTranslateTransform.Y = 0;
                    actionThen.Invoke();
                });
            }).Start();
        }

        private void PlayRestoreStateAnimation(Action actionThen)
        {
            SetAnimationState(true);
            WindowAnimationsScaleTransform.ScaleX = WindowWidth / Bounds.Width;
            WindowAnimationsScaleTransform.ScaleY = WindowHeight / Bounds.Height;
            WindowAnimationsRotateTransform.Angle = -360;

            new Thread(() =>
            {
                Thread.Sleep(AnimationLength);
                Dispatcher.UIThread.Invoke(() =>
                {
                    SetAnimationState(false);
                    WindowAnimationsScaleTransform.ScaleX = 1;
                    WindowAnimationsScaleTransform.ScaleY = 1;
                    WindowAnimationsRotateTransform.Angle = 0;
                    actionThen.Invoke();
                });
            }).Start();
        }

        public void Close(Action? actionThen = null)
        {
            PlayCloseAnimation(() =>
            {
                if (JWindowChild != null)
                    JWindowChild.Closed();
                actionThen?.Invoke();
            });
        }

        public void Dispose()
        {
            
        }
    }
}
