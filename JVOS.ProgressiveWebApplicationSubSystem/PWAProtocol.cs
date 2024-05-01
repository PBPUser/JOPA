using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ProgressiveWebApplicationSubSystem
{
    public class PWAProtocol : IProtocol
    {
        private string protocol;

        internal PWAProtocol(string proto)
        {
            protocol = proto;
        }

        public string Name => protocol;

        public bool Execute(string[] args)
        {
            var jwin = new BrowserWindow();
            Communicator.OpenWindow(jwin);
            jwin.GoTo(string.Join("", args));
            return true;
        }
    }
}
