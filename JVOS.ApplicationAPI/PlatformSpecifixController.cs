using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public static class PlatformSpecifixController
    {
        public static string GetLocalFilePath(string dir)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Path.GetFullPath(dir);
            return System.IO.Path.Combine(AppContext.BaseDirectory, dir);
        }

        public static bool IsAndroid()
        {
            return RuntimeInformation.OSDescription.ToLower().Contains("android");
        }
    }
}
