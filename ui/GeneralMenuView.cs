using Godot;
using System;

public class GeneralMenuView : Control
{
    private Button _weaponViewButton;
    private Button _castleViewButton;
    private Button _buildBuildingButton;

    public event Action ShowWeaponView;
    public event Action ShowCastleView;
    public event Action BuildBuilding;

    public override void _Ready()
    {
        _weaponViewButton = GetNode<Button>("view_weapon_button");
        _castleViewButton = GetNode<Button>("view_castle_button");
        _buildBuildingButton = GetNode<Button>("build_building_button");
    }

    public void SetStateWeaponView()
    {
        _weaponViewButton.Disabled = true;
        _buildBuildingButton.Disabled = true;
        _castleViewButton.Disabled = false;
    }

    public void SetStateCastleView()
    {
        _weaponViewButton.Disabled = false;
        _buildBuildingButton.Disabled = false;
        _castleViewButton.Disabled = true;
    }

    public void SetStateBuildingView()
    {
        _weaponViewButton.Disabled = true;
        _buildBuildingButton.Disabled = true;
        _castleViewButton.Disabled = true;
    }

    public void OnViewWeaponButtonPressed()
    {
        ShowWeaponView?.Invoke();
    }

    public void OnViewCastleButtonPressed()
    {
        ShowCastleView?.Invoke();
    }

    public void OnBuildBuildingPressed()
    {
        BuildBuilding?.Invoke();
    }
}
