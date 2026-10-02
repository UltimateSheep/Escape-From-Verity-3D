using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;

public partial class GlobalMultiplayer : Node
{
	public struct ServerDetail
	{
		public required string Id {get; set;}
		public required short Port {get; set;}
		public required int Max_player {get; set;}
		
		public int Host_id {get; set;}
		public required short Difficulty_level {get; set;}

		public string Password {get; set;}
		public int[] Player_ids {get; set;}

	}

	public static GlobalMultiplayer Instance { get; private set; }

	public ENetMultiplayerPeer Peer;

	private Timer timer;

	private string packetData;

	public bool IsInGame = false;

    public override void _Ready()
    {
        Instance = this;

		Peer = new();
		
    }

    public override void _Process(double delta)
    {
		
    }

#region Server Management
	public Error JoinServer(string address, ushort port)
	{
		GD.Print($"Joining server on {address}:{port}...");

		Error e = Peer.CreateClient(address, port);
		switch (e)
		{
			case Error.Ok:
				Multiplayer.MultiplayerPeer = Peer;
				Peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
				break;
			default:
				GD.PrintErr($"Failed to join server on {address}:{port}: {e}");
				break;
		}
		return e;
	}

	public Error CreateServer(ServerDetail detail)
	{
		GD.Print($"Creating server: {JsonSerializer.Serialize(detail)}");

		string id = Guid.NewGuid().ToString();

		Error e = Peer.CreateServer(detail.Port, detail.Max_player);
		switch (e)
		{
			case Error.Ok:
				Multiplayer.MultiplayerPeer = Peer;
				Peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder); 

				detail.Id = id;
				detail.Host_id = Multiplayer.GetUniqueId();

				DisplayServer.WindowSetTitle("Escape From Verity - Host");
				break;
			default:
				GD.PrintErr($"Failed to create server on port {detail.Port}: {e}");
				break;
		}

		return e;
	}

	// public void OnPlayerJoined(long id)
	// {
	// 	if (!IsInGame)
	// 		return;

		
	// }

#endregion
	public override void _ExitTree()
	{
		if (Multiplayer.MultiplayerPeer != null)
			return;

		GD.PrintErr($"Closing port {Peer.Host.GetLocalPort()} and shutting down server.");

		Peer.Close();
		Multiplayer.MultiplayerPeer = null;
	}

}
