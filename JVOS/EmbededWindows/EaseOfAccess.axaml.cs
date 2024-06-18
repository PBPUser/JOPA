using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using JVOS.Controls;
using JVOS.Screens;
using System;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows
{
    public partial class EaseOfAccess : WindowContentBase
    {
        public EaseOfAccess()
        {
            InitializeComponent();

            textToImage.Click += TextToImage;
            imageToText.Click += ImageToText;
            browse.Click += Browse;

            Title = "Ease of access";
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Lockscreen/easeofaccess.png")));
        }

        Bitmap? image;

        private void Browse(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var files = TopLevel.GetTopLevel(this).StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
            }).GetAwaiter().GetResult();
            if (files.Count != 1)
                return;
            try
            {
                using var fs = files[0].OpenReadAsync().GetAwaiter().GetResult();
                image = new Bitmap(fs);
                img.Source = image;
            }
            catch
            {
                Communicator.ShowMessageDialog(new MessageDialog("Error", "Bitmap failed to load"));
                return;
            }
        }

        private void ImageToText(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(image != null)
            tx.Text = UserOptions.ImageToBase64(image);
        }

        private void TextToImage(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            image = UserOptions.Base64ToImage(tx.Text);
            img.Source = image;
        }
    }
}
