using Godot;
using System;

public partial class Window : Node3D {
    [Export]
    SubViewport subViewport;
    [Export]
    public Control Control;
    [Export]
    public MeshInstance3D Quad;

    [Export]
    public int Width { get => subViewport.Size.X; 
        set { 
            subViewport.Size = subViewport.Size with { X = value };
            CalculateSizes();
        } 
    }
    [Export]
    public int Height {
        get => subViewport.Size.Y;
        set {
            subViewport.Size = subViewport.Size with { Y = value };
            CalculateSizes();
        }
    }
    Area3D area = new();
    CollisionShape3D shape = new() { Shape = new BoxShape3D() };
    CollisionShape3D areaShape = new() { Shape = new BoxShape3D() };

    Vector2 quadSize;
    bool isMouseHeld = false;
    bool isMouseInside = false;
    Vector3? lastMousePosition3D;
    Vector2 lastMousePosition2D = Vector2.Zero;

    public override void _Ready() {
        area.AddChild(areaShape);
        area.MouseEntered += MouseEnteredArea;
        Control.Reparent(subViewport);

        CalculateSizes();
    }

    private void CalculateSizes() {
        var mesh = (QuadMesh)Quad.Mesh;

        var material = new StandardMaterial3D {
            AlbedoTexture = subViewport.GetTexture(),
            CullMode = BaseMaterial3D.CullModeEnum.Disabled
        };

        mesh.Material = material;

        mesh.Size = new Vector2(subViewport.Size.X / 1024f, subViewport.Size.Y / 1024f);

        ((BoxShape3D)shape.Shape).Size = new Vector3(mesh.Size.X, mesh.Size.Y, .1f);
        ((BoxShape3D)areaShape.Shape).Size = new Vector3(mesh.Size.X, mesh.Size.Y, .3f);
    }

    public override void _ExitTree() {
        area.MouseEntered -= MouseEnteredArea;
    }

    private void MouseEnteredArea() {
        isMouseInside = true;
    }

    public override void _UnhandledInput(InputEvent e) {
        if(e is InputEventMouse mouseEvent && (isMouseInside || isMouseHeld)) {
            HandleMouse((InputEventMouse)e);
        } else {
            subViewport.PushInput(e);
        }
    }

    private void HandleMouse(InputEventMouse e) {
        isMouseInside = FindMouse(e.GlobalPosition, out Vector3 position);

        HandleMouseInPosition(e, position);
    }

    public void HandleSynteticMouseMotion(Vector3 position) {
        var ev = new InputEventMouseMotion();

        isMouseInside = true;

        HandleMouseInPosition(ev, position);
    }

    public void HandleSynteticMouseClick(Vector3 position, bool pressed) {
        var ev = new InputEventMouseButton() { ButtonIndex = MouseButton.Left, Pressed = pressed };

        isMouseInside = true;

        HandleMouseInPosition(ev, position);
    }

    private void HandleMouseInPosition(InputEventMouse e, Vector3 position) {
        quadSize = ((QuadMesh)Quad.Mesh).Size;

        if(e is InputEventMouseButton) {
            isMouseHeld = e.IsPressed();
        }

        Vector3 mousePosition3D;

        if(isMouseInside) {
            mousePosition3D = area.GlobalTransform.AffineInverse() * position;
            lastMousePosition3D = mousePosition3D;
        } else {
            if(lastMousePosition3D != null) {
                mousePosition3D = (Vector3)lastMousePosition3D;
            } else {
                mousePosition3D = Vector3.Zero;
            }
        }

        var mousePosition2D = new Vector2(mousePosition3D.X, -mousePosition3D.Y);

        mousePosition2D.X += quadSize.X / 2;
        mousePosition2D.Y += quadSize.Y / 2;

        mousePosition2D.X /= quadSize.X;
        mousePosition2D.Y /= quadSize.Y;

        mousePosition2D.X *= subViewport.Size.X;
        mousePosition2D.Y *= subViewport.Size.Y;

        e.Position = mousePosition2D;
        e.GlobalPosition = mousePosition2D;

        if(e is InputEventMouseMotion mouseEvent) {
            mouseEvent.Relative = mousePosition2D - lastMousePosition2D;
        }

        lastMousePosition2D = mousePosition2D;

        subViewport.PushInput(e);
    }

    private bool FindMouse(Vector2 globalPosition, out Vector3 position) {
        var camera = GetViewport().GetCamera3D();

        var from = camera.ProjectRayOrigin(globalPosition);
        var dist = FindFurtherDistanceTo(camera.Transform.Origin);
        var to = from + camera.ProjectRayNormal(globalPosition) * dist;

        var parameters = new PhysicsRayQueryParameters3D() { From = from, To = to, CollideWithAreas = true, CollisionMask = area.CollisionLayer, CollideWithBodies = false };

        var result = GetWorld3D().DirectSpaceState.IntersectRay(parameters);

        position = Vector3.Zero;

        if(result.Count > 0) {
            position = (Vector3)result["position"];

            return true;
        } else {
            return false;
        }
    }

    private float FindFurtherDistanceTo(Vector3 origin) {
        Vector3[] edges = new Vector3[] {
            area.ToGlobal(new Vector3(quadSize.X / 2, quadSize.Y / 2, 0)),
            area.ToGlobal(new Vector3(quadSize.X / 2, -quadSize.Y / 2, 0)),
            area.ToGlobal(new Vector3(-quadSize.X / 2, quadSize.Y / 2, 0)),
            area.ToGlobal(new Vector3(-quadSize.X / 2, -quadSize.Y / 2, 0)),
        };

        float farDistance = 0;

        foreach(var edge in edges) {
            var tempDistance = origin.DistanceTo(edge);

            if(tempDistance > farDistance) {
                farDistance = tempDistance;
            }
        }

        return farDistance;
    }
}
