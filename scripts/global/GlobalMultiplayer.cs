using Godot;
using System;

public partial class GlobalMultiplayer : Node
{
	public struct ServerDetail
	{
		public required string _id;
		public required string name;
		public required short port;
		public required int max_player;
		
		public required string host_id;
		public required short difficulty_level;

		public string password;
		public string[] player_ids;

	}

	public static GlobalMultiplayer Instance { get; private set; }

	public ENetMultiplayerPeer Peer;

    public override void _Ready()
    {
        Instance = this;

		Peer = new();
    }

	public void CreateServer(ServerDetail detail)
	{
		Peer.CreateServer(detail.port, detail.max_player);
	}

}
