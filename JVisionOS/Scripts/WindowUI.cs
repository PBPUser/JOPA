using Godot;
using JLeb.Estragonia;
using JVOS.ApplicationAPI.Windows;
using System;

public partial class WindowUI : AvaloniaControl
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
