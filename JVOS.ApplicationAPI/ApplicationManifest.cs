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
        public string[] Runners = new string[0];
        public string[] RunnersHuman = new string[0];

        public ApplicationManifest() {
            
        }

        public ApplicationManifest(string name, string internalName, string versionFamily, int version, string[] runners)
        {
            Name = name;
            InternalName = internalName;
            Version = version;
            VersionHuman = versionFamily;
            Runners = runners;
        }
    }
}
