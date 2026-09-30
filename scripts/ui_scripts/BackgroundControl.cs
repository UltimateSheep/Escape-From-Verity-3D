using Godot;
using System;

public partial class BackgroundControl : Control
{
	public override void _GuiInput(InputEvent e)
	{
		if (e is InputEventMouseButton mouse && mouse.IsPressed())
		{
			GetViewport().GuiGetFocusOwner()?.ReleaseFocus();
		}
	}
}
