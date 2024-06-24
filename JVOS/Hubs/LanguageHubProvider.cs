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
    public class LanguageHubProvider : HubProvider
    {
        public LanguageHubProvider()
        {
        }

        public override HubWindow? CreateHub()
        {
            return new LanguageSwitcherHub();
        }

        public override void CreateButtonContent(ref JButton button)
        {
            button.Content = (LanguageWorker.Current ?? new Language() { ShortName = "??" }).ShortName;
        }

        public override void UpdateButtonContent(ref JButton button)
        {
            button.Content = (LanguageWorker.Current ?? new Language() { ShortName = "??" }).ShortName;
        }

        public override string ToString() => "Language Switcher";
    }
}
