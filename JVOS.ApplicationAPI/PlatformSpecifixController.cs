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
        public static string GetLocalFilePath(string dir, bool createSubPathIfNotExists = false)
        {
            string s = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                s = Path.GetFullPath(dir);
            else
                s = System.IO.Path.Combine(AppContext.BaseDirectory, dir);
            if (createSubPathIfNotExists)
            {
                List<string> split = s.Replace('/', '\\').Split('\\').ToList();
                split.RemoveAt(split.Count - 1);
                Directory.CreateDirectory(String.Join('/', split));
            }
            return s;
        }

        public static bool IsAndroid()
        {
            return RuntimeInformation.RuntimeIdentifier.ToLower().Contains("android");
        }
    }
}
