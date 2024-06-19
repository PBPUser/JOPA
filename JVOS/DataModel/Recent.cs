using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.DataModel
{
    public struct Recent
    {
        public string Path;
        public string Title;
        public DateTime Date;

        public Recent(string path, string title)
        {
            Path = path;
            Title = title;
            Date = DateTime.UtcNow;
        }
    }
}
