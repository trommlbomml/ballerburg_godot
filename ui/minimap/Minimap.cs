using Ballerburg.castle.buildings;
using Godot;

public class Minimap : TextureRect
{
    [Export] public int TileWidth { get; set; } = 5;
    [Export] public int TileHeight { get; set; } = 5;

    [Export] public PackedScene Tile1x1 { get; set; }
    [Export] public PackedScene Tile2x2 { get; set; }
    [Export] public PackedScene Tile4x4 { get; set; }

    public void RefreshFromCastle(Castle castle)
    {
        Clear();
        foreach(var building in castle.Buildings)
        {
            var sprite = CreateSpriteFromBuilding(building);
            sprite.Position = new Vector2(building.CastleX * TileWidth, building.CastleZ * TileHeight);
            AddChild(sprite);
        }
    }

    public void Clear()
    {
        foreach(Node child in GetChildren())
        {
            child.QueueFree();
        }
    }

    private Sprite CreateSpriteFromBuilding(IBuilding building)
    {
        if (building.Width == 2 && building.Height == 2) return (Sprite)Tile2x2.Instance();
        if (building.Width == 4 && building.Height == 4) return (Sprite)Tile4x4.Instance();
        return (Sprite)Tile1x1.Instance();
    }
}
