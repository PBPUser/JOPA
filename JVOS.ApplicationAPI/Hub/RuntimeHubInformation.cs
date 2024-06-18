using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Hub
{
    public struct RuntimeHubInformation
    {
        public RuntimeHubInformation()
        {

        }

        public WeakReference WeakReference;
        public HubProvider Provider;
        public HubWindow Window;
        public Assembly Assembly;
        public AssemblyLoadContext Context;
    }
}
