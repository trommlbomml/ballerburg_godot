using Godot;
using System;

public enum SlidingSide
{
    Left,
    Right
}

public class MenuSlider : CanvasLayer
{
    [Export] public float AnimationTime { get; set; } = 0.3f;
    [Export] public float VisibleSpace { get; set; } = 20.0f;
    [Export] public SlidingSide SlidingSide { get; set; } = SlidingSide.Right;

    private Tween _tween;
    private Control _animatee;
    private bool _entered;

    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
        _animatee = GetChild<Control>(1);
        Offset = GetSlideInOffset();
    }

    public override void _Process(float delta)
    {
        var rect = new Rect2(_animatee.RectGlobalPosition, _animatee.RectSize);
        var mousePosition = _animatee.GetGlobalMousePosition();

        var entered = rect.HasPoint(mousePosition);
        if (entered == _entered) return;

        _entered = entered;
        if (_entered)
        {
            _tween.InterpolateProperty(this, "offset", Offset, new Vector2(), AnimationTime, Tween.TransitionType.Quad, Tween.EaseType.In);
        }
        else
        {
            _tween.InterpolateProperty(this, "offset", Offset, GetSlideInOffset(), AnimationTime, Tween.TransitionType.Quad, Tween.EaseType.In);
        }
        _tween.Start();
    }

    private Vector2 GetSlideInOffset()
    {
        return SlidingSide == SlidingSide.Right 
            ? new Vector2(_animatee.RectSize.x - VisibleSpace, 0)
            : new Vector2(-_animatee.RectSize.x + VisibleSpace, 0);
    }
}
