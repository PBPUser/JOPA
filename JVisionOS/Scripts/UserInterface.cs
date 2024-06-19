using Avalonia;
using Godot;
using JLeb.Estragonia;
using System;
using JVOS;
using JVOS.ViewModels;
using JVOS.Views;
using System.Diagnostics;
using Avalonia.Controls;

public partial class UserInterface : JLeb.Estragonia.AvaloniaControl
{
    public static UserInterface Instance;
    public MainView MainView;
    public Grid Grid;

    [STAThread]
    public override void _Ready()
    {
        Instance = this;
        AppBuilder.Configure<App>()
            .UseGodot()
            .SetupWithoutStarting()
            .WithInterFont();
        Grid = new();
        MainView = new MainView() {
            DataContext = new MainViewModel()
        };
        Grid.Children.Add(MainView);
        Control = Grid;
        base._Ready();
        ((App)App.Current).IsGodot = true;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
    }
}
