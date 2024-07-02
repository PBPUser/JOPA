using Avalonia.Controls;
using Avalonia.VisualTree;
using Godot;
using JVOS.ApplicationAPI;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody3D
{
	public static Player Instance;

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
    public float CameraSpeed = 0.01f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready() {
		Instance = this;
        Communicator.WindowSwitching += Communicator_WindowSwitching;
	}

    private void Communicator_WindowSwitching(object? sender, JVOS.ApplicationAPI.Windows.WindowFrameBase e)
    {
        e.GetVisualParent<Grid>().Children.Remove(e);
		var windowScene = GD.Load<PackedScene>("res://Prefabs/WindowFrame.tscn");
		var window = windowScene.Instantiate<WindowFrame>();
        window.Position = new Vector3(0, 1, 0);
        UlanWorld.Ulan.AddChild(window);
        window.SetWindow(e);
    }

    Vector2 cameraPC = new();

    public override void _Input(InputEvent @event)
    {
        if (isMouseCaptured && @event is InputEventMouseMotion e)
            cameraPC -= e.Relative;
        if (Input.IsActionJustPressed("toggle_mouse_capture"))
            ToggleMouseCapture();

        base._Input(@event);
    }

    bool isMouseCaptured = false;
    float cameraRotationX;
    private void ToggleMouseCapture()
    {
        isMouseCaptured = !isMouseCaptured;
        PlayerUI.Instance.Visible = isMouseCaptured;
        if (isMouseCaptured)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
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
        if (Input.IsActionJustPressed("return_to_spawn"))
            Position = new Vector3(0, 0, 0);

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
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
        
        RotationDegrees = RotationDegrees with { 
            X = Mathf.Clamp((cameraPC.Y) * (float)delta * CameraSpeed + RotationDegrees.X, -95, 140),
            Y = Mathf.Wrap((cameraPC.X) * (float)delta * CameraSpeed * (RotationDegrees.X > 90 ? -1 : 1) + RotationDegrees.Y, 0, 360)
        };
        Velocity = velocity;
		MoveAndSlide();
	}
}
