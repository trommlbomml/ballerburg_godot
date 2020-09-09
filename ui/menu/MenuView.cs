using Ballerburg.castle.buildings;
using Godot;
using System;

public class MenuView : Control
{
    private Control _buildMenu;

    public event Action ActivateWeaponView;
    public event Action ActivateCastleView;
    public event Action<BuildingType> Build;

    public override void _Ready()
    {
        _buildMenu = GetNode<Control>("build_menu");
    }

    public void OnViewWeaponButtonPressed() => ActivateWeaponView?.Invoke();

    public void OnViewCastleButtonPressed() => ActivateCastleView?.Invoke();

    public void OnBuildFarmerHousePressed() => Build?.Invoke(BuildingType.FarmerHouse);

    public void OnBuildBuildingPressed() 
    {
        ActivateCastleView?.Invoke();
        _buildMenu.Visible = true;
    }

    public void OnBackFromBuildMenuPressed()
    {
        _buildMenu.Visible = false;
    }
}
