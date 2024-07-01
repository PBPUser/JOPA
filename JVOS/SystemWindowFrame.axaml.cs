using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using JVOS.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JVOS
{
    public partial class SystemWindowFrame : WindowFrameBase
    {
        private static TimeSpan AnimationLength = TimeSpan.FromSeconds(0.35);

        public SystemWindowFrame(WindowOpenRequest request, IWindowSpace space) : base(request, space)
        {
            InitializeComponent();
            Preload();
        }

        public SystemWindowFrame(WindowContentBase content, IWindowSpace space) : base(content, space)
        {
            InitializeComponent();
            Preload();
            Width = content.DefaultSize.Width;
            Height = content.DefaultSize.Height;
        }

        private bool
            isMinimized = false,
            isActivated = false;

        public override bool Minimized
        {
            get => isMinimized;
            set
            {
                if (isMinimized == value)
                    return;
                IsVisible = IsHitTestVisible = isMinimized = value;
            }
        }

        public override bool IsActivated
        {
            get => isActivated;
            set
            {
                if (isActivated == value)
                    return;
                isActivated = value;
                if (value)
                    Activated();
                else
                    Deactivated();
            }
        }

        private void InitializeWindowFrame()
        {
            baseGrid.Children.Add(new JWindowResizeThumb() { Height = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, ParentWindow = this, Cursor = new Cursor(StandardCursorType.TopSide) });
            baseGrid.Children.Add(new JWindowResizeThumb() { Height = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom, ParentWindow = this, Cursor = new Cursor(StandardCursorType.BottomSide) });
            baseGrid.Children.Add(new JWindowResizeThumb() { Width = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch, ParentWindow = this, Cursor = new Cursor(StandardCursorType.LeftSide) });
            baseGrid.Children.Add(new JWindowResizeThumb() { Width = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch, ParentWindow = this, Cursor = new Cursor(StandardCursorType.RightSide) });
            baseGrid.Children.Add(new JWindowResizeThumb() { Width = 8, Height = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, ParentWindow = this, Cursor = new Cursor(StandardCursorType.TopLeftCorner) });
            baseGrid.Children.Add(new JWindowResizeThumb() { Width = 8, Height = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom, ParentWindow = this, Cursor = new Cursor(StandardCursorType.BottomLeftCorner) });
            baseGrid.Children.Add(new JWindowResizeThumb() { Width = 8, Height = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, ParentWindow = this, Cursor = new Cursor(StandardCursorType.TopRightCorner) });
            baseGrid.Children.Add(new JWindowResizeThumb() { Width = 8, Height = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom, ParentWindow = this, Cursor = new Cursor(StandardCursorType.BottomRightCorner) });
        }

        private void Preload()
        {
            InitTranslate();
            InitInactive();
            InitializeWindowFrame();
            if (Communicator.IsInMobileMode())
            {
                SetFrameVisibility(false);
                ChangeState(WindowFrameState.Maximized);
            }
            CloseButtonRect.PointerReleased += (a, b) => Close();
            RestoreButtonRect.PointerReleased += (a, b) => AnimateChangeWindowState();
            MinimizeButtonRect.PointerReleased += (a, b) => ToggleVisibilityState(false);
            GodotButtonRect.PointerReleased += (a, b) => Communicator.OnWindowSwitching(this);
            Loaded += (a, b) => PlayOpenAnimation();
        }

        private void InitInactive()
        {
            InactiveBorder.IsHitTestVisible = false;
            InactiveBorder.PointerReleased += (a, b) =>
            {
                b.Handled = false;
                WindowSpace.BringToFront(this);
                
            };
            ThumbMove.DragStarted += (a, b) => WindowSpace.BringToFront(this);
        }

        public override void ToggleVisibilityState(bool? isVisible = null)
        {
            bool state = isVisible ?? !IsVisible;
            if (state == IsVisible)
                return;
            if (state == true)
                IsVisible = true;
            TransformGroup.Children.Add(AnimationsTransformGroup);
            AnimationsScale.ScaleX = AnimationsScale.ScaleY = Opacity = state ? 1 : 0.01;
            AnimationsTranslate.Y = (!state ? ((Control)Parent).Bounds.Height - (FrameState == WindowFrameState.Maximized ? 0 : WindowTranslateMove.Y) : 0) / 0.01;
            StopAnimationAfterWhile(() =>
            {
                IsVisible = state;
            });
        }

        private void PlayOpenAnimation()
        {
            TransformGroup.Children.Add(AnimationsTransformGroup);
            AnimationsScale.ScaleX = 1;
            AnimationsScale.ScaleY = 1;
            AnimationsSkew.AngleX = 0;
            AnimationsSkew.AngleY = 0;
            AnimationsRotate3D.AngleX = 0;
            AnimationsRotate3D.AngleY = 0;
            AnimationsRotate3D.AngleZ = 0;
            AnimationsTranslate.Y = 0;
            Opacity = 1;
            StopAnimationAfterWhile();
        }

        private void StopAnimationAfterWhile(Action? actionThen = null)
        {
            Task.Run(() =>
            {
                Thread.Sleep((int)AnimationLength.TotalMilliseconds);
                Dispatcher.UIThread.Invoke(() =>
                {
                    TransformGroup.Children.Remove(AnimationsTransformGroup);
                    actionThen?.Invoke();
                });
            });
        }

        private void InitTranslate()
        {
            RenderTransformOrigin = new RelativePoint(.5, .5, RelativeUnit.Relative);
            AnimationsTransformGroup.Children.Add(AnimationsTranslate);
            AnimationsTransformGroup.Children.Add(AnimationsRotate3D);
            AnimationsTransformGroup.Children.Add(AnimationsScale);
            AnimationsTransformGroup.Children.Add(AnimationsSkew);

            Opacity = 0;
            Transitions = new Transitions()
            {

            };
            Transitions.Add(new DoubleTransition() { Property = OpacityProperty, Duration = AnimationLength });
            this.RenderTransform = TransformGroup;
            ChangeWindowStateToDefault();
            ThumbMove.ParentWindow = this;
        }

        public WindowFrameState FrameState = WindowFrameState.Default;
        TransformGroup TransformGroup = new();

        ScaleTransform AnimationsScale = new()
        {
            ScaleX = 0.75,
            ScaleY = 0.75,
            
            Transitions = new Avalonia.Animation.Transitions() {
            new DoubleTransition(){ Property = ScaleTransform.ScaleXProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() },
            new DoubleTransition(){ Property = ScaleTransform.ScaleYProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() }
        }
        };
        SkewTransform AnimationsSkew = new()
        {
            Transitions = new Avalonia.Animation.Transitions() {
            new DoubleTransition(){ Property = SkewTransform.AngleXProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() },
            new DoubleTransition(){ Property = SkewTransform.AngleYProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() }
        }
        };
        Rotate3DTransform AnimationsRotate3D = new()
        {
            AngleY = new Random().Next(-30, 30),
            Transitions = new Avalonia.Animation.Transitions() {
            new DoubleTransition(){ Property = Rotate3DTransform.AngleXProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() },
            new DoubleTransition(){ Property = Rotate3DTransform.AngleYProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() },
            new DoubleTransition(){ Property = Rotate3DTransform.AngleZProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() }
        }
        };
        TranslateTransform AnimationsTranslate = new()
        {
            Y = 128,
            Transitions = new Avalonia.Animation.Transitions() {
            new DoubleTransition(){ Property = TranslateTransform.XProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() },
            new DoubleTransition(){ Property = TranslateTransform.YProperty, Duration = AnimationLength, Easing = new QuadraticEaseInOut() }
        }
        };
        TransformGroup AnimationsTransformGroup = new();

        public TranslateTransform WindowTranslateMove = new();
        public TranslateTransform WindowTranslateGrid = new();
        IDisposable? TitleBinding, IconBinding;
        (IDisposable, IDisposable?)? HBindings, VBindings;

        public override void SubscribeFrameToContent()
        {
            TitleControl.Text = WindowContent.Title;
            IconControl.Source = WindowContent.Icon;
            TitleBinding = TitleControl.Bind(TextBlock.TextProperty, WindowContent.TitleBinding);
            IconBinding = IconControl.Bind(Image.SourceProperty, WindowContent.IconBinding);
            ChildHost.Child = WindowContent;
            base.SubscribeFrameToContent();
        }

        public override void Activated()
        {
            InactiveBorder.IsHitTestVisible = InactiveBorder.IsVisible = false;
            BorderWindowActions.Classes.Remove("Inactive");
            BorderTitle.Classes.Remove("Inactive");
            BorderIcon.Classes.Remove("Inactive");
            base.Activated();
        }

        public override void Deactivated()
        {
            InactiveBorder.IsHitTestVisible = InactiveBorder.IsVisible = true;
            BorderWindowActions.Classes.Add("Inactive");
            BorderTitle.Classes.Add("Inactive");
            BorderIcon.Classes.Add("Inactive");
            base.Deactivated();
        }

        public override void SetFrameVisibility(bool visible)
        {
            TopBorder.IsVisible = visible;
        }

        public override void SetPosition(double x, double y)
        {
            WindowTranslateMove.X = x;
            WindowTranslateMove.Y = y;
        }

        public override bool GetFrameVisibility()
        {
            return TopBorder.IsVisible;
        }

        public override void ChangeState(WindowFrameState FrameState)
        {
            if (this.FrameState == FrameState)
                return;
            switch (FrameState)
            {
                case WindowFrameState.Default:
                    ChangeWindowStateToDefault();
                    return;
                case WindowFrameState.Maximized:
                    ChangeWindowStateToMaximized();
                    return;
            }
        }

        Size DefaultStateSize = new(0,0);

        private void AnimateChangeWindowState()
        {
            if(FrameState == WindowFrameState.Maximized)
            {
                AnimationsRotate3D.AngleY = 360;
                ChangeWindowStateToDefault();
                StopAnimationAfterWhile();
            }
            else
            {
                TransformGroup.Children.Add(AnimationsTransformGroup);
                DefaultStateSize = Bounds.Size;
                AnimationsTranslate.X = -WindowTranslateMove.X;
                AnimationsTranslate.Y = -WindowTranslateMove.Y / (((Control)Parent).Bounds.Height / Bounds.Height);
                AnimationsScale.ScaleX = ((Control)Parent).Bounds.Width / Bounds.Width;
                AnimationsScale.ScaleY = ((Control)Parent).Bounds.Height / Bounds.Height;
                StopAnimationAfterWhile(() =>
                {
                    AnimationsRotate3D.AngleZ = 0;
                    AnimationsTranslate.X = 0;
                    AnimationsTranslate.Y = 0;
                    AnimationsScale.ScaleX = AnimationsScale.ScaleY = 1;
                    ChangeWindowStateToMaximized();
                });
            }

        }

        void ChangeWindowStateToDefault()
        {
            if(HBindings != null && VBindings != null)
            {
                WindowGridController.Unsubscribe(HBindings.Value);
                WindowGridController.Unsubscribe(VBindings.Value);
            }
            TransformGroup.Children.Clear();
            TransformGroup.Children.Add(WindowTranslateMove);
            Margin = new Avalonia.Thickness(0);
            this.FrameState = WindowFrameState.Default;
        }

        void ChangeWindowStateToMaximized()
        {
            if (this.FrameState == WindowFrameState.Maximized)
                return;
            TransformGroup.Children.Remove(WindowTranslateMove);
            HBindings = WindowGridController.SubscribeToHorizontal(this, Avalonia.Layout.HorizontalAlignment.Stretch);
            VBindings = WindowGridController.SubscribeToVertical(this, Avalonia.Layout.VerticalAlignment.Stretch);
            Margin = new Avalonia.Thickness(0, 0, 0, 88);
            this.FrameState = WindowFrameState.Maximized;
        }

        public override void Close(Action? action = null)
        {
            TitleBinding?.Dispose();
            IconBinding?.Dispose();
            TransformGroup.Children.Add(AnimationsTransformGroup);
            AnimationsScale.ScaleX = AnimationsScale.ScaleY = 0.75;
            AnimationsTranslate.Y = 128;
            Opacity = 0;
            StopAnimationAfterWhile(() => base.Close(action));
            base.Close(action);
        }

        public override WindowFrameState GetState()
        {
            return FrameState;
        }
    }
}
