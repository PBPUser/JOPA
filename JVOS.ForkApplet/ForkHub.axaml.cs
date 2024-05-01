using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using JVOS.ApplicationAPI;

namespace JVOS.ForkApplet
{
    public partial class ForkHub : IHub
    {
        public ForkHub()
        {
            InitializeComponent();
            Removed += Remove;
            fork.PointerReleased += ForkRelease;
            fork.RenderTransform = Rotate;
        }

        RotateTransform Rotate = new RotateTransform()
        {
            Transitions = new Avalonia.Animation.Transitions()
            {
                new DoubleTransition()
                {
                    Property = RotateTransform.AngleProperty,
                    Duration = TimeSpan.FromMilliseconds(350)
                }
            }
        };

        private void ForkRelease(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            Rotate.Angle += new Random().Next(-500, 500);
        }

        private void Remove(object? sender, object e)
        {

        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
        }
    }
}
