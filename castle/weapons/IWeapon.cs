using Godot;
using System;

public class BulletData
{
    public Vector3 Position { get; set; }

    public Vector3 Force { get; set; }

    public Action<BulletHitInfo> OnHit { get; set; }
}

public interface IWeapon
{
    Vector3 Position { get; }

    float RotationY { get; }

    int Power { get; set; }

    int ShootCost { get; }

    int ReloadPercentage { get; }

    bool CanShoot { get; }

    Vector3? LastTargetHitPosition { get; }

    void RotateLeft(float delta);

    void RotateRight(float delta);

    BulletData Shoot();
}