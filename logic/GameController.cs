using Ballerburg.castle.buildings;
using Godot;

public enum GameState
{
    WeaponModeActive,
    CastleModeActive,
    BuildingActive
}

public class GameController : Spatial
{
    [Export] public NodePath CameraPath { get; set; }
    [Export] public NodePath CastlePath { get; set; }
    [Export] public NodePath BulletsPath { get; set; }
    [Export] public NodePath WeaponViewPath { get; set; }
    [Export] public NodePath MenuViewPath { get; set; }

    [Export] public NodePath CrossHairPath { get; set; }
    [Export] public NodePath OwnMinimapPath { get; set; }

    private GameState _gameState;
    private Castle _castle;
    private CameraController _cameraController;
    private Bullets _bullets;
    private Timer _inputTimer;
    
    private Minimap _ownMinimap;
    private CrossHair _crossHair;
    private WeaponView _weaponView;
    private MenuView _menuView;

    private IWeapon _activeWeapon;
    private int _powerDirection = 0;

    public override void _Ready()
    {
        _cameraController = GetNode<CameraController>(CameraPath);
        _castle = GetNode<Castle>(CastlePath);
        _bullets = GetNode<Bullets>(BulletsPath);
        _weaponView = GetNode<WeaponView>(WeaponViewPath);
        _menuView = GetNode<MenuView>(MenuViewPath);
        _crossHair = GetNode<CrossHair>(CrossHairPath);
        _ownMinimap = GetNode<Minimap>(OwnMinimapPath);
        _inputTimer = GetNode<Timer>("input_timer");

        _bullets.BulletHitsTarget += OnBulletHitTarget;
        _menuView.ActivateWeaponView += OnShowWeaponView;
        _menuView.ActivateCastleView += OnShowCastleView;
        _menuView.Build += OnBuildBuilding;

        _ownMinimap.AttachCastle(_castle);

        ChangeState(GameState.CastleModeActive);
    }

    public override void _Process(float delta)
    {
        switch(_gameState)
        {
            case GameState.WeaponModeActive:
                ProcessWeaponMode(delta);
                break;
        }
    }

    private void ChangeState(GameState gameState)
    {
        _gameState = gameState;

        switch(gameState)
        {
            case GameState.CastleModeActive:
                OnChangeStateToCastleMode();
                break;
            case GameState.WeaponModeActive:
                OnChangeStateToWeaponMode();
                break;
        }
        
    }

    private void OnChangeStateToCastleMode()
    {
        _activeWeapon = null;
        _cameraController.SwitchToCastle(_castle);
        _inputTimer.Stop();

        _weaponView.HideWeapon();
        _crossHair.Deactivate();
    }

    private void OnChangeStateToWeaponMode()
    {
        SetActiveWeapon(_castle.Weapons[0], false);
        _crossHair.Activate();
    }

    private void OnInputTimerTimeout()
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.Power += _powerDirection;
        }
    }

    private void ProcessWeaponMode(float delta)
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

    private void OnShowWeaponView() 
    {
        if (_gameState == GameState.WeaponModeActive) return;
        ChangeState(GameState.WeaponModeActive);
    }

    private void OnShowCastleView() 
    {
        if (_gameState == GameState.CastleModeActive) return;
        ChangeState(GameState.CastleModeActive);
    }

    private void OnBuildBuilding(BuildingType buildingType)
    {
        _cameraController.SwitchToBuildMode(_castle);
    }
}
