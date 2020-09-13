using System;
using Godot;

public class MainPanelView : PanelContainer
{
    private Minimap _ownMinimap;
    private Minimap _enemyMinimap;
    
    private Label _leftStatus;
    
    private IWeapon _weapon;
    private Control _weaponRoot;
    private Label _angleLabel;
    private Label _powerLabel;
    private Button _shootButton;
    
    public override void _Ready()
    {
        _ownMinimap = GetNode<Minimap>("spacer/root/left/minimap");
        _enemyMinimap = GetNode<Minimap>("spacer/root/right/minimap");
        
        _weaponRoot = GetNode<Control>("spacer/root/weapon");
        _leftStatus = GetNode<Label>("spacer/root/weapon/state/vbox/status_label");
        _angleLabel = GetNode<Label>("spacer/root/weapon/nav/vbox/display/angle_label");
        _powerLabel = GetNode<Label>("spacer/root/weapon/nav/vbox/display/power_label");
        _shootButton = GetNode<Button>("spacer/root/weapon/nav/vbox/controls/shoot_button");

        _weaponRoot.Visible = false;
    }

    public override void _Process(float delta)
    {
        if (_weapon == null) return;

        _angleLabel.Text = $"{GetAngleDegrees(_weapon.RotationY):000}";
        _powerLabel.Text = $"{_weapon.Power:000}";

        if (_weapon.ReloadPercentage < 100)
        {
            _leftStatus.Text = $"Reload ({_weapon.ReloadPercentage:000}%)";
        }
        else
        {
            _leftStatus.Text = $"{_weapon.ShootCost} Gold";
        }

        _shootButton.Disabled = !_weapon.CanShoot;
    }

    public void ShowWeaponView(IWeapon weapon)
    {
        _weapon = weapon;
        _weaponRoot.Visible = true;
    }

    public void HideWeaponView()
    {
        _weapon = null;
        _weaponRoot.Visible = false;
    }

    public void UpdateOwnMinimap(Castle castle)
    {
        _ownMinimap.RefreshFromCastle(castle);
    }

    public void ClearEnemyMiniMap()
    {
        _enemyMinimap.Clear();
    }

    public void UpdateEnemyMiniMap(Castle castle)
    {
        _enemyMinimap.RefreshFromCastle(castle);
    }

    private static int GetAngleDegrees(float radians)
    {
        var angleDegrees = (int)Math.Round(radians / Mathf.Pi * 180.0f);
        while (angleDegrees < 0) angleDegrees += 360;
        while(angleDegrees >= 360) angleDegrees -= 360;
        return angleDegrees;
    }
}
