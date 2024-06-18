using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.App;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Protocol
{
    public class AppProtocol: IProtocol
    {
        public string Name => "app";

        public bool Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Communicator.ShowMessageDialog(new MessageDialog("App protocol", "2 args required"));
                return false;
            }
            if (!Directory.Exists(args[0]))
            {
                Communicator.ShowMessageDialog(new MessageDialog("App protocol", "Directory not exists"));
                return false;
            }
            if (!File.Exists(args[0] + "\\" + "manifest.jvon"))
            {
                Communicator.ShowMessageDialog(new MessageDialog("App protocol", "Manifest not exists"));
                return false;
            }
            var manifest = JsonConvert.DeserializeObject<AppManifest>(File.ReadAllText(args[0]+"\\manifest.jvon"));
            if (manifest == null)
            {
                Communicator.ShowMessageDialog(new MessageDialog("App protocol", "Invalid manifest"));
                return false;
            }
            if (manifest.Activities.Contains(args[1]))
            {
                if (AppCommunicator.OpenApplication(args[0], args[1]))
                    return true;
                Communicator.ShowMessageDialog(new MessageDialog("App protocol", "Failed to run application"));
                return false;
            }
            else
            {
                Communicator.ShowMessageDialog(new MessageDialog("App protocol", "Manifest dosen't contain specified activity"));
                return false;
            }
        }
    }
}
