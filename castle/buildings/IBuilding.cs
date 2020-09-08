namespace Ballerburg.castle.buildings
{
    public enum BuildingType
    {
        FarmerHouse,
        Wall,
        Weapon
    }

    public interface IBuilding
    {
        BuildingType BuildingType { get; }

        int HealthPercentage { get; }

        bool Damage(int amount);

        int Width { get; }

        int Height { get; }
    }
}