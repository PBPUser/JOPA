using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public interface IWindowSpace
    {
        public void OpenWindow(IJWindowFrame window);
        public void CloseWindow(IJWindowFrame window);
        public void BringToFront(IJWindowFrame window);
    }
}
