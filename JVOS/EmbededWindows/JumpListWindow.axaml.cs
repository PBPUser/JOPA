using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using JVOS.ApplicationAPI.Windows;

namespace JVOS.EmbededWindows
{
    public partial class JumpListWindow : WindowContentBase
    {
        public WindowJumpListVM VM;

        public JumpListWindow()
        {
            InitializeComponent();
            DataContext = VM = new WindowJumpListVM();
            closebtn.Click += CloseClick;
            Title = "Jump List";
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            Frame.SetFrameVisibility(false);
            Frame.ToggleVisibilityState(false);
        }

        public override void Deactivated()
        {
            Frame.ToggleVisibilityState();
        }

        public override Size DefaultSize => new Size(384, 224);
        public override bool ShowOnTaskbar => false;

        private void CloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            VM.CloseWindow();
        }
    }
}
