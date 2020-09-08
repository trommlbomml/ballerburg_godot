using Godot;
using System;

public class Cannon : Spatial, IWeapon
{
    public const float RotationSpeed = (float)Math.PI / 4;
    public const float ReloadSpeedSeconds = 5.0f;

    private Spatial _rotatorY;
    private Position3D _spawnPoint;
    private int _power = 40;
    private float _remainingReloadTimeSeconds = 0.0f;

    public Vector3 Position => GlobalTransform.origin;
    
    public float RotationY => _rotatorY.Rotation.y;
    
    public Vector3? LastTargetHitPosition { get; private set; }

    public int Power 
    { 
        get => _power;
        set => _power = Math.Max(10, Math.Min(50, value));
    }

    public int ReloadPercentage => (int)Math.Round((ReloadSpeedSeconds - _remainingReloadTimeSeconds) / ReloadSpeedSeconds * 100.0f, MidpointRounding.AwayFromZero);

    public int ShootCost => 50;

    public bool CanShoot => ReloadPercentage == 100;

    public override void _Ready()
    {
        _rotatorY = GetNode<Spatial>("rotator_y");
        _spawnPoint = GetNode<Position3D>("rotator_y/rotator_x/bullet_spawn");
    }

    public override void _Process(float delta)
    {
        _remainingReloadTimeSeconds = Math.Max(0, _remainingReloadTimeSeconds - delta);
    }

    public void RotateLeft(float delta)
    {
        _rotatorY.RotateY(RotationSpeed * delta);
    }

    public void RotateRight(float delta)
    {
        _rotatorY.RotateY(-RotationSpeed * delta);
    }

    public BulletData Shoot()
    {
        _remainingReloadTimeSeconds = ReloadSpeedSeconds;
        return new BulletData
        {
            Position = _spawnPoint.GlobalTransform.origin,
            Force = -_spawnPoint.GlobalTransform.basis.z * Power,
            OnHit = OnBullitHit
        };
    }

    private void OnBullitHit(BulletHitInfo hitInfo)
    {
        LastTargetHitPosition = hitInfo.WorldCoords;
    }
}
