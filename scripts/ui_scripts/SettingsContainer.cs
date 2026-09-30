using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class SettingsContainer : TabContainer
{
	private SettingsList settingsList = new();
	private readonly Dictionary<string, MarginContainer> CategoryContainers = [];

	// Preload
    private readonly PackedScene template = GD.Load<PackedScene>("res://ui_components/settings_template.tscn");

	private readonly Dictionary<SettingsList.SettingInputType, PackedScene> InputTypeMap = new()
	{
		{SettingsList.SettingInputType.drop_down, GD.Load<PackedScene>("res://ui_components/drop_down_comp.tscn")},
		{SettingsList.SettingInputType.key_map, GD.Load<PackedScene>("res://ui_components/key_map_comp.tscn")},
		{SettingsList.SettingInputType.number, GD.Load<PackedScene>("res://ui_components/number_comp.tscn")},
		{SettingsList.SettingInputType.slider, GD.Load<PackedScene>("res://ui_components/slider_comp.tscn")},
		{SettingsList.SettingInputType.toggle, GD.Load<PackedScene>("res://ui_components/toggle_comp.tscn")}
	};

	private readonly Dictionary<string, SettingsUIClass> Inputs = [];

	public override void _Ready()
	{

		ClearChildren();

		string[] Categories = GetCategories();

		int i = 0;
		foreach (string Category in Categories)
		{
			if (HasNode(Category))
				continue;

            MarginContainer marginCategory = (MarginContainer)template.Instantiate();
			marginCategory.Name = Category;
			CategoryContainers.Add(Category, marginCategory);

            AddChild(marginCategory);
			SetTabTitle(i, Category);

			i++;
		}

		foreach (KeyValuePair<string, SettingsList.SettingsComponent> item in settingsList.Lists)
		{
			MarginContainer category = CategoryContainers[item.Value.category.ToString()];

			PopulateCategory(category, item.Value);
		}
	}

	private void PopulateCategory(MarginContainer container, SettingsList.SettingsComponent settings)
	{
		SettingsList.SettingInputType _input_Type = settings.input_type;
		PackedScene input_type = InputTypeMap[_input_Type];

		SettingsUIClass node = (SettingsUIClass)input_type.Instantiate();

		node._metadata = settings;
		node.Name = settings._id;

		Inputs.Add(settings._id, node);
		container.GetChild(0).GetChild(0).AddChild(node);

		node.InitSettings();
	}

	private void ClearChildren()
	{
		foreach (MarginContainer item in GetChildren().Cast<MarginContainer>())
		{
			item.Free();
		}
	}

	private string[] GetCategories()
	{
		List<string> _v = [];

		foreach (KeyValuePair<string, SettingsList.SettingsComponent> item in settingsList.Lists)
		{
			_v.Add(item.Value.category.ToString());
		}

		return [.. _v];
	}
}
