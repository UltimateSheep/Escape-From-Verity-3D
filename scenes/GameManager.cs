using Godot;
using System;

public partial class GameManager : Node3D
{

	public static readonly string playerPath = "res://objects/player.tscn";
	public readonly PackedScene playerObject = ResourceLoader.Load<PackedScene>(playerPath);

	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void InstantiatePlayer(string id, bool isSingleplayer = false)
    {
		if (!Multiplayer.IsServer())
			return;

        Player player = playerObject.Instantiate<Player>();

		player.Name = id;
		AddChild(player);
    }

}
