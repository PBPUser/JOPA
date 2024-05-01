using Avalonia.Controls;
using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reactive.Subjects;

namespace JVOS.ProgressiveWebApplicationSubSystem
{
    public partial class BrowserWindow : UserControl, IJWindow
    {
        public BrowserWindow()
        {
            InitializeComponent();
        }

        public void GoTo(string url)
        {

        }

        public IJWindow.WindowStartupLocation StartupLocation => IJWindow.WindowStartupLocation.Center;

        private Subject<string> _tileSubject = new Subject<string>();
        private Subject<Bitmap> _iconSubject = new Subject<Bitmap>();
        private Bitmap _icon;
        private string _title = "The Progressive Web Application";

        public string GetPanelId() => "jvos.pwass:browserwindow";

        public Subject<string> Title { get => _tileSubject; set => throw new NotImplementedException(); }
        public string TitleValue { get => _title; set { _title = value; _tileSubject.OnNext(value); } }
        public Subject<Bitmap> Icon { get => _iconSubject; set => throw new NotImplementedException(); }
        public Bitmap IconValue { get => _icon; set { _icon = value; _iconSubject.OnNext(_icon); } }
        public IJWindowFrame WindowFrame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
