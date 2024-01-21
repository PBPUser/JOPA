using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public static class ProtocolWorker
    {
        static ProtocolWorker()
        {
            Communicator.ProtocolRegister += ProtocolRegistration;
        }

        public static List<(string, Action<string, string>)> Protocols = new List<(string, Action<string, string>)>();

        private static void ProtocolRegistration(object? sender, (string, Action<string, string>) e)
        {
            if(Protocols.Where(x => x.Item1 == e.Item1).Count() == 0)
                Protocols.Add(e);
        }
    }
}
