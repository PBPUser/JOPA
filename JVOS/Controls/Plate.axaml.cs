using Avalonia.Controls;
using Avalonia.Media.Imaging;
using JVOS.PanoramaBar;
using System.IO;
using System.Net;
using System.Reflection.PortableExecutable;

namespace JVOS.Controls
{
    public partial class Plate : UserControl
    {
        public Plate()
        {
            InitializeComponent();
        }

        public void LoadInfo(PanoramaSite.Section section, string title)
        {
            this._plateTitle.Text = title;
            string fn;
            DownloadFile(section.Image, out fn);
            this._img.Source = new Bitmap(fn);
            this._title.Text = section.Title;
            //this._img.Text = title;
            
        }

        private void DownloadFile(string url, out string filename)
        {
            filename = Path.GetTempFileName();
            using (var client = new WebClient())
            {
                client.DownloadFile(url, filename);
            }
        }
    }
}
