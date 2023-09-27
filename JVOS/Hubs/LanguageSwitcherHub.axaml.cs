using Avalonia.Controls;
using JVOS.ApplicationAPI;

namespace JVOS.Hubs
{
    public partial class LanguageSwitcherHub : IHub
    {

        public LanguageSwitcherHub()
        {
            InitializeComponent();
            Closed += LanguageSwitcherHub_Closed;
            Opened += LanguageSwitcherHub_Opened;
        }

        private void LanguageSwitcherHub_Opened(object? sender, System.EventArgs e)
        {
            Languages.Children.Clear();
            foreach(LanguageWorker.Language lang in LanguageWorker.Languages)
            {
                var btn = new Button { Content = lang.ShortName, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch };
                btn.Click += (a, b) =>
                {
                    LanguageWorker.SetLanguage(lang);
                    OnClosed(CloseReason.CloseReason);
                };
                Languages.Children.Add(btn);
            }
        }

        private void LanguageSwitcherHub_Closed(object? sender, CloseReason e)
        {
            if (e == CloseReason.CloseReason)
                return;

        }
    }
}
