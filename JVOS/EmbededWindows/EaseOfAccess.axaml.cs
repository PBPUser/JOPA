using Avalonia.Controls;
using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI;
using System.Reactive.Subjects;

namespace JVOS.EmbededWindows
{
    public partial class EaseOfAccess : UserControl, IJWindow
    {
        public EaseOfAccess()
        {
            InitializeComponent();
            apply.Click += (a, b) =>
            {
                ColorScheme.ApplyScheme(ColorScheme.Current, this.useDarkMode.IsChecked == true, useAccentTitle.IsChecked == true, useAccentBar.IsChecked == true);
            };
        }

        public void WhenLoaded()
        {
            ((IJWindow)this).UpdateTitle("Ease of access");
            App.SendNotification("Tess");
        }

        private Subject<string> _title = new Subject<string>();
        private Subject<Bitmap> _icon = new Subject<Bitmap>();

        public Subject<string> Title { get => _title; set => _title = value; }
        public Subject<Bitmap> Icon { get => _icon; set => _icon = value; }
    }
}
