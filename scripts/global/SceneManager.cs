using FusionGodot;
using Godot;
using System;
using System.Threading;

public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }

	public static readonly string gameScenePath = "res://scenes/game.tscn";
	public static readonly string menuScenePath = "res://scenes/main_menu.tscn";

	public Node currentScene;

	public PackedScene gameScene = GD.Load<PackedScene>(gameScenePath);

    public override void _Ready()
    {
        Instance = this;
		currentScene = GetTree().CurrentScene;

		Fusion.SetSceneLoadMode(SceneLoadMode.Auto);
		// Fusion.SceneLoadRequested += OnSceneLoadRequested;
    }


    public void LoadGame_Singleplayer()
	{
		Fusion.DisconnectFromPhoton();

		CallDeferred("LoadGame");
	}

	public void LoadGame_Multiplayer()
	{
		GetTree().CurrentScene.QueueFree();

		if (Fusion.IsMasterClient())
			Fusion.LoadScene(gameScene);
	}

	private GameManager LoadGame()
	{
		currentScene.Free();

		var newScene = gameScene.Instantiate();

		GetTree().Root.AddChild(newScene);

		GetTree().CurrentScene = newScene;
		currentScene = newScene;

		GameManager gameManager = currentScene as GameManager;

		return gameManager;
	}

    // private void OnSceneLoadRequested(int index, PackedScene scene)
    // {
	// 	currentScene.Free();

    //     GameManager gameManager = scene.Instantiate() as GameManager;
		
	// 	GetTree().Root.AddChild(gameManager);

	// 	// GetTree().CurrentScene = gameManager;
	// 	currentScene = gameManager;

	// 	Player single_player = gameManager.GetNode("0") as Player;

	// 	single_player?.Free();

	// 	gameManager?.CallDeferred(GameManager.MethodName.InstantiatePlayer, Fusion.GetLocalPlayerId(), false);

	// 	Fusion.NotifySceneReady(currentScene, index);
    // }


}
