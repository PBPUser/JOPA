﻿using Avalonia.Controls;
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
    public class AssistantHubProvider : HubProvider
    {
        public AssistantHubProvider()
        {
        }

        public override void CreateButtonContent(ref JButton button)
        {
            button.Content = new Image
            {
                Source = new Bitmap(AssetLoader.Open(new("avares://JVOS/Assets/Shell/assistant.png")))
            };
            base.CreateButtonContent(ref button);
        }

        public override string ToString() => "Assistant";

        public override HubWindow? CreateHub()
        {
            return new AssistantHub();
        }
    }
}
