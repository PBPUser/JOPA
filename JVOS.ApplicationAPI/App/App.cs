using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.App
{
    public abstract class App
    {
        public App(AppCommunicator communicator)
        {
            Communicator = communicator;
        }

        public AppCommunicator Communicator;

        public virtual void OnProtocolExec(string name, object[] args)
        {

        }

        public virtual void OnActivity(string name, object[] args)
        {

        }
    }
}
