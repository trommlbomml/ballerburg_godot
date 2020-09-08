using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class Castle : Spatial
{
    private Spatial _buildingsParent; 
    private Spatial _weaponsParent;

    public const int CellsX = 34;
    public const int CellsZ = 24;

    public List<IWeapon> Weapons { get; private set; }
    
    public override void _Ready()
    {
        _buildingsParent = GetNode<Spatial>("buildings");
        _weaponsParent = GetNode<Spatial>("weapons");

        Weapons = _weaponsParent.GetChildren().OfType<IWeapon>().ToList();
    }
}
