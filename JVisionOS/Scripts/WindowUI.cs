using Godot;
using JLeb.Estragonia;
using JVOS.ApplicationAPI.Windows;
using System;
using System.Diagnostics;

public partial class WindowUI : AvaloniaControl
{
    public WindowUI()
    {
        Debug.WriteLine("a new WindowUI created");
    }

    [STAThread]
    public override void _Ready()
    {
        base._Ready();
    }

    public void SetWindow(WindowFrameBase window)
    {
        Debug.WriteLine("Window " + window.Title + " set.");
        Control = window;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
    }
}
