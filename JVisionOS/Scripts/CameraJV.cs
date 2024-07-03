using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Godot;
using JLeb.Estragonia;
using JLeb.Estragonia.Input;
using JVisionOS.Scripts;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reflection;

public partial class CameraJV : Camera3D {
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;
    public float CameraSpeed = 3.2f;


    [Export]
	public RayCast3D Raycast;

    Vector2 cameraPC = new();

    WindowFrame? CurrentFrame;
    Window? Window;

    bool isMouseCaptured = false;

    public override void _Input(InputEvent @event) {
        if(isMouseCaptured && @event is InputEventMouseMotion e)
        {
            cameraPC -= e.Relative;

        }
        if (Window != null)
        {
            if (Window.Control is not UserInterface ui)
                return;
            var relativePoint = (Raycast.GetCollisionPoint() - Window.GlobalPosition) / ((BoxShape3D)Window.Collision.Shape).Size + new Vector3(.5f, .5f, 0);
            var cursorPosition = new Avalonia.Point(ui.Control.Bounds.Size.Width * relativePoint.X, ui.Control.Bounds.Size.Height - (ui.Control.Bounds.Size.Height * relativePoint.Y));
            if (@event is InputEventMouseButton emb)
                EstragoniaTopLevelHelper.SimulateMouseAction(ui, emb, cursorPosition);
            else if (@event is InputEventMouseMotion emm)
                EstragoniaTopLevelHelper.SimulateMouseMove(ui, emm, cursorPosition);
        }
        if(Input.IsActionJustPressed("toggle_mouse_capture"))
            ToggleMouseCapture();
        if (Input.IsActionJustPressed("create_qube"))
        {
            var scene = GD.Load<PackedScene>("res://Prefabs/qube.tscn");
            var qube = scene.Instantiate<RigidBody3D>();
            qube.Position = Raycast.TargetPosition;
            this.AddChild(qube);
        }
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

        processRaycast();
        
    }

    void processRaycast()
    {
        Window = null;
        CurrentFrame = null;
        var collider = Raycast.GetCollider();
        if(collider != null)
        {
            
            PlayerUI.Instance.Label.Text = collider.GetClass();
            if(collider is Window w)
            {
                Window = w;
                var relativePoint = (Raycast.GetCollisionPoint() - w.GlobalPosition) / ((BoxShape3D)w.Collision.Shape).Size + new Vector3(.5f, .5f, 0);
                PlayerUI.Instance.Label.Text = $"Window Detected {relativePoint} {w.Control.GetClass()}";
                if(w.Control is UserInterface ui)
                {
                    var cursorPosition = new Avalonia.Point(ui.Control.Bounds.Size.Width * relativePoint.X, ui.Control.Bounds.Size.Height * relativePoint.Y);
                    PlayerUI.Instance.Label.Text += $"\nUser interface: {cursorPosition}";

                    if (Input.IsActionJustPressed("move_window"))
                    {
                        MovementStructure = new()
                        {
                            ObjectToMove = w,
                            RayTargetStartPosition = Raycast.GetCollisionPoint(),
                            ObjectTargetStartPosition = w.GlobalPosition,
                            RayTargetStartRotation = Raycast.GlobalRotation,
                            ObjectTargetStartRotation = w.GlobalRotation
                        };
                    }
                }

                
            }
            else if(collider is WindowFrame e)
            {
                CurrentFrame = e;
                if (e.Control is WindowUI ui)
                {
                    PlayerUI.Instance.Label.Text = $"WindowFrame Detected";
                    if (Input.IsActionJustPressed("move_window"))
                    {
                        MovementStructure = new()
                        {
                            ObjectToMove = e,
                            RayTargetStartPosition = Raycast.GetCollisionPoint(),
                            ObjectTargetStartPosition = e.GlobalPosition,
                            RayTargetStartRotation = Raycast.GlobalRotation,
                            ObjectTargetStartRotation = e.GlobalRotation
                        };
                    }
                }
            }

        }
        else
            PlayerUI.Instance.Label.Text = "Colider not detected";
        if (Input.IsActionJustReleased("move_window"))
            MovementStructure = null;
        if(MovementStructure != null)
        {
            PlayerUI.Instance.Label.Text += $"\nMovement structure: {MovementStructure.Value.RayTargetStartPosition}, {MovementStructure.Value.ObjectTargetStartPosition}, {MovementStructure.Value.ObjectToMove.GetClass()}";
            MovementStructure.Value.ObjectToMove.GlobalPosition = (Raycast.GetCollisionPoint() - MovementStructure.Value.RayTargetStartPosition) + MovementStructure.Value.ObjectTargetStartPosition;
            MovementStructure.Value.ObjectToMove.GlobalRotation = (Raycast.GlobalRotation - MovementStructure.Value.RayTargetStartRotation) + MovementStructure.Value.ObjectTargetStartRotation;
        }
    }

    

    WindowMovementStructure? MovementStructure = null;

    struct WindowMovementStructure
    {
        public Node3D ObjectToMove;
        public Vector3 RayTargetStartPosition;
        public Vector3 RayTargetStartRotation;
        public Vector3 ObjectTargetStartPosition;
        public Vector3 ObjectTargetStartRotation;

    }
}
