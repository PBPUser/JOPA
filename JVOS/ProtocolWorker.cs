using JVOS.ApplicationAPI;
using JVOS.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public static class ProtocolWorker
    {
        public static void LoadProtocolWorker()
        {
            Communicator.RegisterProtocol += ProtocolRegistration;
            Communicator.Register(new ShellProtocol());
            Communicator.Register(new AppProtocol());
            Communicator.Register(new ApplicationProtocol());
        }

        public static List<IProtocol> Protocols = new List<IProtocol>();

        private static void ProtocolRegistration(object? sender, IProtocol e)
        {
            if(Protocols.Where(x => x.Name == e.Name).Count() == 0)
                Protocols.Add(e);
        }
    }
}
