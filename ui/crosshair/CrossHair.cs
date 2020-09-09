using Godot;
using System;

public class CrossHair : Control
{
    public void Activate()
    {
        Visible = true;
    }

    public void Deactivate()
    {
        Visible = false;
    }
}
