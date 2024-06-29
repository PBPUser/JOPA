using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.App;
using JVOS.ApplicationAPI.Hub;
using JVOS.Views;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JVOS
{
    public static class ApplicationManager
    {
        static bool isInitialized = false;

        public static string ApplicationsDirectory = "";
        public static string LibrariesDirectory = "";

        public static event EventHandler<EventArgs> AppsLoaded;

        public static List<Application> Apps = new List<Application>();
        public static List<IEntryPoint> EntryPoints = new List<IEntryPoint>();
        public static List<HubProvider> HubProviders = new List<HubProvider>();

        public static void Load()
        {
            if (isInitialized)
                return;
            RegisterApplicationEvents();
            isInitialized = true;
            ApplicationsDirectory = PlatformSpecifixController.GetLocalFilePath("Applications");
            LibrariesDirectory = PlatformSpecifixController.GetLocalFilePath("Libraries");
            if (!Directory.Exists(ApplicationsDirectory))
                Directory.CreateDirectory(ApplicationsDirectory);
            if (!Directory.Exists(LibrariesDirectory))
                Directory.CreateDirectory(LibrariesDirectory);
            string[] apps = Directory.GetDirectories(ApplicationsDirectory);
            foreach (var app in apps)
                PreloadApp(app);
            AppDomain.CurrentDomain.AssemblyResolve += (a, b) =>
            {
                foreach (var s in Directory.GetFiles(LibrariesDirectory, "*.dll", SearchOption.AllDirectories))
                    if (b.Name == AssemblyName.GetAssemblyName(s).FullName)
                        return Assembly.LoadFile(System.IO.Path.GetFullPath(s));
                return null;
            };
            if (AppsLoaded != null)
                AppsLoaded.Invoke(null, new EventArgs());
        }

        public static void PreloadApp(string Path)
        {
            string
                manifest = $"{Path}\\manifest.json",
                icon = $"{Path}\\icon.png";

            if (!File.Exists(manifest))
                return;
            var appMani = JsonConvert.DeserializeObject<ApplicationManifest>(File.ReadAllText(manifest));
            if (!appMani.IsWebApplication)
            {
                string appdll = $"{Path}\\application.dll";
                if (!File.Exists(appdll))
                    return;
                if (Directory.Exists($"{Path}\\libraries"))
                    foreach (var s in Directory.GetFiles($"{Path}\\libraries"))
                    {
                        LoadLib(System.IO.Path.GetFullPath(s));
                    }
                PreloadDll(System.IO.Path.GetFullPath(appdll));
            }
            else
            {
                string apphtml = $"{Path}\\index.html";
                if (!File.Exists(apphtml))
                    return;

            }
            Application app = new Application(appMani.Name, appMani.InternalName, appMani.IsWebApplication);
            if (File.Exists(icon))
                app.Icon = new Bitmap(icon);
            Apps.Add(app);
        }

        private static void LoadLib(string file)
        {

        }

        private static void PreloadDll(string Path)
        {
            var assembly = Assembly.LoadFile(Path);
            Type initType = typeof(IInitializer);
            Type[] inits = assembly.GetTypes().Where(x => initType.IsAssignableFrom(x) && x.IsClass).ToArray();
            foreach (var init in inits)
            {
                try
                {
                    ((IInitializer)Activator.CreateInstance(init)).Initialize();
                }
                catch
                {

                }
            }
        }

        public static void UnloadApp(string Path)
        {
            var obj = Apps.Where(x => x.Path == Path).FirstOrDefault();
            if (obj != null)
                UnloadApp(obj);
            else
                throw new Exception(Path);
        }

        public static void UnloadApp(Application app)
        {

        }

        private static void RegisterApplicationEvents()
        {
            Communicator.RegisterApp += ApplicationRegister;
            Communicator.RegisterEntrypoint += RegisterEntryPoint;
            Communicator.RegisterExternalhub += RegisterExtenalHub;
        }

        private static void RegisterExtenalHub(object? sender, HubProvider e)
        {
            HubProviders.Add(e);
        }

        private static void RegisterEntryPoint(object? sender, IEntryPoint e)
        {
            EntryPoints.Add(e);
        }

        private static void ApplicationRegister(object? sender, string e)
        {
            PreloadApp(e);
        }
    }

    public class AppLoader : IAppLoader
    {
        private static Type APP_TYPE = typeof(JVOS.ApplicationAPI.App.App);

        public bool LoadApp(string path, object[]? args, out JVOS.ApplicationAPI.App.App? app)
        {
            app = null;
            if (!Directory.Exists(path))
                return false;
            if (!File.Exists(path + "\\manifest.jvon"))
                return false;

            var manifest = JsonConvert.DeserializeObject<AppManifest>(File.ReadAllText(path + "\\manifest.jvon"));
            if (manifest == null)
                return false;

            if (!File.Exists(path + $"\\{manifest.DllName}"))
            {
                Communicator.ShowMessageDialog(new MessageDialog("App loader", $"Dll not exists {manifest.DllName} not exists"));
                return false;
            }

            Debug.WriteLine(APP_TYPE.FullName);

            Debug.WriteLine(" ");

            var appContext = new AssemblyLoadContext($"domain_{manifest.Name}_{DateTime.Now.ToBinary()}", true);

            appContext.Unloading += (a) =>
            {
                
                Dispatcher.UIThread.Invoke(() => Communicator.ShowMessageDialog(new MessageDialog("App manager", "Application context successfully unloaded")));
                GC.Collect();
                GC.WaitForPendingFinalizers();
            };
            appContext.Resolving += (a, b) =>
            {
                return AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).Assemblies.Where(x => x.FullName == b.FullName).FirstOrDefault();
            };
            Assembly assembly;

            using (var fs = new FileStream(path + $"\\{manifest.DllName}", FileMode.Open))
            {
                assembly = appContext.LoadFromStream(fs);
            }
            foreach (var xz in assembly.GetTypes())
            {
                var type = xz.BaseType;
                string st = "";
                while (type != null)
                {
                    st += ":" + type.FullName;
                    type = type.BaseType;
                }
                Debug.WriteLine(xz.FullName + st);
            }

            Type[] appTypes = assembly.GetTypes().Where(x => APP_TYPE.IsAssignableFrom(x) && x.IsClass).ToArray();

            AppCommunicator appCommunicator = new AppCommunicator();
            appCommunicator.assembly = assembly;
            appCommunicator.assemblyContext = appContext;
            appCommunicator.manifest = manifest;

            app = (JVOS.ApplicationAPI.App.App)(appTypes[0].GetConstructors()[0].Invoke(new object[] { appCommunicator }));
            appCommunicator.app = app;
            app.Communicator = appCommunicator;

            return true;
        }

    }

    public class ApplicationAsseblyLoadContext : AssemblyLoadContext
    {
        public ApplicationAsseblyLoadContext(string name = "") : base(name, true)
        {

        }
    }

}
