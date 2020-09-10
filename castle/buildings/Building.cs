using Ballerburg.castle.buildings;
using Godot;
using System;

public class Building : StaticBody, IBuilding
{
    [Export] public int MaxHitPoints { get; set; } = 10;
    [Export] public int Width { get; set; } = 2;
    [Export] public int Height { get; set; } = 2;
    [Export] public BuildingType BuildingType { get; set; } = BuildingType.FarmerHouse;
    [Export] public float BuildTimeSeconds { get; set; } = 3f;
    [Export] public float BuildingHeight { get; set; } = 2f;

    private int _hitPoints;
    private float _remainingBuildTime;

    public int HealthPercentage => (int)Math.Round((float)_hitPoints / (float)MaxHitPoints * 100.0f, MidpointRounding.AwayFromZero);

    public int CastleX { get; private set; }
    
    public int CastleZ { get; private set; }

    public override void _Ready()
    {
        _hitPoints = MaxHitPoints;
    }

    public override void _Process(float delta)
    {
        if (_remainingBuildTime > 0f)
        {
            _remainingBuildTime = Math.Max(0, _remainingBuildTime - delta);

            var translation = Translation;
            translation.y = -BuildingHeight  + (BuildTimeSeconds - _remainingBuildTime) / BuildTimeSeconds * BuildingHeight;
            Translation = translation;
        }
    }

    public bool Damage(int amount)
    {
        _hitPoints = Math.Max(0, _hitPoints - amount);
        return _hitPoints == 0;
    }

    public void MoveTo(Vector3 localCoords)
    {
        Translation = localCoords;
        UpdateAndClampCastleAndWorldPosition();
    }

    public void Place()
    {
        UpdateAndClampCastleAndWorldPosition();
    }

    private void UpdateAndClampCastleAndWorldPosition()
    {
        CastleX = (int)Math.Round(Translation.x, MidpointRounding.AwayFromZero);
        CastleZ = (int)Math.Round(Translation.z, MidpointRounding.AwayFromZero);

        CastleX = Mathf.Clamp(CastleX, 0, Castle.CellsX - Width);
        CastleZ = Mathf.Clamp(CastleZ, 0, Castle.CellsZ - Height);

        var translation = Translation;
        translation.x = CastleX;
        translation.z = CastleZ;
        Translation = translation;
    }

    public void Build()
    {
        _remainingBuildTime = BuildTimeSeconds;
    }
}
