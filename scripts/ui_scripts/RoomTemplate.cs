using Godot;
using System;

public partial class RoomTemplate : Panel
{
	public Button button;

    public override void _EnterTree()
	{
		button = GetNode<Button>("Button");

		button.Pressed += SelectRoom;
	}

	public void SelectRoom()
	{
		OnlineHostUI online = GetTree().CurrentScene.GetNode<OnlineHostUI>("%Online");

		online.EmitSignal(OnlineHostUI.SignalName.SelectRoom, Name);
	}
}
