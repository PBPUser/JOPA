using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Controls
{
    public class AppRunButtton : JButton
    {
        public (Bitmap?, string, string, string) Runner;

        protected override void OnClick()
        {
            ApplicationManager.Run(Runner);
            base.OnClick();
        }
    }
}
