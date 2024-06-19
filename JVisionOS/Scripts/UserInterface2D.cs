using Avalonia.Controls;
using Godot;
using System;

public partial class UserInterface2D : JLeb.Estragonia.AvaloniaControl
{
	public static UserInterface2D Instance;
	public Grid Grid;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Grid = new();
		Control = Grid;
		Instance = this;
		Visible = false;
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
