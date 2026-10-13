using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.MenuUI.TransferCaseUI
{
	public class TransferCaseHudButton : MonoBehaviour
	{
		[SerializeField]
		private GameObject Root;

		[SerializeField]
		private Button Button;

		[SerializeField]
		private TextMeshProUGUI ModeLabel;

		[SerializeField]
		private TransferCasePanel Panel;

		private Powertrain _powertrain;

		private GameObject _polledCar;

		private bool _polledHasCase;

		private TransferCaseMode _polledMode;

		private void Update()
		{
			GameObject car = V.Car;
			bool flag = _powertrain != null && _powertrain.HasActiveTransferCase;
			if (car == _polledCar && flag == _polledHasCase)
			{
				if (flag && _powertrain.TransferCase.Mode != _polledMode)
				{
					_polledMode = _powertrain.TransferCase.Mode;
					UpdateModeLabel();
				}
			}
			else
			{
				Refresh(car);
				_polledCar = car;
				_polledHasCase = _powertrain != null && _powertrain.HasActiveTransferCase;
				_polledMode = (_polledHasCase ? _powertrain.TransferCase.Mode : _polledMode);
			}
		}

		private void OnEnable()
		{
			V.OnCarEntered += OnCarEntered;
			V.OnCarExit += OnCarExit;
			Button.onClick.AddListener(TogglePanel);
			Refresh(V.Car);
		}

		private void OnDisable()
		{
			V.OnCarEntered -= OnCarEntered;
			V.OnCarExit -= OnCarExit;
			Button.onClick.RemoveListener(TogglePanel);
		}

		private void OnCarEntered(GameObject person, VehicleData vehicleData)
		{
			Refresh(vehicleData ? vehicleData.gameObject : V.Car);
		}

		public void RefreshFromCar()
		{
			Refresh(V.Car);
		}

		private void OnCarExit(GameObject car, VehicleData vehicleData)
		{
			_powertrain = null;
			Root.SetActive(value: false);
			Panel.Hide();
		}

		private void Refresh(GameObject car)
		{
			_powertrain = null;
			if ((bool)car)
			{
				car.TryGetComponent<Powertrain>(out _powertrain);
			}
			bool flag = (bool)_powertrain && _powertrain.HasActiveTransferCase;
			Root.SetActive(flag);
			Panel.Hide();
			if (flag)
			{
				UpdateModeLabel();
			}
		}

		public void UpdateModeLabel()
		{
			if ((bool)_powertrain && _powertrain.HasActiveTransferCase)
			{
				bool flag = _powertrain.GearType == GearboxType.Gear6Wd;
				TextMeshProUGUI modeLabel = ModeLabel;
				modeLabel.text = _powertrain.TransferCase.Mode switch
				{
					TransferCaseMode.H2 => "2H", 
					TransferCaseMode.L4 => flag ? "6L" : "4L", 
					_ => flag ? "6H" : "4H", 
				};
			}
		}

		private void TogglePanel()
		{
			if ((bool)_powertrain && _powertrain.HasActiveTransferCase)
			{
				SoundEventsDataHelper.PlayUISound(Panel.Toggle(_powertrain, this) ? SoundEventsDataHelper.SmallClickSwitch : SoundEventsDataHelper.ClickBackSwitch, base.gameObject);
			}
		}
	}
}
