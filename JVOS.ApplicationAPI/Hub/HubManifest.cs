using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Hub
{
    public class HubManifest
    {
        public string Name = string.Empty;
        public string PackageName = string.Empty;
        public string Description = string.Empty;
        public string DllPath = string.Empty;
        public string Version = string.Empty;
        public int PackageVersion = 0;
        public string[] ProvidenNames = new string[0];

        public HubManifest() { 
        
        }
    }
}
