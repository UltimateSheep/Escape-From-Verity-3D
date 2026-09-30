using Godot;
using System;

public partial class DropDownComp : SettingsUIClass
{
	[Export]
	private OptionButton optionButton;
	public long _value = 0;

	[Export]
	public string[] _content = [];

    public override void _EnterTree()
    {
		SetContentArray(_content);

		optionButton.ItemSelected += Submit;
    }

	public void SetContentArray(string[] items)
	{
		_content = items;
		for (int i = 0; i < optionButton.ItemCount; i++)
		{
			optionButton.RemoveItem(i);
		}

		foreach (string item in items)
		{
			AddContent(item);
		}

	}

	public void AddContent(string item)
	{
		optionButton.AddItem(item);
	}

    // Private Functions

    private void Submit(long index)
    {
		_value = index;
		SetValue(index);
	}

	// implements
	public override void OnValueChanged(Variant newValue)
	{
		optionButton.Select(newValue.As<int>());
	}



	public override T GetValue<T>()
	{
		return (T)(object)_value;
	}

	public override void SetValue<T>(T value)
	{
		if (value is Variant variantValue)
		{
			_value = variantValue.As<long>();
			OnValueChanged(variantValue.As<long>());
		}
		else
		{
			_value = (long)(object)value;
			OnValueChanged((long)(object)value);
		}
	}
}
