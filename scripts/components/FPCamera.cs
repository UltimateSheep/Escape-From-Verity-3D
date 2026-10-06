using Godot;
using System;

[GlobalClass]
public partial class FPCamera : AuthorityDep
{
    [Export]
    public CharacterBody3D characterBody3D;

    [Export]
	public Marker3D CamMarker;
	[Export]
	public Marker3D cameraMarker;
	[Export]
	public Camera3D Camera;

    [Export]
	public float mouse_sensitivity = 0.005f;

    public InputEventMouseMotion Mouse_Motion;


    public void Pol()
    {
        if (!IsAuthority)
            return;

		Camera.Rotation = new Vector3(-CamMarker.Rotation.X * 1.15f, Utils.Deg2Rad(-180), 0);

        double cam_rot = Math.Clamp(CamMarker.Rotation.X + Mouse_Motion.Relative.Y * mouse_sensitivity, Utils.Deg2Rad(-75), Utils.Deg2Rad(60));

        CamMarker.Rotation = new Vector3((float)cam_rot, 0, 0);
        characterBody3D.RotateY(-Mouse_Motion.Relative.X * mouse_sensitivity);
    }

    public void Process()
    {
        Camera.GlobalPosition = cameraMarker.GlobalPosition;
    }
}
