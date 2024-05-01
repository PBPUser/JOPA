using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public class MessageDialogButton : IMessageDialogButton
    {
        private string _name;
        private Action _onClick;

        public MessageDialogButton(string name, Action action)
        {
            _name = name;
            _onClick = action;
        }

        public string Title => _name;

        public void Run()
        {
            _onClick();
        }
    }
}
