using Ballerburg.castle.buildings;
using Godot;
using System;

public enum MenuState
{
    RootMenuCastleView,
    RootMenuWeaponView,
    BuildingSelection,
    Building,
    WeaponMode
}

public class MenuView : Control
{
    private MenuState _menuState;
    private Control _buildMenu;
    private Button _buildBuildingButton;
    private Button _weaponViewButton;
    private Button _castleViewButton;
    
    public event Action ActivateWeaponView;
    public event Action ActivateCastleView;
    public event Action<BuildingType> Build;

    public override void _Ready()
    {
        _buildMenu = GetNode<Control>("build_menu");
        _buildBuildingButton = GetNode<Button>("slide/general_menu/build_building_button");
        _weaponViewButton = GetNode<Button>("slide/general_menu/view_weapon_button");
        _castleViewButton = GetNode<Button>("slide/general_menu/view_castle_button");
    }

    public void OnViewWeaponButtonPressed()
    {
        ActivateWeaponView?.Invoke();
        SetMenuState(MenuState.RootMenuWeaponView);
    }

    public void OnViewCastleButtonPressed() 
    {
        ActivateCastleView?.Invoke();
        SetMenuState(MenuState.RootMenuCastleView);
    }

    public void OnBuildBuildingPressed() 
    {
        ActivateCastleView?.Invoke();
        SetMenuState(MenuState.BuildingSelection);
    }

    public void OnBuildFarmerHousePressed() 
    {
        Build?.Invoke(BuildingType.FarmerHouse);
        SetMenuState(MenuState.Building);
    }

    public void OnBackFromBuildMenuPressed()
    {
        SetMenuState(MenuState.RootMenuCastleView);
    }

    public void SetMenuState(MenuState state)
    {
        _menuState = state;

        var isBuilding = state == MenuState.Building;
        
        _buildMenu.Visible = !isBuilding && state == MenuState.BuildingSelection;
        _castleViewButton.Disabled = isBuilding || state == MenuState.RootMenuCastleView || state == MenuState.BuildingSelection;
        _weaponViewButton.Disabled = isBuilding || state == MenuState.RootMenuWeaponView;
        _buildBuildingButton.Disabled = isBuilding || state == MenuState.BuildingSelection;
    }
}
