using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class ResourceBunch
    {
        public ResourceBunch()
        {

        }

        public virtual T? GetResource<T>(string name)
        {
            return default(T);
        }

        public virtual void SetResource(string name, object? value)
        {

        }
    }
}
