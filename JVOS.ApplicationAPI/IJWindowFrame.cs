using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IJWindowFrame
    {
        public IWindowSpace? WindowSpace { get; set; }
        public bool AllowMinimize { get; set; }
        public int ID { get; set; }

        public virtual void Close(Action? action = null)
        {

        }
    }
}
