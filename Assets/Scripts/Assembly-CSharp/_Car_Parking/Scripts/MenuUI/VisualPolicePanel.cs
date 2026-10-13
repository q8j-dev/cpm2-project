using System;
using System.Collections.Generic;
using CPM.Database;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Police;

namespace _Car_Parking.Scripts.MenuUI
{
	public class VisualPolicePanel : MonoBehaviour
	{
		[SerializeField]
		private VisualTuningPoliceElement VisualTuningPoliceElementPrefab;

		[SerializeField]
		private VisualTuningWindow VisualTuningWindow;

		[SerializeField]
		private GameObject BuyPoliceButtonPrefab;

		[SerializeField]
		private SirenSoundControlPanel SirenSoundControlPanel;

		private List<VisualTuningPoliceElement> _elements = new List<VisualTuningPoliceElement>();

		private VehicleData _vehicleData;

		private IPoliceSystem _policeSystem;

		private GameObject _buyPoliceButton;

		private IPrefabFactory _prefabFactory;

		private CarInfo CarInfo
		{
			get
			{
				if (!_vehicleData)
				{
					return null;
				}
				return _vehicleData.CarInfoProvider.CarInfo;
			}
		}

		[Inject]
		public void Construct(IPoliceSystem policeSystem, IPrefabFactory prefabFactory)
		{
			_policeSystem = policeSystem;
			_prefabFactory = prefabFactory;
		}

		private void OnEnable()
		{
			_policeSystem.OnBoughtUpdated += OnPoliceSystemBoughtUpdated;
			VisualTuningWindow.OnTuningValuesChanged += OnTuningValuesChanged;
		}

		private void OnDisable()
		{
			_policeSystem.OnBoughtUpdated -= OnPoliceSystemBoughtUpdated;
			VisualTuningWindow.OnTuningValuesChanged -= OnTuningValuesChanged;
		}

		public void Init(VisualTuningWindow visualTuningWindow, VehicleData vehicleData)
		{
			base.enabled = true;
			_vehicleData = vehicleData;
			InitUI();
		}

		public void Disable()
		{
			SirenSoundControlPanel.gameObject.SetActive(value: false);
			base.enabled = false;
		}

		public void SetDefaultSelected()
		{
			for (int i = 0; i < _elements.Count; i++)
			{
				SetItemDefaultIsOn(_elements[i]);
			}
		}

		private void OnPoliceSystemBoughtUpdated(CarInfo carInfo, bool isBought)
		{
			if (CarInfo != null && CarInfo == carInfo)
			{
				InitUI();
			}
		}

		private void OnTuningValuesChanged()
		{
			InitItemsUI();
		}

		private void InitUI()
		{
			if ((bool)_buyPoliceButton)
			{
				UnityEngine.Object.Destroy(_buyPoliceButton);
				_buyPoliceButton = null;
			}
			if (CarInfo != null)
			{
				if (_policeSystem.IsBought(CarInfo))
				{
					SirenSoundControlPanel.gameObject.SetActive(value: true);
					InitElements();
				}
				else
				{
					SirenSoundControlPanel.gameObject.SetActive(value: false);
					_buyPoliceButton = _prefabFactory.Instantiate(BuyPoliceButtonPrefab, VisualTuningWindow.UiScrollRectOcclusiion.transform);
				}
			}
		}

		private void InitElements()
		{
			PoliceCarBodyKitsConfig config = _policeSystem.PoliceSystemConfig.PoliceCarConfigs.GetConfig(CarInfo.CarID);
			if (!config)
			{
				return;
			}
			List<PoliceCarBodyKitsConfig.BodyKitData> list = new List<PoliceCarBodyKitsConfig.BodyKitData>();
			foreach (PoliceBodyKitType value in Enum.GetValues(typeof(PoliceBodyKitType)))
			{
				List<PoliceCarBodyKitsConfig.BodyKitData> bodyKits = config.GetBodyKits(value);
				if (bodyKits.Count > 0)
				{
					if (list.Count > 0)
					{
						list.Add(null);
					}
					list.AddRange(bodyKits);
				}
			}
			_elements.Clear();
			VisualTuningWindow.VisualTuningElementSpawner.SpawnList(list, VisualTuningPoliceElementPrefab, InitPoliceBodyKit, cleanPanel: true);
		}

		public void InitPoliceBodyKit(GameObject go, object data)
		{
			if (go.TryGetComponent<VisualTuningPoliceElement>(out var component))
			{
				_elements.Add(component);
				component.Init(_vehicleData.CarInfoProvider.CarInfo, data as PoliceCarBodyKitsConfig.BodyKitData);
				component.OnClicked += OnVisualElementDown;
				SetItemDefaultIsOn(component);
			}
		}

		private void OnVisualElementDown(VisualTuningPoliceElement visualTuningPoliceElement)
		{
			VisualTuningWindow.OnClickExecute(VisualTuningTypes.Police, "SelectBodyKit", visualTuningPoliceElement.BodyKitData);
			VisualTuningWindow.SetSelectedVisualItemElement(visualTuningPoliceElement);
		}

		private void InitItemsUI()
		{
			if (VisualTuningWindow.PoliceVisualScope == null)
			{
				return;
			}
			for (int i = 0; i < _elements.Count; i++)
			{
				if (_elements[i].BodyKitData != null)
				{
					if (VisualTuningWindow.PoliceVisualScope.SelectedBodyKitData != null && VisualTuningWindow.PoliceVisualScope.SelectedBodyKitData.BodyKitType == _elements[i].BodyKitData.BodyKitType)
					{
						_elements[i].Button.SetIsOnWithoutNotify(VisualTuningWindow.PoliceVisualScope.SelectedBodyKitData == _elements[i].BodyKitData);
					}
					else
					{
						SetItemDefaultIsOn(_elements[i]);
					}
				}
			}
			VisualTuningWindow.SetActiveControlButtons(VisualTuningWindow.PoliceVisualScope.SelectedBodyKitData != null);
		}

		private void SetItemDefaultIsOn(VisualTuningPoliceElement visualTuningPoliceElement)
		{
			if ((bool)visualTuningPoliceElement && visualTuningPoliceElement.BodyKitData != null)
			{
				if (CarInfo.AdditionalData.Police.InstalledBodyKits.GetIndex(visualTuningPoliceElement.BodyKitData.BodyKitType) == visualTuningPoliceElement.BodyKitData.Index)
				{
					visualTuningPoliceElement.Button.SetIsOnWithoutNotify(value: true);
				}
				else
				{
					visualTuningPoliceElement.Button.SetIsOnWithoutNotify(value: false);
				}
			}
		}
	}
}
