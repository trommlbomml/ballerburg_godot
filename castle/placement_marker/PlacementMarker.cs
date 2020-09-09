using Ballerburg.castle.buildings;
using Godot;

public class PlacementMarker : Spatial
{
    private MeshInstance _meshInstance;

    [Export] public Material AllowMaterial { get; set; }
    [Export] public Material DenyMaterial { get; set; }
    
    public override void _Ready()
    {
        _meshInstance = GetNode<MeshInstance>("MeshInstance");
    }

    public void SetAllow()
    {
        _meshInstance.SetSurfaceMaterial(0, AllowMaterial);
    }

    public void SetDeny()
    {
        _meshInstance.SetSurfaceMaterial(0, DenyMaterial);
    }

    public void AttachTo(IBuilding building)
    {
        Scale = new Vector3(building.Width, 1.0f, building.Height);
    }
}
