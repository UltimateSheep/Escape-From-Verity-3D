using FusionGodot;
using Godot;
using System;

public partial class GameManager : Node3D
{

	public static readonly string playerPath = "res://objects/player.tscn";
	public readonly PackedScene playerObject = ResourceLoader.Load<PackedScene>(playerPath);

	public FusionSpawner fusionSpawner;

	[Export]
	public Marker3D SpawnPosition;

	private PlayerClass localPlayer;

    public override void _Ready()
	{

		if (!Fusion.IsConnectedToPhoton())
			return;
			
		PlayerClass single_player = GetNodeOrNull("single") as PlayerClass;
		single_player?.Free();

		fusionSpawner = this.GetSpawner("FusionSpawner");
		
		// OnPlayerJoined(Fusion.GetLocalPlayerId(), "");
		OnRoomJoined();

		// Fusion.PlayerJoined += OnPlayerJoined;
		if (Fusion.IsMasterClient())
			Fusion.RoomJoined += OnRoomJoined;
	}

    // public override void _EnterTree()
    // {
	// 	if (Fusion.IsMasterClient())
	// 		OnPlayerJoined(Fusion.GetLocalPlayerId(), "");
			
    // }


	public override void _Process(double delta)
	{
	}

	private void OnRoomJoined()
	{
		InstantiatePlayer(Fusion.GetLocalPlayerId());

		if (Fusion.IsMasterClient())
		{
			Fusion.RegisterCurrentScene();
		}
	}

	public void InstantiatePlayer(int id, bool isSingleplayer = false)
    {
		if (localPlayer is not null)
			fusionSpawner.Despawn(localPlayer);

        PlayerClass player = fusionSpawner.Spawn(playerObject) as PlayerClass;

		localPlayer = player;
		// Fusion.RpcToPlayer(id, player.Teleport, SpawnPosition.Position);
    }

}
