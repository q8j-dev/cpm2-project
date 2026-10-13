using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeCategoryWithScrolls : PhotoModeCategory
	{
		[SerializeField]
		private ScrollRect Scroll;

		[SerializeField]
		private GameObject ScrollPanel;

		[SerializeField]
		private List<PhotoModeCameraParameter> Parameters;

		private PhotoModeCameraParameter _currentParameter;

		public override int ParameterCount => Parameters.Count;

		private void OnDestroy()
		{
			foreach (PhotoModeCameraParameter parameter in Parameters)
			{
				parameter.OnSelected -= OnParameterSelect;
			}
		}

		public override void Init(PhotoModePrototype photoMode, PhotoModeConfig config)
		{
			Config = config;
			foreach (PhotoModeCameraParameter parameter in Parameters)
			{
				parameter.Init(photoMode, config);
				parameter.OnSelected += OnParameterSelect;
				parameter.SetSelectionTypeActive(active: false);
			}
			_currentParameter = Parameters[0];
			base.CurrentInfo = _currentParameter.Info;
		}

		public override void Activate(bool isActive)
		{
			ItemsParent.SetActive(isActive);
			ScrollPanel.SetActive(isActive);
		}

		public override void ChooseParameter(int index)
		{
			SkipSettingsSwitchSound = true;
			SetToggleState(isOn: true);
			Parameters[index].SetToggleState(isOn: true);
			for (int i = 0; i < Parameters.Count; i++)
			{
				if (i != index)
				{
					Parameters[i].SetToggleState(isOn: false);
				}
				if (Parameters[i] == _currentParameter)
				{
					OnParameterSelect(Parameters[i]);
				}
			}
			SkipSettingsSwitchSound = false;
		}

		public override void ResetAll()
		{
			foreach (PhotoModeCameraParameter parameter in Parameters)
			{
				parameter.ResetSelection();
			}
		}

		public override void ResetCurrentParameter()
		{
			_currentParameter.ResetValues();
		}

		public override void NavigateParameter(int offset)
		{
			int num = Parameters.IndexOf(_currentParameter);
			int num2 = Mathf.Clamp(num + offset, 0, Parameters.Count - 1);
			if (num2 != num)
			{
				ChooseParameter(num2);
			}
		}

		public override void AdjustCurrentValue(int direction)
		{
			if (_currentParameter is PhotoModeCameraParameterWithScroll photoModeCameraParameterWithScroll)
			{
				photoModeCameraParameterWithScroll.AdjustValue(direction);
			}
			else if (_currentParameter is PhotoModeCameraParameterWithToggles photoModeCameraParameterWithToggles)
			{
				photoModeCameraParameterWithToggles.AdjustValue(direction);
			}
		}

		public override void OnCategoryChosen(bool isOn)
		{
			base.OnCategoryChosen(isOn);
			if (isOn)
			{
				ScrollPanel.SetActive(_currentParameter.ParameterType == PhotoModeParameterType.Scroll);
				foreach (PhotoModeCameraParameter parameter in Parameters)
				{
					parameter.SetSelectionActive(parameter == _currentParameter);
					if (parameter.ParameterType != _currentParameter.ParameterType)
					{
						parameter.SetSelectionTypeActive(active: false);
					}
				}
				_currentParameter.ResetValueText();
			}
			else
			{
				_currentParameter.SetSelectionActive(active: false);
				ScrollPanel.SetActive(value: false);
			}
		}

		private void OnParameterSelect(PhotoModeCameraParameter parameter)
		{
			_currentParameter = parameter;
			_currentParameter.SetSelectionTypeActive(active: true);
			base.CurrentInfo = _currentParameter.Info;
			base.OnParameterSelected?.Invoke(base.CurrentInfo);
			MoveCurrentToCenter();
			foreach (PhotoModeCameraParameter parameter2 in Parameters)
			{
				if (parameter2.ParameterType != _currentParameter.ParameterType)
				{
					parameter2.SetSelectionTypeActive(active: false);
				}
			}
		}

		private void MoveCurrentToCenter()
		{
			if (Parameters.Count >= 2)
			{
				float num = 1f / (float)(Parameters.Count - 1);
				int num2 = Parameters.Count - 1 - Parameters.IndexOf(_currentParameter);
				float endValue = num * (float)num2;
				Scroll.DOVerticalNormalizedPos(endValue, 0.1f).SetUpdate(isIndependentUpdate: true);
			}
		}
	}
}
