using Godot;
using System;

[GlobalClass]
public abstract partial class SettingsUIClass : Node
{
	[Export]
	public Label Label;
	public SettingsList.SettingsComponent _metadata;

	public static float? GetNumberInputValue(string _value, float _min_value, float _max_value)
	{
		if (float.TryParse(_value, out float value))
		{
			return Math.Clamp(value, _min_value, _max_value);
		}
		else
		{
			return null;
		}
	}

	public static double? GetNumberInputValue(string _value, double _min_value, double _max_value)
	{
		if (double.TryParse(_value, out double value))
		{
			return Math.Clamp(value, _min_value, _max_value);
		}
		else
		{
			return null;
		}
	}

	public virtual void InitSettings()
	{
		Label.Text = _metadata.name;

		SetValue(_metadata.value);
		if (!_metadata.min_value.Equals(null)) SetMin(_metadata.min_value);
		if (!_metadata.max_value.Equals(null)) SetMin(_metadata.max_value);
		if (!_metadata.step.Equals(null)) SetMin(_metadata.step);
	}

	public abstract T GetValue<T>();

	public abstract void SetValue<T>(T value);

	public virtual void SetMin(double _min){}

	public virtual void SetMax(double _max){}

	public virtual void SetStep(double _step){}

	public abstract void OnValueChanged(Variant newValue);
}
