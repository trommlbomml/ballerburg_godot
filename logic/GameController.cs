using Godot;
using System;

public class GameController : Spatial
{
    [Export] public NodePath CameraPath { get; set; }
    [Export] public NodePath CastlePath { get; set; }
    [Export] public NodePath BulletsPath { get; set; }
    [Export] public NodePath WeaponViewPath { get; set; }

    private Castle _castle;
    private CameraController _cameraController;
    private Bullets _bullets;
    private WeaponView _weaponView;
    private Timer _inputTimer;

    private IWeapon _activeWeapon;
    private int _powerDirection = 0;

    public override void _Ready()
    {
        _cameraController = GetNode<CameraController>(CameraPath);
        _castle = GetNode<Castle>(CastlePath);
        _bullets = GetNode<Bullets>(BulletsPath);
        _weaponView = GetNode<WeaponView>(WeaponViewPath);
        _inputTimer = GetNode<Timer>("input_timer");

        _bullets.BulletHitsTarget += OnBulletHitTarget;
        System.Diagnostics.Debug.WriteLine(_castle.Weapons == null);
        var w = _castle.Weapons[0];
        SetActiveWeapon(_castle.Weapons[0], false);
    }

    public override void _Process(float delta)
    {
        HandleWeaponControlInput(delta);
    }

    private void OnInputTimerTimeout()
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.Power += _powerDirection;
        }
    }

    private void HandleWeaponControlInput(float delta)
    {
        if (_activeWeapon == null) return;

        if (_castle.Weapons.Count > 1)
        {
            if (Input.IsActionJustPressed("next_weapon"))
            {
                var index = _castle.Weapons.IndexOf(_activeWeapon);
                var newIndex = index == _castle.Weapons.Count-1 ? 0 : index+1;
                SetActiveWeapon(_castle.Weapons[newIndex]);

            }
            else if (Input.IsActionJustPressed("previous_weapon"))
            {
                var index = _castle.Weapons.IndexOf(_activeWeapon);
                var newIndex = index == 0 ? _castle.Weapons.Count-1 : index-1;
                SetActiveWeapon(_castle.Weapons[newIndex]);
            }
        }
        
        if (Input.IsActionPressed("weapon_rotate_left"))
        {
            _activeWeapon.RotateLeft(delta);
        }
        else if (Input.IsActionPressed("weapon_rotate_right"))
        {
            _activeWeapon.RotateRight(delta);
        }

        if (Input.IsActionJustPressed("weapon_increase_power"))
        {
            _inputTimer.Stop();
            _inputTimer.Start();
            _powerDirection = 1;
            _activeWeapon.Power += _powerDirection;
        }
        else if (Input.IsActionJustPressed("weapon_decrease_power"))
        {
            _inputTimer.Stop();
            _inputTimer.Start();
            _powerDirection = -1;
            _activeWeapon.Power += _powerDirection;
        }
        else
        {
            if (!Input.IsActionPressed("weapon_increase_power") 
                && !Input.IsActionPressed("weapon_decrease_power")
                && !_inputTimer.IsStopped())
            {
                _inputTimer.Stop();
            }
        }
        if (Input.IsActionJustPressed("weapon_shoot") && _activeWeapon.CanShoot)
        {
            var bullet = _activeWeapon.Shoot();
            _bullets.SpawnBullet(bullet);
        }
    }

    private void SetActiveWeapon(IWeapon weapon, bool animate = true)
    {
        _cameraController.AttachTo(weapon, animate);
        _activeWeapon = weapon;
        _inputTimer.Stop();
        _weaponView.ShowWeapon(weapon);
    }

    private void OnBulletHitTarget(BulletHitInfo hitInfo)
    {
        var buildingHit = hitInfo.Building?.BuildingType;
        System.Diagnostics.Debug.WriteLine($"Hitting {hitInfo.Building?.BuildingType.ToString() ?? "Nothing"} at {hitInfo.WorldCoords}");
    }
}
