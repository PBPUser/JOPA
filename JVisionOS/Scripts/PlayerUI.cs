using Godot;
using System;
using System.Diagnostics;

public partial class PlayerUI : Control
{
	[Export]
	public TextureRect Crosshair;

	public static PlayerUI Instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
