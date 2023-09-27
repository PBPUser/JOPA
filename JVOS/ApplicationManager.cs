using Avalonia.Controls;
using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public static class ApplicationManager
    {
        static bool isInitialized = false;

        public static List<Application> Apps = new List<Application>();
        public static List<(Bitmap?, string, string)> Runners = new List<(Bitmap?, string, string)>();

        public static void Load()
        {
            if (isInitialized)
                return;
            isInitialized = true;
        }

        public static void PreloadApp(string Path)
        {
            string 
                manifest = $"{Path}\\manifest.json",
                icon = $"{Path}\\icon.png",
                library = $"{Path}\\application.dll";

            if (!File.Exists(manifest))
                return;
            if (!File.Exists(icon))
                return;
            var appMani = JsonConvert.DeserializeObject<ApplicationManifest>(manifest);
            Application app = new Application(appMani.Name, appMani.InternalName);
            if (File.Exists(icon))
                app.Icon = new Bitmap(icon);

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
            List<(Bitmap?, string, string)>  Runners = new List<(Bitmap?, string, string)>();
            foreach (var runnable in Runners)
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
    }
}
