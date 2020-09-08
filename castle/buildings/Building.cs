using Ballerburg.castle.buildings;
using Godot;
using System;

public class Building : StaticBody, IBuilding
{
    [Export] public int MaxHitPoints { get; set; } = 10;
    [Export] public int Width { get; set; } = 2;
    [Export] public int Height { get; set; } = 2;
    [Export] public BuildingType BuildingType { get; set; } = BuildingType.FarmerHouse;

    private int _hitPoints;

    public int HealthPercentage => (int)Math.Round((float)_hitPoints / (float)MaxHitPoints * 100.0f, MidpointRounding.AwayFromZero);

    public override void _Ready()
    {
        _hitPoints = MaxHitPoints;
    }

    public bool Damage(int amount)
    {
        _hitPoints = Math.Max(0, _hitPoints - amount);
        return _hitPoints == 0;
    }
}
