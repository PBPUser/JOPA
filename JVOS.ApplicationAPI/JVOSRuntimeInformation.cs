using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public static class JVOSRuntimeInformation
    {
        internal static bool isGodot = false;

        public static bool IsDesktop => OSRuntimeType.Contains("Desktop");
        public static bool IsGodot => isGodot;
        public static bool IsAndroid => OSRuntimeType.Contains("Android");
        public static string APIVersion => Assembly.GetExecutingAssembly().FullName?.Split(", ").Where(x=>x.StartsWith("Version=")).First().Split("=").Last() ?? "between 34 and 69";
        public static string OSVersion => AssemblyLoadContext.Default.Assemblies.Where(x=>x.FullName?.Contains("JVOS,")??false).FirstOrDefault()?.FullName.Split(", ").Where(x => x.StartsWith("Version=")).First().Split("=").Last() ?? "between 34 and 69";
        public static string OSRuntimeVersion => Assembly.GetEntryAssembly()?.FullName?.Split(", ").Where(x => x.StartsWith("Version=")).First().Split("=").Last() ?? "between 34 and 69";
        public static string OSRuntimeType => isGodot ? "JVisionOS" : Assembly.GetEntryAssembly()?.FullName?.Split(", ").First() ?? "Undefined or Joyousmicor itself";
        public static string LibrariesVersions => string.Join('\n',AssemblyLoadContext.Default.Assemblies.Select(x => $"{x.FullName}:{x.ImageRuntimeVersion}"));
    }
}
