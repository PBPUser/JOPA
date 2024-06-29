using Avalonia.Platform;
using JVOS.ApplicationAPI.Widgets;
using JVOS.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Widgets
{
    public static class WidgetManager
    {
        public static Widget? GetWidget(DesktopWidget manifest)
        {
            Widget? x = null;
            if (manifest.DllName == "")
                x = GetInternalWidget(manifest.ClassName);
            else
            {
                bool prevLoaded;
                x = ExternalWidgetLoadContext.GetWidgetContext(manifest.DllName, out prevLoaded).CreateWidget(manifest.ClassName);
            }
            if (x == null)
                return null;
            if(manifest.AutoUpdate)
                x.SetUpdatingTimer(manifest.AutoUpdateCycleLength);
            return x;
        }

        public static List<WidgetPreview> GetWidgetPreviews()
        {
            List<WidgetPreview> widgetsPreviews = new();

            var assets = AssetLoader.GetAssets(new("avares://JVOS/Resources/WidgetManifests"), null);
            foreach(var a in assets)
            {
                using var file = AssetLoader.Open(a);
                using var fileReader = new StreamReader(file);
                var x = JsonConvert.DeserializeObject<WidgetManifest>(fileReader.ReadToEnd());
                if (x == null)
                    continue;
                widgetsPreviews.Add(new()
                {
                    AutoUpdating = x.IsAutoUpdating,
                    AutoUpdateCycleLength = x.AutoUpdateCycleLength,
                    Title = x.Name,
                    Icon = UserOptions.Base64ToImage(x.Base64Icon),
                    Preview = UserOptions.Base64ToImage(x.Base64Preview),
                    ClassName = x.ClassName
                });
            }
            return widgetsPreviews;
        }

        private static Widget? GetInternalWidget(string className)
        {
            var type = Assembly.GetExecutingAssembly().GetType(className);
            if (type == null)
                return null;
            var constructor = type.GetConstructors().Where(x => x.GetParameters().Count() == 0).FirstOrDefault();
            if (constructor == null)
                return null;
            return (Widget)constructor.Invoke(new object[0]);
        }
    }
}
