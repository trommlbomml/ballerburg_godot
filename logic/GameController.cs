using Ballerburg.castle.buildings;
using Godot;

public enum GameState
{
    WeaponView,
    CastleView,
    Building
}

public class GameController : Spatial
{
    [Export] public NodePath CameraPath { get; set; }
    [Export] public NodePath CastlePath { get; set; }
    [Export] public NodePath EnemyCastlePath { get; set; }
    [Export] public NodePath BulletsPath { get; set; }
    [Export] public NodePath MainPanelPath { get; set; }
    [Export] public NodePath MenuViewPath { get; set; }

    [Export] public NodePath CrossHairPath { get; set; }

    private GameState _gameState;
    private Castle _castle;
    private Castle _enemyCastle;
    private CameraController _cameraController;
    private Bullets _bullets;
    private Timer _inputTimer;
    
    private CrossHair _crossHair;
    private MainPanelView _mainPanelView;
    private MenuView _menuView;

    private IWeapon _activeWeapon;
    private int _powerDirection = 0;

    public override void _Ready()
    {
        _cameraController = GetNode<CameraController>(CameraPath);
        _castle = GetNode<Castle>(CastlePath);
        _enemyCastle = GetNode<Castle>(EnemyCastlePath);
        _bullets = GetNode<Bullets>(BulletsPath);
        _mainPanelView = GetNode<MainPanelView>(MainPanelPath);
        _menuView = GetNode<MenuView>(MenuViewPath);
        _crossHair = GetNode<CrossHair>(CrossHairPath);
        _inputTimer = GetNode<Timer>("input_timer");

        _bullets.BulletHitsTarget += OnBulletHitTarget;
        _menuView.ActivateWeaponView += OnShowWeaponView;
        _menuView.ActivateCastleView += OnShowCastleView;
        _menuView.Build += OnBuildBuilding;
        _castle.BuildingAdded += OnBuildingAdded;

        _mainPanelView.UpdateOwnMinimap(_castle);
        _mainPanelView.UpdateEnemyMiniMap(_enemyCastle);

        _menuView.SetMenuState(MenuState.RootMenuCastleView);
        ChangeState(GameState.CastleView);
    }

    public override void _Process(float delta)
    {
        switch(_gameState)
        {
            case GameState.WeaponView:
                ProcessWeaponMode(delta);
                break;
            case GameState.Building:
                ProcessBuildingMode(delta);
                break;
        }
    }

    private void OnBulletHitTarget(BulletHitInfo hitInfo)
    {
        var buildingHit = hitInfo.Building?.BuildingType;
        System.Diagnostics.Debug.WriteLine($"Hitting {hitInfo.Building?.BuildingType.ToString() ?? "Nothing"} at {hitInfo.WorldCoords}");
    }

    private void OnShowWeaponView()
    {
        if (_gameState == GameState.WeaponView) return;
        ChangeState(GameState.WeaponView);
    }

    private void OnShowCastleView() 
    {
        if (_gameState == GameState.CastleView) return;
        ChangeState(GameState.CastleView);
    }

    private void OnBuildBuilding(BuildingType buildingType)
    {
        _castle.StartPlaceBuilding(buildingType, _cameraController);
        ChangeState(GameState.Building);
    }

    private void OnBuildingAdded(IBuilding obj)
    {
        _mainPanelView.UpdateOwnMinimap(_castle);
    }

    private void OnInputTimerTimeout()
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.Power += _powerDirection;
        }
    }

    private void ChangeState(GameState gameState)
    {
        _gameState = gameState;

        switch(gameState)
        {
            case GameState.CastleView:
                OnChangeStateToCastleMode();
                break;
            case GameState.WeaponView:
                OnChangeStateToWeaponMode();
                break;
            case GameState.Building:
                OnChangeStateToBuildingMode();
                break;
        }
    }

    private void OnChangeStateToCastleMode()
    {
        _activeWeapon = null;
        _cameraController.SwitchToCastle(_castle);
        _inputTimer.Stop();

        _mainPanelView.HideWeaponView();
        _crossHair.Deactivate();
    }

    private void OnChangeStateToWeaponMode()
    {
        SetActiveWeapon(_castle.Weapons[0], false);
        _crossHair.Activate();
    }

    private void OnChangeStateToBuildingMode()
    {
        _cameraController.SwitchToBuildMode(_castle);
    }

    private void ProcessBuildingMode(float delta)
    {
        if (Input.IsActionJustPressed("cancel_build"))
        {
            _castle.StopPlaceBuildings();
            _menuView.SetMenuState(MenuState.BuildingSelection);
            ChangeState(GameState.CastleView);
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
        _mainPanelView.ShowWeaponView(weapon);
    }
}
