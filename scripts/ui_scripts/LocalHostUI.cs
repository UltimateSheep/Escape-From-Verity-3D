using Godot;
using System;
using System.Collections.Generic;

public partial class LocalHostUI : MarginContainer
{

private Timer JoinTimeout;
private bool isJoining = false;

#region Host Field
	[Export, ExportCategory("Host")] private Button HostButton;
	[Export] private TextEdit HostErrorEdit;
	[Export] private LineEdit PortEdit;
	[Export] private LineEdit MaxPlayerEdit;
	[Export] private OptionButton DifficultyEdit;
#endregion

#region Join Field
	[Export, ExportCategory("Join")] private Button JoinButton;
	[Export] private TextEdit JoinErrorEdit;
	[Export] private LineEdit JoinAddressEdit;
	[Export] private LineEdit JoinPortEdit;

#endregion

	public override void _Ready()
	{
		JoinTimeout = new Timer
		{
			WaitTime = 5.0f,
			OneShot = true,
		};

		AddChild(JoinTimeout);

		JoinTimeout.Timeout += () =>
		{
			JoinErrorEdit.Text = "Failed to join server: Timeout.";
			isJoining = false;
		};

        PortEdit.TextSubmitted += _=> SubmitPortFields(PortEdit, PortEdit.Text);
		PortEdit.FocusExited += () => SubmitPortFields(PortEdit, PortEdit.Text);

		MaxPlayerEdit.TextSubmitted += _ => SubmitMaxPlayerFields(MaxPlayerEdit, MaxPlayerEdit.Text);
		MaxPlayerEdit.FocusExited += () => SubmitMaxPlayerFields(MaxPlayerEdit, MaxPlayerEdit.Text);

		JoinPortEdit.TextSubmitted += _ => SubmitPortFields(JoinPortEdit, JoinPortEdit.Text);
		JoinPortEdit.FocusExited += () => SubmitPortFields(JoinPortEdit, JoinPortEdit.Text);

		HostButton.Pressed += HostButton_Pressed;
		JoinButton.Pressed += JoinButton_Pressed;
	}

	public override void _Process(double delta)
	{
		Validate();
	}

#region Validation
	private void Validate()
	{
		bool isHostValid = ValidateHostFields();
		HostButton.Disabled = !isHostValid;
			
		bool isJoinValid = ValidateJoinFields();
		JoinButton.Disabled = !isJoinValid;
	}

	private static void SubmitPortFields(LineEdit lineEdit, string text)
	{
		lineEdit.Text = ValidateNumberField(text, 1024, 65535);
	}

	private static void SubmitMaxPlayerFields(LineEdit lineEdit, string text)
	{
		lineEdit.Text = ValidateNumberField(text, 1, 100);
	}


	private bool ValidateHostFields()
	{
		string newText = "";

		if (!short.TryParse(PortEdit.Text, out short port) || port < 1024)
		{
			newText = "Port must be a number between 1024 and 65535.";
		}

		if (!int.TryParse(MaxPlayerEdit.Text, out int maxPlayers) || maxPlayers < 1 || maxPlayers > 100)
		{
			newText = "Max players must be a number between 1 and 100.";
		}

		if (DifficultyEdit.Selected < 0)
		{
			newText = "Please select a difficulty level.";
		}

		if (!string.IsNullOrWhiteSpace(newText))
		{
			HostErrorEdit.Text = newText;
			return false;
		}

		return newText == "";
	}

	private bool ValidateJoinFields()
	{
		if (isJoining)
			return false;

		string newText = "";

		if (!ushort.TryParse(JoinPortEdit.Text, out ushort port) || port < 1024)
		{
			newText = "Port must be a number between 1024 and 65535.";
		}

		if (string.IsNullOrEmpty(JoinAddressEdit.Text))
		{
			newText = "Address cannot be empty.";
		}
		else if (JoinAddressEdit.Text.IsValidIPAddress() == false && JoinAddressEdit.Text != "localhost")
		{
			newText = "Address must be a valid IP address.";
		}

		if (!string.IsNullOrWhiteSpace(newText))
		{
			JoinErrorEdit.Text = newText;
			return false;
		}

		return newText == "";
	}

	private static string ValidateNumberField(string text, int min, int max)
	{
		if (int.TryParse(text, out int number))
		{
			if (number < min)
				return min.ToString();
			else if (number > max)
				return max.ToString();
			else
				return number.ToString();
		}
		else
		{
			return min.ToString();
		}
	}
#endregion

#region Hosting
	private void HostButton_Pressed()
	{
		GlobalMultiplayer.ServerDetail detail = new()
        {
			Id = Guid.NewGuid().ToString(),
			Port = short.Parse(PortEdit.Text),
			Max_player = int.Parse(MaxPlayerEdit.Text),
			Host_id = Multiplayer.GetUniqueId(),
			Difficulty_level = (short)DifficultyEdit.Selected
        };

		Error e = GlobalMultiplayer.Instance.CreateServer(detail);

		if (e != Error.Ok)
		{
			HostErrorEdit.Text = $"Failed to create server: {e}";
			return;
		}

		SceneManager.Instance.CallDeferred(SceneManager.MethodName.StartGameAsHost);
	}

#endregion

#region Joining

	private void JoinButton_Pressed()
	{
		string address = JoinAddressEdit.Text;
		ushort port = ushort.Parse(JoinPortEdit.Text);

		isJoining = true;
		JoinButton.Disabled = true;
		JoinErrorEdit.Text = "Joining server...";

		Error e = GlobalMultiplayer.Instance.JoinServer(address, port);
		switch (e)
		{
			case Error.Ok:
				GD.Print($"Successfully Joined {address}:{port}!");

				SceneManager.Instance.CallDeferred(SceneManager.MethodName.StartGameAsClient);

				return;
			default:
				JoinErrorEdit.Text = $"Join server failed: {e}";
				break;
		}

		JoinTimeout.Start();
	}

#endregion

}
