using Avalonia.Media.Imaging;
using System;

namespace JVOS.DataModel
{
    public class WidgetPreview
    {
        public Bitmap? Icon { get; set; }
        public Bitmap? Preview { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ClassName = "";
        public string DllName = "";
        public bool AutoUpdating = false;
        public ulong AutoUpdateCycleLength = 1000;
    }
}
