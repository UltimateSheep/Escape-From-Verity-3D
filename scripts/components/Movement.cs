using Godot;
using System;

[GlobalClass]
public partial class Movement : AuthorityDep
{
    [Export]
    public CharacterBody3D characterBody3D;
    
	[Export]
	public float WalkSpeed = 2.5f;
	[Export]
	public float RunSpeed = 3.5f;
    [Export]
    public float JumpVelocity = 4.5f;

    public Vector2 current_input = Vector2.Zero;
	public Vector3 last_position = Vector3.Zero;

    // Inputs
    public Vector2 inputDir = Vector2.Zero;
    public bool IsRunning = false;

    public void PhysicsProcess(double delta)
    {
        if (!IsAuthority)
            return;

        Vector3 velocity = characterBody3D.Velocity;
		float Speed = IsRunning ? RunSpeed : WalkSpeed;

		// Add the gravity.
		if (!characterBody3D.IsOnFloor())
		{
			velocity += characterBody3D.GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && characterBody3D.IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		// Vector2 inputDir = Input.GetVector("move_right", "move_left", "move_down", "move_up");
		Vector3 direction = (characterBody3D.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(characterBody3D.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(characterBody3D.Velocity.Z, 0, Speed);
		}

		characterBody3D.Velocity = velocity;

		float current_speed = last_position.DistanceTo(characterBody3D.Position * new Vector3(1, 0, 1)) / (float)delta;

		Vector3 relative_vector = current_speed * new Vector3(inputDir.X, 0, inputDir.Y);

		float anim_x = Math.Abs(relative_vector.X) > 0f? -relative_vector.X/RunSpeed : 0;
		float anim_y = Math.Abs(relative_vector.Z) > 0f? relative_vector.Z/RunSpeed : 0;

		current_input = new Vector2(anim_x, anim_y);

		last_position = characterBody3D.Position * new Vector3(1, 0, 1);

		characterBody3D.MoveAndSlide();
    }
}
