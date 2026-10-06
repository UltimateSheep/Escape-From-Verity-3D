using Godot;
using System;

public partial class Utils : Node
{
    public static float Deg2Rad(float angle)
	{
		return (float)Math.PI / 180 * angle;
	}
}
