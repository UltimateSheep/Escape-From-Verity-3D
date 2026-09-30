using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public Marker3D CamMarker;

	[Export]
	public Camera3D Camera;
	[Export]
	public BoneAttachment3D cameraAttach;
	[Export]
	public Vector3 CameraOffset = new();

	[Export]
	public string LocomotionBlend;
	[Export]
	public AnimationTree animationTree;

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	[Export]
	public float mouse_sensitivity = 0.001f;

	[Export]
	private float transition = 10f;

	private Vector2 current_input = new();
	private Vector2 current_velocity = new();

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
		if (Input.IsActionJustPressed("pause"))
		{
			GetTree().Quit();
		}

        if (@event is InputEventMouseMotion motion)
		{
			double cam_rot = Math.Clamp(CamMarker.Rotation.X + motion.Relative.Y * mouse_sensitivity, Deg2Rad(-75), Deg2Rad(60));

			CamMarker.Rotation = new Vector3((float)cam_rot, 0, 0);
			RotateY(-motion.Relative.X * mouse_sensitivity);
		}
    }

    public override void _Process(double delta)
    {
		Camera.Position = cameraAttach.Position + CameraOffset;
		Camera.Rotation = new Vector3(-CamMarker.Rotation.X * 1.15f, Deg2Rad(-180), 0);

        Vector2 new_delta = current_input - current_velocity;
		if (new_delta.Length() > transition * (float)delta)
			new_delta = new_delta.Normalized() * transition * (float)delta;

		current_velocity += new_delta;

		animationTree.Set(LocomotionBlend, current_velocity);
    }


	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_right", "move_left", "move_down", "move_up");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;

		current_input = inputDir * new Vector2(-1, 1);
		MoveAndSlide();
	}

	private static float Deg2Rad(float angle)
	{
		return (float)Math.PI / 180 * angle;
	}
}
