using Avalonia.Controls;
using JVOS.ApplicationAPI;

namespace JVOS.Controls
{
    public partial class TaskbarItemContextMenu : UserControl
    {
        public TaskbarItemContextMenu()
        {
            InitializeComponent();
            clBtn.Click += (a, b) =>
            {
                if (jWindowFrame != null)
                    jWindowFrame.Close();
            };
        }

        public IJWindowFrame jWindowFrame;
    }
}
