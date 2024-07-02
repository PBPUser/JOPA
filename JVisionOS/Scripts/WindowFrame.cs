using Avalonia.Controls.Shapes;
using Godot;
using JVOS.ApplicationAPI.Windows;
using System;
using System.Reflection.Metadata;

public partial class WindowFrame : Node3D
{
    public WindowFrame()
    {
        var scene = GD.Load<PackedScene>("res://Prefabs/WindowUI.tscn");
        Interface = scene.Instantiate<WindowUI>();
        Control = Interface;
    }

    [Export]
    SubViewport subViewport;
    [Export]
    public Control Control;
    [Export]
    public MeshInstance3D Quad;
    [Export]
    public int Width
    {
        get => subViewport.Size.X;
        set
        {
            subViewport.Size = subViewport.Size with { X = value };
            CalculateSizes();
        }
    }
    [Export]
    public int Height
    {
        get => subViewport.Size.Y;
        set
        {
            subViewport.Size = subViewport.Size with { Y = value };
            CalculateSizes();
        }
    }
    private void CalculateSizes()
    {
        var mesh = (QuadMesh)Quad.Mesh;

        var material = new StandardMaterial3D
        {
            AlbedoTexture = subViewport.GetTexture(),
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled
        };

        mesh.Material = material;

        mesh.Size = new Vector2(subViewport.Size.X / 1024f, subViewport.Size.Y / 1024f);

        ((BoxShape3D)shape.Shape).Size = new Vector3(mesh.Size.X, mesh.Size.Y, .1f);
        ((BoxShape3D)areaShape.Shape).Size = new Vector3(mesh.Size.X, mesh.Size.Y, .3f);
    }

    CollisionShape3D shape = new() { Shape = new BoxShape3D() };
    CollisionShape3D areaShape = new() { Shape = new BoxShape3D() };
    Area3D area = new();
    public WindowUI Interface;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        area.AddChild(areaShape);
        subViewport.AddChild(Control);
        CalculateSizes();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetWindow(WindowFrameBase window)
	{
		Interface.SetWindow(window);
        Width = (int)window.Bounds.Width;
        Height = (int)window.Bounds.Height;
	}
}
