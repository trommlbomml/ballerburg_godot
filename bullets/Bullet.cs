using Ballerburg.castle.buildings;
using Godot;
using System;

public class Bullet : RigidBody
{
    public event Action<BulletHitInfo> TargetHit;

    private Vector3 _worldCoords;

    public void OnBodyEntered(Node body)
    {
        TargetHit?.Invoke(new BulletHitInfo
        {
            WorldCoords = _worldCoords,
            Building = body as IBuilding
        });
        _worldCoords = Vector3.Zero;
        QueueFree();
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        if (state.GetContactCount() > 0)
        {
            var colliderObject = (Spatial)state.GetContactColliderObject(0);
            _worldCoords =  colliderObject.GlobalTransform.origin + state.GetContactLocalPosition(0);  
        }
    }
}
