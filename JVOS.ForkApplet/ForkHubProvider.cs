using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ForkApplet
{
    internal class ForkHubProvider : IHubProvider
    {
        public ForkHubProvider()
        {
            Name = "Fork Hub Provider";
            InternalName = "org.jvos.forkapplet.forkhub";
        }

        public override IHub Create(out object ButtonContent)
        {
            ButtonContent = "2";
            return new ForkHub();
        }
    }
}
