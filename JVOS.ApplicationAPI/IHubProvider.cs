using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public abstract class IHubProvider
    {
        public string Name;
        public string InternalName;

        public virtual IHub Create(out object ButtonContent)
        {
            ButtonContent = "Sample Text";
            return new IHub();
        }
    }
}
