using FusionGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public partial class OnlineHostUI : MarginContainer
{
	[Export] private Timer RefreshTimer;
	[Export] private VBoxContainer RoomContainer;
	[Export] public string RoomTemplatePath;

	[Export] private Control MainOnline;
	[Export] private Control ConnectionError;
	[Export] private Button RetryButton;
	[Export] private Timer RetryTimeout;

	[Export, ExportCategory("Host")] private LineEdit Host_PlayerName; 
	[Export] private Button Host_Button; 
	[Export] private TextEdit Host_ErrorField; 
	[Export] private LineEdit Host_RoomName; 
	[Export] private LineEdit Host_MaxPlayer; 
	[Export] private LineEdit Host_Password; 
	private readonly Dictionary<ulong, string> Host_Errors = [];

	[Export, ExportCategory("Join")] private LineEdit Join_PlayerName; 
	[Export] private Button Join_Button; 
	[Export] private TextEdit Join_ErrorField; 
	[Export] private LineEdit Join_Password; 
	[Export] private ScrollContainer Join_Container;
	private readonly Dictionary<ulong, string> Join_Errors = [];

	private PackedScene RoomTemplate;
	private List<Node> roomButtons = [];
	private List<FusionRoomListing> roomListings = [];

	private FusionRoomListing selectedRoom = null;

#region Signals
	[Signal]
	public delegate void SelectRoomEventHandler(string roomId);

	public override void _Ready()
	{
		// Preload
		RoomTemplate = GD.Load<PackedScene>(RoomTemplatePath);

		InitUI();

		Host_Errors.Clear();
		Join_Errors.Clear();

		// Input Fields Binding
		Bind_RequiredField(Host_PlayerName, Host_Errors, Host_ErrorField, "Player Name");
		Bind_RequiredField(Host_RoomName, Host_Errors, Host_ErrorField, "Room Name");

		Bind_NumberValidate(Host_MaxPlayer, 2, 6);
		Bind_RequiredField(Host_MaxPlayer, Host_Errors, Host_ErrorField, "Max Player");

		Bind_RequiredField(Join_PlayerName, Join_Errors, Join_ErrorField, "Player Name");

		// Photon
		// CallDeferred(Fusion.MethodName.ConnectToPhoton, GlobalMultiplayer.Instance.Name);

		// Signals
		RefreshTimer.Timeout += RefreshRooms;
		Host_Button.Pressed += OnHostButton_Pressed;
		Join_Button.Pressed += OnJoinButton_Pressed;
		SelectRoom += OnRoomSelected;

		// Photon Signals
		RetryButton.Pressed += ConnectToPhoton;
		Fusion.ConnectedToPhoton += OnPhotonConnection;
		Fusion.ConnectionStatusChanged += OnPhotonStatusChanged;
		Fusion.RoomJoined += OnRoomJoined;

		RetryTimeout.Timeout += () => {
			RetryButton.Disabled = false;
		};

		CallDeferred(MethodName.ConnectToPhoton);
	}



    public override void _Process(double delta)
	{
		
	}
#endregion


#region Public Functions
#endregion

#region Private Functions

	private void InitUI()
	{
		ConnectionError.Visible = true;
		MainOnline.Visible = false;

		selectedRoom = null;
		UpdateJoin();
	}

	private void OnRoomSelected(string name)
	{
		FusionRoomListing roomListing = roomListings.Find(l => l.Name == name);

		if (roomListing is not null)
		{
			selectedRoom = roomListing;
		}

		UpdateJoin();
		RefreshRooms();
	}

	private void UpdateJoin()
	{
		if (selectedRoom is not null)
		{
			var (_, has_password, _, _) = GetRoomInfo(selectedRoom);
			
			Join_Container.Visible = true;
			Join_Password.Visible = has_password;
		} else
		{
			Join_Container.Visible = false;
		}
	}

	private void UpdateRoomListing(List<FusionRoomListing> newListings)
	{
		ReconcileRooms(newListings);

		foreach (FusionRoomListing newListing in newListings)
		{
			// GD.Print($"get new room! {newListing.Name}");
			if (roomButtons.Any(l => l.Name == newListing.Name))
				continue;

			Node listing = RoomTemplate.Instantiate<Node>();

            SetRoomTemplate(listing, newListing);

			roomButtons.Add(listing);
			RoomContainer.AddChild(listing);
		}

		roomListings = newListings;
	}

	private void ReconcileRooms(List<FusionRoomListing> newListings)
	{
		foreach (Node listing in roomButtons)
		{
			if (!newListings.Any(l => l.Name == listing.Name))
			{
				roomButtons.Remove(listing);
				listing.QueueFree();
				continue;
			}

			FusionRoomListing newListing = newListings.Find(l => l.Name == listing.Name);

			if (selectedRoom is not null)
				listing.GetNode<Button>("Button").Disabled = selectedRoom.Name == newListing.Name;
		
            SetRoomTemplate(listing, newListing);
		}
	}

	private static void SetRoomTemplate(Node roomNode, FusionRoomListing listing)
	{
		roomNode.Name = listing.Name;

		Label RoomName = roomNode.GetNode<Label>("%RoomName"); 
		Label PlayersCount = roomNode.GetNode<Label>("%Players");
		TextureRect PasswordReq = roomNode.GetNode<TextureRect>("%PasswordReq");

		string room_name = listing.CustomProperties.TryGetValue("room_name", out Variant n) ? n.AsString() : "[Error]"; 
		bool password_visible = listing.CustomProperties.TryGetValue("has_password", out Variant hp) && hp.AsBool(); 
		RoomName.Text = room_name;
		PlayersCount.Text = $"{listing.PlayerCount}/{listing.MaxPlayers}";
		PasswordReq.Visible = password_visible;
	}

#region Photon
	private void OnRoomJoined()
	{
		SceneManager.Instance.LoadGame_Multiplayer();
	}

	private void RefreshRooms()
	{
		if (!Fusion.IsConnectedToPhoton())
			return;

		List<FusionRoomListing> list = Fusion.GetRoomList();

		UpdateRoomListing(list);
	}

	private void ConnectToPhoton()
	{
		RetryButton.Disabled = true;

        GlobalMultiplayer.ConnectPhoton();

		RetryTimeout.Start();
	}

	private void OnPhotonConnection()
	{
		ConnectionError.Visible = false;
		MainOnline.Visible = true;
		RefreshTimer.Start();
	}

	private void OnPhotonStatusChanged(ConnectionStatus connectionStatus)
	{
		switch (connectionStatus)
		{
			case ConnectionStatus.Disconnected:
				ConnectionError.Visible = true;
				MainOnline.Visible = false;
				break;
			default:
				break;
		}
	}

	private void OnHostButton_Pressed()
	{
		if (string.IsNullOrWhiteSpace(Host_PlayerName.Text)) return;
		if (string.IsNullOrWhiteSpace(Host_RoomName.Text)) return;
		if (string.IsNullOrWhiteSpace(Host_MaxPlayer.Text)) return;

		bool has_password = !string.IsNullOrWhiteSpace(Host_Password.Text);
		string salt = GlobalMultiplayer.GenerateSalt();
		string newGuid = Guid.NewGuid().ToString();
		string password = GlobalMultiplayer.GenerateHashedPassword(Host_Password.Text, salt);

        FusionRoomOptions roomOptions = new()
        {

            MaxPlayers = (byte)StrToInt(Host_MaxPlayer.Text),
            CustomProperties = new Godot.Collections.Dictionary
            {
                ["room_name"] = Host_RoomName.Text,
				["has_password"] = has_password,
				["salt"] = salt,
				["password"] = password,
            },
			LobbyProperties = ["room_name", "has_password", "salt", "password"]
        };

		RefreshTimer.Stop();

        Fusion.CreateRoom(newGuid, roomOptions);

		GlobalMultiplayer.Instance.ChangeNickname(Host_PlayerName.Text);
    }
	 private void OnJoinButton_Pressed()
    {
		if (string.IsNullOrWhiteSpace(Join_PlayerName.Text)) return;
        
		var (_, has_password, password, salt) = GetRoomInfo(selectedRoom);

		if (has_password 
			&& !string.IsNullOrWhiteSpace(Join_Password.Text) 
			&& password == GlobalMultiplayer.GenerateHashedPassword(Join_Password.Text, salt))
		{
			AddError(Join_Errors, Join_ErrorField, 67, "Incorrect Password.");
			return;
		}


		Fusion.JoinRoom(selectedRoom.Name);

		GlobalMultiplayer.Instance.ChangeNickname(Join_PlayerName.Text);
    }

#endregion

#region Validation
	private static void AddError(Dictionary<ulong, string> errorList, TextEdit errorField, ulong id, string error_msg)
	{
		if (errorList.Any(e => e.Key == id))
			return;

		errorList.Add(id, error_msg);

		PolError(errorField, errorList);
	}

	private static void RemoveError(Dictionary<ulong, string> errorList, TextEdit errorField, ulong id)
	{
		if (errorList.TryGetValue(id, out string _))
		{
			errorList.Remove(id);
		}

		PolError(errorField, errorList);
	}

	private static void PolError(TextEdit errorField, Dictionary<ulong, string> errorList)
	{
		errorField.Text = "";

		foreach (KeyValuePair<ulong, string> error in errorList)
		{
			errorField.Text += $"{error.Value}\n";
		}
	}
#endregion

#endregion

#region Utils

	private (string name, bool has_password, string password, string salt) GetRoomInfo(FusionRoomListing listing)
	{
		string room_name = selectedRoom.CustomProperties.TryGetValue("room_name", out Variant n) ? n.AsString() : "[Error]"; 
		bool has_password = selectedRoom.CustomProperties.TryGetValue("has_password", out Variant hp) && hp.AsBool(); 
		string password = selectedRoom.CustomProperties.TryGetValue("password", out Variant p) ? p.AsString(): null; 
		string salt = selectedRoom.CustomProperties.TryGetValue("password", out Variant s) ? s.AsString(): null; 

		return (room_name, has_password, password, salt);
	}

	private void Bind_NumberValidate(LineEdit lineEdit, int min, int max)
	{
		void on_submit()
		{
			int number = ValidateNumberField(lineEdit, min, max);
			lineEdit.Text = number.ToString();
		}

		lineEdit.TextSubmitted += _ => on_submit();
		lineEdit.FocusExited += on_submit;

		on_submit();
	}

	private void Bind_RequiredField(LineEdit lineEdit, Dictionary<ulong, string> errorList, TextEdit errorField, string requiredName)
	{
		void on_submit()
		{
			if (string.IsNullOrWhiteSpace(lineEdit.Text))
			{
                AddError(errorList, errorField, lineEdit.GetInstanceId(), $"{requiredName} is required.");
			} else
			{
                RemoveError(errorList, errorField, lineEdit.GetInstanceId());
			}
		}

		lineEdit.TextSubmitted += _ => on_submit();
		lineEdit.FocusExited += on_submit;

		on_submit();
	}

	private static int ValidateNumberField(LineEdit lineEdit, int min, int max)
	{
		if (int.TryParse(lineEdit.Text, out int result))
		{
			return Math.Clamp(result, min, max);
		}

		return min;
	}

	private static int StrToInt(string input)
	{
		if (int.TryParse(input, out int output))
		{
			return output;
		} else
		{
			GD.PushError("Cannot convert to int");
			throw new Exception("Cannot convert to int");
		}
	}

#endregion

}
