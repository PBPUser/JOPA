using Avalonia.Controls;
using HarfBuzzSharp;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Hub;
using System.Collections.Generic;

namespace JVOS.Hubs
{
    public partial class LanguageSwitcherHub : HubWindow
    {

        public LanguageSwitcherHub()
        {
            InitializeComponent();
            Closed += LanguageSwitcherHub_Closed;
            Opened += LanguageSwitcherHub_Opened;
        }

        private void LanguageSwitcherHub_Opened(object? sender, System.EventArgs e)
        {
            languages.ItemsSource = new List<Language>();
            languages.ItemsSource = LanguageWorker.Languages;
        }

        private void LanguageSwitcherHub_Closed(object? sender, CloseReason e)
        {
            if (e == CloseReason.CloseReason)
                return;
        }
    }
}
