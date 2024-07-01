using Godot;
using JLeb.Estragonia;
using JVOS.ApplicationAPI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVisionOS.Scripts
{
    public partial class WindowFrameInterface : AvaloniaControl
    {
        [STAThread]
        public override void _Ready()
        {
            base._Ready();
        }

        public void SetWindow(WindowFrameBase window)
        {
            Control = window;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }
    }
}
