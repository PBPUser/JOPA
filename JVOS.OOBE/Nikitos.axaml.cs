using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using JVOS.ApplicationAPI.Windows;

namespace JVOS.OOBE
{
    public partial class Nikitos : WindowContentBase
    {
        public Nikitos()
        {
            InitializeComponent();
            TransformPerson = new TranslateTransform()
            {
                Transitions = new Avalonia.Animation.Transitions()
            {
                new DoubleTransition()
                {
                    Property = TranslateTransform.YProperty,
                    Duration = TimeSpan.FromMilliseconds(300)
                }
            }
            }; 
            TransformFork = new TranslateTransform()
            {
                Transitions = new Avalonia.Animation.Transitions()
            {
                new DoubleTransition()
                {
                    Property = TranslateTransform.YProperty,
                    Duration = TimeSpan.FromMilliseconds(300)
                }
            }
            };
            new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(300);
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        IsPressed = !IsPressed;
                    });
                }
            }).Start();

            Person.RenderTransform = TransformPerson;
            Fork.RenderTransform = TransformFork;

            Title = "Prearing JVOS...";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.OOBE/Assets/Icon.png")));

            Person.Source = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.OOBE/Assets/Nikitos.png")));
            Fork.Source = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS.OOBE/Assets/Fork.png")));
        
            using var oobeTextStream = AssetLoader.Open(new Uri("avares://JVOS.OOBE/OOBE.txt"));
            using var oobeTextReader = new StreamReader(oobeTextStream);
            while (!oobeTextReader.EndOfStream)
                texts.Add(oobeTextReader.ReadLine() ?? "");
        }

        List<string> texts = new();
        
        protected override void OnLoaded(RoutedEventArgs e)
        {
            Frame.SetFrameVisibility(false);
            Frame.ChangeState(WindowFrameState.Maximized);
            base.OnLoaded(e);
        }

        public override bool ShowOnTaskbar => base.ShowOnTaskbar;

        bool isPressed = false;

        bool IsPressed
        {
            get => isPressed;
            set
            {
                isPressed = value;
                TransformFork.Y = value ? Fork.Bounds.Height : 0;
                TransformPerson.Y = value ? -Person.Bounds.Height : 0;
            }
        }

        TranslateTransform TransformFork, TransformPerson;
        
    }
}
