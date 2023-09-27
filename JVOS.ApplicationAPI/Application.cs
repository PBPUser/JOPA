using Avalonia.Media.Imaging;

namespace JVOS.ApplicationAPI
{
    public class Application
    {
        public Application(string name, string Internal, Bitmap? icon = null)
        {
            Name = name;
            InternalName = Internal;
            Icon = icon;
        }

        public string? Path;
        public string Name;
        public string InternalName;
        public Bitmap? Icon;
    }
}
