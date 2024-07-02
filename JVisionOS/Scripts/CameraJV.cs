using Godot;
using System;

public partial class CameraJV : Godot.Camera3D
{
	[Export]
	public RayCast3D Raycast;

    public static CameraJV Instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        
    }
}
