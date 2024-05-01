using Avalonia.Data.Converters;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public struct ApplicationManifest
    {
        public string Name = "";
        public string InternalName = "";
        public string VersionHuman = "";
        public int Version = -1;
        public bool IsWebApplication = false;

        public ApplicationManifest() {
            
        }

        public ApplicationManifest(string name, string internalName, string versionFamily, int version)
        {
            Name = name;
            InternalName = internalName;
            Version = version;
            VersionHuman = versionFamily;
        }
    }
}
