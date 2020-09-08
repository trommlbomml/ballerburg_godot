using Godot;
using System;

public class GeneralMenuView : Control
{
    private Button _weaponViewButton;
    private Button _castleViewButton;

    public event Action ShowWeaponView;
    public event Action ShowCastleView;

    public override void _Ready()
    {
        _weaponViewButton = GetNode<Button>("view_weapon_button");
        _castleViewButton = GetNode<Button>("view_castle_button");
    }

    public void SetStateWeaponView()
    {
        _weaponViewButton.Disabled = true;
        _castleViewButton.Disabled = false;
    }

    public void SetStateCastleView()
    {
        _weaponViewButton.Disabled = false;
        _castleViewButton.Disabled = true;
    }

    private void OnViewWeaponButtonPressed()
    {
        ShowWeaponView?.Invoke();
    }

    private void OnViewCastleButtonPressed()
    {
        ShowCastleView?.Invoke();
    }
}
