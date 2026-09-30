using Godot;
using System;

public partial class SliderComp : SettingsUIClass

{
	[Export]
	private Slider slider;
	[Export]
	private LineEdit valueLabel;

	[Export]
	public double min_value = 0f;
	[Export]
	public double max_value = 100f;
	[Export]
	public double step = 1f;
	[Export]
	public bool round = false;

	public double _value = 0f;
	private bool is_dragging;

    public override void _EnterTree()
    {
        slider.ValueChanged += _ => Submit();
        slider.DragStarted += () => is_dragging = true;
        slider.DragEnded += _ => is_dragging = false;

		valueLabel.TextSubmitted += _ => SubmitString();
		valueLabel.FocusExited += SubmitString;
    }
	
	public override void SetMin(double _min)
	{
		min_value = _min;
		slider.MinValue = min_value;
		valueLabel.PlaceholderText = $"{min_value}-{max_value}";

	}

	public override void SetMax(double _max)
	{
		max_value = _max;
		slider.MaxValue = max_value;
		valueLabel.PlaceholderText = $"{min_value}-{max_value}";
	}

	public override void SetStep(double _step)
	{
		step = _step;
		slider.Step = step;
	}

	private void SubmitString()
    {
			double? _v = GetNumberInputValue(valueLabel.Text, min_value, max_value);
			_v ??= min_value; // Null Coalescing
			SetValue(_v);
	}

	private void Submit()
	{
		SetValue(slider.Value);
	}

	// implements
	public override void OnValueChanged(Variant newValue)
	{
		if (newValue.As<double>() is double doubleValue)
		{
			valueLabel.Text = doubleValue.ToString();
			if (!is_dragging) slider.Value = doubleValue;
		}
		else
		{
			throw new InvalidOperationException("Unsupported type for OnValueChanged");
		}
	}

	public override T GetValue<T>()
	{
		if (typeof(T) == typeof(double))
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
			OnValueChanged(variantValue.As<double>());
		}
		else
		{
			_value = round ? Math.Round((double)(object)value) : (double)(object)value;
			OnValueChanged((double)(object)value);
		}
	}
}
