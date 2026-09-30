using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class NumberComp : SettingsUIClass

{
	[Export, Required]
	private LineEdit NumberInput;

	[Export]
	public double min_value = 0f;
	[Export]
	public double max_value = 9999f;
	[Export]
	public bool round = false;

	public double _value = 0f;

    public override void _EnterTree()
    {
		NumberInput.FocusExited += Submit;
		NumberInput.TextSubmitted += _ => Submit();
    }

	public override void SetMin(double _min)
	{
		min_value = _min;
		NumberInput.PlaceholderText = $"{min_value}-{max_value}";
	}

	public override void SetMax(double _max)
	{
		max_value = _max;
		NumberInput.PlaceholderText = $"{min_value}-{max_value}";
	}

	// Private Functions
    private void Submit()
    {
			double? _v = GetNumberInputValue(NumberInput.Text, min_value, max_value);
			_v ??= min_value; // Null Coalescing
			SetValue(_v);
	}

    

	// implements
	public override void OnValueChanged(Variant newValue)
	{
		if (newValue.As<double>() is double floatValue)
		{
			NumberInput.Text = floatValue.ToString();
		}
		else
		{
			throw new InvalidOperationException("Unsupported type for OnValueChanged");
		}
	}

	public override T GetValue<T>()
	{
		if (typeof(T) == typeof(float))
		{
			return (T)(object)_value;
		}
		else
		{
			throw new InvalidOperationException("Unsupported type for GetValue");
		}
	}

	public override void SetValue<T>(T value)
	{
		if (value is Variant variantValue)
		{
			_value = round ? Math.Round(variantValue.As<double>()) : variantValue.As<double>();
			OnValueChanged(variantValue.As<float>());
		}
		else
		{
			_value = round ? Math.Round((double)(object)value) : (double)(object)value;
			OnValueChanged((double)(object)value);
		}
	}
}

