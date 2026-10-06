using Godot;
using System;

[GlobalClass]
public partial class AnimationHandler : AuthorityDep
{
	[Export]
	public string LocomotionBlend;
	[Export]
	public AnimationTree animationTree;
	
	[Export]
	private float transition = 10f;

    public Vector2 current_input = Vector2.Zero;
	public Vector2 current_velocity = Vector2.Zero;
    
    public void Process(double delta)
    {
        if (!IsAuthority)
            return;

        Vector2 new_delta = current_input - current_velocity;
		if (new_delta.Length() > transition * (float)delta)
			new_delta = new_delta.Normalized() * transition * (float)delta;

		current_velocity += new_delta;

		animationTree.Set(LocomotionBlend, current_velocity);
    }
}
