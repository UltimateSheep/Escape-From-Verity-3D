using Godot;
using System;

public partial class MenuContainer : TabContainer
{
	[Export]
	private Button[] mainButtons = [];
	[Export]
	private Button startButton;
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

		startButton.Pressed += () =>
		{
			SceneManager.Instance.CallDeferred(SceneManager.MethodName.StartGame, true);
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
