using Avalonia.Controls;
using Godot;
using JVOS.ApplicationAPI;
using System;

public partial class Player : CharacterBody3D
{
	public static Player Instance;

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready() {
		Instance = this;
        Communicator.WindowSwitching += Communicator_WindowSwitching;
	}

    private void Communicator_WindowSwitching(object? sender, JVOS.ApplicationAPI.Windows.WindowFrameBase e)
    {
		((Grid)e.Parent).Children.Remove(e);
		var window = new WindowFrame();
		GetParent().AddChild(window);
		window.Position = Position + new Vector3(0, 0, 1);
		window.SetWindow(e);
    }

    public override void _Input(InputEvent @event)
    {
		var vec = Input.GetVector("left", "right", "forward", "backward");
        base._Input(@event);
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
