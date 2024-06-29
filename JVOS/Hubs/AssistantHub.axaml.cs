using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Hub;
using JVOS.Screens;
using NetCoreAudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace JVOS.Hubs
{
    public partial class AssistantHub : HubWindow
    {
        static AssistantHub()
        {
            personRotateTransitions.Add(new DoubleTransition()
            {
                Property = RotateTransform.AngleProperty,
                Duration = TimeSpan.FromSeconds(rotateDuration)
            });
            personTransitions.Add(new ThicknessTransition()
            {
                Property = MarginProperty,
                Duration = TimeSpan.FromSeconds(offsetDuration)
            });
            personTranslateTransitions.Add(new DoubleTransition()
            {
                Property = TranslateTransform.XProperty,
                Duration = TimeSpan.FromSeconds(moveDuration)
            });
            personTranslateTransitions.Add(new DoubleTransition()
            {
                Property = TranslateTransform.YProperty,
                Duration = TimeSpan.FromSeconds(moveDuration)
            });

        }

        public AssistantHub() {
            InitializeComponent();

            Width = 384;
            Height = 512;

            Person.Source = new Bitmap(AssetLoader.Open(new("avares://JVOS/Assets/Shell/assistant.png")));
            Person.Transitions = personTransitions;

            var transform = new TransformGroup();
            transform.Children.Add(personTranslateTransform);
            transform.Children.Add(personRotateTransform);

            personTranslateTransform.Transitions = personTranslateTransitions;
            personRotateTransform.Transitions = personRotateTransitions;

            Person.RenderTransform = transform;

            Opened += HubOpen;
            Closed += HubClose;
        }

        private void HubClose(object? sender, CloseReason e)
        {
            StopAnimation();
        }

        private void HubOpen(object? sender, EventArgs e)
        {
            PlaySound();
        }

        const double rotateDuration = .125;
        const double moveDuration = 2;
        const double offsetDuration = .5;

        static Transitions personRotateTransitions = new();
        static Transitions personTranslateTransitions = new();
        static Transitions personTransitions = new();

        TranslateTransform personTranslateTransform = new();
        RotateTransform personRotateTransform = new();

        List<DispatcherTimer> Timers = new();

        DispatcherTimer LoopAnimation(double interval, Action action)
        {
            DispatcherTimer timer = new() {
                Interval = TimeSpan.FromSeconds(interval),
            };
            timer.Tick += (a, b) => Dispatcher.UIThread.Invoke(action);
            Timers.Add(timer);
            timer.IsEnabled = true;
            return timer;
        }

        void StopAnimation()
        {
            DesktopScreen.CurrentDesktop.CloseAllHubs();
            var c = Timers.Count;
            for (int i = 0; i < c; i++)
            {
                var timer = Timers[0];
                timer.Stop();
                Timers.RemoveAt(0);
            }
        }
        
        void PlaySound()
        {
            int j = new Random().Next(0, 10);
            new Thread(() =>
            {
                if (PlatformSpecifixController.IsAndroid())
                    return;
                var fname = PlatformSpecifixController.GetLocalFilePath("Assets\\Sounds\\search.mp3", true);
                if (!File.Exists(fname))
                {
                    using var stream = AssetLoader.Open(new("avares://JVOS/Assets/Sounds/search.mp3"));
                    using var fs = File.OpenWrite(fname);
                    stream.CopyTo(fs);
                    fs.Close();
                }

                double c = 0;

                LoopAnimation(rotateDuration, () => {
                    c++;
                    personRotateTransform.Angle += Math.Sin(c) * 16;
                });
                LoopAnimation(moveDuration, () =>
                {
                    personTranslateTransform.Y = DateTime.Now.Microsecond % 51;
                });

                var player = new Player();
                player.PlaybackFinished += (a, b) =>
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        if (j != 0x10)
                            PlayTTS();
                        else
                            StopAnimation();
                    });
                };
                player.Play(fname);
            }).Start();
        }

        private void PlayTTS()
        {
            new Thread(() =>
            {
                //using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    //string voices = "";
                    //foreach (var v in synth.GetInstalledVoices())
                    //    voices += $"{v.VoiceInfo.Name}\n";
                    //InstalledVoice j;
                    //try
                    //{
                    //    j = synth.GetInstalledVoices().First(x => x.VoiceInfo.Name.ToLower().Contains("pavel"));
                    //}
                    //catch
                    //{
                    //    j = synth.GetInstalledVoices().First();
                    //}
                    //synth.SelectVoice(j.VoiceInfo.Name);
                    //synth.Speak("Speech System Cortana Systems Says " + phrases[new Random().Next(0, phrases.Length)]);
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        StopAnimation();
                    });

                }
            }).Start();
        }

    }
}
