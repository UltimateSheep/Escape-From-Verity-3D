using Godot;
using System;

public partial class MenuContainer : TabContainer
{
	[Export]
	private Button[] mainButtons = [];
	[Export]
	private Button quitButton;

    public override void _Ready()
    {
		SetMenu("Main");

		foreach (Button item in mainButtons)
		{
			item.Pressed += () =>
			{
				SetMenu(item.Name);	
			};
		}

		quitButton.Pressed += () =>
		{
			GetTree().Quit();	
		};
    }


	public void SetMenu(string name)
	{
        if (!HasNode(name))
            return;

		Control node = GetNode<Control>(name);
		node.Visible = true;
	}
}
