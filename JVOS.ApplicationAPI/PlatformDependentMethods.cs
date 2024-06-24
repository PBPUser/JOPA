using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public static class PlatformDependentMethods
    {
        public static string Platform = "windows";

        public static CancellationToken PlaySound(string filename)
        {
            return CancellationToken.None;
        }
    }
}
