using FusionGodot;
using Godot;
using System;


public partial class GlobalMultiplayer : Node
{
	public static GlobalMultiplayer Instance { get; private set; }

	public string nickname;

#region Signals
    public override void _Ready()
    {
        Instance = this;
    }


    public override void _Process(double delta)
    {
		
    }

	public override void _ExitTree()
	{
		Fusion.DisconnectFromPhoton();
	}
#endregion


#region Server Management

	public static void ConnectPhoton()
	{
		Fusion.ConnectToPhoton();
	}

	public void ChangeNickname(string _nickname)
	{
		nickname = _nickname;
	}



#endregion

#region Utils

	public static string GenerateSalt()
	{
		string _salt = GD.Randi().ToString().Sha256Text();

		return _salt;
	}

	public static string GenerateHashedPassword(string password, string salt)
	{
		return (password + salt).Sha256Text();
	}

#endregion
}
