using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.DataModel
{
    public struct Language
    {
        public string Name;
        public string Description;
        
        public Language()
        {
            Name = "mi";
            Description = "Missing Description";
        }

        public Language(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
