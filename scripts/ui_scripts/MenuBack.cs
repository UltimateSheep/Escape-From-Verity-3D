using Godot;
using System;

public partial class MenuBack : Button
{
	[Export]
	MenuContainer menuContainer;

    public override void _Ready()
    {
        menuContainer = GetNode<MenuContainer>("/root/MainMenu/CanvasLayer/BackgroundControl/MenuContainer");
    }


    public override void _Pressed()
    {
		menuContainer.SetMenu("Main");
    }
}
