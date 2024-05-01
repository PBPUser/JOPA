using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IInitializer
    {
        public void Initialize();
        public void Deinitialize();
    }
}
