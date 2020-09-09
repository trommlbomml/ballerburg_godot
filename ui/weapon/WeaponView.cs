using Godot;
using System;

public class WeaponView : Control
{
    private IWeapon _weapon;
    private Label _angleLabel;
    private Label _powerLabel;
    private Label _stateLabel;
    private Button _shootButton;

    public override void _Ready()
    {
        Visible = false;
        _angleLabel = GetNode<Label>("hbox/nav/vbox/display/angle_label");
        _powerLabel = GetNode<Label>("hbox/nav/vbox/display/power_label");
        _stateLabel = GetNode<Label>("hbox/state/vbox/status_label");
        _shootButton = GetNode<Button>("hbox/nav/vbox/controls/shoot_button");
    }

    public override void _Process(float delta)
    {
        if (_weapon == null) return;

        _angleLabel.Text = $"{GetAngleDegrees(_weapon.RotationY):000}";
        _powerLabel.Text = $"{_weapon.Power:000}";

        if (_weapon.ReloadPercentage < 100)
        {
            _stateLabel.Text = $"Reload ({_weapon.ReloadPercentage:000}%)";
        }
        else
        {
            _stateLabel.Text = $"{_weapon.ShootCost} Gold";
        }

        _shootButton.Disabled = !_weapon.CanShoot;
    }

    public void ShowWeapon(IWeapon weapon)
    {
        _weapon = weapon;
        Visible = true;
    }

    public void HideWeapon()
    {
        _weapon = null;
        Visible = false;
    }

    private static int GetAngleDegrees(float radians)
    {
        var angleDegrees = (int)Math.Round(radians / (float)Math.PI * 180.0f);
        while (angleDegrees < 0) angleDegrees += 360;
        while(angleDegrees >= 360) angleDegrees -= 360;
        return angleDegrees;
    }
}
