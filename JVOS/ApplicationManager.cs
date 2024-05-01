using Avalonia.Controls;
using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI;
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
        public static List<IHubProvider> HubProviders = new List<IHubProvider>();

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
                if(Directory.Exists($"{Path}\\libraries"))
                    foreach(var s in Directory.GetFiles($"{Path}\\libraries")) {
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
            foreach(var init in inits)
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

        private static void RegisterExtenalHub(object? sender, IHubProvider e)
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
}
