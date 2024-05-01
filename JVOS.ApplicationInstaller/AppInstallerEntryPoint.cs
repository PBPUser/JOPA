using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationInstaller
{
    public class AppInstallerEntryPoint : IEntryPoint
    {
        public string Name => "appinstaller";

        public void Run(string[] args)
        {
            Communicator.OpenWindow(new AppInstallerWindow());
        }
    }
}
