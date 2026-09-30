using Godot;
using System;

public partial class LocalHostUI : MarginContainer
{

	private PackedScene serverTemplate = GD.Load<PackedScene>("res://ui_components/server_button_template.tscn");

	[Export]
	private Button RefreshButton;

	[Export]
	private VBoxContainer ServerBrowser;

#region Host Field
	[Export, ExportCategory("Host")] private Button HostButton;
	[Export] private LineEdit NameEdit;
	[Export] private LineEdit PortEdit;
	[Export] private LineEdit MaxPlayerEdit;
	[Export] private LineEdit PasswordEdit;
	[Export] private OptionButton DifficultyEdit;
#endregion

#region Join Field

	[Export, ExportCategory("Join")] private Button JoinButton;
	[Export] private LineEdit JoinNameEdit;
	[Export] private LineEdit JoinPortEdit;
	[Export] private LineEdit JoinPasswordEdit;
#endregion

	private GlobalMultiplayer.ServerDetail[] servers = [];

	public override void _Ready()
	{
		Refresh();

		RefreshButton.Pressed += Refresh;
	}

	private void Refresh()
	{
		foreach (Node item in ServerBrowser.GetChildren())
		{
			item.Free();
		}

		foreach (GlobalMultiplayer.ServerDetail item in servers)
		{
			Button serverButton = serverTemplate.Instantiate<Button>();

			serverButton.Name = item._id;
			((Label)serverButton.GetNode("Name")).Text = item.name;
			((Label)serverButton.GetNode("PlayerCount")).Text = $"{item.player_ids.Length}/{item.max_player}";

			ServerBrowser.AddChild(serverButton);
		}
	}
}
