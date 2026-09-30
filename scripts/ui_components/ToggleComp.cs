using Godot;
using System;

public partial class ToggleComp : SettingsUIClass
{
	[Export]
	private CheckButton checkButton;
	public bool _value = false;

    public override void _EnterTree()
    {
		checkButton.Toggled += Submit;
    }


    // Private Functions

    private void Submit(bool value)
    {
		_value = value;
		SetValue(value);
	}

	// implements
	public override void OnValueChanged(Variant newValue)
	{
		checkButton.ButtonPressed = newValue.As<bool>();
	}

	public override T GetValue<T>()
	{
		return (T)(object)_value;
	}

	public override void SetValue<T>(T value)
	{
		if (value is Variant variantValue)
		{
			_value = variantValue.As<bool>();
			OnValueChanged(variantValue.As<bool>());
		}
		else
		{
			_value = (bool)(object)value;
			OnValueChanged((bool)(object)value);
		}
	}
}
