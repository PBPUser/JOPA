using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IJWindow
    {
        public enum WindowStartupLocation { TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight }

        public WindowStartupLocation StartupLocation { get; }

        public Subject<string> Title { get; set; }
        public string TitleValue { get; set; }
        public Subject<Bitmap> Icon { get; set; }
        public Bitmap IconValue { get; set; }
        public IJWindowFrame WindowFrame { get; set;}
        public virtual void Closed()
        {

        }

        public virtual void WhenLoaded()
        {

        }

        public void UpdateTitle(string newTitle)
        {
            Title.OnNext(newTitle);
            TitleValue = newTitle;
        }

        public void UpdateIcon(Bitmap newIcon)
        {
            Icon.OnNext(newIcon);
            IconValue = newIcon;
        }
    }
}
