using System;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Police
{
	public class VisualTuningPoliceElement : VisualItemElement
	{
		[SerializeField]
		private CombinedPriceView CombinedPriceView = default;

		[SerializeField]
		private GameObject Root = default;

		private PoliceSystemConfig _policeSystemConfig;

		private IPoliceSystem _policeSystem;

		private PoliceCarBodyKitsConfig.BodyKitData _bodyKitData;

		private CarInfo _carInfo;

		public PoliceCarBodyKitsConfig.BodyKitData BodyKitData => _bodyKitData;

		public event Action<VisualTuningPoliceElement> OnClicked;

		[Inject]
		public void Construct(PoliceSystemConfig policeSystemConfig, IPoliceSystem policeSystem)
		{
			_policeSystemConfig = policeSystemConfig;
			_policeSystem = policeSystem;
		}

		public override void Init(VisualTuningCellData visualTuningCellData)
		{
			base.Init(visualTuningCellData);
			Button.onValueChanged.AddListener(OnButtonValueChanged);
		}

		public void Init(CarInfo carInfo, PoliceCarBodyKitsConfig.BodyKitData bodyKitData)
		{
			_bodyKitData = bodyKitData;
			_carInfo = carInfo;
			if (_bodyKitData == null)
			{
				Root.SetActive(value: false);
				return;
			}
			UpdateIsBought();
			Root.SetActive(value: true);
			if (_bodyKitData.Index == -1)
			{
				CombinedPriceView.gameObject.SetActive(value: false);
				elementImage.gameObject.SetActive(value: false);
			}
			else
			{
				LoadTexture();
				UpdateUI();
			}
		}

		private void OnEnable()
		{
			_policeSystem.BoughtBodyKit.OnBoughted += PoliceBoughtBodyKitsAdded;
		}

		private void OnDisable()
		{
			_policeSystem.BoughtBodyKit.OnBoughted -= PoliceBoughtBodyKitsAdded;
		}

		private void PoliceBoughtBodyKitsAdded(PoliceBodyKitType type, int index)
		{
			UpdateIsBought();
			UpdateUI();
		}

		private void UpdateIsBought()
		{
			if (_bodyKitData != null)
			{
				IsBought = _policeSystem.BoughtBodyKit.Contains(_bodyKitData);
				PoliceBodyKitSettingsConfig.BodyKitSettings bodyKitSettings = _policeSystemConfig.GetBodyKitSettings(_bodyKitData);
				if (bodyKitSettings != null && (int)bodyKitSettings.Price == 0)
				{
					IsBought = true;
				}
			}
		}

		private void UpdateUI()
		{
			if (_bodyKitData == null)
			{
				return;
			}
			PoliceBodyKitSettingsConfig.BodyKitSettings bodyKitSettings = _policeSystemConfig.GetBodyKitSettings(_bodyKitData.BodyKitType, _bodyKitData.Index);
			if (bodyKitSettings != null)
			{
				CombinedPrice combinedPrice = new CombinedPrice(bodyKitSettings.Price, 0);
				CombinedPriceView.Init(combinedPrice);
				if (((int)combinedPrice.Money == 0 && (int)combinedPrice.Coin == 0) || _policeSystem.BoughtBodyKit.Contains(_bodyKitData))
				{
					CombinedPriceView.gameObject.SetActive(value: false);
				}
				TextMeshProUGUI textMeshProUGUI = elementPrice;
				ObscuredInt price = bodyKitSettings.Price;
				textMeshProUGUI.text = price.ToString() ?? "";
			}
		}

		private void LoadTexture()
		{
			PoliceBodyKitSettingsConfig.BodyKitSettings bodyKitSettings = _policeSystemConfig.GetBodyKitSettings(_bodyKitData.BodyKitType, _bodyKitData.Index);
			if (bodyKitSettings != null)
			{
				Texture2D texture = Resources.Load<Texture2D>(bodyKitSettings.ResourcePathToTexture);
				elementImage.texture = texture;
			}
		}

		private void OnButtonValueChanged(bool isOn)
		{
			OnClicked?.Invoke(this);
		}
	}
}
