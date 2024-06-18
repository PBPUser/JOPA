using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.App
{
    public class AppManifest
    {
        public string Name = "";
        public string Base64Icon = "";
        public string DllName = "";
        public string[] Activities = new string[0];
        public string[] Protocols = new string[0];
        public bool System = false;
    }
}
