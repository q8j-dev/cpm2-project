using System;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.UIElement;

public class SliderWithCountPrefix : MonoBehaviour
{
	[SerializeField]
	private SliderValueHandler SliderValueHandler;

	[SerializeField]
	private TextMeshProUGUI HeaderText;

	private int _index;

	private Action<int, float> _onValueChange;

	private void Start()
	{
		SliderValueHandler.AddListener(OnSliderValueChange);
	}

	public void SetValue(float val)
	{
		SliderValueHandler.SetValue(val);
	}

	public void Init(int index, string prefix, Action<int, float> action, float minVal, float maxValue, float value, float step, bool roundValue = false)
	{
		_index = index;
		HeaderText.text = prefix;
		SliderValueHandler.Init(minVal, maxValue, step, roundValue, removeSpecialListener: false, 3);
		_onValueChange = action;
	}

	public void Reset()
	{
		SliderValueHandler.RemoveAllListeners();
	}

	public void OnSliderValueChange(float value)
	{
		_onValueChange?.Invoke(_index, value);
	}
}
