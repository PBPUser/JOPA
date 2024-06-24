using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using JVOS.ApplicationAPI.Hub;
using JVOS.Screens;
using System;
using System.Collections.Generic;
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
                Duration = TimeSpan.FromMilliseconds(rotateDuration)
            });
            personTransitions.Add(new ThicknessTransition()
            {
                Property = MarginProperty,
                Duration = TimeSpan.FromMilliseconds(offsetDuration)
            });
            personTranslateTransitions.Add(new DoubleTransition()
            {
                Property = TranslateTransform.XProperty,
                Duration = TimeSpan.FromMilliseconds(moveDuration)
            });
            personTranslateTransitions.Add(new DoubleTransition()
            {
                Property = TranslateTransform.YProperty,
                Duration = TimeSpan.FromMilliseconds(moveDuration)
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

            Opened += HubOpen;
            Closed += HubClose;
        }

        private void HubClose(object? sender, CloseReason e)
        {
            StopAnimation();
        }

        private void HubOpen(object? sender, EventArgs e)
        {

        }

        const double rotateDuration = .125;
        const double moveDuration = 2;
        const double offsetDuration = .5;

        static Transitions personRotateTransitions = new();
        static Transitions personTranslateTransitions = new();
        static Transitions personTransitions = new();

        TranslateTransform personTranslateTransform = new();
        RotateTransform personRotateTransform = new();

        bool isPreviousPlaying = false;

        List<CancellationToken> Tokens = new();

        CancellationToken LoopAnimation(double length, Action action, Action action2)
        {
            CancellationToken token = new();
            bool statement = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;
                    statement = !statement;
                    Dispatcher.UIThread.Invoke(statement ? action : action2);
                    Thread.Sleep((int)length);
                }
            });
            return token;
        }

        void StopAnimation()
        {
            DesktopScreen.CurrentDesktop.CloseAllHubs();
            isPreviousPlaying = false;
            var c = Tokens.Count;
            for (int i = 0; i < c; i++)
            {
                var token = Tokens[0];
                token.Register(() => { });
                Tokens.RemoveAt(0);
            }
        }
        
        void PlaySound()
        {
            isPreviousPlaying = true;
            int j = new Random().Next(0, 10);
            switch (j)
            {
                case 1:
                    Person.Margin = new Thickness(0);
                    Tokens.Add(LoopAnimation(rotateDuration, () => personRotateTransform.Angle = 360, () => personRotateTransform.Angle = 0));
                    break;
                case 4:
                    Tokens.Add(LoopAnimation(rotateDuration, () => personRotateTransform.Angle = 45, () => personRotateTransform.Angle = -45));
                    Tokens.Add(LoopAnimation(moveDuration, () => personTranslateTransform.X = 40, () => personTranslateTransform.X = -40));
                    break;
                default:
                    Tokens.Add(LoopAnimation(offsetDuration, () => Person.Margin = new(0, -128, 0, 128), () => Person.Margin = new(0, 128, 0, -128)));
                    if (j > 7)
                        Tokens.Add(LoopAnimation(rotateDuration, () => personRotateTransform.Angle = 360, () => personRotateTransform.Angle = 0));
                    else
                        personTranslateTransform.Y = 0;
                    if (j > 5 && j < 8)
                        Tokens.Add(LoopAnimation(moveDuration, () => personTranslateTransform.X = 50, () => personTranslateTransform.X = -50));
                    else
                        personTranslateTransform.X = 0;
                    break;
            }
            new Thread(() =>
            {
                //using (var ms = File.OpenRead(j != 1 ? "assets/search.mp3" : "assets/pole_chudes.mp3"))
                //using (var rdr = new Mp3FileReader(ms))
                //using (var wavStream = WaveFormatConversionStream.CreatePcmStream(rdr))
                //using (var baStream = new BlockAlignReductionStream(wavStream))
                //using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    //waveOut.Init(rdr);
                    //waveOut.Play();
                    //while (waveOut.PlaybackState == PlaybackState.Playing)
                    Thread.Sleep(2000);
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        if (j != 0x10)
                        {
                            PlayTTS();
                        }
                        else
                        {
                            StopAnimation();
                        }
                    });
                }
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
