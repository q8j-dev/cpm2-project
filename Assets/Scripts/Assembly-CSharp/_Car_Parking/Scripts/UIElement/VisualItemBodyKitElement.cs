using System;
using CPM.Database;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.UIElement
{
	public class VisualItemBodyKitElement : VisualItemElement
	{
		[SerializeField]
		private CombinedPriceView CombinedPriceView = default;

		private BodyKitUIElementData _bodyKitUIElementData;

		private CarInfo _carInfo;

		public BodyKitUIElementData BodyKitUIElementData => _bodyKitUIElementData;

		public event Action<VisualItemBodyKitElement> OnClicked;

		public override void Init(VisualTuningCellData visualTuningCellData)
		{
			base.Init(visualTuningCellData);
			Button.onValueChanged.AddListener(OnButtonValueChangedCallback);
		}

		public void Init(BodyKitUIElementData bodyKitUIElementData, CarInfo carInfo)
		{
			_bodyKitUIElementData = bodyKitUIElementData;
			_carInfo = carInfo;
			UpdateUI();
		}

		public void SetSelected(bool isSelected)
		{
			Button.SetIsOnWithoutNotify(isSelected);
			elementImage.color = (isSelected ? Color.green : Color.white);
		}

		private void OnEnable()
		{
			CarInfoExtensions.OnBoughtBodyKitsUpdated += OnBoughtBodyKitsUpdatedCallback;
		}

		private void OnDisable()
		{
			CarInfoExtensions.OnBoughtBodyKitsUpdated -= OnBoughtBodyKitsUpdatedCallback;
		}

		private void OnButtonValueChangedCallback(bool isOn)
		{
			OnClicked?.Invoke(this);
		}

		private void OnBoughtBodyKitsUpdatedCallback(BoughtBodyKits boughtBodyKits, BodyKitType bodyKitType, int index)
		{
			if (_bodyKitUIElementData.BodyKitType == bodyKitType && _bodyKitUIElementData.Index == index)
			{
				UpdateUI();
			}
		}

		private void UpdateUI()
		{
			if (_bodyKitUIElementData.isSpacer)
			{
				canvasGroup.alpha = 0f;
				canvasGroup.interactable = false;
				return;
			}
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			CombinedPriceView.Init(_bodyKitUIElementData.CombinedPrice);
			bodyKitType = _bodyKitUIElementData.BodyKitType;
			titleText.text = _bodyKitUIElementData.partNumber;
			elementImage.texture = _bodyKitUIElementData.ImgTexture;
			index = _bodyKitUIElementData.Index;
			IsBought = _carInfo.BoughtBodyKits.GetBodyKitPaidStatus(_bodyKitUIElementData.BodyKitType, _bodyKitUIElementData.Index) || _bodyKitUIElementData.CombinedPrice.IsZero;
			CombinedPriceView.gameObject.SetActive(!IsBought);
		}
	}
}
