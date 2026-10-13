using CPMEngine;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.MenuUI.TransferCaseUI
{
	public class TransferCaseDiffIndicators : MonoBehaviour
	{
		[SerializeField]
		private Image FrontDiff;

		[SerializeField]
		private Image CenterDiff;

		[SerializeField]
		private Image RearDiff;

		[SerializeField]
		private GameObject FrontOff;

		[SerializeField]
		private GameObject CenterOff;

		[SerializeField]
		private GameObject RearOff;

		private Powertrain _powertrain;

		private GameObject _trackedCar;

		private int _lastState = -1;

		private void OnDisable()
		{
			_lastState = -1;
		}

		private void Update()
		{
			GameObject car = V.Car;
			if (car != _trackedCar)
			{
				_trackedCar = car;
				_powertrain = null;
				if ((bool)car)
				{
					car.TryGetComponent<Powertrain>(out _powertrain);
				}
			}
			TransferCase transferCase = (((bool)_powertrain && _powertrain.HasActiveTransferCase) ? _powertrain.TransferCase : null);
			int num = 0;
			if (transferCase != null)
			{
				if (transferCase.FrontLockAvailable)
				{
					num |= 1;
				}
				if (transferCase.FrontLockOn)
				{
					num |= 2;
				}
				if (transferCase.IsFullTime && transferCase.HasCenterLock)
				{
					num |= 4;
				}
				if (transferCase.CenterLockedEffective)
				{
					num |= 8;
				}
				if (transferCase.HasRearLock)
				{
					num |= 0x10;
				}
				if (transferCase.RearLockOn)
				{
					num |= 0x20;
				}
				num |= 0x40;
			}
			if (num != _lastState)
			{
				_lastState = num;
				Apply(FrontDiff, FrontOff, (num & 1) != 0, (num & 2) != 0);
				Apply(CenterDiff, CenterOff, (num & 4) != 0, (num & 8) != 0);
				Apply(RearDiff, RearOff, (num & 0x10) != 0, (num & 0x20) != 0);
			}
		}

		private void Apply(Image image, GameObject off, bool available, bool engaged)
		{
			if ((bool)image)
			{
				if (image.gameObject.activeSelf != available)
				{
					image.gameObject.SetActive(available);
				}
				if (available && (bool)off && off.activeSelf == engaged)
				{
					off.SetActive(!engaged);
				}
			}
		}
	}
}
