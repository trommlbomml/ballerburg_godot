using Godot;
using System;
using Ballerburg.castle.buildings;

public class FarmerHouse : StaticBody, IBuilding
{
    private int _hitPoints;

    public const int MaxHitPoints = 10;

    public int HealthPercentage => (int)Math.Round((float)_hitPoints / (float)MaxHitPoints * 100.0f, MidpointRounding.AwayFromZero);

    public int Width => 2;

    public int Height => 2;

    public BuildingType BuildingType => BuildingType.FarmerHouse;

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
