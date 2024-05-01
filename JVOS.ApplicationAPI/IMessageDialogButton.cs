using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IMessageDialogButton
    {
        public string Title { get; }
        public void Run();
    }
}
