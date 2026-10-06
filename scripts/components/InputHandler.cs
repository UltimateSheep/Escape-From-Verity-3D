using Godot;
using System;

[GlobalClass]
public partial class InputHandler : AuthorityDep
{

    public Vector2 InputDir = Vector2.Zero;
    public bool IsRunning = false;
	public bool IsMouselocked = true;

    public InputEventMouseMotion Motion;

    public void UnHandledInput(InputEvent @event)
    {
        if (!IsAuthority)
            return;

		if (Input.IsActionJustPressed("toggle_mouse_lock"))
		{
			IsMouselocked = !IsMouselocked;
		}

		if (Input.IsActionJustPressed("sprint"))
		{
			IsRunning = true;
		}

		if (Input.IsActionJustReleased("sprint"))
		{
			IsRunning = false;
		}

		if (Input.IsActionJustPressed("pause"))
		{
			GetTree().Quit();
		}

        Motion = new();
        if (@event is InputEventMouseMotion motion)
        {
            Motion = motion;
        }
    }

    public void PhysicalProcess()
    {    
        InputDir = Input.GetVector("move_right", "move_left", "move_down", "move_up");
    }


    public void Pol()
    {
        if (!IsAuthority)
            return;

		Input.MouseMode = IsMouselocked ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
    }
}
