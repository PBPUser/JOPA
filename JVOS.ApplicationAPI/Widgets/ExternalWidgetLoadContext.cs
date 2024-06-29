using JVOS.ApplicationAPI.Widgets;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Widgets
{
    public class ExternalWidgetLoadContext : AssemblyLoadContext, IDisposable
    {
        ExternalWidgetLoadContext(string dllPath) : base(GetExternalWidgetLoadContextName(dllPath), true)
        {
            Resolving += ResolveUnknownDependences;

            Contexts.Add(this);
            using var stream = File.OpenRead(dllPath);
            LoadFromStream(stream);
        }


        public static ExternalWidgetLoadContext GetWidgetContext(string dllPath, out bool previouslyLoaded)
        {
            var list = Contexts.Where(x => x.Name == GetExternalWidgetLoadContextName(dllPath));
            if (list.Count() == 0)
            {
                previouslyLoaded = false;
                return new ExternalWidgetLoadContext(dllPath);
            }
            previouslyLoaded = true;
            return list.First();
        }

        public static string GetExternalWidgetLoadContextName(string dllname)
        {
            return $"WIDGET_{dllname}";
        }

        private static System.Reflection.Assembly? ResolveUnknownDependences(AssemblyLoadContext arg1, System.Reflection.AssemblyName arg2)
        {
            var l = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).Assemblies.Where(x => x.FullName == arg2.FullName).FirstOrDefault();
            if(l != null)
                return l;
            return null;
        }

        static List<ExternalWidgetLoadContext> Contexts = new();
        List<Widget> LoadedWidgets = new();

        public Widget? CreateWidget(string className)
        {
            var s = Assemblies.Select(x => x.GetType(className)).Where(x => x.IsAssignableTo(typeof(Widget))).FirstOrDefault();
            if (s == null)
                try { return null; }
                finally { UnloadIfUnused(); }
            var widgetConstructor = s.GetConstructors().Where(x => x.GetParameters().Count() == 0).FirstOrDefault();
            if (widgetConstructor == null)
                try { return null; }
                finally { UnloadIfUnused(); }
            var widget = (Widget)widgetConstructor.Invoke(new object[0]);
            LoadedWidgets.Add(widget);
            widget.LoadContext = this;
            return widget;
        }

        public void UnloadWidget(Widget widget)
        {
            widget.Dispose();
            LoadedWidgets.Remove(widget);
            UnloadIfUnused();
        }

        public bool UnloadIfUnused()
        {
            if(LoadedWidgets.Count < 0)
                return false;
            Contexts.Remove(this);
            Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return true;
        }

        bool isDisposed = false;

        public void Dispose()
        {
            if (isDisposed)
                return;
            while (LoadedWidgets.Count > 0)
                LoadedWidgets[0].Dispose();
            UnloadIfUnused();
            isDisposed = true;
        }
    }
}
