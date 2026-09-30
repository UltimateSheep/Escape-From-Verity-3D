using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

[GlobalClass]
public partial class SettingsList : Node
{
	public enum SettingsCategory
	{
		General,
		Graphic,
		Audio,
		Input
	}

	public enum SettingInputType
	{
		drop_down,
		key_map,
		number,
		slider,
		toggle,
	}

	public struct SettingsComponent
	{

		public required string _id;
		public required SettingsCategory category;
		public required SettingInputType input_type;

		public required string name;
		public required Type _type; // Dropdown - string, Slider - double, Number - float

		public required Variant value;
		public double min_value;
		public double max_value;
		public double step; // for slider
		public bool round;

	}

	public Dictionary<string, SettingsComponent> Lists = new()
    {
		// General
		{"test", new SettingsComponent()
			{
				_id = "test",
				_type = typeof(float),
				name = "Test",
				input_type = SettingInputType.number,
				category = SettingsCategory.General,
				min_value = 0,
				max_value = 100,
				value = 0f,
			}
		},

		// Graphic
		{"test2", new SettingsComponent()
			{
				_id = "test2",
				_type = typeof(bool),
				name = "Test 2",
				input_type = SettingInputType.toggle,
				category = SettingsCategory.Graphic,
				value = true,
			}
		},

		// Audio
		{"master_audio", new SettingsComponent()
			{
				_id = "master_audio",
				_type = typeof(double),
				name = "Master Volume",
				input_type = SettingInputType.slider,
				category = SettingsCategory.Audio,
				value = 75f,
				round = true,
			}
		},

	};

	
}
