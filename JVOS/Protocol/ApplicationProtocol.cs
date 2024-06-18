using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Protocol
{
    public class ApplicationProtocol : IProtocol
    {
        public string Name => "application";

        public bool Execute(string[] args)
        {
            var app = ApplicationManager.EntryPoints.Where(x => x.Name == args[0]);
            if(app.Count() > 0)
            {
                app.First().Run(args);
                return true;
            }
            Communicator.ShowMessageDialog(new MessageDialog("System", $"Application with package name \"{args[0]}\" not found"));
            return false;
        }
    }
}
