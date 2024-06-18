using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.App
{
    public interface IAppLoader
    {
        public bool LoadApp(string path, object[]? args, out App? app);
    }
}
