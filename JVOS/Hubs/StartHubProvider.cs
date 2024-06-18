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
    public class StartHubProvider : HubProvider
    {
        public StartHubProvider()
        {
        }

        public override string ToString() => "Start";

        public override HubWindow? CreateHub()
        {
            return new StartHub();
        }

        public override void CreateButtonContent(ref JButton button)
        {
            button.Content = new Image() { Source = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Taskbar/start.png"))) };
        }
    }
}
