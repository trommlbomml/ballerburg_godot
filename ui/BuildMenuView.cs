using Godot;
using System;

public class BuildMenuView : VBoxContainer
{
    public event Action BuildFarmerHouse;
    public event Action Back;

    public void OnFarmerHouseButtonPressed() => BuildFarmerHouse?.Invoke();

    public void OnBackButtonPressed() => Back?.Invoke();

    public override void _Ready()
    {
        Visible = false;
    }

    public void Activate()
    {
        Visible = true;
    }

    public void Deactivate()
    {
        Visible = false;
    }
}
