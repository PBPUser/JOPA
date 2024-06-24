using Avalonia;
using Avalonia.Controls;
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
            Loaded += (a,b) => Frame.SetFrameVisibility(false);
            Loaded += (a,b) => Frame.Minimize();
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
