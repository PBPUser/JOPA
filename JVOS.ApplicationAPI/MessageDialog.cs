using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public class MessageDialog : IMessageDialog
    {

        private string _title, _message;
        private Bitmap? _icon;
        private List<IMessageDialogButton> _buttons = new List<IMessageDialogButton>();

        public MessageDialog(string name, string message, Bitmap? icon = null)
        {
            _title = name;
            _message = message;
            _icon = icon;
        }

        public void AddButton(MessageDialogButton button)
        {
            _buttons.Add(button);
        }

        public string Title => _title;

        public string Message => _message;

        public Bitmap? Icon => _icon;

        public List<IMessageDialogButton> Buttons => _buttons;
    }
}
