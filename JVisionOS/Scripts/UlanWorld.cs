using Godot;
using System;

public partial class UlanWorld : Node3D
{
	public static UlanWorld Ulan;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Ulan = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("create_qube"))
        {
			var scene = GD.Load<PackedScene>("res://Prefabs/qube.tscn");
			var qube = scene.Instantiate<RigidBody3D>();
			this.AddChild(qube);
        }
    }
}
