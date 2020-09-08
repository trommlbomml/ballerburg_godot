using Ballerburg.castle.buildings;
using Godot;
using System;

public class BulletHitInfo
{
    public Vector3 WorldCoords { get; set; }

    public IBuilding Building { get; set; }
}

public class Bullets : Spatial
{
    [Export] public PackedScene BulletScene { get; set; }
    [Export] public PackedScene HitHoleScene { get; set; }

    public void SpawnBullet(BulletData data)
    {
        var bullet = (Bullet)BulletScene.Instance();
        bullet.Translation = data.Position;
        AddChild(bullet);
        bullet.ApplyImpulse(data.Position, data.Force);
        bullet.TargetHit += OnTargetHit;
        bullet.TargetHit += data.OnHit;
    }

    public event Action<BulletHitInfo> BulletHitsTarget;

    private void OnTargetHit(BulletHitInfo hitInfo)
    {
        var hitHole = (Spatial)HitHoleScene.Instance();
        AddChild(hitHole);
        hitHole.Translation = hitInfo.WorldCoords;

        BulletHitsTarget?.Invoke(hitInfo);
    }
}
