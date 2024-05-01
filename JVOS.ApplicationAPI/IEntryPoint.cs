using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IEntryPoint
    {
        public string Name { get; }
        public void Run(string[] args);
    }
}
