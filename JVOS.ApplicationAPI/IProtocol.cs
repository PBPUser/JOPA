using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IProtocol
    {
        public string Name { get; }
        public bool Execute(string[] args);
    }
}
