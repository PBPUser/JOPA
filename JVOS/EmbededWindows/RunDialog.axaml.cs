using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using System;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows
{
    public partial class RunDialog : WindowContentBase
    {
        public RunDialog()
        {
            InitializeComponent();
            Title = "Run";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/run.png")));
            okBtn.Click += OkButton;
            cnBtn.Click += CancelButton;

        }

        public override Size DefaultSize => new (512, 280);

        private void CancelButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowManager.CloseWindowFrame(Frame);
        }

        private void OkButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowManager.CloseWindowFrame(Frame);
            Communicator.RunCommand(promptTb.Text);
        }
    }
}
