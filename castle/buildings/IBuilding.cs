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

        int Width { get; }

        int Height { get; }

        int CastleX { get; }

        int CastleZ { get; }

        bool Damage(int amount);

        void Place();
    }
}