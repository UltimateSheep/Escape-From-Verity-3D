using Godot;
using System;
using System.Threading;

public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }

	public static readonly string gameScenePath = "res://scenes/game.tscn";
	public static readonly string menuScenePath = "res://scenes/main_menu.tscn";

	public Node currentScene;

    public override void _Ready()
    {
        Instance = this;

		currentScene = GetTree().CurrentScene;
    }

	public void StartGame(bool isSingleplayer = false)
	{
		GlobalMultiplayer.Instance.IsInGame = true;

		currentScene.Free();

		var nextScene = GD.Load<PackedScene>(gameScenePath).Instantiate();

		currentScene = nextScene;

		GetTree().Root.AddChild(currentScene);

		GameManager gameManager = currentScene as GameManager;
		gameManager.GetNodeOrNull("Player")?.QueueFree();

		if (isSingleplayer)
			gameManager?.InstantiatePlayer("0", true);
	}

	public void NetworkListen()
	{
		Multiplayer.PeerConnected += (id) =>
		{
			GD.Print($"Player {id} Joined!");
		
			GameManager gameManager = currentScene as GameManager;

			gameManager?.InstantiatePlayer(id.ToString());
		};
	}

	public void StartGameAsHost()
	{
		StartGame();
		NetworkListen();

		GameManager gameManager = currentScene as GameManager;

		gameManager.InstantiatePlayer("1");
	}

	public void StartGameAsClient()
	{
		StartGame();
		NetworkListen();

		GameManager gameManager = currentScene as GameManager;

		gameManager.InstantiatePlayer(Multiplayer.GetUniqueId().ToString());
	}
}
