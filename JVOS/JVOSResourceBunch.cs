using JVOS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    internal class JVOSResourceBunch : ResourceBunch
    {
        public static void SetResourceBunchToColorScheme()
        {
            ColorScheme.ResourceBunch = new JVOSResourceBunch();
        }

        public override T GetResource<T>(string name)
        {
            if (App.Current == null)
                return default(T);
            var res = App.Current.Resources[name];
            if (res == null)
                return default(T);
            try
            {
                var rs = (T)res;
                if (rs == null)
                    return default(T);
                return (T)res;
            }
            catch
            {
                return default(T);
            }
        }

        public override void SetResource(string name, object? value)
        {
            if (App.Current != null)
                App.Current.Resources[name] = value;
        }
    }
}
