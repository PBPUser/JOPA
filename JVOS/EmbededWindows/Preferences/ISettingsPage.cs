using Avalonia.Media.Imaging;

namespace JVOS.EmbededWindows.Preferences
{
    public interface ISettingsPage
    {
        public string Title { get; }
        public string InternalLink { get; }
        public Bitmap Icon { get; }
    }
}