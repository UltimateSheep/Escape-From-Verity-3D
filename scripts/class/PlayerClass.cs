using FusionGodot;
using Godot;
using System;

public partial class PlayerClass : CharacterBody3D
{
	[Export] public InputHandler inputHandler;
	[Export] public Movement movement;
	[Export] public FPCamera fPCamera;
	[Export] public AnimationHandler animationHandler;


	public bool IsSingleplayer = false;

	private FusionSharedReplicator replicator;

    public override void _Ready()
    {
        if (!IsSingleplayer)
            Name = replicator.GetOwnerId().ToString();
    }

    public override void _EnterTree()
    {
		replicator = this.GetSharedReplicator("Replicator");

		IsSingleplayer = Name == "single";

		if (replicator.HasAuthority() ||  IsSingleplayer)
		{
			GetNode<Camera3D>("Camera").Current = true;
		}

		SetAuthority(inputHandler, movement, fPCamera, animationHandler);

        replicator.AuthorityChanged += _ =>
        {
            Name = replicator.GetOwnerId().ToString();
        };
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        inputHandler.UnHandledInput(@event);

		fPCamera.Mouse_Motion = inputHandler.Motion;
		fPCamera.Pol();
    }


    public override void _Process(double delta)
    {
		inputHandler.Pol();
        fPCamera.Process(); 
    }


	public override void _PhysicsProcess(double delta)
	{
        inputHandler.PhysicalProcess();

		movement.inputDir = inputHandler.InputDir;
		movement.IsRunning = inputHandler.IsRunning;
		movement.PhysicsProcess(delta);

        animationHandler.current_input = movement.current_input;
		animationHandler.Process(delta);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Teleport(Vector3 position)
	{
		Position = position;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void ChangeModel(int idx)
	{
		
	}


	private bool IsAuthority()
	{
		return IsSingleplayer || replicator.HasAuthority();
	}

	private void SetAuthority(params AuthorityDep[] what)
	{
		foreach (AuthorityDep ad in what)
		{
			ad.IsAuthority = IsAuthority();
		}
	}
}

