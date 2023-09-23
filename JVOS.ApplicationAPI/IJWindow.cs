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
        public Subject<string> Title { get; set; }
        public Subject<Bitmap> Icon { get; set; }
        public virtual void Closed()
        {

        }

        public virtual void WhenLoaded()
        {

        }

        public void UpdateTitle(string newTitle)
        {
            Title.OnNext(newTitle);
        }

        public void UpdateIcon(Bitmap newIcon)
        {
            Icon.OnNext(newIcon);
        }
    }
}
