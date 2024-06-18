using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Hub
{
    public static class HubManager
    {
        public static HubManifest? GetHubManifest(string path)
        {
            return JsonConvert.DeserializeObject<HubManifest>(File.ReadAllText(path));
        }

        public static bool LoadHub(string path, string hubname, out RuntimeHubInformation? runtimeInfo, out string error)
        {
            error = string.Empty;
            runtimeInfo = null;

            var hubManifest = GetHubManifest(Path.Combine(path, "manifest.jvon"));
            if(hubManifest == null)
            {
                error = "Invalid hub manifest";
                return false;
            }
            if(!hubManifest.ProvidenNames.Contains(hubname)) {
                error = "Hub manifest dosen't contains " + hubname;
                return false;
            }
            var hubDllPath = Path.Combine(path, hubManifest.DllPath);
            if (!File.Exists(hubDllPath))
            {
                error = "Hub library not found on specified path.";
                return false;
            }

            AssemblyLoadContext assemblyLoadContext = new("hub_" + DateTime.Now.ToBinary() + "_" + hubname, true);
            if (Communicator.IsGodot)
            {
                foreach (var s in AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).Assemblies)
                    assemblyLoadContext.LoadFromAssemblyName(s.GetName());
            }
            Assembly assembly;
            using (var fs = new FileStream(Path.GetFullPath(hubDllPath), FileMode.Open))
            {
                assembly = assemblyLoadContext.LoadFromStream(fs);
            }

            IEnumerable<Type> types = assembly.GetTypes()
                .Where(x => x.IsAssignableTo(typeof(HubProvider)));
            var type = types.Where(x => x.ToString().Equals(hubname)).FirstOrDefault();
            if(type == null)
            {
                assemblyLoadContext.Unload();
                error = $"Library dosen't contain hub with name {hubname}.\nAvailable types are: \n{string.Join("\n", types.Select(x => x.FullName))}";
                return false;
            }
            var constructor = type.GetConstructors().Where(x => x.GetParameters().Length == 0).FirstOrDefault();
            if(constructor == null)
            {
                assemblyLoadContext.Unload();
                error = "Hub dosen't contain correct constructor.";
                return false;
            }
            var hubProvider = (HubProvider)constructor.Invoke(null);
            var hubWindow = hubProvider.CreateHub();
            
            runtimeInfo = new()
            {
                Assembly = assembly,
                Context = assemblyLoadContext,
                Provider = hubProvider,
                WeakReference = new WeakReference(assemblyLoadContext),
                Window = hubWindow
            };

            return true;
        }
        
        public static void UnloadHub(RuntimeHubInformation info)
        {
            info.Context.Unload();

            for(int i = 0; info.WeakReference.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Communicator.ShowMessageDialog(new MessageDialog("Test", "Unloaded: " + info.WeakReference.IsAlive));
        }
    }
}
