using Godot;
using System;

public class CameraController : Spatial
{
    private const float AnimationSpeed = 0.5f;
    private const float AnimationSpeedTargetHit = 0.25f;
    
    private static readonly Vector3 DistanceToHitTarget = new Vector3(1, 1, 8);
    private static readonly Vector3 DistanceToWeapon = new Vector3(0.4f, 0.5f, 0.8f);

    private IWeapon _attachedWeapon;
    private Tween _tween;
    private Camera _camera;

    public bool IsAnimating { get; private set; }

    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
        _camera = GetNode<Camera>("Camera");
    }

    public override void _Process(float delta)
    {
        if (_attachedWeapon == null) return;
        
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
        _tween.InterpolateProperty(this, "translation", Translation, _attachedWeapon.Position, AnimationSpeed, Tween.TransitionType.Linear, Tween.EaseType.In);
        _tween.InterpolateProperty(this, "rotation", Rotation, new Vector3(0, _attachedWeapon.RotationY, 0), AnimationSpeed, Tween.TransitionType.Linear, Tween.EaseType.In);
        _tween.Start();
    }

    public void AttachTo(IWeapon weapon, bool animate)
    {
        _attachedWeapon = weapon;
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

    public void OnTweenAllCompleted()
    {
        IsAnimating = false;
    }
}
