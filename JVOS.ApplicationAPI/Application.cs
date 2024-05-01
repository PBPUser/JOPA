using Avalonia.Media.Imaging;

namespace JVOS.ApplicationAPI
{
    public class Application
    {
        public Application(string name, string Internal, bool isWeb, Bitmap? icon = null)
        {
            Name = name;
            InternalName = Internal;
            IsWeb = isWeb;
            Icon = icon;
        }

        public bool IsWeb;
        public string? Path;
        public string Name;
        public string InternalName;
        public List<IEntryPoint> EntryPoints = new List<IEntryPoint>();
        
        public Bitmap? Icon;
    }
}
