using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.DataModel
{
    public class Shortcut
    {
        public Shortcut(string command, string name, string base64image, string args = "")
        {
            Command = command;
            Name = name;
            Base64Image = base64image;
            Arguments = args;
        }

        public string Command;
        public string Name;
        public string Base64Image;
        public string Arguments;
    }
}
