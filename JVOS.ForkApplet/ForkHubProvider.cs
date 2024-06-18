using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Hub;
using JVOS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ForkApplet
{
    internal class ForkHubProvider : HubProvider
    {
        public ForkHubProvider()
        {

        }

        public override HubWindow? CreateHub() => new ForkHub();

        public override void CreateButtonContent(ref JButton button)
        {
            button.Content = "6";
        }

        public override string ToString() => "Fork";
    }
}
