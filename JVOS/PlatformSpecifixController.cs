using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public static class PlatformSpecifixController
    {
        public static string GetLocalFilePath(string dir)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return dir;
            return System.IO.Path.Combine(AppContext.BaseDirectory, dir);
        }
    }
}
