using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

public class UIPartCategoryChecker : MonoBehaviour
{
	[SerializeField]
	private RawImage StatusRawImage;

	[SerializeField]
	private Texture2D WarningTexture;

	[SerializeField]
	private Texture2D ErrorTexture;

	[SerializeField]
	private Image StatusImage;

	[SerializeField]
	private Color WarningColor;

	[SerializeField]
	private Color ErrorColor;

	[SerializeField]
	private Color NormalColor;

	[SerializeField]
	private CarTuningUISlider CarTuningUISlider;

	[SerializeField]
	private bool DontUseSlider;

	[SerializeField]
	private bool DontChangeColor;

	public Button UIPartCategoryButton;

	public Toggle UIPartCategoryToggle;

	private AllParts _allParts;

	private ErrorState _currentState;

	[Inject]
	public void Construct(AllParts allParts)
	{
		_allParts = allParts;
	}

	public void ResetState()
	{
		_currentState = ErrorState.Normal;
	}

	public void Check(List<PartData> partDatas)
	{
		bool flag = false;
		bool flag2 = false;
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < partDatas.Count; i++)
		{
			if (partDatas[i] != null && partDatas[i].InstanceID != -1 && partDatas[i].ModelId != 0)
			{
				float num4 = partDatas[i].GetDurability();
				if (float.IsNaN(num4) || float.IsInfinity(num4))
				{
					num4 = 1f;
				}
				num += num4;
				num3++;
				if (num4 < 0.5f && num4 > 0.2f)
				{
					flag = true;
				}
				else if (num4 < 0.2f)
				{
					flag2 = true;
				}
			}
			else if (partDatas[i] == null || partDatas[i].InstanceID == -1)
			{
				num2++;
			}
		}
		if (!DontUseSlider)
		{
			num = ((num3 > 0) ? (num / (float)num3) : 1f);
			if (num > 0.5f)
			{
				CarTuningUISlider.Init(NormalColor, num);
			}
			else
			{
				CarTuningUISlider.Init((num < 0.2f) ? WarningColor : ErrorColor, num);
			}
		}
		if (!DontChangeColor)
		{
			StatusImage.gameObject.SetActive(flag2 | flag);
			StatusImage.color = (flag2 ? ErrorColor : WarningColor);
		}
		else
		{
			StatusRawImage.gameObject.SetActive(flag2 | flag);
			StatusRawImage.texture = (flag2 ? ErrorTexture : WarningTexture);
		}
	}

	public void CheckRange(float values, float optimalValue, float minPercent, float optimalPercent)
	{
		float num = optimalValue - optimalValue * optimalPercent;
		float num2 = optimalValue + optimalValue * optimalPercent;
		float num3 = optimalValue - optimalValue * minPercent;
		float num4 = optimalValue + optimalValue * minPercent;
		if (num < values && num2 > values && _currentState == ErrorState.Normal)
		{
			StatusRawImage.gameObject.SetActive(value: false);
			return;
		}
		ErrorState state = ((!(num3 < values) || !(num4 > values)) ? ErrorState.Error : ErrorState.Warning);
		ChangeCurrentState(state);
	}

	private void ChangeCurrentState(ErrorState state)
	{
		switch (state)
		{
		case ErrorState.Error:
			StatusRawImage.texture = ErrorTexture;
			break;
		case ErrorState.Warning:
			if (_currentState == ErrorState.Error)
			{
				return;
			}
			StatusRawImage.texture = WarningTexture;
			break;
		}
		_currentState = state;
		bool flag = state == ErrorState.Normal;
		StatusRawImage.gameObject.SetActive(!flag);
	}

	public void Check(List<float> values)
	{
		ErrorState state = ErrorState.Normal;
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i] < 50f)
			{
				state = ErrorState.Warning;
				if (values[i] < 20f)
				{
					state = ErrorState.Error;
				}
			}
		}
		ChangeCurrentState(state);
	}

	public void Hide()
	{
		base.transform.parent.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		UIPartCategoryButton?.onClick.RemoveAllListeners();
		UIPartCategoryToggle?.onValueChanged.RemoveAllListeners();
	}
}
