using Avalonia.Controls;
using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public static class ApplicationManager
    {
        static bool isInitialized = false;

        public static List<Application> Apps = new List<Application>();
        public static List<(Bitmap?, string, string, string)> Runners = new List<(Bitmap?, string, string, string)>();
        public static List<Runnable> Runnables = new List<Runnable>();

        public static void Load()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            if (!Directory.Exists("Applications"))
                Directory.CreateDirectory("Applications");
            string[] apps = Directory.GetDirectories("Applications");
            foreach (var app in apps)
                PreloadApp(app);
        }

        public static void PreloadApp(string Path)
        {
            string 
                manifest = $"{Path}\\manifest.json",
                icon = $"{Path}\\icon.png";

            if (!File.Exists(manifest))
                return;
            if (!File.Exists(icon))
                return;
            var appMani = JsonConvert.DeserializeObject<ApplicationManifest>(manifest);
            Application app = new Application(appMani.Name, appMani.InternalName);
            if (File.Exists(icon))
                app.Icon = new Bitmap(icon);
            int i = 0;
            foreach(string runn in appMani.Runners)
            {
                string ic = $"{Path}\\Runners\\{runn}.png";
                Bitmap? bmp = null;
                if (File.Exists(ic))
                    bmp = new Bitmap(ic);
                Runners.Add((bmp, app.InternalName, runn, appMani.RunnersHuman[i]));
                i++;
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
            List<(Bitmap?, string, string, string)> Runners = new List<(Bitmap?, string, string, string)>();
            foreach (var runnable in ApplicationManager.Runners)
            {
                if (runnable.Item2 != app.InternalName)
                    Runners.Add(runnable);
            }
            ApplicationManager.Runners = Runners;
            Apps.Remove(app);
        }

        private static void RegisterApplicationEvents()
        {
            Communicator.RegisterApp += ApplicationRegister;

        }

        private static void ApplicationRegister(object? sender, string e)
        {
            PreloadApp(e);
        }

        /// <summary>
        /// runnable hint API
        /// bitmap - icon
        /// string1 - appid
        /// string2 - runid
        /// string3 - humanname
        /// </summary>
        /// <param name="runner"></param>

        public static void Run((Bitmap?, string, string, string) runner)
        {
            var runnable = Runnables.Where(x => x.InternalName == runner.Item2).FirstOrDefault();
            if(runnable != null)
            {
                runnable.Run();
                return;
            }
            var app = Apps.Where(x => x.InternalName == runner.Item2).FirstOrDefault();
            if (app != null)
            {
                string assemblyPath = $"{app.Path}\\application.dll";
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                Type interfaceType = typeof(Runnable);
                Type[] x = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass)
                    .ToArray();
                foreach(var assembli in x)
                {
                    object? template = Activator.CreateInstance(assembli);
                    Runnable info = template as Runnable;
                    Runnables.Add(info);
                    if (info.InternalName == runner.Item2)
                        Runnables.Add(info);
                }
            }
            else
            {
                throw new Exception($"{runner.Item2} not registred in JOPA.");
            }
        }
    }
}
