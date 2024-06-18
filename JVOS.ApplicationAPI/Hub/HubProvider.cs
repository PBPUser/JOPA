using JVOS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Hub
{
    public abstract class HubProvider
    {
        public HubProvider() { 
        
        }

        public Dictionary<string, string> Properties = new();

        public virtual void UpdateButtonContent(ref JButton button)
        {

        }

        public virtual HubWindow? CreateHub()
        {
            return default(HubWindow);
        }

        public virtual void CreateButtonContent(ref JButton button)
        {

        }
    }
}
