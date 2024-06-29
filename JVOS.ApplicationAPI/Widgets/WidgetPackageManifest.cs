using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Widgets
{
    public class WidgetPackageManifest
    {
        public WidgetPackageManifest() {
        }

        public List<WidgetManifest> Widgets = new();
        public string DllName = "";
        [JsonIgnore]
        public bool System = false;
    }

    public class WidgetManifest
    {
        public WidgetManifest()
        {

        }

        public string Name = "";
        public string Base64Icon = "";
        public string Base64Preview = "";
        public string ClassName = "";

        public bool IsAutoUpdating = false;
        /// <summary>
        /// Auto update Cycle length in ms
        /// </summary>
        public ulong AutoUpdateCycleLength = 1000;
    }
}
