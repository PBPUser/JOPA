using Godot;
using System;

public partial class CameraJV : Camera3D {
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;
    public float CameraSpeed = 3.2f;
    [Export]
	public RayCast3D Raycast;

    Vector2 cameraPC = new();

    bool isMouseCaptured = false;

    public override void _Input(InputEvent @event) {
        if(isMouseCaptured && @event is InputEventMouseMotion e)
            cameraPC -= e.Relative;
        if(Input.IsActionJustPressed("toggle_mouse_capture"))
            ToggleMouseCapture();

        base._Input(@event);
    }
    private void ToggleMouseCapture() {
        isMouseCaptured = !isMouseCaptured;
        PlayerUI.Instance.Visible = isMouseCaptured;
        if(isMouseCaptured) {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        } else {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta) {
        RotationDegrees = RotationDegrees with {
            X = Mathf.Clamp(cameraPC.Y * (float)delta * CameraSpeed + RotationDegrees.X, -90, 90),
            Y = Mathf.Wrap(cameraPC.X * (float)delta * CameraSpeed + RotationDegrees.Y, 0, 360)
        };
        cameraPC = Vector2.Zero;

        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized() * (float)delta;
        Position += direction;
    }
}
