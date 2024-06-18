using Avalonia;
using Godot;
using JLeb.Estragonia;
using System;
using JVOS;
using JVOS.ViewModels;
using JVOS.Views;
using System.Diagnostics;

public partial class UserInterface : JLeb.Estragonia.AvaloniaControl
{
    [STAThread]
    public override void _Ready()
    {
        AppBuilder.Configure<App>()
            .UseGodot()
            .SetupWithoutStarting()
            .WithInterFont();
        Control = new MainView() {
            DataContext = new MainViewModel()
        };
        base._Ready();
        ((App)App.Current).IsGodot = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("toggle_jvos"))
            Visible = !Visible;
        Debug.WriteLine("Input");
        base._Input(@event);
    }
}
