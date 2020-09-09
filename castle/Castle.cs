using Godot;
using System.Linq;
using System.Collections.Generic;
using Ballerburg.castle.buildings;

public class Castle : Spatial
{
    private Spatial _buildingsParent; 
    private Spatial _weaponsParent;

    public const int CellsX = 34;
    public const int CellsZ = 24;

    public List<IWeapon> Weapons { get; private set; }
    public List<IBuilding> Buildings { get; private set; }
    
    public override void _Ready()
    {
        _buildingsParent = GetNode<Spatial>("buildings");
        _weaponsParent = GetNode<Spatial>("weapons");

        Weapons = _weaponsParent.GetChildren().OfType<IWeapon>().ToList();
        Buildings = _buildingsParent.GetChildren().OfType<IBuilding>().ToList();

        Buildings.ForEach(b => {
            b.Place();
            GD.Print(b.CastleX, ":", b.CastleZ);
        });
    }

    public Vector3 GetCenter()
    {
        return GlobalTransform.origin + new Vector3(CellsX * 0.5f, 0.0f, CellsZ * 0.5f);
    }
}
