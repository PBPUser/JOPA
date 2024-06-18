using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI.Hub;
using JVOS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Hubs
{
    public class VolumeHubProvider : HubProvider
    {
        public VolumeHubProvider()
        {
        }

        public override HubWindow? CreateHub()
        {
            return new VolumeHub();
        }

        public override void CreateButtonContent(ref JButton button)
        {
            button.Content = "A";
        }

        public override string ToString() => "Volume";
    }
}
