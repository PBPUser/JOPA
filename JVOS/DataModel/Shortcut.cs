using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.DataModel
{
    public class Shortcut
    {
        public Shortcut(string command, string base64image, string args = "", string description = "")
        {
            Command = command;
            Base64Image = base64image;
            Arguments = args;
            Description = description;
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }

        public static Shortcut Load(string path)
        {
            return JsonConvert.DeserializeObject<Shortcut>(File.ReadAllText(path)) ?? new Shortcut("", "", "");
        }

        public string Command;
        public string Base64Image;
        public string Description = "";
        public string Arguments;
    }
}
