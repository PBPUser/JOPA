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
        public string ButtonFont = "Jcons";
        public Dictionary<string, string> Properties = new();

        public virtual IHub Create(out object ButtonContent)
        {
            ButtonContent = "Sample Text";
            return new IHub();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
