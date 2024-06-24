using Avalonia.Controls;
using Avalonia.Media;
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
    public class ClockHubProvider : HubProvider
    {
        public ClockHubProvider()
        {
        }

        public override HubWindow? CreateHub()
        {
            return new ClockHub();
        }

        public override void CreateButtonContent(ref JButton button)
        {
            button.Content = "14:88";
            button.Width = 160;
            button.FontFamily = (FontFamily)App.Current.Resources["AntonFont"];
        }

        public override string ToString() => "Clock";
    }
}
