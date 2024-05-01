using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IMessageDialog
    {
        public string Title { get; }
        public string Message { get; }
        public Bitmap? Icon { get; }
        public List<IMessageDialogButton> Buttons { get; }
    }
}
