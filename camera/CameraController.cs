using Godot;
using System;

public class CameraController : Spatial
{
    [Export] public float AnimationSpeedCannon { get; set; } = 0.5f;
    [Export] public float AnimationSpeedTargetHit { get; set; } = 0.25f;
    
    private static readonly Vector3 DistanceToHitTarget = new Vector3(1, 1, 8);
    private static readonly Vector3 DistanceToWeapon = new Vector3(0.4f, 0.5f, 0.8f);

    private static readonly Vector3 DistanceToCastle = new Vector3(0, 0, 20);

    private Castle _castle;
    private IWeapon _attachedWeapon;
    private Tween _tween;

    private Spatial _rotatorX;
    private Camera _camera;

    public bool IsAnimating { get; private set; }

    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
        _camera = GetNode<Camera>("rotator_x/Camera");
        _rotatorX = GetNode<Spatial>("rotator_x");
    }

    public override void _Process(float delta)
    {
        if (_attachedWeapon != null)
        {
            HandleAttachedToWeapon();
        }
        
        if (_castle != null)
        {
            HandleCastleMode();
        }
    }

    private void HandleAttachedToWeapon()
    {
        if (!IsAnimating)
        {
            Rotation = new Vector3(0, _attachedWeapon.RotationY, 0);
        }

        if (Input.IsActionJustPressed("weapon_show_last_target_hit") && _attachedWeapon.LastTargetHitPosition.HasValue)
        {
            AnimateToTargetPosition(_attachedWeapon.LastTargetHitPosition.Value);
        }
        
        if (Input.IsActionJustReleased("weapon_show_last_target_hit"))
        {
            AnimateToWeapon(_attachedWeapon);
        }
    }

    private void HandleCastleMode()
    {

    }

    private void AnimateToTargetPosition(Vector3 position)
    {
        _camera.Translation = DistanceToHitTarget;
        IsAnimating = true;
        _tween.InterpolateProperty(this, "translation", Translation, position, AnimationSpeedTargetHit, Tween.TransitionType.Linear, Tween.EaseType.In);
        _tween.Start();
    }

    private void AnimateToWeapon(IWeapon weapon)
    {
        _camera.Translation = DistanceToWeapon;
        IsAnimating = true;
        _tween.InterpolateProperty(this, "translation", Translation, _attachedWeapon.Position, AnimationSpeedCannon, Tween.TransitionType.Linear, Tween.EaseType.In);
        _tween.InterpolateProperty(this, "rotation", Rotation, new Vector3(0, _attachedWeapon.RotationY, 0), AnimationSpeedCannon, Tween.TransitionType.Linear, Tween.EaseType.In);
        _tween.Start();
    }

    public void AttachTo(IWeapon weapon, bool animate)
    {
        _attachedWeapon = weapon;
        _castle = null;
        _camera.Translation = DistanceToWeapon;
        if (animate)
        {
            AnimateToWeapon(_attachedWeapon);
        }
        else
        {
            Translation = _attachedWeapon.Position;
        }
    }

    public void SwitchToCastle(Castle castle)
    {
        _attachedWeapon = null;
        _castle = castle;

        _camera.Translation = DistanceToCastle;
    }

    public void OnTweenAllCompleted()
    {
        IsAnimating = false;
    }
}
