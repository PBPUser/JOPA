using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using JVOS.ApplicationAPI;
using System;
using System.Threading;

namespace JVOS.Screens
{
    public partial class LoadingScreen : ScreenBase
    {
        public LoadingScreen()
        {
            InitializeComponent();
            bool isDebuging = false;
#if DEBUG
            isDebuging = true;
#endif
            if (!isDebuging)
                debugModeEnabled.IsVisible = false;
            
        }

        (Animation, CancellationTokenSource)?
            LoadingText1Animation,
            LoadingText2Animation;

        public override void ScreenOverlap()
        {
            StopLoadingAnimation();
            base.ScreenOverlap();
        }

        public override void ScreenShown()
        {
            StartLoadingAnimation();
            base.ScreenShown();
        }

        public void StopLoadingAnimation()
        {
            LoadingText1Animation.Value.Item2.Cancel();
            LoadingText2Animation.Value.Item2.Cancel();
        }

        public void StartLoadingAnimation()
        {
            LoadingText1Animation = AnimateOpacity(loadtext, 0.5, 1, 200);
            LoadingText2Animation = AnimateOpacity(loadtext2, 1, 0.5, 200);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
        }

        private static (Animation, CancellationTokenSource) AnimateOpacity(Animatable animatable, double start, double end, double length)
        {
            var anim1 = new Animation() { Duration = TimeSpan.FromMilliseconds(length), IterationCount = IterationCount.Infinite };
            var keyframe1_1 = new KeyFrame() { KeyTime = TimeSpan.Zero };
            var keyframe2_1 = new KeyFrame() { KeyTime = TimeSpan.FromMilliseconds(length) };
            keyframe1_1.Setters.Add(new Setter(OpacityProperty, start));
            keyframe2_1.Setters.Add(new Setter(OpacityProperty, end));
            anim1.Children.Add(keyframe1_1);
            anim1.Children.Add(keyframe2_1);
            var tokenCancel = new System.Threading.CancellationTokenSource();
            anim1.RunAsync(animatable,tokenCancel.Token);
            return (anim1,tokenCancel);
        }
    }
}
