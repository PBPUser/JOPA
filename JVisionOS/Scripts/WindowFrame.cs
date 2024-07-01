using Godot;
using JVisionOS.Scripts;
using JVOS.ApplicationAPI.Windows;
using System;

public partial class WindowFrame : Node3D
{
    [Export]
    SubViewport subViewport;
    [Export]
    public Control Control;
    [Export]
    public MeshInstance3D Quad;

	public WindowFrameInterface Interface;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Interface = new();
		Control = Interface;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetWindow(WindowFrameBase window)
	{
		Interface.SetWindow(window);
	}
}
