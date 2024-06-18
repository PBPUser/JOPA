using Avalonia.Input;
using JVOS.ApplicationAPI.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI.Windows
{
    public struct WindowOpenRequest
    {
        public WindowContentBase Window;
        public AppCommunicator Communicatior;

        public WindowOpenRequest(WindowContentBase window, AppCommunicator communicator)
        {
            Window = window;
            Communicatior = communicator;
        }
    }
}
